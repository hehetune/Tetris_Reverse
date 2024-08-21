using System;
using UnityEngine;
using UnityEngine.UI;

namespace UI.MainMenu
{
    public class MainMenuUI : MonoBehaviour
    {
        [SerializeField] private Button _playButton;
        [SerializeField] private Button _settingButton;
        [SerializeField] private Button _quitButton;

        [SerializeField] private GameObject _settingPanel;

        private void Awake()
        {
            _playButton.onClick.AddListener(Play);
            _settingButton.onClick.AddListener(SettingOnClick);
            _quitButton.onClick.AddListener(QuitOnClick);
        }

        private void Play()
        {
            Loader.Load(Loader.Scene.GameScene);
        }

        private void SettingOnClick()
        {
            _settingPanel.SetActive(true);
        }

        private void QuitOnClick()
        {
            Application.Quit();
        }
    }
}
