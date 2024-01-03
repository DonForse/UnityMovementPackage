using System.Collections;
using System.Collections.Generic;
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
        private Vector3 hookDestination;
        private MovementProcessesEvents _movementProcessesEvents;

        private float hookTimer = 0f;
        private CoroutineHelper _coroutineHelper;
        private float startTime;
        private float journeyLength;

        public void Initialize(PlayerMovementData playerMovementData,
            PlayerMovementInputDataSo playerMovementInputDataSo,
            HookParameters hookParameters, Transform player, MovementProcessesEvents movementProcessesEvents,
            CoroutineHelper coroutineHelper)
        {
            _playerMovementData = playerMovementData;
            _playerMovementInputDataSo = playerMovementInputDataSo;
            _hookParameters = hookParameters;
            _playerTransform = player;
            _movementProcessesEvents = movementProcessesEvents;
            _coroutineHelper = coroutineHelper;
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
            StartHooking();
            return;
        }

        private void StartHooking()
        {
            if (_hookParameters.continueHookDirection == HookDropOffEnum.ContinueDirection)
            {
                hookDestination = closestHook.transform;
                startTime = Time.fixedTime;
                journeyLength = Vector3.Distance(_playerTransform.position,  hookDestination);
                _playerMovementInputDataSo.inputBlocked = true;
                _playerMovementData.hooking = true;
                _playerMovementData.gravityMultiplier = 0f;
                return;
            }
            else if (_hookParameters.continueHookDirection == HookDropOffEnum.PredefinedPositions)
            {
                return;
            }

        }

        private void ProcessHook()
        {
            float distCovered = (Time.fixedTime - startTime) * _hookParameters.hookJumpInSpeed;
            float fractionOfJourney = distCovered / journeyLength;

            // Calculate the next position
            Vector3 nextPosition = Vector3.Lerp(_playerTransform.position, hookDestination, fractionOfJourney) - _playerTransform.position;
            _playerMovementData.playerHorizontalSpeed = nextPosition.x;
            _playerMovementData.playerVerticalSpeed = nextPosition.y;
            _playerMovementData.playerForwardSpeed = nextPosition.z;

            if (Vector3.Distance(_playerTransform.position, hookDestination) <
                _hookParameters.hookInStopDistance)
            {
                _playerMovementInputDataSo.inputBlocked = false;
                _playerMovementData.hooking = false;
                _playerMovementData.gravityMultiplier = 1f;
            }
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