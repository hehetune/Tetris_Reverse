using System;
using UnityCommunity.UnitySingleton;
using UnityEngine;
using UnityEngine.UI;

namespace TetrisCore
{
    public class TetrisGameManager : MonoSingleton<TetrisGameManager>
    {
        public Board mainBoard;

        public GameObject gameStartUI;
        public GameObject gameOverUI;

        public Action OnGameStart;
        public Action OnGameStop;
        public Action OnGameResume;

        public void StartGame()
        {
            OnGameStart?.Invoke();
        }

        public void StopGame()
        {
            OnGameStop?.Invoke();
        }

        public void ResumeGame()
        {
            OnGameResume?.Invoke();
        }
    }
}