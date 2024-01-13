using MovementPackage.Runtime.Scripts.Parameters;
using UnityEngine;

namespace MovementPackage.Runtime.Scripts.MovementProcesses
{
    public class PlayerDashProcess : IMovementProcess
    {
        private DashParameters _dashParameters;
        private PlayerMovementData _playerMovementData;
        private PlayerMovementInputDataSo _playerMovementInputDataSo;
        private float _timer;
        private MovementProcessesEvents _movementProcessesEvents;
        private Vector3 _playerInitialPosition;
        private Transform _playerTransform;
        private Vector3 _playerDestination;

        public void Initialize(PlayerMovementData playerMovementData,
            PlayerMovementInputDataSo playerMovementInputDataSo, DashParameters dashParameters,
            Transform playerTransform, MovementProcessesEvents movementProcessesEvents)
        {
            _playerMovementData = playerMovementData;
            _playerMovementInputDataSo = playerMovementInputDataSo;
            _dashParameters = dashParameters;
            _movementProcessesEvents = movementProcessesEvents;
            _playerTransform = playerTransform;
        }

        public void ProcessFixedUpdate()
        {
            ProcessDashTimer();
            if (_playerMovementData.dashing)
            {
                float curveValue = _dashParameters.dashMovement.Evaluate(_timer / _dashParameters.dashTime);
                // Calculate the next position
                
                Vector3 nextPosition = (Vector3.Lerp(_playerInitialPosition, _playerDestination, curveValue) -
                                        _playerTransform.position);
                _playerMovementData.playerHorizontalSpeed = nextPosition.x;
                _playerMovementData.playerVerticalSpeed = nextPosition.y;
                _playerMovementData.playerForwardSpeed = nextPosition.z;
            }

            if (!_playerMovementInputDataSo.dashPressed) return;
            StartDash();
        }

        private void StartDash()
        {
            if (_playerMovementData.playerHorizontalSpeed == 0 && _playerMovementData.playerForwardSpeed == 0)
                return;

            _playerInitialPosition = _playerTransform.position;

            _playerDestination = _playerInitialPosition + (Vector3.right * ((_dashParameters.enableDashDirectionX? 1f : 0f ) * _playerMovementInputDataSo.horizontalPressed) +
                                                            // Vector3.up * ((_dashParameters.enableDashDirectionY? 1f : 0f ) * _playerMovementData.playerVerticalSpeed) +
                                                            Vector3.forward * ((_dashParameters.enableDashDirectionZ? 1f : 0f ) * _playerMovementInputDataSo.verticalPressed))
                                                           .normalized *
                                                           _dashParameters.dashDistance;
            _playerMovementInputDataSo.dashPressed = false;
            _playerMovementData.dashing = true;

            if (_dashParameters.blockInput)
            {
                _playerMovementInputDataSo.inputBlocked = true;
                _playerMovementData.playerHorizontalSpeed = 0f;
                _playerMovementData.playerForwardSpeed = 0f;
                _playerMovementInputDataSo.horizontalPressed = 0f;
                _playerMovementInputDataSo.verticalPressed = 0f;
            }

            _movementProcessesEvents.DashStart?.Invoke();
        }

        private void ProcessDashTimer()
        {
            if (!_playerMovementData.dashing) return;
            if (_dashParameters.dashTime > _timer)
                _timer += Time.fixedDeltaTime;
            else
            {
                _playerMovementData.dashing = false;
                if (_dashParameters.blockInput)
                    _playerMovementInputDataSo.inputBlocked = false;
                _timer = 0f;
                _movementProcessesEvents.DashFinish?.Invoke();
            }
        }
    }
}