using System;
using System.Collections;
using Character.Health;
using Managers;
using UnityEngine;

namespace Character.Movement
{
    public class CharacterMovement : MonoBehaviour
    {
        [Header("Cache Components")] [SerializeField]
        private SpriteRenderer _sprite;

        [Header("Movement Settings")] [SerializeField]
        private Rigidbody2D _rb;

        public Rigidbody2D Rb => this._rb;

        [SerializeField] private float moveSpeed = 6f;
        [SerializeField] private float jumpForce = 700f;
        [SerializeField] private float gravityScale = 3f;
        [Range(0, 1)] [SerializeField] private float _slideSpeed = 0.5f;
        [Range(0, 0.3f)] [SerializeField] private float _movementSmoothing = 0.05f;

        [Header("Jump Settings")] [SerializeField]
        private int totalJumps = 1;

        [SerializeField] private bool _airControl = false;

        [Header("Ground & Wall Check")] [SerializeField]
        private Transform _wallCheckPosition;

        [SerializeField] private Transform _groundCheckPosition;
        [SerializeField] private Transform _groundEffectPosition;
        [SerializeField] private LayerMask _whatIsWall;
        [SerializeField] private LayerMask _whatIsGround;
        [SerializeField] private bool canSlide = true;

        private int _availableJumps;
        private bool _isDoubleJump;
        private bool _coyoteJump;
        private bool _grounded;
        private bool _isSliding;
        private bool _wasGrounded;

        private Vector3 _velocity = Vector3.zero;
        private bool _canMove;
        private bool _wasSliding;

        private Vector2 MoveInput => GameInput.Instance.CharacterMovement;

        private Coroutine _groundEffectCoroutine;
        private Coroutine _wallEffectCoroutine;

        private const float k_WallRadius = 0.2f;
        private const float k_GroundedRadius = 0.2f;

        public bool Grounded => _grounded;
        public bool IsSliding => _isSliding;

        private bool _isOnWater = true;

        private bool _isDisabled = false;

        [SerializeField] private CharacterHealth _characterHealth; 

        private void Awake()
        {
            _availableJumps = totalJumps;
            _characterHealth.OnDie += DisableThis;
        }

        private void Start()
        {
            _rb = GetComponent<Rigidbody2D>();
        }

        private void OnEnable()
        {
            GameInput.Instance.OnJumpAction += OnJumpEvent;
        }

        private void OnDisable()
        {
            GameInput.Instance.OnJumpAction -= OnJumpEvent;
        }

        private void Update()
        {
            if (_isDisabled) return;
            HandleMoveHorizontal();
            UpdateGroundedStatus();
            ApplyGravityScale();
            HandleEffects();
        }

        // private void FixedUpdate()
        // {
        // }

        private void DisableThis()
        {
            this._isDisabled = true;
        }

        private void HandleMoveHorizontal()
        {
            if (!_grounded && !_airControl) return;

            Vector2 targetVelocity = new Vector2(MoveInput.x * moveSpeed, _rb.velocity.y);

            _wasSliding = _isSliding;
            _isSliding = false;

            if (canSlide && ShouldSlide())
            {
                targetVelocity.y = -_slideSpeed;
                _isSliding = true;
                ResetJumpStatus();
            }

            _rb.velocity = Vector3.SmoothDamp(_rb.velocity, targetVelocity, ref _velocity, _movementSmoothing);

            
        }

        private bool ShouldSlide()
        {
            return Physics2D.OverlapCircle(_wallCheckPosition.position, k_WallRadius, _whatIsWall) &&
                   Mathf.Abs(MoveInput.x) > 0 && !_grounded && _rb.velocity.y < 0;
        }

        private void ResetJumpStatus()
        {
            _availableJumps = totalJumps;
            _isDoubleJump = false;
        }

        private void OnJumpEvent(object sender, EventArgs e)
        {
            if (totalJumps < 0) return;

            if (_grounded || _isSliding)
            {
                _availableJumps = totalJumps;
                HandleJump();
                _grounded = false;
                StartCoroutine(CoyoteJumpDelay());
            }
            else if (_availableJumps > 0)
            {
                HandleAirJump();
            }
        }

        private void HandleJump(float alpha = 1f)
        {
            _availableJumps--;
            _rb.velocity = new Vector2(_rb.velocity.x, 0f);
            _rb.AddForce(Vector2.up * jumpForce * alpha);
        }

        private void HandleAirJump()
        {
            if (_coyoteJump)
            {
                _isDoubleJump = true;
                _coyoteJump = false;
                HandleJump(1.2f);
            }
            else
            {
                HandleJump();
            }
        }

        private IEnumerator CoyoteJumpDelay()
        {
            _coyoteJump = true;
            yield return new WaitForSeconds(0.4f);
            _coyoteJump = false;
        }

        private void ApplyGravityScale()
        {
            _rb.gravityScale = (_rb.velocity.y < 0 && !_isSliding) ? gravityScale * 1.2f : gravityScale;
        }

        private void UpdateGroundedStatus()
        {
            _wasGrounded = _grounded;
            _grounded = false;

            Collider2D[] colliders =
                Physics2D.OverlapCircleAll(_groundCheckPosition.position, k_GroundedRadius, _whatIsGround);
            if (colliders.Length > 0)
            {
                _grounded = true;
            }
        }

        private void HandleEffects()
        {
            HandleGroundEffect();
            HandleWallEffect();
        }

        private void HandleGroundEffect()
        {
            if (_grounded && !_isOnWater && MoveInput.x != 0 && _groundEffectCoroutine == null)
            {
                _groundEffectCoroutine = StartCoroutine(SpawnGroundEffect());
            }
        }

        private void HandleWallEffect()
        {
            if (_isSliding && _wallEffectCoroutine == null)
            {
                _wallEffectCoroutine = StartCoroutine(SpawnWallEffect());
            }
        }

        [SerializeField] private float _groundParticleEmissionRate = 0.1f;
        [SerializeField] private float _wallParticleEmissionRate = 0.25f;
        [SerializeField] private Prefab _groundParticlePrefab;
        [SerializeField] private Prefab _wallParticlePrefab;

        private IEnumerator SpawnGroundEffect()
        {
            while (_grounded && !_isOnWater && MoveInput.x != 0)
            {
                PoolManager.Get<PoolObject>(_groundParticlePrefab, out var effectGo);
                effectGo.transform.position = _groundEffectPosition.position;
                effectGo.transform.rotation = Quaternion.identity;
                effectGo.GetComponent<PoolObject>().ReturnToPoolByLifeTime(1f);
                yield return new WaitForSeconds(_groundParticleEmissionRate);
            }

            _groundEffectCoroutine = null;
        }

        private IEnumerator SpawnWallEffect()
        {
            while (_isSliding)
            {
                PoolManager.Get<PoolObject>(_wallParticlePrefab, out var effectGo);
                effectGo.transform.position = _wallCheckPosition.position;
                effectGo.transform.rotation = Quaternion.identity;
                effectGo.GetComponent<PoolObject>().ReturnToPoolByLifeTime(1f);
                yield return new WaitForSeconds(_wallParticleEmissionRate);
            }

            _wallEffectCoroutine = null;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.tag.Equals("Larva"))
            {
                _isOnWater = true;
            }
        }
        
        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.gameObject.tag.Equals("Larva"))
            {
                _isOnWater = false;
            }
        }
    }
}