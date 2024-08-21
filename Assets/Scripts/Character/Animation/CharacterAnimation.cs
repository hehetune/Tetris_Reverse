using Character.Movement;
using UnityEngine;

namespace Character.Animation
{
    public class CharacterAnimation
    {
        [SerializeField] private CharacterMovement _characterMovement;
        
        private void UpdateAnimationState()
        {
            // if (isRespawn) return;
            // else if (isSliding)
            // {
            //     this.playerCtrl.PlayerAnimation.ChangeAnimationState(PlayerAnimationState.PLAYER_WALL_JUMP);
            //     return;
            // }
            // else if (!grounded)
            // {
            //     if (isDoubleJump)
            //         this.playerCtrl.PlayerAnimation.ChangeAnimationState(PlayerAnimationState.PLAYER_DOUBLE_JUMP);
            //     else if (rb.velocity.y >= 0)
            //         this.playerCtrl.PlayerAnimation.ChangeAnimationState(PlayerAnimationState.PLAYER_JUMP);
            //     else if (rb.velocity.y < 0)
            //         this.playerCtrl.PlayerAnimation.ChangeAnimationState(PlayerAnimationState.PLAYER_FALL);
            //     if (dirX > 0)
            //     {
            //         if (!facingRight) Flip();
            //     }
            //     else if (dirX < 0)
            //     {
            //         if (facingRight) Flip();
            //     }
            //
            //     return;
            // }
            // else if (grounded)
            // {
            //     if (dirX > 0)
            //     {
            //         this.playerCtrl.PlayerAnimation.ChangeAnimationState(PlayerAnimationState.PLAYER_RUN);
            //         if (!facingRight) Flip();
            //     }
            //     else if (dirX < 0)
            //     {
            //         this.playerCtrl.PlayerAnimation.ChangeAnimationState(PlayerAnimationState.PLAYER_RUN);
            //         if (facingRight) Flip();
            //     }
            //     else
            //     {
            //         this.playerCtrl.PlayerAnimation.ChangeAnimationState(PlayerAnimationState.PLAYER_IDLE);
            //     }
            // }
        }
    }
}