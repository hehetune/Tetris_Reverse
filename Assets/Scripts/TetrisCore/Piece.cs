using System;
using Managers;
using UnityEngine;

namespace TetrisCore
{
    public class Piece : MonoBehaviour
    {
        public Board board { get; private set; }
        public TetrominoData data { get; private set; }
        public Vector3Int[] cells { get; private set; }

        public Vector3Int Position
        {
            get => Vector3Int.RoundToInt(transform.position);
            private set { transform.position = value; }
        }

        public int rotationIndex { get; private set; }

        [SerializeField] private float[] stepDelay = new float[] { 0.8f, 0.75f, 0.70f, 0.65f, 0.6f, 0.55f, 0.5f, 0.45f, 0.4f };
        [SerializeField] private float fastStepDelay = 0.07f;
        [SerializeField] public float moveDelay = 0.05f;
        [SerializeField] private float nextLevelDelay = 5f;
        [SerializeField] private float moveHoldingDelay = 0.2f;

        private float stepTime;
        private float lockTime;
        private float moveTime;
        private float nextLevelTime;
        private float moveLeftHoldingTime;
        private float moveRightHoldingTime;

        private bool softDropEnabled = false;
        private bool movingLeft = false;
        private bool movingRight = false;

        private int currentLevel = 0;

        private Vector2Int _moveDownVector = Vector2Int.down;
        private Vector2Int _moveLeftVector = Vector2Int.left;
        private Vector2Int _moveRightVector = Vector2Int.right;

        public Block[] blocks = new Block[4];
        [SerializeField] private Prefab _blockPrefab;

        private bool _gameStop = true;

        private void OnEnable()
        {
            GameInput.Instance.OnRotateBlockLeftAction += OnRotateBlockLeft;
            GameInput.Instance.OnRotateBlockRightAction += OnRotateBlockRight;
            GameInput.Instance.OnMoveBlockLeftPerformed += OnMoveLeftPerformed;
            GameInput.Instance.OnMoveBlockLeftCancel += OnMoveLeftCancel;
            GameInput.Instance.OnMoveBlockRightPerformed += OnMoveRightPerformed;
            GameInput.Instance.OnMoveBlockRightCancel += OnMoveRightCancel;
            GameInput.Instance.OnBlockSoftDropAction += OnSoftDropPerformed;
            GameInput.Instance.OnBlockSoftDropCancel += OnSoftDropCanceled;
            GameInput.Instance.OnBlockHardDropAction += OnHardDrop;

            TetrisGameManager.Instance.OnGameStart += OnGameStart;
            TetrisGameManager.Instance.OnGameStop += OnGameStop;
            TetrisGameManager.Instance.OnGameResume += OnGameResume;

            GameManager.Instance.OnGameOver += OnGameStop;
        }

        private void OnDisable()
        {
            GameInput.Instance.OnRotateBlockLeftAction -= OnRotateBlockLeft;
            GameInput.Instance.OnRotateBlockRightAction -= OnRotateBlockRight;
            GameInput.Instance.OnMoveBlockLeftPerformed -= OnMoveLeftPerformed;
            GameInput.Instance.OnMoveBlockLeftCancel -= OnMoveLeftCancel;
            GameInput.Instance.OnMoveBlockRightPerformed -= OnMoveRightPerformed;
            GameInput.Instance.OnMoveBlockRightCancel -= OnMoveRightCancel;
            GameInput.Instance.OnBlockSoftDropAction -= OnSoftDropPerformed;
            GameInput.Instance.OnBlockSoftDropCancel -= OnSoftDropCanceled;
            GameInput.Instance.OnBlockHardDropAction -= OnHardDrop;
            
            TetrisGameManager.Instance.OnGameStart -= OnGameStart;
            TetrisGameManager.Instance.OnGameStop -= OnGameStop;
            TetrisGameManager.Instance.OnGameResume -= OnGameResume;
            
            GameManager.Instance.OnGameOver -= OnGameStop;
        }

        public void Initialize(Board board, Vector3Int position, TetrominoData data)
        {
            this.board = board;
            Position = position;
            this.data = data;

            this.rotationIndex = 0;
            this.stepTime = Time.time + this.stepDelay[currentLevel];
            moveTime = Time.time + moveDelay;
            this.lockTime = 0f;

            if (this.cells == null)
            {
                this.cells = new Vector3Int[data.cells.Length];
            }

            for (int i = 0; i < cells.Length; i++)
            {
                this.cells[i] = (Vector3Int)data.cells[i];
            }

            this.CreateBlocks();

            CheckCanMove(_moveDownVector);
        }

        private void Update()
        {
            if (_gameStop) return;

            this.lockTime += Time.deltaTime;

            if (this.currentLevel < this.stepDelay.Length - 1)
            {
                this.nextLevelTime += Time.deltaTime;
            
                if (nextLevelTime >= nextLevelDelay)
                {
                    currentLevel++;
                    nextLevelTime = 0f;
                }
            }
            
            if (Time.time >= this.stepTime)
            {
                Step();
            }

            if (Time.time >= moveTime)
            {
                if (movingLeft && Time.time >= moveLeftHoldingTime) Move(Vector2Int.left);
                if (movingRight && Time.time >= moveRightHoldingTime) Move(Vector2Int.right);
            }
        }

        private void OnGameStop()
        {
            this._gameStop = true;
        }

        private void OnGameStart()
        {
            this._gameStop = false;
        }

        private void OnGameResume()
        {
            this._gameStop = false;
        }

        private void OnRotateBlockLeft(object sender, EventArgs e)
        {
            // Debug.Log("Piece::OnRotateBlockLeft");
            if(_gameStop) return;
            Rotate(-1);
        }

        private void OnRotateBlockRight(object sender, EventArgs e)
        {
            // Debug.Log("Piece::OnRotateBlockRight");
            if(_gameStop) return;
            Rotate(1);
        }

        private void OnHardDrop(object sender, EventArgs e)
        {
            // Debug.Log("Piece::OnHardDrop at " + Time.time);
            if(_gameStop) return;
            HardDrop();
        }

        private void OnSoftDropPerformed(object sender, EventArgs e)
        {
            // Debug.Log("Piece::OnSoftDropPerformed");
            if(_gameStop) return;
            softDropEnabled = true;
            if (this.stepTime - Time.time > this.fastStepDelay) this.stepTime = Time.time + fastStepDelay;
        }

        private void OnSoftDropCanceled(object sender, EventArgs e)
        {
            // Debug.Log("Piece::OnSoftDropCanceled");
            if(_gameStop) return;
            softDropEnabled = false;
        }

        private void OnMoveLeftPerformed(object sender, EventArgs e)
        {
            if(_gameStop) return;
            movingLeft = true;
            Move(_moveLeftVector);
            moveLeftHoldingTime = Time.time + moveHoldingDelay;
        }

        private void OnMoveLeftCancel(object sender, EventArgs e)
        {
            if(_gameStop) return;
            movingLeft = false;
        }

        private void OnMoveRightPerformed(object sender, EventArgs e)
        {
            if(_gameStop) return;
            movingRight = true;
            Move(_moveRightVector);
            moveRightHoldingTime = Time.time + moveHoldingDelay;
        }
        
        private void OnMoveRightCancel(object sender, EventArgs e)
        {
            if(_gameStop) return;
            movingRight = false;
        }

        private void Step()
        {
            if (softDropEnabled) this.stepTime = Time.time + this.fastStepDelay;
            else this.stepTime = Time.time + this.stepDelay[currentLevel];
            Move(_moveDownVector);

            if (!CheckCanMove(_moveDownVector))
            {
                Lock();
            }
        }

        private void Lock(bool isHardDrop = false)
        {
            this.board.AddBlockToMatrix(this, isHardDrop);
            // foreach (Transform t in this.transform) Destroy(t.gameObject);
            this.board.SpawnPiece();
        }

        private void HardDrop()
        {
            while (Move(_moveDownVector))
            {
            }

            Lock(true);
        }

        private Block CreateBlock(Sprite sprite, Vector3Int position)
        {
            PoolManager.Get<PoolObject>(_blockPrefab, out var blockGO);
            blockGO.GetComponent<SpriteRenderer>().sprite = sprite;
            Vector3 realPosition = position + Vector3.one * 0.5f;
            blockGO.transform.parent = this.transform;
            blockGO.transform.localPosition = realPosition;
            blockGO.transform.rotation = Quaternion.identity;
            return blockGO.GetComponent<Block>();
        }

        private void CreateBlocks()
        {
            for (int i = 0; i < cells.Length; i++)
            {
                Block block = CreateBlock(data.sprite, cells[i]);
                this.blocks[i] = block;
            }
        }

        private bool CheckCanMove(Vector2Int translation)
        {
            Vector3Int newPosition = GetPositionAfterTranslation(Position, translation);

            return this.board.IsValidPosition(this, newPosition);
        }

        private Vector3Int GetPositionAfterTranslation(Vector3Int curPosition, Vector2Int translation)
        {
            Vector3Int newPosition = Vector3Int.RoundToInt(curPosition);
            newPosition.x += translation.x;
            newPosition.y += translation.y;

            return newPosition;
        }

        private bool Move(Vector2Int translation)
        {
            if (!CheckCanMove(translation)) return false;

            Vector3Int newPosition = GetPositionAfterTranslation(Position, translation);
            Position = newPosition;
            moveTime = Time.time + moveDelay;
            this.lockTime = 0f;
            return true;
        }

        private void Rotate(int direction)
        {
            // Debug.Log(this.cells[0] + " --- " + this.cells[1] + " --- " + this.cells[2] + " --- " + this.cells[3]);
            int maxRotationIndex = GetMaxRotationIndex(data.tetromino);
            if (maxRotationIndex == 4)
            {
                ApplyRotationMatrix(direction);
            }
            else if (maxRotationIndex == 2)
            {
                int originalRotation = this.rotationIndex;
                this.rotationIndex = Wrap(this.rotationIndex + direction, 0, GetMaxRotationIndex(data.tetromino));
                direction *= (direction * (this.rotationIndex - originalRotation) > 0 ? 1 : -1);
                ApplyRotationMatrix(direction);
            }
            else
            {
            }

            if (maxRotationIndex != 1 && !this.board.IsValidPosition(this, Position))
            {
                ApplyRotationMatrix(-direction);
            }

            // Debug.Log(this.cells[0] + " --- " + this.cells[1] + " --- " + this.cells[2] + " --- " + this.cells[3]);
            UpdateBlocksPosition();
        }

        private int GetMaxRotationIndex(Tetromino t)
        {
            switch (t)
            {
                case Tetromino.O:
                    return 1;
                case Tetromino.I:
                case Tetromino.Z:
                case Tetromino.S:
                    return 2;
                case Tetromino.J:
                case Tetromino.L:
                case Tetromino.T:
                    return 4;
            }

            return 1;
        }

        private void ApplyRotationMatrix(int direction)
        {
            float[] matrix = Data.RotationMatrix;

            for (int i = 0; i < this.cells.Length; i++)
            {
                // Vector3 cell = cells[i];
                // int x, y;
                // switch (data.tetromino)
                // {
                //     case Tetromino.I:
                //     case Tetromino.O:
                //         // "I" and "O" are rotated from an offset center point
                //         // cell.x -= 0.5f;
                //         // cell.y -= 0.5f;
                //         // x = Mathf.CeilToInt((cell.x * matrix[0] * direction) + (cell.y * matrix[1] * direction));
                //         // y = Mathf.CeilToInt((cell.x * matrix[2] * direction) + (cell.y * matrix[3] * direction));
                //         x = Mathf.RoundToInt((cell.x * matrix[0] * direction) + (cell.y * matrix[1] * direction));
                //         y = Mathf.RoundToInt((cell.x * matrix[2] * direction) + (cell.y * matrix[3] * direction));
                //         break;
                //
                //     default:
                //         x = Mathf.RoundToInt((cell.x * matrix[0] * direction) + (cell.y * matrix[1] * direction));
                //         y = Mathf.RoundToInt((cell.x * matrix[2] * direction) + (cell.y * matrix[3] * direction));
                //         break;
                // }
                int x = Mathf.RoundToInt((cells[i].x * matrix[0] * direction) + (cells[i].y * matrix[1] * direction));
                int y = Mathf.RoundToInt((cells[i].x * matrix[2] * direction) + (cells[i].y * matrix[3] * direction));

                cells[i] = new Vector3Int(x, y, 0);
            }
        }

        // private bool TestWallKicks(int rotationIndex, int rotationDirection)
        // {
            // int wallKickIndex = GetWallKickIndex(rotationIndex, rotationDirection);
            //
            // for (int i = 0; i < this.data.wallKicks.GetLength(1); i++)
            // {
            //     Vector2Int translation = this.data.wallKicks[wallKickIndex, i];
            //
            //     if (CheckCanMove(translation)) return true;
            // }
            //
            // return false;
        // }

        private void UpdateBlocksPosition()
        {
            int i = 0;
            foreach (Transform t in this.transform)
            {
                Vector3 realPosition = cells[i] + Vector3.one * 0.5f;
                t.localPosition = realPosition;
                i++;
            }
        }

        // private int GetWallKickIndex(int rotationIndex, int rotationDirection)
        // {
        //     int wallKickIndex = rotationIndex * 2;
        //
        //     if (rotationDirection < 0) wallKickIndex--;
        //
        //     return Wrap(wallKickIndex, 0, this.data.wallKicks.GetLength(0));
        // }

        private int Wrap(int input, int min, int max)
        {
            if (input < min)
            {
                return max - (min - input) % (max - min);
            }
            else
            {
                return min + (input - min) % (max - min);
            }
        }
    }
}