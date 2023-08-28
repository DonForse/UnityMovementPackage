using MovementPackage.Runtime.Scripts.Parameters;
using UnityEngine;

namespace MovementPackage.Runtime.Scripts.MovementProcesses
{
    public class PlayerWalkProcess : IMovementProcess
    {
        private PlayerMovementData _playerMovementData;
        private PlayerMovementInputData _playerMovementInputData;
        private WalkParameters _walkParameters;

        public void Initialize(PlayerMovementInputData playerMovementInputData, PlayerMovementData playerMovementData, WalkParameters walkParameters)
        {
            _playerMovementInputData = playerMovementInputData;
            _playerMovementData = playerMovementData;
            _playerMovementData.movementSpeedMultiplier = 1f;
            _walkParameters = walkParameters;
        }

        public void ProcessFixedUpdate()
        {
            if (_playerMovementData.wallJumped) return;
            _playerMovementData.playerHorizontalSpeed = _playerMovementInputData.horizontalPressed * (_walkParameters.movementSpeed * Time.fixedDeltaTime) * _playerMovementData.movementSpeedMultiplier;
            _playerMovementData.playerForwardSpeed = _playerMovementInputData.verticalPressed * (_walkParameters.movementSpeed * Time.fixedDeltaTime)* _playerMovementData.movementSpeedMultiplier;
        }
    }
}