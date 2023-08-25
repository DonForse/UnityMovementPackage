using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace MovementPackage.Runtime.Scripts
{
    public class PlayerCrouchComponent : MonoBehaviour, IMovementComponent
    {
        [SerializeField] private float movementSpeedMultiplier = 0.5f;
        [SerializeField] private bool changeHeightOnCrouch = true;
        [SerializeField] private float crouchHeight =1f;
        [SerializeField] private Vector3 characterControllerCenterOnCrouch = Vector3.up * -0.5f;
        private PlayerMovementInputData _playerMovementInputData;
        private PlayerMovementData _playerMovementData;
        private CharacterController _characterController;
        private float _normalHeight;
        private Vector3 _normalCenter;
        private List<Action> _crouchActions;
        private List<Action> _unCrouchActions;

        public void Initialize(PlayerMovementData playerMovementData, PlayerMovementInputData playerMovementInputData, CharacterController characterController)
        {
            _playerMovementInputData = playerMovementInputData;
            _playerMovementData = playerMovementData;
            _characterController = characterController;
            _normalHeight = _characterController.height;
            _normalCenter = _characterController.center;
            _crouchActions = new List<Action>() { Crouch };
            
            _unCrouchActions = new List<Action> { ResetCrouch };
            
            if (changeHeightOnCrouch)
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
            _playerMovementData.movementSpeedMultiplier = movementSpeedMultiplier;
            _playerMovementData.crouching = true;
        }

        private void ResetCrouch()
        {
            _playerMovementData.crouching = false;
            _playerMovementData.movementSpeedMultiplier = 1f;
        }

        private void SetCrouchHeight()
        {
            _characterController.height = crouchHeight;
            _characterController.center = characterControllerCenterOnCrouch;
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