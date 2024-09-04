using System;
using UnityEngine;
using UnityEngine.UI;

namespace UI.MainMenu
{
    public class MainMenuUI : MonoBehaviour
    {
        [SerializeField] private Button _playButton;
        [SerializeField] private Button _htpButton;
        [SerializeField] private Button _quitButton;

        [SerializeField] private GameObject _htpPanel;

        private void Awake()
        {
            _playButton.onClick.AddListener(Play);
            _htpButton.onClick.AddListener(HtpOnClick);
            _quitButton.onClick.AddListener(QuitOnClick);
        }

        private void Play()
        {
            Loader.Load(Loader.Scene.GameScene);
        }

        private void HtpOnClick()
        {
            _htpPanel.SetActive(true);
            this.gameObject.SetActive(false);
        }

        private void QuitOnClick()
        {
            Application.Quit();
        }

        public void OnBack()
        {
            _htpPanel.SetActive(false);
            this.gameObject.SetActive(true);
        }
    }
}
