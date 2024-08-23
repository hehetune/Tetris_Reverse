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

        private Vector3Int position;
        public Vector3Int Position
        {
            get => position;
            private set
            {
                position = value;
                transform.position = position;
            }
        }

        public int rotationIndex { get; private set; }

        private float[] stepDelay = new float[] { 0.8f, 0.75f, 0.70f, 0.65f, 0.6f, 0.55f, 0.5f, 0.45f, 0.4f };
        public float moveDelay = 0.1f;
        public float nextLevelDelay = 5f;

        private float stepTime;
        private float lockTime;
        private float moveTime;
        private float nextLevelTime;

        private int currentLevel = 0;

        // private bool _cacheCanMove = false;
        // private Vector3Int _cacheNewPosition;
        private Vector2Int _moveVector = Vector2Int.down;

        public Block[] blocks = new Block[4];
        [SerializeField] private Prefab _blockPrefab;

        private bool _isSoftDropEnabled = false;

        private void OnEnable()
        {
            GameInput.Instance.OnRotateBlockLeftAction += OnRotateBlockLeft;
            GameInput.Instance.OnRotateBlockRightAction += OnRotateBlockRight;
            GameInput.Instance.OnMoveBlockLeftAction += OnMoveLeft;
            GameInput.Instance.OnMoveBlockRightAction += OnMoveRight;
            GameInput.Instance.OnDropBlockFastAction += OnSoftDrop;
            // GameInput.Instance.OnDropBlockFastCancel += OnRotateBlockLeft;
        }

        private void OnDisable()
        {
            GameInput.Instance.OnRotateBlockLeftAction -= OnRotateBlockLeft;
            GameInput.Instance.OnRotateBlockRightAction -= OnRotateBlockRight;
            GameInput.Instance.OnMoveBlockLeftAction -= OnMoveLeft;
            GameInput.Instance.OnMoveBlockRightAction -= OnMoveRight;
            GameInput.Instance.OnDropBlockFastAction -= OnSoftDrop;
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

            CheckCanMove(_moveVector);
        }

        private void Update()
        {
            if (!TetrisGameManager.Instance.isGameStarted) return;

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
        }

        private void OnRotateBlockLeft(object sender, EventArgs e)
        {
            Rotate(-1);
        }

        private void OnRotateBlockRight(object sender, EventArgs e)
        {
            Rotate(1);
        }

        private void OnHardDrop(object sender, EventArgs e)
        {
            HardDrop();
        }

        private void OnSoftDrop(object sender, EventArgs e)
        {
            _isSoftDropEnabled = true;
        }

        private void OnMoveLeft(object sender, EventArgs e)
        {
            if (Time.time <= moveTime) return;
            Move(Vector2Int.left);
        }

        private void OnMoveRight(object sender, EventArgs e)
        {
            if (Time.time <= moveTime) return;
            Move(Vector2Int.right);
        }

        private void Step()
        {
            this.stepTime = Time.time + this.stepDelay[currentLevel];
            Move(_moveVector);

            if (!CheckCanMove(_moveVector)) Lock();
        }

        private void Lock()
        {
            // this.board.Set(this);
            // this.board.ClearLines();
            this.board.AddBlockToMatrix(this);
            this.board.SpawnPiece();
        }

        private void HardDrop()
        {
            while(CheckCanMove(_moveVector)) Move(_moveVector);

            Lock();
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
            // blockGO.name = $"Block_{position.x}_{position.y}";
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
            newPosition.x += translation.x;
            newPosition.y += translation.y;

            return this.board.IsValidPosition(this, newPosition);
        }

        private Vector3Int GetPositionAfterTranslation(Vector3Int curPosition, Vector2Int translation)
        {
            Vector3Int newPosition = Vector3Int.RoundToInt(transform.position);
            newPosition.x += translation.x;
            newPosition.y += translation.y;

            return newPosition;
        }

        private void Move(Vector2Int translation)
        {
            if (CheckCanMove(translation))
            {
                Vector3Int newPosition = GetPositionAfterTranslation(Position, translation);
                Position = newPosition;
                moveTime = Time.time + moveDelay;
                this.lockTime = 0f;
            }
        }

        private void Rotate(int direction)
        {
            int originalRotation = this.rotationIndex;
            this.rotationIndex = Wrap(this.rotationIndex + direction, 0, 4);

            ApplyRotationMatrix(direction);

            if (!TestWallKicks(this.rotationIndex, direction))
            {
                this.rotationIndex = originalRotation;
                ApplyRotationMatrix(-direction);
            }
        }

        private void ApplyRotationMatrix(int direction)
        {
            float[] matrix = Data.RotationMatrix;

            for (int i = 0; i < this.cells.Length; i++)
            {
                Vector3 cell = cells[i];

                int x, y;

                switch (data.tetromino)
                {
                    case Tetromino.I:
                    case Tetromino.O:
                        // "I" and "O" are rotated from an offset center point
                        cell.x -= 0.5f;
                        cell.y -= 0.5f;
                        x = Mathf.CeilToInt((cell.x * matrix[0] * direction) + (cell.y * matrix[1] * direction));
                        y = Mathf.CeilToInt((cell.x * matrix[2] * direction) + (cell.y * matrix[3] * direction));
                        break;

                    default:
                        x = Mathf.RoundToInt((cell.x * matrix[0] * direction) + (cell.y * matrix[1] * direction));
                        y = Mathf.RoundToInt((cell.x * matrix[2] * direction) + (cell.y * matrix[3] * direction));
                        break;
                }

                cells[i] = new Vector3Int(x, y, 0);
            }
        }

        private bool TestWallKicks(int rotationIndex, int rotationDirection)
        {
            int wallKickIndex = GetWallKickIndex(rotationIndex, rotationDirection);

            for (int i = 0; i < this.data.wallKicks.GetLength(1); i++)
            {
                Vector2Int translation = this.data.wallKicks[wallKickIndex, i];

                if (CheckCanMove(translation)) return true;
            }

            return false;
        }

        private int GetWallKickIndex(int rotationIndex, int rotationDirection)
        {
            int wallKickIndex = rotationIndex * 2;

            if (rotationDirection < 0) wallKickIndex--;

            return Wrap(wallKickIndex, 0, this.data.wallKicks.GetLength(0));
        }

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