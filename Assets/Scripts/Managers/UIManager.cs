using System;
using TMPro;
using UnityCommunity.UnitySingleton;
using UnityEngine;
using UnityEngine.UI;

namespace Managers
{
    public class UIManager : MonoSingleton<UIManager>
    {
        [Header("Debug")]
        [SerializeField] private Button reloadSceneBtn;
        [SerializeField] private Button startGameSceneBtn;
        [SerializeField] private Button stopGameSceneBtn;
        [SerializeField] private Button resumeGameSceneBtn;

        [Header("Game Over")] [SerializeField] private Button gameOverBtn;
        [SerializeField] private GameObject gameOverUI;
        [SerializeField] private TextMeshProUGUI highestScoreTxt;
        [SerializeField] private TextMeshProUGUI scoreTxt;
        
        [Header("Pause Menu")]
        [SerializeField] private GameObject pauseMenuUI;
        [SerializeField] private Button resumeBtn_PauseUI;
        [SerializeField] private Button restartBtn_PauseUI;
        [SerializeField] private Button returnMenuBtn_PauseUI;

        [Header("HUD")] [SerializeField] private TextMeshProUGUI countdownText;
        [SerializeField] private TextMeshProUGUI heightText;
        [SerializeField] private Button pauseBtn;

        private void Start()
        {
            reloadSceneBtn.onClick.AddListener(() => GameManager.Instance.ReloadScene());
            startGameSceneBtn.onClick.AddListener(() => GameManager.Instance.StartGame());
            stopGameSceneBtn.onClick.AddListener(() => GameManager.Instance.StopGame());
            resumeGameSceneBtn.onClick.AddListener(() => GameManager.Instance.ResumeGame());

            gameOverBtn.onClick.AddListener(() => GameManager.Instance.ReloadScene());
            
            resumeBtn_PauseUI.onClick.AddListener(() => GameManager.Instance.ResumeGame());
            restartBtn_PauseUI.onClick.AddListener(() => GameManager.Instance.ReloadScene());
            returnMenuBtn_PauseUI.onClick.AddListener(() => GameManager.Instance.ReturnToMenu());
            
            pauseBtn.onClick.AddListener(() => GameManager.Instance.OnPauseClick());
        }

        public void ToggleGameOverUI(bool show, int score = 0, int highestScore = 0)
        {
            gameOverUI.SetActive(show);
            if (show)
            {
                highestScoreTxt.text = highestScore.ToString();
                scoreTxt.text = score.ToString();
            }
        }

        public void TogglePauseMenu(bool show)
        {
            pauseMenuUI.SetActive(show);
        }

        public void UpdateCountdownText(string text)
        {
            countdownText.text = text;
        }

        public void ToggleCountdownText(bool show)
        {
            countdownText.gameObject.SetActive(show);
        }

        public void UpdateHeightText(int height)
        {
            heightText.text = "Height\n" + height;
        }
    }
}