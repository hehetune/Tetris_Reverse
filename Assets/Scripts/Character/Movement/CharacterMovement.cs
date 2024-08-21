using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Character.Movement
{
    public class CharacterMovement : MonoBehaviour
    {
        [SerializeField] private Rigidbody2D rb;

        [Range(0, 1)] [SerializeField] private float m_SlideSpeed = .5f; // How much to smooth out the movement

        [Range(0, .3f)] [SerializeField]
        private float m_MovementSmoothing = .05f; // How much to smooth out the movement

        [SerializeField] int totalJumps = 1; // Total jump player can perform
        [SerializeField] private bool m_AirControl = false; // Whether or not a player can steer while jumping;

        private float dirX = 0f; // X direction value
        private bool facingRight = true; // is player facing right
        private bool jumpPressed = false;

        private int availableJumps;
        private bool isDoubleJump;
        private bool coyoteJump;

        [SerializeField] private float moveSpeed = 6f;
        [SerializeField] private float jumpHeight = 700f;
        [SerializeField] private float gravityScale = 3f;

        // ground & wall attributes
        private bool grounded = true;
        private bool isSliding = false;
        const float k_WallRadius = .2f; // Radius of the overlap circle to determine if the player can stand up
        const float k_GroundedRadius = .27f; // Radius of the overlap circle to determine if grounded
        [SerializeField] private Transform m_WallCheck; // A position marking where to check for ceilings
        [SerializeField] public Transform groundCheck;
        [SerializeField] private LayerMask m_WhatIsWall; // A mask determining what is ground to the character
        [SerializeField] private LayerMask m_WhatIsGround;

        private Vector3 m_Velocity = Vector3.zero;

        private bool _canMove;

        private void Awake()
        {
            availableJumps = totalJumps;
        }

        // Start is called before the first frame update
        private void Start()
        {
            this.rb = GetComponent<Rigidbody2D>();
        }

        public void ApplyGravity()
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.gravityScale = gravityScale;
        }

        // Update is called once per frame
        private void Update()
        {
            if (!_canMove) return;
            HandlePCInput();
        }

        protected void HandlePCInput()
        {
            dirX = Input.GetAxisRaw("Horizontal");

            if (Input.GetButtonDown("Jump") && totalJumps > 0)
            {
                jumpPressed = true;
            }
        }

        private void Move()
        {
            if (jumpPressed)
            {
                Jump();
            }

            Vector2 targetVelocity = new Vector2(dirX * moveSpeed, rb.velocity.y);

            bool wasSliding = isSliding;
            isSliding = false;
            // Check if character is sliding on the wall
            if (Physics2D.OverlapCircle(m_WallCheck.position, k_WallRadius, m_WhatIsWall) && Mathf.Abs(dirX) > 0 &&
                !grounded && rb.velocity.y < 0)
            {
                targetVelocity.y = -m_SlideSpeed;
                isSliding = true;
                // if (!wasSliding) wallDustCoroutineAllow = true;
                availableJumps = totalJumps;
                isDoubleJump = false;
            }
            // else if (wasSliding) wallDustCoroutineAllow = false;

            rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);
        }

        void Jump()
        {
            if (grounded || isSliding)
            {
                availableJumps = totalJumps;
                DoJump();
                grounded = false;

                StartCoroutine(CoyoteJumpDelay());
            }
            else if (availableJumps > 0)
            {
                if (coyoteJump)
                {
                    isDoubleJump = true;
                    coyoteJump = false;
                    DoJump(1.2f);
                }
                else
                {
                    DoJump();
                }
            }

            jumpPressed = false;
        }

        void DoJump(float alpha = 1f)
        {
            --availableJumps;
            rb.velocity = new Vector2(rb.velocity.x, 0f);
            rb.AddForce(Vector2.up * jumpHeight * alpha);
            // AudioManager.Ins.PlaySFX(EffectSound.JumpSound);
        }

        IEnumerator CoyoteJumpDelay()
        {
            coyoteJump = true;
            yield return new WaitForSeconds(0.4f);
            coyoteJump = false;
        }

        private void FixedUpdate()
        {
            UpdateAnimationState();

            if (!_canMove) return;

            bool wasGrounded = grounded;
            grounded = false;
            Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheck.position, k_GroundedRadius, m_WhatIsGround);
            if (colliders.Length > 0)
            {
                grounded = true;
                if (!wasGrounded)
                {
                    // FXSpawner.Instance.Spawn(FXSpawner.GroundDust, groundCheck.position + groundDustOffset,
                    //     Quaternion.Euler(0, 0, 0));
                    // groundDustCoroutineAllow = true;
                }
            }
            else if (wasGrounded)
            {
                // groundDustCoroutineAllow = false;
            }

            // HandleGroundEffect();
            // HandleWallEffect();

            if (rb.velocity.y < 0 && !isSliding)
            {
                rb.gravityScale = gravityScale * 1.2f;
            }
            else
            {
                rb.gravityScale = gravityScale;
            }

            if (grounded || m_AirControl)
            {
                Move();
            }
        }

        

        private void UpdateAnimationState()
        {
        }

        private void Flip()
        {
            facingRight = !facingRight;
            transform.parent.Rotate(0f, 180f, 0f);
        }

        public void OnDoubleJumpEnd()
        {
            if (isDoubleJump) isDoubleJump = false;
        }

        
    }
}