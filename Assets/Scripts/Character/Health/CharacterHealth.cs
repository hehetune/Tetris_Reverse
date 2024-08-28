using System;
using System.Collections;
using Managers;
using UnityEngine;

namespace Character.Health
{
    public class CharacterHealth : MonoBehaviour
    {
        private int _hp;
        public int Hp => _hp;
        public int maxHp;

        private bool _isImmortal = false;
        public float immortalTimeAfterHit = 1f;

        public Action OnHit;
        public Action OnDie;
        public Action OnHealthChange;

        private void OnEnable()
        {
            this._hp = this.maxHp;

            this.OnHit += StartImmortal;
        }

        public void IncreaseHp(int amount)
        {
            if (this._hp == this.maxHp) return;
            this._hp += amount;
            if (this._hp > this.maxHp) this._hp = this.maxHp;
            OnHealthChange?.Invoke();
        }

        public void DecreaseHp(int amount)
        {
            Debug.LogError($"CharacterHealth::DecreaseHp --- ${amount}");
            if (this._isImmortal || this._hp == 0) return;
            this._hp -= amount;
            if (this._hp < 0) this._hp = 0;
            if (this._hp == 0) OnDie?.Invoke();
            else
            {
                StartImmortal();
                OnHit?.Invoke();
            }
            OnHealthChange?.Invoke();
        }

        private void StartImmortal()
        {
            if (_immortalCoroutine != null) StopCoroutine(_immortalCoroutine);
            _immortalCoroutine = StartCoroutine(ImmortalCoroutine());
        }

        private Coroutine _immortalCoroutine;

        private IEnumerator ImmortalCoroutine()
        {
            this._isImmortal = true;
            yield return immortalTimeAfterHit.Wait();
            this._isImmortal = false;
            _immortalCoroutine = null;
        }

        public void InstantDead()
        {
            StopAllCoroutines();
            this._isImmortal = false;
            this._hp = 0;
            this.OnDie?.Invoke();
        }
    }
}