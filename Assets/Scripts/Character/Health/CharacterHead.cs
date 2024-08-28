using System;
using UnityEngine;

namespace Character.Health
{
    public class CharacterHead : MonoBehaviour
    {
        [SerializeField] private CharacterHealth _characterHealth;

        private bool deadTriggered = false;
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.tag.Equals("TetrisBlock"))
            {
                deadTriggered = true;
                _characterHealth.InstantDead();
            }
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if(!deadTriggered && other.gameObject.tag.Equals("TetrisBlock"))
            {
                deadTriggered = true;
                _characterHealth.InstantDead();
            }
        }
    }
}