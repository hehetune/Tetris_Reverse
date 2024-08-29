using System;
using System.Collections.Generic;
using Character.Health;
using UnityEngine;
using UnityEngine.UI;

namespace UI.GameHUD
{
    public class HeartDisplay : MonoBehaviour
    {
        [SerializeField] private CharacterHealth _characterHealth;

        [SerializeField] private List<Image> hearts;

        [SerializeField] private Sprite heartSprite;
        [SerializeField] private Sprite emptyHeartSprite;

        private void OnEnable()
        {
            _characterHealth.OnHealthChange += OnHealthChange;
        }

        private void OnDisable()
        {
            _characterHealth.OnHealthChange -= OnHealthChange;
        }

        private void OnHealthChange()
        {
            for (int i = 0; i < hearts.Count; i++)
            {
                hearts[i].sprite = i < _characterHealth.Hp ? heartSprite : emptyHeartSprite;
            }
        }
    }
}