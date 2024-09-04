using System;
using System.Collections;
using Character.Health;
using Managers;
using UnityEngine;

namespace Damage
{
    public class TriggerSendDamage : MonoBehaviour
    {
        public int hpDecreaseAmount;
        private CharacterHealth _characterHealth;
        private Coroutine _sendDamageCoroutine;

        // private void OnEnable()
        // {
        //     GameManager.Instance.OnGamePaused
        // }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.tag.Equals("Health"))
            {
                Debug.LogError("Trigger Send Damage");
                _characterHealth = other.gameObject.GetComponent<CharacterHealth>();
                if (_sendDamageCoroutine != null) StopCoroutine(_sendDamageCoroutine);
                _sendDamageCoroutine = StartCoroutine(SendDamageCoroutine());
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.gameObject.tag.Equals("Health"))
            {
                Debug.LogError("Exit Trigger Send Damage");
                _characterHealth = null;
            }
        }

        private IEnumerator SendDamageCoroutine()
        {
            while (_characterHealth != null)
            {
                _characterHealth.DecreaseHp(hpDecreaseAmount);
                yield return _characterHealth.immortalTimeAfterHit.Wait();
            }

            _sendDamageCoroutine = null;
        }
    }
}