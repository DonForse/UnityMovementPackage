using UnityEngine;
using UnityEngine.Serialization;

namespace MovementPackage.Runtime.Scripts
{
    public class PlayerWallJumpComponent : MonoBehaviour, IMovementComponent
    {
        [Header("Remember to enable Close Wall Detection in Player Movement Collision.")]
        [Space]
        [Space]
        [SerializeField] private float jumpHeight = 4f;
        [SerializeField] private float jumpOppositeSpeed = 4f;
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
            
            if (TryWallJumpRightWall()) return;

            if (TryWallJumpLeftWall()) return;

            if (TryWallJumpForwardWall()) return;

            TryWallJumpBackWall();
        }

        private bool TryWallJumpBackWall()
        {
            if (!_playerMovementData.closeBackWall) return false;
            _playerMovementData.playerForwardSpeed = jumpOppositeSpeed * Time.fixedDeltaTime;
            _playerMovementData.playerVerticalSpeed = Mathf.Sqrt(jumpHeight * Time.fixedDeltaTime);
            _playerMovementData.wallJumped = true;
            wallJumpTimer = 0f;
            return true;
        }

        private bool TryWallJumpForwardWall()
        {
            if (!_playerMovementData.closeForwardWall) return false;
            _playerMovementData.playerForwardSpeed = -1 * jumpOppositeSpeed * Time.fixedDeltaTime;
            _playerMovementData.playerVerticalSpeed = Mathf.Sqrt(jumpHeight * Time.fixedDeltaTime);
            _playerMovementData.wallJumped = true;
            wallJumpTimer = 0f;
            return true;

        }

        private bool TryWallJumpLeftWall()
        {
            if (!_playerMovementData.closeLeftWall) return false;
            _playerMovementData.playerHorizontalSpeed = jumpOppositeSpeed * Time.fixedDeltaTime;
            _playerMovementData.playerVerticalSpeed = Mathf.Sqrt(jumpHeight * Time.fixedDeltaTime);
            _playerMovementData.wallJumped = true;
            wallJumpTimer = 0f;
            return true;

        }

        private bool TryWallJumpRightWall()
        {
            if (!_playerMovementData.closeRightWall) return false;
            _playerMovementData.playerHorizontalSpeed = -1 * jumpOppositeSpeed * Time.fixedDeltaTime;
            _playerMovementData.playerVerticalSpeed = Mathf.Sqrt(jumpHeight * Time.fixedDeltaTime);
            _playerMovementData.wallJumped = true;
            wallJumpTimer = 0f;
            return true;

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