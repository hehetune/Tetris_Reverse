using System;
using System.Collections;
using Character.Health;
using Character.Movement;
using Managers;
using UnityEngine;

namespace Character.Animation
{
    public class CharacterAnimation : MonoBehaviour
    {
        [Header("Cache Components")] [SerializeField]
        private SpriteRenderer _sprite;

        [SerializeField] private CharacterMovement _characterMovement;
        [SerializeField] private Animator _animator;
        [SerializeField] private Rigidbody2D _rigidbody;

        private int _runningAnimIndex = Animator.StringToHash("running");
        private int _groundedAnimIndex = Animator.StringToHash("grounded");
        private int _appearAnimIndex = Animator.StringToHash("appear");
        private int _disappearAnimIndex = Animator.StringToHash("disappear");
        private int _hitAnimIndex = Animator.StringToHash("hit");
        private int _yVeloAnimIndex = Animator.StringToHash("yVelo");

        private bool _flipX;
        private Vector2 MoveInput => GameInput.Instance.CharacterMovement;

        [SerializeField] private CharacterHealth _characterHealth;

        [SerializeField] private float gameOverWaitTime = 0.5f;

        private void Awake()
        {
            _characterHealth.OnHit += OnHit;
            _characterHealth.OnDie += OnDie;
        }

        // private void OnEnable()
        // {
        // }
        //
        // private void OnDisable()
        // {
        // }

        private void OnHit()
        {
            Debug.LogError("CharacterAnimation::OnHit");
            _animator.SetTrigger(_hitAnimIndex);
        }

        public void OnAfterHit()
        {
            if(_afterHitAnimationCoroutine!=null) StopAllCoroutines();
            _afterHitAnimationCoroutine = StartCoroutine(AfterHitAnimationCoroutine());
        }

        private void OnDie()
        {
            _animator.SetTrigger(_disappearAnimIndex);
            StopAllCoroutines();
            // StartCoroutine(OnDieCoroutine());
        }

        // private IEnumerator OnDieCoroutine()
        // {
        //     yield return gameOverWaitTime.Wait();
        //     OnGameOver();
        // }

        public void OnGameOver()
        {
            _sprite.enabled = false;
            GameManager.Instance.GameOver();
        }

        private void Update()
        {
            UpdateAnimation();
        }

        private void UpdateAnimation()
        {
            if (MoveInput.x != 0)
            {
                _flipX = MoveInput.x < 0;
                _sprite.flipX = _flipX;
            }

            _animator.SetBool(_runningAnimIndex, MoveInput.x != 0 && _characterMovement.Grounded);
            _animator.SetBool(_groundedAnimIndex, _characterMovement.Grounded);
            
            _animator.SetFloat(_yVeloAnimIndex, _rigidbody.velocity.y);
        }

        [SerializeField] private float hitOpacity = 0.5f;
        [SerializeField] private int numberOpacityChange = 3;
        private Coroutine _afterHitAnimationCoroutine;
        private IEnumerator AfterHitAnimationCoroutine()
        {
            float opacityChangeTime = _characterHealth.immortalTimeAfterHit / (numberOpacityChange * 2);
            for (int i = 0; i < numberOpacityChange; i++)
            {
                yield return ChangeOpacity(hitOpacity, opacityChangeTime);
                yield return ChangeOpacity(1f, opacityChangeTime);
            }

            _afterHitAnimationCoroutine = null;
        }
        
        private IEnumerator ChangeOpacity(float targetOpacity, float duration)
        {
            float currentOpacity = _sprite.color.a;
            float t = 0;

            while (t < 1)
            {
                t += Time.deltaTime / duration;
                float newOpacity = Mathf.Lerp(currentOpacity, targetOpacity, t);
                Color spriteColor = _sprite.color;
                spriteColor.a = newOpacity;
                _sprite.color = spriteColor;
                yield return null;
            }
        }
        
    }
}