using System;
using System.Collections.Generic;
using System.Linq;
using Features.Game.Movement;
using UnityEngine;

namespace UnityPackages.MovementPackage.Scripts
{
    public class PlayerMovementCollision : MonoBehaviour, IMovementComponent
    {
        [Header("Layers")] public LayerMask groundLayer;

        [Space] [Header("Collision")] [SerializeField]
        float collisionRadius = 0.25f;

        [SerializeField] private bool groundDetection = true;
        [SerializeField] private Vector3 bottomOffset = Vector3.down;
        [SerializeField] private bool xAxisDetection= true;
        [SerializeField] private Vector3 rightOffset = new Vector3(1f, 0.5f, 0);
        [SerializeField] private Vector3 leftOffset = new Vector3(-1f, 0.5f, 0);
        [SerializeField] private bool zAxisDetection= true;
        [SerializeField] private Vector3 forwardOffset = new Vector3(0, 0.5f, 1f);
        [SerializeField] private Vector3 backOffset = new Vector3(0, 0.5f, -1f);

        [Space] [Header("Detection")] [SerializeField]
        private float detectionCollisionRadius = 0.5f;

        [SerializeField] private bool closeWallDetection= false;
        [SerializeField] private Vector3 wallRightOffset = new Vector3(1.5f, 0.5f, 0);
        [SerializeField] private Vector3 wallLeftOffset = new Vector3(-1.5f, 0.5f, 0);
        [SerializeField] private Vector3 wallForwardOffset = new Vector3(0, 0.5f, 1.5f);
        [SerializeField] private Vector3 wallBackOffset = new Vector3(0, 0.5f, -1.5f);
        private PlayerMovementData _playerMovementData;
        private List<Action> _actions;

        public void Initialize(PlayerMovementData playerMovementData)
        {
            _playerMovementData = playerMovementData;

            _actions = new List<Action>();
            if (groundDetection)
            {
                _actions.Add(DetectGroundCollision);
            }

            if (xAxisDetection)
            {
                _actions.Add(DetectXAxisCollision);
            }

            if (zAxisDetection)
            {
                _actions.Add(DetectZAxisCollision);
            }

            if (closeWallDetection)
            {
                _actions.Add(DetectCloseWalls);
            }
        }

        public void ProcessFixedUpdate()
        {
            foreach (var action in _actions)
            {
                action();
            }
        }

        private void DetectCloseWalls()
        {
            if (xAxisDetection)
            {
                _playerMovementData.closeRightWall = Physics
                    .OverlapSphere(transform.position + wallRightOffset, detectionCollisionRadius, groundLayer)
                    .Any();
                _playerMovementData.closeLeftWall = Physics
                    .OverlapSphere(transform.position + wallLeftOffset, detectionCollisionRadius, groundLayer)
                    .Any();
            }
            if (zAxisDetection)
            {
                _playerMovementData.closeForwardWall = Physics
                    .OverlapSphere(transform.position + wallForwardOffset, detectionCollisionRadius, groundLayer)
                    .Any();
                _playerMovementData.closeBackWall = Physics
                    .OverlapSphere(transform.position + wallBackOffset, detectionCollisionRadius, groundLayer)
                    .Any();
            }
        }

        private void DetectZAxisCollision()
        {
            _playerMovementData.collidingForwardWall = Physics
                .OverlapSphere(transform.position + forwardOffset, collisionRadius, groundLayer)
                .Any();
            _playerMovementData.collidingBackWall =
                Physics.OverlapSphere(transform.position + backOffset, collisionRadius, groundLayer).Any();
        }

        private void DetectXAxisCollision()
        {
            _playerMovementData.collidingRightWall = Physics
                .OverlapSphere(transform.position + rightOffset, collisionRadius, groundLayer)
                .Any();
            _playerMovementData.collidingLeftWall =
                Physics.OverlapSphere(transform.position + leftOffset, collisionRadius, groundLayer).Any();
        }

        private void DetectGroundCollision()
        {
            _playerMovementData.collidingGround =
                Physics.OverlapSphere(transform.position + bottomOffset, collisionRadius, groundLayer).Any();
        }

        void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            if (groundDetection)
                Gizmos.DrawWireSphere(transform.position + bottomOffset, collisionRadius);
            if (xAxisDetection)
            {
                Gizmos.DrawWireSphere(transform.position + rightOffset, collisionRadius);
                Gizmos.DrawWireSphere(transform.position + leftOffset, collisionRadius);
            }

            if (zAxisDetection)
            {
                Gizmos.DrawWireSphere(transform.position + backOffset, collisionRadius);
                Gizmos.DrawWireSphere(transform.position + forwardOffset, collisionRadius);
            }

            Gizmos.color = Color.cyan;
            if (!closeWallDetection) return;
            if (xAxisDetection)
            {
                Gizmos.DrawWireSphere(transform.position + wallRightOffset, detectionCollisionRadius);
                Gizmos.DrawWireSphere(transform.position + wallLeftOffset, detectionCollisionRadius);
            }

            if (zAxisDetection)
            {
                Gizmos.DrawWireSphere(transform.position + wallForwardOffset, detectionCollisionRadius);
                Gizmos.DrawWireSphere(transform.position + wallBackOffset, detectionCollisionRadius);
            }
        }
    }
}