using System;
using System.Collections.Generic;
using MovementPackage.Runtime.Scripts.Parameters;
using UnityEngine;

namespace MovementPackage.Runtime.Scripts.MovementProcesses
{
    public class PlayerCrouchProcess : IMovementProcess
    {


        private CrouchParameters _crouchParameters;
        private PlayerMovementInputData _playerMovementInputData;
        private PlayerMovementData _playerMovementData;
        private CharacterController _characterController;
        private float _normalHeight;
        private Vector3 _normalCenter;
        private List<Action> _crouchActions;
        private List<Action> _unCrouchActions;

        public void Initialize(PlayerMovementData playerMovementData, PlayerMovementInputData playerMovementInputData, CharacterController characterController, CrouchParameters crouchParameters)
        {
            _playerMovementInputData = playerMovementInputData;
            _playerMovementData = playerMovementData;
            _characterController = characterController;
            _normalHeight = _characterController.height;
            _normalCenter = _characterController.center;
            _crouchParameters = crouchParameters;
            
            _crouchActions = new List<Action>() { Crouch };
            
            _unCrouchActions = new List<Action> { ResetCrouch };
            
            if (_crouchParameters.changeHeightOnCrouch)
            {
                _unCrouchActions.Add(ResetCrouchHeight);
                _crouchActions.Add(SetCrouchHeight);
            }

        }

        public void ProcessFixedUpdate()
        {
            if (!IsPressingCrouch() || !IsGrounded())
            {
                foreach (var action in _unCrouchActions) 
                    action.Invoke();

                return;
            }

            foreach (var action in _crouchActions)
            {
                action.Invoke();
            }
        }

        private void Crouch()
        {
            _playerMovementData.movementSpeedMultiplier = _crouchParameters.movementSpeedMultiplier;
            _playerMovementData.crouching = true;
        }

        private void ResetCrouch()
        {
            _playerMovementData.crouching = false;
            _playerMovementData.movementSpeedMultiplier = 1f;
        }

        private void SetCrouchHeight()
        {
            _characterController.height = _crouchParameters.crouchHeight;
            _characterController.center = _crouchParameters.characterControllerCenterOnCrouch;
        }

        private void ResetCrouchHeight()
        {
            _characterController.height = _normalHeight;
            _characterController.center = _normalCenter;

        }

        private bool IsGrounded() => _playerMovementData.closeGround;
        private bool IsPressingCrouch() => _playerMovementInputData.crouchPressed;
    }
}