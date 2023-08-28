using MovementPackage.Runtime.Scripts.Parameters;
using UnityEngine;

namespace MovementPackage.Runtime.Scripts.MovementProcesses
{
    public class PlayerGravityProcess : IMovementProcess
    {
        private GravityParameters _gravityParameters;
        private PlayerMovementData _playerMovementData;

        public void Initialize(PlayerMovementData playerMovementData, GravityParameters gravityParameters)
        {
            _playerMovementData = playerMovementData;
            _playerMovementData.gravityMultiplier = 1f;
            _gravityParameters = gravityParameters;
        }

        public void ProcessFixedUpdate()
        {
            AddGravity();
            Grounded();
        }
       
        private void Grounded()
        {
            if (!_playerMovementData.collidingGround)
                return;

            _playerMovementData.playerVerticalSpeed = 0f;
        }
        

        private void AddGravity() => _playerMovementData.playerVerticalSpeed += _gravityParameters.gravityValue * _playerMovementData.gravityMultiplier * Time.fixedDeltaTime;
        
    }
}