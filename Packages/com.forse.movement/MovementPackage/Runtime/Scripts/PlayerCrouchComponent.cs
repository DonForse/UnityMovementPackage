using UnityEngine;

namespace MovementPackage.Runtime.Scripts
{
    public class PlayerCrouchComponent : MonoBehaviour, IMovementComponent
    {
        [SerializeField] private float movementSpeedMultiplier;
        private PlayerMovementInputData _playerMovementInputData;
        private PlayerMovementData _playerMovementData;
        

        public void Initialize(PlayerMovementData playerMovementData, PlayerMovementInputData playerMovementInputData)
        {
            _playerMovementInputData = playerMovementInputData;
            _playerMovementData = playerMovementData;
        }

        public void ProcessFixedUpdate()
        {
            if (!IsPressingCrouch() || !IsGrounded())
            {
                _playerMovementData.crouching = false;
                _playerMovementData.movementSpeedMultiplier = 1f;
                return;
            }

            _playerMovementData.movementSpeedMultiplier = movementSpeedMultiplier;
            _playerMovementData.crouching = true;
        }

        private bool IsGrounded() => _playerMovementData.collidingGround;
        private bool IsPressingCrouch() => _playerMovementInputData.crouchPressed;
    }
}