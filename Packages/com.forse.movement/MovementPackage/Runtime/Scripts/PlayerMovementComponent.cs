using System;
using System.Collections.Generic;
using MovementPackage.Runtime.Scripts.CustomAttributes;
using MovementPackage.Runtime.Scripts.MovementProcesses;
using MovementPackage.Runtime.Scripts.Parameters;
using UnityEngine;

namespace MovementPackage.Runtime.Scripts
{
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(PlayerMovementCollision))]
    [RequireComponent(typeof(CoroutineHelper))]
    public class PlayerMovementComponent : MonoBehaviour
    {
        public event EventHandler Jumped;
        public event EventHandler<float> Moving;
        public event EventHandler<bool> Grabbing;
        public event EventHandler<bool> Crouching;

        public PlayerMovementInputDataSo playerMovementInputDataSo;
        public bool lookAtMovementDirection = false;

        public bool gravityEnabled = true;
        public bool walkEnabled = true;
        public bool jumpEnabled = true;
        public bool wallJumpEnabled = false;
        public bool crouchEnabled = false;
        public bool hookEnabled = false;
        public bool wallGrabEnabled = false;

        [TabMenu("Walk")] [SerializeField] private WalkParameters walkParameters;
        [TabMenu("Gravity")][SerializeField] private GravityParameters gravityParameters;
        [TabMenu("Jump")][SerializeField] private JumpParameters jumpParameters;
        [TabMenu("Crouch")][SerializeField] private CrouchParameters crouchParameters;
        [TabMenu("Wall Grab")][SerializeField] private WallGrabParameters wallGrabParameters;
        [TabMenu("Wall Jump")][SerializeField] private WallJumpParameters wallJumpParameters;
        [TabMenu("Hook")][SerializeField] private HookParameters hookParameters;
        private PlayerMovementData _playerMovementData;

        private PlayerGravityProcess _playerGravityProcess;
        private PlayerMovementCollision _playerMovementCollision;
        private CharacterController _characterController;
        private PlayerJumpProcess _playerJumpProcess;
        private PlayerWalkProcess _playerWalkProcess;
        private PlayerWallJumpProcess _playerWallJumpProcess;
        private PlayerWallGrabProcess _playerWallGrabProcess;
        private PlayerCrouchProcess _playerCrouchProcess;
        private PlayerHookProcess _playerHookProcess;
        private List<IMovementProcess> _actions;
        public MovementProcessesEvents Events = new MovementProcessesEvents();

        private void OnEnable()
        {
            _actions = new List<IMovementProcess>();

            _playerMovementData = new PlayerMovementData();
            _characterController = GetComponent<CharacterController>();
            
            InitializeCollisionComponent();

            AddGravityProcess();
            AddCrouchProcess();
            AddJumpProcess();
            AddWallJumpProcess();
            AddWalkProcess();
            AddWallGrabProcess();
            AddHookProcess();

            void AddWalkProcess()
            {
                if (!jumpEnabled) return;
                if (!walkEnabled) return;
                _playerWalkProcess = new PlayerWalkProcess();
                _playerWalkProcess.Initialize(playerMovementInputDataSo, _playerMovementData, walkParameters);
                _actions.Add(_playerWalkProcess);
            }

            void AddWallJumpProcess()
            {
                if (!jumpEnabled) return;
                if (!wallJumpEnabled) return;
                _playerWallJumpProcess = new PlayerWallJumpProcess();
                _playerWallJumpProcess.Initialize(_playerMovementData, playerMovementInputDataSo, wallJumpParameters);
                _actions.Add(_playerWallJumpProcess);
            }

            void AddWallGrabProcess()
            {
                if (!wallGrabEnabled) return;
                _playerWallGrabProcess = new PlayerWallGrabProcess();
                _playerWallGrabProcess.Initialize(_playerMovementData, playerMovementInputDataSo, wallGrabParameters);
                _actions.Add(_playerWallGrabProcess);
            }

            void AddJumpProcess()
            {
                if (!jumpEnabled) return;
                _playerJumpProcess = new PlayerJumpProcess();
                _playerJumpProcess.Initialize(_playerMovementData, playerMovementInputDataSo, jumpParameters,
                    GetComponent<CoroutineHelper>());
                _playerJumpProcess.Jumped += OnJump;
                _actions.Add(_playerJumpProcess);
            }

            void AddCrouchProcess()
            {
                if (!crouchEnabled) return;
                _playerCrouchProcess = new PlayerCrouchProcess();
                _playerCrouchProcess.Initialize(_playerMovementData, playerMovementInputDataSo, _characterController,
                    crouchParameters);
                _actions.Add(_playerCrouchProcess);
            }

            void AddHookProcess()
            {
                if (!hookEnabled) return;
                _playerHookProcess = new PlayerHookProcess();
                _playerHookProcess.Initialize(_playerMovementData, playerMovementInputDataSo, hookParameters, this.transform, Events, GetComponent<CoroutineHelper>());
                _actions.Add(_playerHookProcess);
            }

            void AddGravityProcess()
            {
                if (!gravityEnabled) return;
                _playerGravityProcess = new PlayerGravityProcess();
                _playerGravityProcess.Initialize(_playerMovementData, gravityParameters);
                _actions.Add(_playerGravityProcess);
            }

            void InitializeCollisionComponent()
            {
                _playerMovementCollision = GetComponent<PlayerMovementCollision>();
                _playerMovementCollision.Initialize(_playerMovementData);
                _actions.Add(_playerMovementCollision);
            }
        }

        private void OnDisable() => _playerJumpProcess.Jumped -= OnJump;

        private void FixedUpdate()
        {
            foreach (var action in _actions)
                action.ProcessFixedUpdate();
            
            ExecuteMovement();
            SendAnimationEvents();
            if (lookAtMovementDirection)
                LookAtDirection();
            playerMovementInputDataSo.jumpPressed = false;
            playerMovementInputDataSo.jumpReleased = false;
        }

        private void OnJump(object sender, EventArgs args) => Jumped?.Invoke(this, null);

        private void SendAnimationEvents()
        {
            Moving?.Invoke(this,Mathf.Abs(playerMovementInputDataSo.horizontalPressed) +
                          Mathf.Abs(playerMovementInputDataSo.verticalPressed));

            if (wallGrabEnabled)
            {
                Grabbing?.Invoke(this,_playerMovementData.IsGrabbedToWall());
            }

            if (crouchEnabled)
                Crouching?.Invoke(this,_playerMovementData.crouching);
        }

        private void ExecuteMovement()
        {
            _characterController.Move(new Vector3(_playerMovementData.playerHorizontalSpeed,
                _playerMovementData.playerVerticalSpeed, _playerMovementData.playerForwardSpeed));
        }

        private void LookAtDirection()
        {
            this.transform.LookAt(
                this.transform.position +
                Vector3.right * _playerMovementData.playerHorizontalSpeed
                + Vector3.forward * _playerMovementData.playerForwardSpeed, Vector3.up);
        }


    }
}