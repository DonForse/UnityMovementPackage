using System;
using System.Collections;
using MovementPackage.Runtime.Scripts.Parameters;
using UnityEngine;

namespace MovementPackage.Runtime.Scripts.MovementProcesses
{
    public class PlayerJumpProcess : IMovementProcess
    {
        private JumpParameters _jumpParameters;
        public event EventHandler Jumped;
       

        private bool isOnCoyoteTime;
        private bool cancelCoyote;

        private float jumpCooldownTimer = 0f;

        private bool lastFrameGrounded;
        private PlayerMovementData _playerMovementData;
        private PlayerMovementInputDataSo _playerMovementInputDataSo;
        private bool coyoteGroundedPlayer;
        private CoroutineHelper _coroutineHelper;

        public void Initialize(PlayerMovementData playerMovementData, PlayerMovementInputDataSo playerMovementInputDataSo,
            JumpParameters jumpParameters, CoroutineHelper coroutineHelper)
        {
            _playerMovementData = playerMovementData;
            _playerMovementInputDataSo = playerMovementInputDataSo;
            _jumpParameters = jumpParameters;
            _coroutineHelper = coroutineHelper;
        }

        public void ProcessFixedUpdate()
        {
            Grounded();
            CoyoteJumpTime();
            Jump();
            HoldJump();
        }

        private void JumpBuffering()
        {
            if (_jumpParameters.jumpBufferingEnabled && (!_playerMovementInputDataSo.jumpPressed && _playerMovementInputDataSo.jumpHold &&
                                         !lastFrameGrounded))
                _playerMovementInputDataSo.jumpPressed = true;
        }

        private void HoldJump()
        {
            if (!_jumpParameters.holdJumpEnabled)
                return;
            _playerMovementData.gravityMultiplier = _playerMovementInputDataSo.jumpHold ? _jumpParameters.holdJumpGravity : 1f;
        }

        private void Grounded()
        {
            RestartJumpCooldownTimer();

            if (_playerMovementData.collidingGround)
            {
                cancelCoyote = true;
            }

            if (_playerMovementData.collidingGround || isOnCoyoteTime)
            {
                _playerMovementData.playerVerticalSpeed = 0f;
                JumpBuffering();
                DecreaseJumpCooldownTimer();

                lastFrameGrounded = true;
                return;
            }

            lastFrameGrounded = false;
        }

        private void DecreaseJumpCooldownTimer()
        {
            if (jumpCooldownTimer > 0f)
                jumpCooldownTimer -= Time.fixedDeltaTime;
        }

        private void RestartJumpCooldownTimer()
        {
            if (!_jumpParameters.jumpCooldownEnabled) return;
            if (!lastFrameGrounded)
                jumpCooldownTimer = _jumpParameters.jumpCooldown;
        }

        private void Jump()
        {
            if (!_playerMovementInputDataSo.jumpPressed)
                return;

            if (!_playerMovementData.collidingGround && (!_jumpParameters.coyoteEnabled || !isOnCoyoteTime)) return;

            if (_jumpParameters.jumpCooldownEnabled && jumpCooldownTimer > 0f)
                return;

            _playerMovementData.gravityMultiplier = 1f;
            _playerMovementData.playerVerticalSpeed = Mathf.Sqrt(_jumpParameters.jumpHeight * Time.fixedDeltaTime);
            cancelCoyote = true;
            coyoteGroundedPlayer = false;
            Jumped?.Invoke(this, null);
        }

        private void CoyoteJumpTime()
        {
            if (!_jumpParameters.coyoteEnabled)
                return;

            var isFloating = coyoteGroundedPlayer && (coyoteGroundedPlayer != _playerMovementData.collidingGround);
            if (!isOnCoyoteTime && isFloating)
                _coroutineHelper.StartCoroutineFrom(StartCoyote);

            if (!coyoteGroundedPlayer)
                coyoteGroundedPlayer = _playerMovementData.collidingGround;
        }

        private IEnumerator StartCoyote()
        {
            isOnCoyoteTime = true;
            cancelCoyote = false;
            var coyoteTimer = 0f;
            while (coyoteTimer < _jumpParameters.coyoteTime)
            {
                if (cancelCoyote)
                {
                    coyoteGroundedPlayer = false;
                    isOnCoyoteTime = false;
                    yield break;
                }

                yield return null;
                coyoteTimer += Time.fixedDeltaTime;
            }

            coyoteGroundedPlayer = false;
            isOnCoyoteTime = false;
        }
    }
}