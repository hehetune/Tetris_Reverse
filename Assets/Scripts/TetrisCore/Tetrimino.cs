﻿using UnityEngine;

namespace TetrisCore
{
    public enum Tetromino
    {
        I,
        O,
        T,
        J,
        L,
        S,
        Z,
    }

    [System.Serializable]
    public struct TetrominoData
    {
        public Tetromino tetromino;
        public Sprite sprite;
        public Vector2Int[] cells { get; private set; }
        public Vector2Int[,] wallKicks { get; private set; }

        public void Initialize()
        {
            cells = Data.Cells[this.tetromino];
            wallKicks = Data.WallKicks[tetromino];
        }
    }
}