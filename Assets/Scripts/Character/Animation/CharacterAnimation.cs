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
    }
}