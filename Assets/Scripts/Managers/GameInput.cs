using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Managers
{
    public class GameInput : MonoBehaviour
    {
        public static GameInput Instance { get; private set; }

        public event EventHandler OnInteractAction;
        public event EventHandler OnPauseAction;
        public event EventHandler OnMoveBlockLeftAction;
        public event EventHandler OnMoveBlockRightAction;
        public event EventHandler OnRotateBlockLeftAction;
        public event EventHandler OnRotateBlockRightAction;
        public event EventHandler OnDropBlockFastAction;
        public event EventHandler OnDropBlockFastCancel;

        public event EventHandler OnCheatAction;

        public event EventHandler OnJumpAction;

        public event EventHandler OnYesAction;
        public event EventHandler OnNoAction;
        public Vector2 CharacterMovement => playerInputActions.GamePlay.Move.ReadValue<Vector2>();
        private PlayerInputActions playerInputActions;

        private void Awake()
        {
            Instance = this;
            playerInputActions = new PlayerInputActions();
            playerInputActions.GamePlay.Enable();
            playerInputActions.GamePlay.PauseGame.performed += Pause_performed;
            playerInputActions.GamePlay.MoveBlockLeft.performed += MoveBlockLeft_performed;
            playerInputActions.GamePlay.MoveBlockRight.performed += MoveBlockRight_performed;
            playerInputActions.GamePlay.RotateBlockLeft.performed += RotateBlockLeft_performed;
            playerInputActions.GamePlay.RotateBlockRight.performed += RotateBlockRight_performed;
            playerInputActions.GamePlay.DropBlockFast.performed += DropBlockFast_performed;
            playerInputActions.GamePlay.DropBlockFast.canceled += DropBlockFast_canceled;
            playerInputActions.GamePlay.Jump.performed += Jump_performed;
            playerInputActions.GamePlay.Cheat.performed += Cheat_performed;
            playerInputActions.GamePlay.Yes.performed += YesPerformed;
            playerInputActions.GamePlay.No.performed += NoPerformed;
        }

        private void OnDestroy()
        {
            playerInputActions.GamePlay.PauseGame.performed -= Pause_performed;
            playerInputActions.GamePlay.MoveBlockLeft.performed -= MoveBlockLeft_performed;
            playerInputActions.GamePlay.MoveBlockRight.performed -= MoveBlockRight_performed;
            playerInputActions.GamePlay.RotateBlockLeft.performed -= RotateBlockLeft_performed;
            playerInputActions.GamePlay.RotateBlockRight.performed -= RotateBlockRight_performed;
            playerInputActions.GamePlay.DropBlockFast.performed -= DropBlockFast_performed;
            playerInputActions.GamePlay.DropBlockFast.canceled -= DropBlockFast_canceled;
            playerInputActions.GamePlay.Jump.performed -= Jump_performed;
            playerInputActions.GamePlay.Cheat.performed -= Cheat_performed;
            playerInputActions.GamePlay.Yes.performed -= YesPerformed;
            playerInputActions.GamePlay.No.performed -= NoPerformed;
            playerInputActions.Dispose();
        }

        public void DisableGameInput()
        {
            playerInputActions.GamePlay.Disable();
        }

        private void Pause_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
        {
            OnPauseAction?.Invoke(this, EventArgs.Empty);
        }

        private void MoveBlockLeft_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
        {
            OnMoveBlockLeftAction?.Invoke(this, EventArgs.Empty);
        }

        private void MoveBlockRight_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
        {
            OnMoveBlockRightAction?.Invoke(this, EventArgs.Empty);
        }

        private void RotateBlockLeft_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
        {
            OnRotateBlockLeftAction?.Invoke(this, EventArgs.Empty);
        }

        private void RotateBlockRight_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
        {
            OnRotateBlockRightAction?.Invoke(this, EventArgs.Empty);
        }

        private void DropBlockFast_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
        {
            OnDropBlockFastAction?.Invoke(this, EventArgs.Empty);
        }

        private void DropBlockFast_canceled(UnityEngine.InputSystem.InputAction.CallbackContext obj)
        {
            OnDropBlockFastCancel?.Invoke(this, EventArgs.Empty);
        }

        private void Jump_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
        {
            OnJumpAction?.Invoke(this, EventArgs.Empty);
        }

        private void Cheat_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
        {
            OnCheatAction?.Invoke(this, EventArgs.Empty);
        }

        private void YesPerformed(InputAction.CallbackContext obj)
        {
            OnYesAction?.Invoke(this, EventArgs.Empty);
        }

        private void NoPerformed(InputAction.CallbackContext obj)
        {
            OnNoAction?.Invoke(this, EventArgs.Empty);
        }
    }
}