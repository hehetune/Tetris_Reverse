using System.Collections.Generic;
using CustomCamera;
using Managers;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

namespace TetrisCore
{
    public class Board : MonoBehaviour
    {
        [SerializeField] private Piece _activePiece;
        public NextPiece nextPiece;
        public TetrominoData[] tetrominoes;

        private int highestYOfBLocks = 0;
        [SerializeField] private Transform spawnOffsetPointTransform;
        private Vector3Int GetSpawnOffset() => Vector3Int.RoundToInt(spawnOffsetPointTransform.position);
        private Vector3Int GetSpawnPoint => GetSpawnOffset() + Vector3Int.up * highestYOfBLocks;

        public int boardWidth = 12;
        [SerializeField] private int _xMin = 0;
        [SerializeField] private int _xMax = 0;
        [SerializeField] private int _yMin = 0;
        public int XMin => _xMin;
        public int XMax => _xMax;
        public int YMin => _yMin;
        public int YMax => highestYOfBLocks + Mathf.RoundToInt(spawnOffsetPointTransform.transform.position.y) + 1 + pieceYPrefix;

        private int currentPieceIndex = 0;
        private int nextPieceIndex = 0;
        [SerializeField] private List<BlockRow> _blockRows = new List<BlockRow>();

        private int pieceYPrefix = 1;

        private bool IsWithinBoard(Vector3Int pos)
        {
            return pos.x >= _xMin && pos.x < _xMax && pos.y >= _yMin && pos.y < YMax;
        }

        private void Awake()
        {
            _xMin = 0;
            _xMax = boardWidth;
            _yMin = 0;
            for (int i = 0; i < this.tetrominoes.Length; i++)
            {
                this.tetrominoes[i].Initialize();
            }

            for (int i = _yMin; i <= spawnOffsetPointTransform.position.y + 1 + pieceYPrefix; i++)
            {
                BlockRow row = new()
                {
                    cols = new Block[boardWidth]
                };
                // Block[] row = new Block[boardWidth];
                _blockRows.Add(row);
            }
        }

        private void OnEnable()
        {
            TetrisGameManager.Instance.OnGameStart += OnGameStart;
            TetrisGameManager.Instance.OnGameStop += OnGameStop;
            
            GameManager.Instance.OnGameOver += OnGameStop;
        }

        private void OnDisable()
        {
            TetrisGameManager.Instance.OnGameStart -= OnGameStart;
            TetrisGameManager.Instance.OnGameStop -= OnGameStop;
            
            GameManager.Instance.OnGameOver -= OnGameStop;
        }

        // private void Update()
        // {
        //     
        // }

        private void IncreaseRowNumber()
        {
            int rowsToAdd = Mathf.CeilToInt(highestYOfBLocks + spawnOffsetPointTransform.transform.position.y + 1 + pieceYPrefix) - _blockRows.Count;

            if (rowsToAdd > 0)
            {
                for (int i = 0; i < rowsToAdd; i++)
                {
                    _blockRows.Add(new()
                    {
                        cols = new Block[boardWidth]
                    });
                }
            }
        }

        public bool IsValidPosition(Piece piece, Vector3Int position, bool log = false)
        {
            for (int i = 0; i < piece.cells.Length; i++)
            {
                Vector3Int tilePosition = piece.cells[i] + position;
                if (!IsWithinBoard(tilePosition))
                {
                    if(log) Debug.LogError("!IsWithinBoard(tilePosition) " + tilePosition);
                    return false;
                }

                if (HasBlock(tilePosition))
                {
                    if(log) Debug.LogError("HasBlock(tilePosition)");
                    return false;
                }
            }

            return true;
        }

        private bool _isGameStarted = false;
        private void OnGameStart()
        {
            if (_isGameStarted) return;
            nextPieceIndex = Random.Range(0, tetrominoes.Length);
            SpawnPiece();
            _isGameStarted = true;
        }

        private void OnGameStop()
        {
            _isGameStarted = false;
        }

        private void GetNextRandomPieceIndex()
        {
            currentPieceIndex = nextPieceIndex;
            nextPieceIndex = Random.Range(0, tetrominoes.Length);
            nextPiece.ChangePiece(tetrominoes[nextPieceIndex].cells, tetrominoes[nextPieceIndex].sprite);
        }

        public void AddBlockToMatrix(Piece piece, bool isHardDrop = false)
        {
            CameraShake.Instance.ShakeCamera(isHardDrop ? 3f : 1f, 0.2f);
            int minY = int.MaxValue, maxY = int.MinValue;
            for (int i = 0; i < piece.blocks.Length; i++)
            {
                piece.blocks[i].transform.parent = this.transform;
                int y = piece.cells[i].y + piece.Position.y, x = piece.cells[i].x + piece.Position.x;
                _blockRows[y].cols[x] = piece.blocks[i];
                piece.blocks[i] = null;

                highestYOfBLocks = Mathf.Max(highestYOfBLocks, piece.Position.y + piece.cells[i].y);
                minY = Mathf.Min(minY, y);
                maxY = Mathf.Max(maxY, y);
            }
            Debug.LogError($"AddBlockToMatrix minY: ${minY}, maxY: ${maxY}");
            ClearLines(minY, maxY);
        }

        public void SpawnPiece()
        {
            IncreaseRowNumber();
            GetNextRandomPieceIndex();
            TetrominoData data = this.tetrominoes[currentPieceIndex];

            this._activePiece.Initialize(this, GetSpawnPoint, data);
        }

        public void Clear(Piece piece)
        {
            for (int i = 0; i < piece.cells.Length; i++)
            {
                Vector3Int cellPosition = piece.cells[i] + piece.Position;
                DestroyBlock(cellPosition);
            }
        }

        public void ClearLines(int startRow, int endRow)
        {
            int row = startRow;

            // if (IsLineFull(row)) LineClear(row);
            int linesClear = 0;
            while (row < endRow)
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

            if (linesClear == 0) return;

            row = startRow;
            int emptyRows = 0;
            while (row <= highestYOfBLocks)
            {
                bool isLineEmpty = true;
                for (int col = _xMin; col < _xMax; col++)
                {
                    if (_blockRows[row].cols[col])
                    {
                        isLineEmpty = false;
                        MoveBlock(col, row, col, row - emptyRows);
                    }
                }

                if (isLineEmpty) emptyRows++;
                row++;
            }
            // if (linesClear > 0) TetrisGameManager.Instance.IncreaseScore(linesClear);
        }

        private bool IsLineEmpty(int row)
        {
            for (int col = _xMin; col < _xMax; col++)
            {
                Vector3Int position = new Vector3Int(col, row, 0);

                if (HasBlock(position))
                {
                    return false;
                }
            }

            return true;
        }

        private bool IsLineFull(int row)
        {
            for (int col = _xMin; col < _xMax; col++)
            {
                Vector3Int position = new Vector3Int(col, row, 0);

                if (!HasBlock(position))
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
                DestroyBlock(position);
            }
            
            Debug.Log($"Board::LineClear start row: ${row}, highestRow: ${highestYOfBLocks}");

            
        }

        private void DestroyBlock(Vector3Int position)
        {
            Block block = _blockRows[position.y].cols[position.x];
            if (block) block.GetComponent<PoolObject>().ReturnToPool();
            _blockRows[position.y].cols[position.x] = null;
        }

        private bool HasBlock(Vector3Int position)
        {
            return _blockRows[position.y].cols[position.x] != null;
        }

        private void MoveBlock(int fromCol, int fromRow, int toCol, int toRow) //Vector3Int fromPosition, Vector3Int toPosition
        {
            Block child = _blockRows[fromRow].cols[fromCol];
            if (child != null)
            {
                Debug.LogError($"Board::MoveBlock fromCol: ${fromCol}, fromRow: ${fromRow}, toCol: ${toCol}, toRow: ${toRow}");
                child.transform.position = new Vector3(toCol + 0.5f, toRow + 0.5f, 0f);
            }

            _blockRows[toRow].cols[toCol] = child;
            _blockRows[fromRow].cols[fromCol] = null;
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