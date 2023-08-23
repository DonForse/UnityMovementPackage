using UnityEngine;
using UnityPackages.MovementPackage.Scripts;

namespace Features.Game.Movement
{
    public class PlayerWallGrabComponent : MonoBehaviour, IMovementComponent
    {
        // [HideInInspector]public UnityEvent GrabbedToWall = new();
        private PlayerMovementData _playerMovementData;
        [SerializeField] private bool slideOff;
        [SerializeField] private float slideOffStartAtTime;
        [SerializeField] private float slideOffSpeed;
        private float _slideOffTimer = 0f;
        public void Initialize(PlayerMovementData playerMovementData)
        {
            _playerMovementData = playerMovementData;
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

            if (_playerMovementData.collidingRightWall)
            {
                if (IsGrabbingRightWall())
                {
                    _playerMovementData.playerVerticalSpeed = slideOff ?
                        (_slideOffTimer < slideOffStartAtTime ? 0f : slideOffSpeed*Time.fixedDeltaTime ) :
                        0f;
                }

                
                if (!IsGrabbingRightWall() && IsMovingTowardRightWall()) _slideOffTimer = 0f;

                _playerMovementData.grabbedToRightWall = IsMovingTowardRightWall();
            }
            else
            {
                _playerMovementData.grabbedToRightWall = false;
            }

            if (_playerMovementData.collidingLeftWall)
            {
                if (IsGrabbingLeftWall())
                    _playerMovementData.playerVerticalSpeed = slideOff ?
                        (_slideOffTimer < slideOffStartAtTime ? 0f : slideOffSpeed * Time.fixedDeltaTime) :
                        0f;

                if (!IsGrabbingLeftWall() && IsMovingTowardLeftWall()) _slideOffTimer = 0f;
                
                _playerMovementData.grabbedToLeftWall = IsMovingTowardLeftWall();
            }
            else
            {
                _playerMovementData.grabbedToLeftWall = false;
            }
        }

        private bool IsMovingTowardLeftWall() => _playerMovementData.playerHorizontalSpeed < 0;
        private bool IsGrabbingLeftWall() => _playerMovementData.grabbedToLeftWall;
        private bool IsGrabbingRightWall() => _playerMovementData.grabbedToRightWall;
        private bool IsMovingTowardRightWall() => _playerMovementData.playerHorizontalSpeed > 0;
    }
}