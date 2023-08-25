using UnityEngine;

namespace MovementPackage.Runtime.Scripts
{
    public class PlayerWallGrabComponent : MonoBehaviour, IMovementComponent
    {
        // [HideInInspector]public UnityEvent GrabbedToWall = new();
        private PlayerMovementData _playerMovementData;
        private PlayerMovementInputData _playerMovementInputData;
        
        [SerializeField] private bool canMoveInOtherAxisWhileGrabbing = false;
        [SerializeField] private bool canMoveInSameAxisWhileGrabbing = false;
        [SerializeField] private bool slideOff = true;
        [SerializeField] private float slideOffStartAtTime = 0.5f;
        [SerializeField] private float slideOffSpeed = -0.25f;
        private float _slideOffTimer = 0f;

        public void Initialize(PlayerMovementData playerMovementData, PlayerMovementInputData playerMovementInputData)
        {
            _playerMovementData = playerMovementData;
            _playerMovementInputData = playerMovementInputData;
        }

        public void ProcessFixedUpdate()
        {
            if (_playerMovementData.collidingGround)
            {
                _playerMovementData.grabbedToLeftWall = false;
                _playerMovementData.grabbedToRightWall = false;
                return;
            }

            if (slideOff)
                _slideOffTimer += Time.fixedDeltaTime;

            RightWall();

            LeftWall();

            ForwardWall();

            BackWall();
        }

        private void BackWall()
        {
            if (_playerMovementData.collidingBackWall)
            {
                if (IsGrabbingBackWall())
                {
                    AdjustSpeedInZAxis();
                }

                if (!IsGrabbingBackWall() && IsMovingTowardBackWall()) _slideOffTimer = 0f;

                _playerMovementData.grabbedToBackWall = IsMovingTowardBackWall();
            }
            else
            {
                _playerMovementData.grabbedToBackWall = false;
            }
        }

        private void ForwardWall()
        {
            if (_playerMovementData.collidingForwardWall)
            {
                if (IsGrabbingForwardWall())
                {
                    AdjustSpeedInZAxis();
                }

                if (!IsGrabbingForwardWall() && IsMovingTowardForwardWall()) _slideOffTimer = 0f;

                _playerMovementData.grabbedToForwardWall = IsMovingTowardForwardWall();
            }
            else
            {
                _playerMovementData.grabbedToForwardWall = false;
            }
        }

        private void LeftWall()
        {
            if (_playerMovementData.collidingLeftWall)
            {
                if (IsGrabbingLeftWall())
                {
                    AdjustSpeedInXAxis();
                }

                if (!IsGrabbingLeftWall() && IsMovingTowardLeftWall()) _slideOffTimer = 0f;

                _playerMovementData.grabbedToLeftWall = IsMovingTowardLeftWall();
            }
            else
            {
                _playerMovementData.grabbedToLeftWall = false;
            }
        }

        private void RightWall()
        {
            if (_playerMovementData.collidingRightWall)
            {
                if (IsGrabbingRightWall())
                {
                    AdjustSpeedInXAxis();
                }

                if (!IsGrabbingRightWall() && IsMovingTowardRightWall()) _slideOffTimer = 0f;

                _playerMovementData.grabbedToRightWall = IsMovingTowardRightWall();
            }
            else
            {
                _playerMovementData.grabbedToRightWall = false;
            }
        }

        private void AdjustSpeedInXAxis()
        {
            if (!canMoveInOtherAxisWhileGrabbing)
                _playerMovementData.playerForwardSpeed = 0f;
            if (!canMoveInSameAxisWhileGrabbing)
                _playerMovementData.playerHorizontalSpeed = 0f;
            _playerMovementData.playerVerticalSpeed = slideOff
                ? (_slideOffTimer < slideOffStartAtTime ? 0f : slideOffSpeed * Time.fixedDeltaTime)
                : 0f;
        }

        private void AdjustSpeedInZAxis()
        {
            if (!canMoveInSameAxisWhileGrabbing)
                _playerMovementData.playerForwardSpeed = 0f;
            if (!canMoveInOtherAxisWhileGrabbing)
                _playerMovementData.playerHorizontalSpeed = 0f;
            _playerMovementData.playerForwardSpeed = slideOff
                ? (_slideOffTimer < slideOffStartAtTime ? 0f : slideOffSpeed * Time.fixedDeltaTime)
                : 0f;
        }

        private bool IsMovingTowardForwardWall() => _playerMovementInputData.verticalPressed > 0;
        private bool IsMovingTowardBackWall() => _playerMovementInputData.verticalPressed < 0;
        private bool IsMovingTowardLeftWall() => _playerMovementInputData.horizontalPressed < 0;
        private bool IsMovingTowardRightWall() => _playerMovementInputData.horizontalPressed > 0;
        private bool IsGrabbingLeftWall() => _playerMovementData.grabbedToLeftWall;
        private bool IsGrabbingForwardWall() => _playerMovementData.grabbedToForwardWall;
        private bool IsGrabbingBackWall() => _playerMovementData.grabbedToBackWall;
        private bool IsGrabbingRightWall() => _playerMovementData.grabbedToRightWall;
    }
}