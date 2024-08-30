using System;
using Managers;
using UnityEngine;

namespace UI.GameHUD
{
    public class PlayerScoreUpdate : MonoBehaviour
    {
        private int highestScore = 0;

        private void OnEnable()
        {
            GameManager.Instance.OnGameOver += OnGameOver;
        }

        private void OnDisable()
        {
            GameManager.Instance.OnGameOver -= OnGameOver;
        }

        private void FixedUpdate()
        {
            int score = Mathf.FloorToInt(transform.position.y);
            UIManager.Instance.UpdateHeightText(Mathf.Max(0, score));
            highestScore = Mathf.Max(highestScore, score);
        }

        private void OnGameOver()
        {
            int saveScore = PlayerPrefs.GetInt("HighestScore");
            if(highestScore > saveScore) PlayerPrefs.SetInt("HighestScore", highestScore);
        }
    }
}