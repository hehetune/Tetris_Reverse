using System;
using System.Collections;
using TetrisCore;
using UnityCommunity.UnitySingleton;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Managers
{
    public enum GameState
    {
        DEFAULT,
        WAIT_TO_STATRT,
        COUNTDOWN_TO_START,
        PLAYING,
        PAUSED,
        GAMEOVER,
    }
    public class GameManager : MonoSingleton<GameManager>
    {
        public event EventHandler OnStateChanged;
        public event EventHandler OnGamePaused;
        public event EventHandler OnGameUnpaused;

        public event EventHandler OnGameOver;
        
        private GameState _gameState = GameState.DEFAULT;
        public GameState GameState => this._gameState;

        [SerializeField] private float _countdownTime = 3f;
        [SerializeField] private float _waitingToStartTimer = 1f;
        [SerializeField] private float _countdownToStartTimer = 3f;
        private void Start()
        {
            this._gameState = GameState.WAIT_TO_STATRT;
            StartCountDown();
        }

        private void Update()
        {
            switch (_gameState)
            {
                case GameState.WAIT_TO_STATRT:
                    _waitingToStartTimer -= Time.deltaTime;
                    if (_waitingToStartTimer < 0f)
                    {
                        _gameState = GameState.COUNTDOWN_TO_START;
                        OnStateChanged?.Invoke(this, EventArgs.Empty);
                    }
                    break;
                case GameState.COUNTDOWN_TO_START:
                    _countdownToStartTimer -= Time.deltaTime;
                    if (_countdownToStartTimer < 0f)
                    {
                        _gameState = GameState.PLAYING;
                        OnStateChanged?.Invoke(this, EventArgs.Empty);
                    }
                    break;
                case GameState.PLAYING: break;
                case GameState.PAUSED: break;
                case GameState.GAMEOVER: break;
                case GameState.DEFAULT: break;
                default: break;
            }
        }
        
        public void ReloadScene()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        private void StartCountDown()
        {
            StartCoroutine(StartCountdownCoroutine());
        }
        private IEnumerator StartCountdownCoroutine()
        {
            yield return _countdownTime.Wait();
            _gameState = GameState.PLAYING;
        }

        public void StartGame()
        {
            TetrisGameManager.Instance.StartGame();
        }

        public void StopGame()
        {
            TetrisGameManager.Instance.StopGame();
        }

        public void ResumeGame()
        {
            TetrisGameManager.Instance.ResumeGame();
        }
    }
}
