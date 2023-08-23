using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace MovementPackage.Runtime.Scripts
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerJumpComponent : MonoBehaviour, IMovementComponent
    {
        [HideInInspector] public UnityEvent Jumped = new UnityEvent();
        [SerializeField] private float jumpHeight = 4f;

        [Header("Feature Toggle")]
        [Tooltip("Jump buffering. " +
                 "If you press and hold the jump button a short time before landing," +
                 " you will jump on the exact frame that you land.")]
        [SerializeField]
        private bool jumpBufferingEnabled;

        [SerializeField] private bool coyoteEnabled;
        [SerializeField] private bool holdJumpEnabled;
        [SerializeField] private bool jumpCooldownEnabled;

        [Header("Hold Jump")] [SerializeField] private float holdJumpGravity = 0.35f;
        [Header("Coyote")] [SerializeField] private float coyoteTime = 0.25f;

        [Header("Jump Cooldown")] [SerializeField]
        private float jumpCooldown = 0.1f;

        private bool isOnCoyoteTime;
        private bool cancelCoyote;

        private float jumpCooldownTimer = 0f;

        private bool lastFrameGrounded;
        private PlayerMovementData _playerMovementData;
        private PlayerMovementInputData _playerMovementInputData;
        private bool coyoteGroundedPlayer;

        public void Initialize(PlayerMovementData playerMovementData, PlayerMovementInputData playerMovementInputData)
        {
            _playerMovementData = playerMovementData;
            _playerMovementInputData = playerMovementInputData;
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
            if (jumpBufferingEnabled && (!_playerMovementInputData.jumpPressed && _playerMovementInputData.jumpHold &&
                                         !lastFrameGrounded))
                _playerMovementInputData.jumpPressed = true;
        }

        private void HoldJump()
        {
            if (!holdJumpEnabled)
                return;
            _playerMovementData.gravityMultiplier = _playerMovementInputData.jumpHold ? holdJumpGravity : 1f;
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
            if (!jumpCooldownEnabled) return;
            if (!lastFrameGrounded)
                jumpCooldownTimer = jumpCooldown;
        }

        private void Jump()
        {
            if (!_playerMovementInputData.jumpPressed)
                return;

            if (!_playerMovementData.collidingGround && (!coyoteEnabled || !isOnCoyoteTime)) return;

            if (jumpCooldownEnabled && jumpCooldownTimer > 0f)
                return;

            _playerMovementData.gravityMultiplier = 1f;
            _playerMovementData.playerVerticalSpeed += Mathf.Sqrt(jumpHeight * Time.fixedDeltaTime);
            cancelCoyote = true;
            coyoteGroundedPlayer = false;
            Jumped.Invoke();
        }

        private void CoyoteJumpTime()
        {
            if (!coyoteEnabled)
                return;

            var isFloating = coyoteGroundedPlayer && (coyoteGroundedPlayer != _playerMovementData.collidingGround);
            if (!isOnCoyoteTime && isFloating)
                StartCoroutine(StartCoyote());

            if (!coyoteGroundedPlayer)
                coyoteGroundedPlayer = _playerMovementData.collidingGround;
        }

        private IEnumerator StartCoyote()
        {
            isOnCoyoteTime = true;
            cancelCoyote = false;
            var coyoteTimer = 0f;
            while (coyoteTimer < coyoteTime)
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