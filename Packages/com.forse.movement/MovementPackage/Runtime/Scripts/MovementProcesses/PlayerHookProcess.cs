using System.Collections;
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
        private Transform hookDestination;
        private MovementProcessesEvents _movementProcessesEvents;
        
        private float hookTimer = 0f;
        private CoroutineHelper _coroutineHelper;

        public void Initialize(PlayerMovementData playerMovementData,
            PlayerMovementInputDataSo playerMovementInputDataSo,
            HookParameters hookParameters, Transform player, MovementProcessesEvents movementProcessesEvents, CoroutineHelper coroutineHelper)
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
            GetHookInArea();

            if (!_playerMovementInputDataSo.hookPressed) return;
            _playerMovementInputDataSo.hookPressed = false;
            Debug.Log("HOOK");
            if (closestHook == null) return;
            if (_playerMovementData.hooking) return;
            Debug.Log("HOOK2");
                if (_hookParameters.continueHookDirection == HookDropOffEnum.ContinueDirection)
                {
                    hookDestination = closestHook.transform;
                    _coroutineHelper.StartCoroutineFrom(HookToDestination);;
                    return;
                }
                else if (_hookParameters.continueHookDirection == HookDropOffEnum.PredefinedPositions)
                {
                    return;
                }
                return;
            
            
        }

        private IEnumerator HookToDestination()
        {
            Debug.Log("start hook");
            _playerMovementInputDataSo.inputBlocked = true;
            _playerMovementData.hooking = true;
            _playerMovementData.gravityMultiplier = 0;
            Debug.Log(Vector3.Distance(_playerTransform.transform.position, hookDestination.position));
            while (Vector3.Distance(_playerTransform.transform.position, hookDestination.position) >
                   _hookParameters.hookInStopDistance)
            {
                Vector3 direction = (hookDestination.position - _playerTransform.position);
                _playerMovementData.playerHorizontalSpeed =
                    direction.x * _hookParameters.hookJumpInSpeed * Time.fixedDeltaTime;
                _playerMovementData.playerForwardSpeed =
                    direction.z * _hookParameters.hookJumpInSpeed * Time.fixedDeltaTime;
                _playerMovementData.playerVerticalSpeed =
                    direction.y * _hookParameters.hookJumpInSpeed * Time.fixedDeltaTime;
                yield return null;
            }

            Debug.Log("end hook");
            {
                // Arrived at target
                _playerMovementData.gravityMultiplier = 1;
                _playerMovementData.hooking = false;
                _playerMovementInputDataSo.inputBlocked = false;

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