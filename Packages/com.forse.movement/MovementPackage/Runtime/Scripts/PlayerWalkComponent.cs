using UnityEngine;

namespace MovementPackage.Runtime.Scripts
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerWalkComponent : MonoBehaviour, IMovementComponent
    {
        [SerializeField] private float movementSpeed = 6f;

        private float playerHeight;
        private float moveSpeed;
        private bool calculatingCoyote;
        private PlayerMovementData _playerMovementData;
        private PlayerMovementInputData _playerMovementInputData;
        
        public void Initialize(PlayerMovementInputData playerMovementInputData, PlayerMovementData playerMovementData)
        {
            _playerMovementInputData = playerMovementInputData;
            _playerMovementData = playerMovementData;
        }

        public void ProcessFixedUpdate()
        {
            if (_playerMovementData.wallJumped) return;
            _playerMovementData.playerHorizontalSpeed = _playerMovementInputData.horizontalPressed * (movementSpeed * Time.fixedDeltaTime) * _playerMovementData.movementSpeedMultiplier;
            _playerMovementData.playerForwardSpeed = _playerMovementInputData.verticalPressed * (movementSpeed * Time.fixedDeltaTime)* _playerMovementData.movementSpeedMultiplier;
        }
    }
}