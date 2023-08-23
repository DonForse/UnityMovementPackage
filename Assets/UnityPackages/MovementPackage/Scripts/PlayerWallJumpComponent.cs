using Features.Game.Movement;
using UnityEngine;

namespace UnityPackages.MovementPackage.Scripts
{
    public class PlayerWallJumpComponent : MonoBehaviour, IMovementComponent
    {
        [Header("Remember to enable Close Wall Detection in Player Movement Collision.")]
        [Space]
        [Space]
        [SerializeField] private float jumpHeight = 4f;
        [SerializeField] private float jumpHorizontal = 4f;
        private PlayerMovementData _playerMovementData;
        private PlayerMovementInputData _playerMovementInputData;
        public float wallJumpTimer = 0f;
        public float wallJumpCooldown = 0.2f;


        public void Initialize(PlayerMovementData playerMovementData, PlayerMovementInputData playerMovementInputData)
        {
            _playerMovementData = playerMovementData;
            _playerMovementInputData = playerMovementInputData;
        }

        public void ProcessFixedUpdate()
        {
            ProcessWallJumpCooldownTimer();

            if (_playerMovementData.collidingGround)
                return;
            
            if (!_playerMovementInputData.jumpPressed) return;
            
            if (!_playerMovementData.closeRightWall && !_playerMovementData.closeLeftWall)
                return;

            _playerMovementData.playerVerticalSpeed += Mathf.Sqrt(jumpHeight * Time.fixedDeltaTime);
            _playerMovementData.playerHorizontalSpeed = (_playerMovementData.closeRightWall  ? -1 : 1) * jumpHorizontal * Time.fixedDeltaTime;

            _playerMovementData.wallJumped = true;
            wallJumpTimer = 0f;
        }

        private void ProcessWallJumpCooldownTimer()
        {
            if (wallJumpCooldown > wallJumpTimer)
                wallJumpTimer += Time.fixedDeltaTime;
            else
                _playerMovementData.wallJumped = false;
        }
    }
}