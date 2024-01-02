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
        public float wallJumpTimer = 0f;
        public float wallJumpCooldown = 0.2f;
        private Transform _playerTransform;
        private Collider closestHook;
        private MovementProcessesEvents _movementProcessesEvents;

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
            GetHookInArea();

            if (!_playerMovementInputDataSo.hookPressed) return;
            _playerMovementInputDataSo.hookPressed = false;
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
            Debug.Log(Vector3.Angle(_playerTransform.forward, (target.transform.position - _playerTransform.position)));
            return Vector3.Angle(_playerTransform.forward, (_playerTransform.position - target.transform.position)) <
                   _hookParameters.detectionAngle;
        }
    }
}