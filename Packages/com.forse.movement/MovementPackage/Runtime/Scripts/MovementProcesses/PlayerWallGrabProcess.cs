using MovementPackage.Runtime.Scripts.Parameters;
using UnityEngine;

namespace MovementPackage.Runtime.Scripts.MovementProcesses
{
    public class PlayerWallGrabProcess : IMovementProcess
    {
        private WallGrabParameters _wallGrabParameters;

        // [HideInInspector]public UnityEvent GrabbedToWall = new();
        private PlayerMovementData _playerMovementData;
        private PlayerMovementInputDataSo _playerMovementInputDataSo;


        private float _slideOffTimer = 0f;

        public void Initialize(PlayerMovementData playerMovementData,
            PlayerMovementInputDataSo playerMovementInputDataSo, WallGrabParameters wallGrabParameters)
        {
            _playerMovementData = playerMovementData;
            _playerMovementInputDataSo = playerMovementInputDataSo;
            _wallGrabParameters = wallGrabParameters;
        }

        public void ProcessFixedUpdate()
        {
            if (_playerMovementData.collidingGround)
            {
                _playerMovementData.grabbedToLeftWall = false;
                _playerMovementData.grabbedToRightWall = false;
                _playerMovementData.grabbedToForwardWall = false;
                _playerMovementData.grabbedToBackWall = false;

                return;
            }

            if (_wallGrabParameters.slideOff)
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
                if (IsGrabbingBackWall() && IsMovingTowardBackWall())
                {
                    AdjustSpeedInZAxis();
                    AdjustSpeedOnYAxis();
                }
                else
                {
                    _slideOffTimer = 0f;
                }

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
                if (IsGrabbingForwardWall() && IsMovingTowardForwardWall())
                {
                    AdjustSpeedInZAxis();
                    AdjustSpeedOnYAxis();
                }
                else
                {
                    _slideOffTimer = 0f;
                }

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
                if (IsGrabbingLeftWall() && IsMovingTowardLeftWall())
                {
                    AdjustSpeedInXAxis();
                    AdjustSpeedOnYAxis();
                }
                else
                {
                    _slideOffTimer = 0f;
                }

                _playerMovementData.grabbedToLeftWall = IsMovingTowardLeftWall();
            }
            else
            {
                _playerMovementData.grabbedToLeftWall = false;
            }
        }

        private void RightWall()
        {
            if (_playerMovementData.collidingRightWall && IsMovingTowardRightWall())
            {
                if (IsGrabbingRightWall())
                {
                    AdjustSpeedInXAxis();
                    AdjustSpeedOnYAxis();
                }
                else
                {
                    _slideOffTimer = 0f;
                }
                _playerMovementData.grabbedToRightWall = IsMovingTowardRightWall();
            }
            else
            {
                _playerMovementData.grabbedToRightWall = false;
            }
        }

        private void AdjustSpeedInXAxis()
        {
            if (!_wallGrabParameters.canMoveInOtherAxisWhileGrabbing)
                _playerMovementData.playerHorizontalSpeed = 0f;
            if (!_wallGrabParameters.canMoveInSameAxisWhileGrabbing)
                _playerMovementData.playerForwardSpeed = 0f;
        }

        private void AdjustSpeedInZAxis()
        {
            if (!_wallGrabParameters.canMoveInSameAxisWhileGrabbing)
                _playerMovementData.playerForwardSpeed = 0f;
            if (!_wallGrabParameters.canMoveInOtherAxisWhileGrabbing)
                _playerMovementData.playerHorizontalSpeed = 0f;
        }

        private void AdjustSpeedOnYAxis()
        {
            _playerMovementData.playerVerticalSpeed = _wallGrabParameters.slideOff
                ? (_slideOffTimer < _wallGrabParameters.slideOffStartAtTime
                    ? 0f
                    : _wallGrabParameters.slideOffSpeed * Time.fixedDeltaTime)
                : 0f;
        }

        private bool IsMovingTowardForwardWall() => _playerMovementInputDataSo.verticalPressed > 0 &&
                                                    _playerMovementData.playerForwardSpeed >= 0;

        private bool IsMovingTowardBackWall() => _playerMovementInputDataSo.verticalPressed < 0 &&
                                                 _playerMovementData.playerForwardSpeed <= 0;

        private bool IsMovingTowardLeftWall() => _playerMovementInputDataSo.horizontalPressed < 0 &&
                                                 _playerMovementData.playerHorizontalSpeed <= 0;

        private bool IsMovingTowardRightWall() => _playerMovementInputDataSo.horizontalPressed > 0f &&
                                                  _playerMovementData.playerHorizontalSpeed >= 0f;

        private bool IsGrabbingLeftWall() => _playerMovementData.grabbedToLeftWall;
        private bool IsGrabbingForwardWall() => _playerMovementData.grabbedToForwardWall;
        private bool IsGrabbingBackWall() => _playerMovementData.grabbedToBackWall;
        private bool IsGrabbingRightWall() => _playerMovementData.grabbedToRightWall;
    }
}