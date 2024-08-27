using System;
using UnityEngine;
using UnityEngine.UI;

namespace Managers
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private Button reloadSceneBtn;
        [SerializeField] private Button startGameSceneBtn;
        [SerializeField] private Button stopGameSceneBtn;
        [SerializeField] private Button resumeGameSceneBtn;

        private void Start()
        {
            reloadSceneBtn.onClick.AddListener(() => GameManager.Instance.ReloadScene());
            startGameSceneBtn.onClick.AddListener(() => GameManager.Instance.StartGame());
            stopGameSceneBtn.onClick.AddListener(() => GameManager.Instance.StopGame());
            resumeGameSceneBtn.onClick.AddListener(() => GameManager.Instance.ResumeGame());
        }
    }
}