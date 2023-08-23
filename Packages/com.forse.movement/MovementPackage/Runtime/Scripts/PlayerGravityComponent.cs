using UnityEngine;

namespace MovementPackage.Runtime.Scripts
{
    public class PlayerGravityComponent : MonoBehaviour, IMovementComponent
    {
        [SerializeField] private float gravityValue = -2.21f;
        
        private bool coyoteGroundedPlayer;
        private PlayerMovementData _playerMovementData;

        public void Initialize(PlayerMovementData playerMovementData)
        {
            _playerMovementData = playerMovementData;
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
        

        private void AddGravity() => _playerMovementData.playerVerticalSpeed += gravityValue * _playerMovementData.gravityMultiplier * Time.fixedDeltaTime;
        
    }
}