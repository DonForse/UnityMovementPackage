using MovementPackage.Runtime.Scripts.Parameters;
using UnityEngine;

namespace MovementPackage.Runtime.Scripts.MovementProcesses
{
    public class PlayerWalkProcess : IMovementProcess
    {
        private PlayerMovementData _playerMovementData;
        private PlayerMovementInputDataSo _playerMovementInputDataSo;
        private WalkParameters _walkParameters;

        public void Initialize(PlayerMovementInputDataSo playerMovementInputDataSo, PlayerMovementData playerMovementData, WalkParameters walkParameters)
        {
            _playerMovementInputDataSo = playerMovementInputDataSo;
            _playerMovementData = playerMovementData;
            _playerMovementData.movementSpeedMultiplier = 1f;
            _walkParameters = walkParameters;
        }

        public void ProcessFixedUpdate()
        {
            if (_playerMovementData.dashing) return;
            if (_playerMovementData.wallJumping) return;
            _playerMovementData.playerHorizontalSpeed = _playerMovementInputDataSo.horizontalPressed * (_walkParameters.movementSpeed * Time.fixedDeltaTime) * _playerMovementData.movementSpeedMultiplier;
            _playerMovementData.playerForwardSpeed = _playerMovementInputDataSo.verticalPressed * (_walkParameters.movementSpeed * Time.fixedDeltaTime)* _playerMovementData.movementSpeedMultiplier;
        }
    }
}