using System;
using UnityCommunity.UnitySingleton;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Managers
{
    public class GameInput : MonoSingleton<GameInput>
    {
        // private static GameInput instance;
        // public static GameInput Instance 
        // {
        //     get 
        //     {
        //         if (firsttimecallInstance)
        //         {
        //             Debug.LogError("first time call GameInput instance");
        //             firsttimecallInstance = false;
        //         }
        //         if (instance == null)
        //         {
        //             Debug.LogError("GameInput's Instance null, try to find it's object");
        //             instance = FindObjectOfType<GameInput>();
        //             if (instance == null)
        //             {
        //                 Debug.LogError("Can't find it's object, create new object");
        //                 GameObject obj = new GameObject("GameInput");
        //                 instance = obj.AddComponent<GameInput>();
        //             }
        //         }
        //         return instance;
        //     }
        // }

        private static bool firsttimecallInstance = true;
        public event EventHandler OnInteractAction;
        public event EventHandler OnPauseAction;
        public event EventHandler OnMoveBlockLeftAction;
        public event EventHandler OnMoveBlockRightAction;
        public event EventHandler OnRotateBlockLeftAction;
        public event EventHandler OnRotateBlockRightAction;
        public event EventHandler OnBlockSoftDropAction;
        public event EventHandler OnBlockSoftDropCancel;
        public event EventHandler OnBlockHardDropAction;

        public event EventHandler OnCheatAction;

        public event EventHandler OnJumpAction;

        public event EventHandler OnYesAction;
        public event EventHandler OnNoAction;
        public Vector2 CharacterMovement => playerInputActions.GamePlay.Move.ReadValue<Vector2>();
        private PlayerInputActions playerInputActions;

        private void Awake()
        {
            // if(instance!=null)
            // {
            //     Destroy(this.gameObject);
            //     return;
            // }
            // instance = this;
            playerInputActions = new PlayerInputActions();
            playerInputActions.GamePlay.Enable();
            playerInputActions.GamePlay.PauseGame.performed += Pause_performed;
            playerInputActions.GamePlay.MoveBlockLeft.performed += MoveBlockLeft_performed;
            playerInputActions.GamePlay.MoveBlockRight.performed += MoveBlockRight_performed;
            playerInputActions.GamePlay.RotateBlockLeft.performed += RotateBlockLeft_performed;
            playerInputActions.GamePlay.RotateBlockRight.performed += RotateBlockRight_performed;
            playerInputActions.GamePlay.BlockSoftDrop.performed += BlockSoftDrop_performed;
            playerInputActions.GamePlay.BlockSoftDrop.canceled += BlockSoftDrop_canceled;
            playerInputActions.GamePlay.BlockHardDrop.performed += BlockHardDrop_performed;
            playerInputActions.GamePlay.Jump.performed += Jump_performed;
            playerInputActions.GamePlay.Cheat.performed += Cheat_performed;
            playerInputActions.GamePlay.Yes.performed += YesPerformed;
            playerInputActions.GamePlay.No.performed += NoPerformed;
        }

        private void OnDestroy()
        {
            if (playerInputActions != null)
            {
                playerInputActions.GamePlay.PauseGame.performed -= Pause_performed;
                playerInputActions.GamePlay.MoveBlockLeft.performed -= MoveBlockLeft_performed;
                playerInputActions.GamePlay.MoveBlockRight.performed -= MoveBlockRight_performed;
                playerInputActions.GamePlay.RotateBlockLeft.performed -= RotateBlockLeft_performed;
                playerInputActions.GamePlay.RotateBlockRight.performed -= RotateBlockRight_performed;
                playerInputActions.GamePlay.BlockSoftDrop.performed -= BlockSoftDrop_performed;
                playerInputActions.GamePlay.BlockSoftDrop.canceled -= BlockSoftDrop_canceled;
                playerInputActions.GamePlay.BlockHardDrop.performed -= BlockHardDrop_performed;
                playerInputActions.GamePlay.Jump.performed -= Jump_performed;
                playerInputActions.GamePlay.Cheat.performed -= Cheat_performed;
                playerInputActions.GamePlay.Yes.performed -= YesPerformed;
                playerInputActions.GamePlay.No.performed -= NoPerformed;
                playerInputActions.Dispose();
            }
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
            Debug.LogError("GameInput::MoveBlockRight_performed");
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

        private void BlockSoftDrop_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
        {
            OnBlockSoftDropAction?.Invoke(this, EventArgs.Empty);
        }

        private void BlockSoftDrop_canceled(UnityEngine.InputSystem.InputAction.CallbackContext obj)
        {
            OnBlockSoftDropCancel?.Invoke(this, EventArgs.Empty);
        }
        
        private void BlockHardDrop_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
        {
            OnBlockHardDropAction?.Invoke(this, EventArgs.Empty);
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