using System.Linq;
using MovementPackage.Runtime.Scripts.Parameters;
using UnityEngine;

namespace MovementPackage.Runtime.Scripts.MovementProcesses
{
    public class PlayerHookProcess : IMovementProcess
    {
        private HookParameters _hookParameters;
        private PlayerMovementData _playerMovementData;
        private PlayerMovementInputDataSo _playerMovementInputDataSo;

        private Transform _playerTransform;
        private Collider closestHook;
        private Vector3 _hookDestination;
        private MovementProcessesEvents _movementProcessesEvents;

        private float hookTimer = 0f;
        private float _startTime;
        private Vector3 _initialPlayerPosition;
        private bool _hookOff;
        private float startHookOffTime;
        private Vector3 _hookOffDestination;
        private GameObject _hookTarget;
        private bool _windup;
        private float _windupTimer;

        public void Initialize(PlayerMovementData playerMovementData,
            PlayerMovementInputDataSo playerMovementInputDataSo,
            HookParameters hookParameters, Transform player, MovementProcessesEvents movementProcessesEvents)
        {
            _playerMovementData = playerMovementData;
            _playerMovementInputDataSo = playerMovementInputDataSo;
            _hookParameters = hookParameters;
            _playerTransform = player;
            _movementProcessesEvents = movementProcessesEvents;
        }

        public void ProcessFixedUpdate()
        {
            if (_playerMovementData.hooking)
            {
                ProcessHook();
                return;
            }

            GetHookInArea();
            if (!_playerMovementInputDataSo.hookPressed) return;
            _playerMovementInputDataSo.hookPressed = false;
            if (closestHook == null) return;
            InitializeHook();
            return;
        }

        private void ProcessHook()
        {
            if (_hookParameters.windUp && _windup)
            {
                ProcessWindUp();
                return;
            }
            if (_hookOff)
            {
                ProcessHookOff();
                return;
            }
            ProcessHookIn();
        }

        private void ProcessWindUp()
        {
            _windupTimer += Time.fixedDeltaTime;
            if (!(_windupTimer > _hookParameters.windUpTime)) return;
            
            _windup = false;
            _startTime = Time.fixedTime;
            _initialPlayerPosition = _playerTransform.position;
            
            _movementProcessesEvents.HookWindUpComplete?.Invoke(_hookTarget);
        }

        private void InitializeHook()
        {
            _hookTarget = closestHook.gameObject;
            _hookDestination = closestHook.transform.position;
            _startTime = Time.fixedTime;
            _initialPlayerPosition = _playerTransform.position;

            _hookOff = false;
            _playerMovementData.hooking = true;
            if (_hookParameters.blockInput)
                _playerMovementInputDataSo.inputBlocked = true;
            if (_hookParameters.turnOffGravity)
                _playerMovementData.gravityMultiplier = 0f;

            if (_hookParameters.windUp)
            {
                _windup = true;
                _windupTimer = 0f;
            }

            _movementProcessesEvents.HookStart?.Invoke(_hookTarget);
        }

        private void ProcessHookIn()
        {
            var timePassed = Time.fixedTime - _startTime;
            float curveValue = _hookParameters.hookInCurve.Evaluate(timePassed / _hookParameters.hookInTime);

            // Calculate the next position
            Vector3 nextPosition = (Vector3.Lerp(_initialPlayerPosition, _hookDestination, curveValue) -
                                    _playerTransform.position);
            _playerMovementData.playerHorizontalSpeed = nextPosition.x;
            _playerMovementData.playerVerticalSpeed = nextPosition.y;
            _playerMovementData.playerForwardSpeed = nextPosition.z;

            if (Vector3.Distance(_playerTransform.position, _hookDestination) <
                _hookParameters.hookInStopDistance)
            {
                _movementProcessesEvents.HookReachPosition?.Invoke(_hookTarget);

                if (_hookParameters.continueForce)
                {
                    CompleteHook();
                    return;
                }

                InitializeHookOff();
            }
        }

        private void InitializeHookOff()
        {
            startHookOffTime = Time.fixedTime;
            _playerMovementData.playerHorizontalSpeed = 0;
            _playerMovementData.playerVerticalSpeed = 0;
            _playerMovementData.playerForwardSpeed = 0;
            _hookOffDestination = _playerTransform.position +
                                  (_hookDestination - _initialPlayerPosition).normalized *
                                  _hookParameters.hookOutDistance;
            _hookOff = true;
        }

        private void ProcessHookOff()
        {
            var timePassed = Time.fixedTime - startHookOffTime;
            float curveValue = _hookParameters.hookInCurve.Evaluate(timePassed / _hookParameters.hookOffTime);

            // Calculate the next position
            Vector3 nextPosition = (Vector3.Lerp(_hookDestination, _hookOffDestination, curveValue) -
                                    _playerTransform.position);
            _playerMovementData.playerHorizontalSpeed = nextPosition.x;
            _playerMovementData.playerVerticalSpeed = nextPosition.y;
            _playerMovementData.playerForwardSpeed = nextPosition.z;

            if (Vector3.Distance(_playerTransform.position, _hookOffDestination) <
                _hookParameters.hookInStopDistance)
            {
                CompleteHook();
            }
        }

        private void CompleteHook()
        {
            if (_hookParameters.blockInput)
                _playerMovementInputDataSo.inputBlocked = false;
            if (_hookParameters.turnOffGravity)
                _playerMovementData.gravityMultiplier = 1f;
            _hookOff = false;
            _playerMovementData.hooking = false;
            _movementProcessesEvents.HookEnd?.Invoke(_hookTarget);
        }

        private void GetHookInArea()
        {
            var colliders = Physics.OverlapSphere(_playerTransform.position, _hookParameters.hookRangeDistance,
                    _hookParameters.hookLayerMask)
                .Where(IsInAngle);

            if (!colliders.Any() && closestHook != null)
            {
                _movementProcessesEvents.HookLostFocus?.Invoke(closestHook?.gameObject);
                closestHook = null;
            }

            float closestDistance = Mathf.Infinity;

            foreach (var hitCollider in colliders)
            {
                var directionToTarget = hitCollider.transform.position - _playerTransform.position;
                var distance = directionToTarget.magnitude;

                if (hitCollider == closestHook)
                    return;
                if (!(distance < closestDistance)) continue;

                closestDistance = distance;
                closestHook = hitCollider;
                _movementProcessesEvents.HookFocus?.Invoke(closestHook.gameObject);
            }
        }

        private bool IsInAngle(Collider target)
        {
            return Vector3.Angle(_playerTransform.forward, (_playerTransform.position - target.transform.position)) <
                   _hookParameters.detectionAngle;
        }
    }
}