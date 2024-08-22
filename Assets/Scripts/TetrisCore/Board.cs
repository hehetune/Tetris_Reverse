using System.Collections.Generic;
using Managers;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace TetrisCore
{
    public class Board : MonoBehaviour
    {
        [SerializeField] private Piece _activePiece;
        public NextPiece nextPiece;
        public TetrominoData[] tetrominoes;

        public Vector3Int spawnPosition = new Vector3Int(0, 7, 0);
        public int boardWidth = 12;
        private int currentSpawnHeight = 0;
        private int _xMin = 0;
        private int _xMax = 0;
        private int _yMin = 0;
        private int _yMax = 0;
        public int XMin => _xMin;
        public int XMax => _xMax;
        public int YMin => _yMin;
        public int YMax => _yMax;

        private int currentPieceIndex = 0;
        private int nextPieceIndex = 0;

        [SerializeField] private Prefab _blockPrefab;

        private bool IsWithinBoard(int x) => x >= _xMin && x <= _xMax;

        private List<Block[]> _blockRows = new List<Block[]>();

        private void Awake()
        {
            _xMin = -boardWidth / 2;
            _xMax = boardWidth / 2;
            _yMax = currentSpawnHeight;
            for (int i = 0; i < this.tetrominoes.Length; i++)
            {
                this.tetrominoes[i].Initialize();
            }
        }
        
        public bool IsValidPosition(Piece piece, Vector3Int position)
        {
            for (int i = 0; i < piece.cells.Length; i++)
            {
                Vector3Int tilePosition = piece.cells[i] + position;

                if (!IsWithinBoard(tilePosition.x)) return false;

                if (HasSprite(position)) return false;
            }

            return true;
        }

        public void StartGame()
        {
            nextPieceIndex = Random.Range(0, tetrominoes.Length);
            SpawnPiece();
        }

        private void GetNextRandomPieceIndex()
        {
            currentPieceIndex = nextPieceIndex;
            nextPieceIndex = Random.Range(0, tetrominoes.Length);
            nextPiece.ChangePiece(tetrominoes[nextPieceIndex].cells, tetrominoes[nextPieceIndex].sprite);
        }

        public void SpawnPiece()
        {
            GetNextRandomPieceIndex();
            TetrominoData data = this.tetrominoes[currentPieceIndex];

            this._activePiece.Initialize(this, spawnPosition, data);

            Set(this._activePiece);
        }

        public void Set(Piece piece)
        {
            for (int i = 0; i < piece.cells.Length; i++)
            {
                Vector3Int cellPosition = piece.cells[i] + piece.position;
                CreateSprite(piece.data.sprite, cellPosition);
            }
        }

        public void Clear(Piece piece)
        {
            for (int i = 0; i < piece.cells.Length; i++)
            {
                Vector3Int cellPosition = piece.cells[i] + piece.position;
                DestroySprite(cellPosition);
            }
        }

        public void ClearLines()
        {
            int row = _yMin;

            int linesClear = 0;

            while (row < _yMax)
            {
                if (IsLineFull(row))
                {
                    LineClear(row);
                    linesClear++;
                }
                else
                {
                    row++;
                }
            }

            // if (linesClear > 0) TetrisGameManager.Instance.IncreaseScore(linesClear);
        }

        private bool IsLineFull(int row)
        {
            for (int col = _xMin; col < _xMax; col++)
            {
                Vector3Int position = new Vector3Int(col, row, 0);

                if (!HasSprite(position))
                {
                    return false;
                }
            }

            return true;
        }

        private void LineClear(int row)
        {
            for (int col = _xMin; col < _xMax; col++)
            {
                Vector3Int position = new Vector3Int(col, row, 0);
                DestroySprite(position);
            }

            while (row < _yMax)
            {
                for (int col = _xMin; col < _xMax; col++)
                {
                    Vector3Int abovePosition = new Vector3Int(col, row + 1, 0);
                    Vector3Int currentPosition = new Vector3Int(col, row, 0);

                    MoveSprite(abovePosition, currentPosition);
                }

                row++;
            }
        }

        private void CreateSprite(Sprite sprite, Vector3Int position)
        {
            PoolManager.Get<PoolObject>(_blockPrefab, out var blockGO);
            blockGO.GetComponent<SpriteRenderer>().sprite = sprite;
            blockGO.transform.position = position;
            blockGO.transform.rotation = Quaternion.identity;
            // blockGO.name = $"Block_{position.x}_{position.y}";
        }

        private void DestroySprite(Vector3Int position)
        {
            Block block = _blockRows[position.y][position.x];
            if (block) block.GetComponent<PoolObject>().ReturnToPool();
            // Transform child = transform.Find($"Block_{position.x}_{position.y}");
            // if (child != null)
            // {
            //     Destroy(child.gameObject);
            // }
        }

        private bool HasSprite(Vector3Int position)
        {
            return _blockRows[position.y][position.x] != null;
        }

        private void MoveSprite(Vector3Int fromPosition, Vector3Int toPosition)
        {
            Block child = _blockRows[fromPosition.y][fromPosition.x];
            if (child != null)
            {
                child.transform.position = toPosition;
            }
        }

        private void ClearBoard()
        {
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }
        }
    }
}