using MovementPackage.Runtime.Scripts.Parameters;
using UnityEngine;

namespace MovementPackage.Runtime.Scripts.MovementProcesses
{
    public class PlayerWallJumpProcess : IMovementProcess
    {
        private WallJumpParameters _wallJumpParameters;
        private PlayerMovementData _playerMovementData;
        private PlayerMovementInputDataSo _playerMovementInputDataSo;
        private float wallJumpTimer = Mathf.Infinity;


        public void Initialize(PlayerMovementData playerMovementData, PlayerMovementInputDataSo playerMovementInputDataSo,
            WallJumpParameters wallJumpParameters)
        {
            _playerMovementData = playerMovementData;
            _playerMovementInputDataSo = playerMovementInputDataSo;
            _wallJumpParameters = wallJumpParameters;
        }

        public void ProcessFixedUpdate()
        {
            ProcessWallJumpCooldownTimer();

            if (_playerMovementData.collidingGround)
                return;
            
            if (!_playerMovementInputDataSo.jumpPressed) return;
            
            if (TryWallJumpRightWall()) return;

            if (TryWallJumpLeftWall()) return;

            if (TryWallJumpForwardWall()) return;

            TryWallJumpBackWall();
        }

        private bool TryWallJumpBackWall()
        {
            if (!_playerMovementData.closeBackWall) return false;
            _playerMovementData.playerForwardSpeed = _wallJumpParameters.jumpOppositeSpeed * Time.fixedDeltaTime;
            _playerMovementData.playerVerticalSpeed = Mathf.Sqrt(_wallJumpParameters.jumpHeight * Time.fixedDeltaTime);
            _playerMovementData.wallJumping = true;
            _playerMovementData.grabbedToBackWall = false;
            wallJumpTimer = 0f;
            return true;
        }

        private bool TryWallJumpForwardWall()
        {
            if (!_playerMovementData.closeForwardWall) return false;
            _playerMovementData.playerForwardSpeed = -1 * _wallJumpParameters.jumpOppositeSpeed * Time.fixedDeltaTime;
            _playerMovementData.playerVerticalSpeed = Mathf.Sqrt(_wallJumpParameters.jumpHeight * Time.fixedDeltaTime);
            _playerMovementData.wallJumping = true;
            wallJumpTimer = 0f;
            _playerMovementData.grabbedToForwardWall = false;
            return true;

        }

        private bool TryWallJumpLeftWall()
        {
            if (!_playerMovementData.closeLeftWall) return false;
            _playerMovementData.playerHorizontalSpeed = _wallJumpParameters.jumpOppositeSpeed * Time.fixedDeltaTime;
            _playerMovementData.playerVerticalSpeed = Mathf.Sqrt(_wallJumpParameters.jumpHeight * Time.fixedDeltaTime);
            _playerMovementData.wallJumping = true;
            _playerMovementData.grabbedToLeftWall = false;
            wallJumpTimer = 0f;
            return true;

        }

        private bool TryWallJumpRightWall()
        {
            if (!_playerMovementData.closeRightWall) return false;
            _playerMovementData.playerHorizontalSpeed = -1 * _wallJumpParameters.jumpOppositeSpeed * Time.fixedDeltaTime;
            _playerMovementData.playerVerticalSpeed = Mathf.Sqrt(_wallJumpParameters.jumpHeight * Time.fixedDeltaTime);
            _playerMovementData.wallJumping = true;
            _playerMovementData.grabbedToRightWall = false;
            wallJumpTimer = 0f;
            return true;

        }

        private void ProcessWallJumpCooldownTimer()
        {
            if (_wallJumpParameters.wallJumpCooldown > wallJumpTimer)
                wallJumpTimer += Time.fixedDeltaTime;
            else
                _playerMovementData.wallJumping = false;
        }
    }
}