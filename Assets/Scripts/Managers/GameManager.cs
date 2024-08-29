using System;
using System.Collections;
using TetrisCore;
using TMPro;
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
        public Action OnStateChanged;
        public Action OnGamePaused;
        public Action OnGameUnpaused;

        public Action OnGameStart;
        public Action OnGameOver;
        
        private GameState _gameState = GameState.DEFAULT;
        public GameState GameState => this._gameState;

        [SerializeField] private float _countdownTime = 3f;
        [SerializeField] private float _waitingToStartTimer = .5f;
        [SerializeField] private float _countdownToStartTimer = 3f;

        protected override void Awake()
        {
            base.Awake();
            OnGameOver += () => UIManager.Instance.ToggleGameOverUI(true);
        }

        private void Start()
        {
            this._gameState = GameState.WAIT_TO_STATRT;
            // StartCountDown();
        }
        
        private void OnEnable()
        {
            GameInput.Instance.OnPauseAction += TogglePauseGame;
        }

        private void OnDisable()
        {
            GameInput.Instance.OnPauseAction -= TogglePauseGame;
        }

        private void Update()
        {
            switch (_gameState)
            {
                case GameState.WAIT_TO_STATRT:
                    _waitingToStartTimer -= Time.deltaTime;
                    if (_waitingToStartTimer < 0f)
                    {
                        UIManager.Instance.ToggleCountdownText(true);
                        _gameState = GameState.COUNTDOWN_TO_START;
                        OnStateChanged?.Invoke();
                    }
                    break;
                case GameState.COUNTDOWN_TO_START:
                    _countdownToStartTimer -= Time.deltaTime;
                    UIManager.Instance.UpdateCountdownText(Mathf.CeilToInt(_countdownToStartTimer).ToString());
                    if (_countdownToStartTimer < 0f)
                    {
                        UIManager.Instance.ToggleCountdownText(false);
                        _gameState = GameState.PLAYING;
                        OnStateChanged?.Invoke();
                        StartGame();
                    }
                    break;
                case GameState.PLAYING: break;
                case GameState.PAUSED: break;
                case GameState.GAMEOVER: break;
                case GameState.DEFAULT: break;
                default: break;
            }
        }

        public bool IsGameOver() => _gameState == GameState.GAMEOVER;
        public bool IsGamePlaying() => _gameState == GameState.PLAYING;
        
        public void ReloadScene()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        // private void StartCountDown()
        // {
        //     StartCoroutine(StartCountdownCoroutine());
        // }
        // private IEnumerator StartCountdownCoroutine()
        // {
        //     yield return _countdownTime.Wait();
        //     _gameState = GameState.PLAYING;
        // }

        public void StartGame()
        {
            if (_gameState != GameState.PLAYING) return;
            TetrisGameManager.Instance.StartGame();
            OnGameStart?.Invoke();
        }

        private void TogglePauseGame(object sender, EventArgs e)
        {
            if (_gameState != GameState.PLAYING && _gameState != GameState.PAUSED) return;
            bool isPaused = _gameState == GameState.PAUSED;
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                StopGame();
            }
        }

        public void StopGame()
        {
            _gameState = GameState.PAUSED;
            TetrisGameManager.Instance.StopGame();
            UIManager.Instance.TogglePauseMenu(true);
        }

        public void ResumeGame()
        {
            _gameState = GameState.PLAYING;
            TetrisGameManager.Instance.ResumeGame();
            UIManager.Instance.TogglePauseMenu(false);
        }

        public void GameOver()
        {
            OnGameOver?.Invoke();
        }

        public void ReturnToMenu()
        {
            Loader.Load(Loader.Scene.MainMenu);
        }
    }
}
