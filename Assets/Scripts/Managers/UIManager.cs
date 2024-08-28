using System;
using UnityCommunity.UnitySingleton;
using UnityEngine;
using UnityEngine.UI;

namespace Managers
{
    public class UIManager : MonoSingleton<UIManager>
    {
        [SerializeField] private Button reloadSceneBtn;
        [SerializeField] private Button startGameSceneBtn;
        [SerializeField] private Button stopGameSceneBtn;
        [SerializeField] private Button resumeGameSceneBtn;

        [Header("Game Over")] [SerializeField] private Button gameOverBtn;

        [SerializeField] private GameObject gameOverUI;

        private void Start()
        {
            reloadSceneBtn.onClick.AddListener(() => GameManager.Instance.ReloadScene());
            startGameSceneBtn.onClick.AddListener(() => GameManager.Instance.StartGame());
            stopGameSceneBtn.onClick.AddListener(() => GameManager.Instance.StopGame());
            resumeGameSceneBtn.onClick.AddListener(() => GameManager.Instance.ResumeGame());

            gameOverBtn.onClick.AddListener(() => GameManager.Instance.ReloadScene());
        }

        public void ToggleGameOverUI(bool show)
        {
            gameOverUI.SetActive(show);
        }
    }
}