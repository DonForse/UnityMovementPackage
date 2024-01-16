using System;
using System.Collections.Generic;
using MovementPackage.Runtime.Scripts.CustomAttributes;
using MovementPackage.Runtime.Scripts.MovementProcesses;
using MovementPackage.Runtime.Scripts.Parameters;
using UnityEngine;
using UnityEngine.Serialization;

namespace MovementPackage.Runtime.Scripts
{
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(PlayerMovementCollision))]
    [RequireComponent(typeof(CoroutineHelper))]
    public class PlayerMovementComponent : MonoBehaviour
    {
        public bool enableXAxis;
        public bool enableYAxis;
        public bool enableZAxis;
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
        public bool dashEnabled = false;
        public bool wallGrabEnabled = false;

        [TabMenu("Walk")] [SerializeField] private WalkParameters walkParameters;
        [TabMenu("Gravity")][SerializeField] private GravityParameters gravityParameters;
        [TabMenu("Jump")][SerializeField] private JumpParameters jumpParameters;
        [TabMenu("Crouch")][SerializeField] private CrouchParameters crouchParameters;
        [TabMenu("Wall Grab")][SerializeField] private WallGrabParameters wallGrabParameters;
        [TabMenu("Wall Jump")][SerializeField] private WallJumpParameters wallJumpParameters;
        [TabMenu("Hook")][SerializeField] private HookParameters hookParameters;
        [TabMenu("Dash")] [SerializeField] private DashParameters dashParameters;
        public PlayerMovementData PlayerMovementData;

        private PlayerGravityProcess _playerGravityProcess;
        private PlayerMovementCollision _playerMovementCollision;
        private CharacterController _characterController;
        private PlayerJumpProcess _playerJumpProcess;
        private PlayerWalkProcess _playerWalkProcess;
        private PlayerWallJumpProcess _playerWallJumpProcess;
        private PlayerWallGrabProcess _playerWallGrabProcess;
        private PlayerCrouchProcess _playerCrouchProcess;
        private PlayerHookProcess _playerHookProcess;
        private PlayerDashProcess _playerDashProcess;
        private List<IMovementProcess> _actions;
        public MovementProcessesEvents Events = new MovementProcessesEvents();

        private void OnEnable()
        {
            _actions = new List<IMovementProcess>();
            PlayerMovementData = new PlayerMovementData();
            _characterController = GetComponent<CharacterController>();
            
            InitializeCollisionComponent();

            AddGravityProcess();
            AddCrouchProcess();
            AddWallJumpProcess();
            AddWalkProcess();
            AddWallGrabProcess();
            AddHookProcess();
            AddPlayerDashProcess();
            AddJumpProcess();

            void AddWalkProcess()
            {
                if (!walkEnabled) return;
                _playerWalkProcess = new PlayerWalkProcess();
                _playerWalkProcess.Initialize(playerMovementInputDataSo, PlayerMovementData, walkParameters);
                _actions.Add(_playerWalkProcess);
            }

            void AddWallJumpProcess()
            {
                if (!jumpEnabled) return;
                if (!wallJumpEnabled) return;
                _playerWallJumpProcess = new PlayerWallJumpProcess();
                _playerWallJumpProcess.Initialize(PlayerMovementData, playerMovementInputDataSo, wallJumpParameters);
                _actions.Add(_playerWallJumpProcess);
            }

            void AddWallGrabProcess()
            {
                if (!jumpEnabled) return;
                if (!wallGrabEnabled) return;
                _playerWallGrabProcess = new PlayerWallGrabProcess();
                _playerWallGrabProcess.Initialize(PlayerMovementData, playerMovementInputDataSo, wallGrabParameters);
                _actions.Add(_playerWallGrabProcess);
            }

            void AddJumpProcess()
            {
                if (!jumpEnabled) return;
                _playerJumpProcess = new PlayerJumpProcess();
                _playerJumpProcess.Initialize(PlayerMovementData, playerMovementInputDataSo, jumpParameters,
                    GetComponent<CoroutineHelper>(), Events);
                _actions.Add(_playerJumpProcess);
            }

            void AddCrouchProcess()
            {
                if (!crouchEnabled) return;
                _playerCrouchProcess = new PlayerCrouchProcess();
                _playerCrouchProcess.Initialize(PlayerMovementData, playerMovementInputDataSo, _characterController,
                    crouchParameters);
                _actions.Add(_playerCrouchProcess);
            }

            void AddHookProcess()
            {
                if (!hookEnabled) return;
                _playerHookProcess = new PlayerHookProcess();
                _playerHookProcess.Initialize(PlayerMovementData, playerMovementInputDataSo, hookParameters, this.transform, Events);
                _actions.Add(_playerHookProcess);
            }

            void AddPlayerDashProcess()
            {
                if (!dashEnabled) return;
                _playerDashProcess = new PlayerDashProcess();
                _playerDashProcess.Initialize(PlayerMovementData, playerMovementInputDataSo, dashParameters, this.transform, Events);
                _actions.Add(_playerDashProcess);
            }

            void AddGravityProcess()
            {
                if (!gravityEnabled) return;
                _playerGravityProcess = new PlayerGravityProcess();
                _playerGravityProcess.Initialize(PlayerMovementData, gravityParameters);
                _actions.Add(_playerGravityProcess);
            }

            void InitializeCollisionComponent()
            {
                _playerMovementCollision = GetComponent<PlayerMovementCollision>();
                _playerMovementCollision.Initialize(PlayerMovementData, Events);
                _actions.Add(_playerMovementCollision);
            }
        }
        
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
        
        private void SendAnimationEvents()
        {
            Moving?.Invoke(this,Mathf.Abs(playerMovementInputDataSo.horizontalPressed) +
                          Mathf.Abs(playerMovementInputDataSo.verticalPressed));

            if (wallGrabEnabled)
            {
                Grabbing?.Invoke(this,PlayerMovementData.IsGrabbedToWall());
            }

            if (crouchEnabled)
                Crouching?.Invoke(this,PlayerMovementData.crouching);
        }

        private void ExecuteMovement()
        {
            _characterController.Move(new Vector3(
                PlayerMovementData.playerHorizontalSpeed * BoolToInt(enableXAxis),
                PlayerMovementData.playerVerticalSpeed * BoolToInt(enableYAxis), 
                PlayerMovementData.playerForwardSpeed * BoolToInt(enableZAxis)) );
        }

        private void LookAtDirection()
        {
            this.transform.LookAt(
                this.transform.position +
                Vector3.right * PlayerMovementData.playerHorizontalSpeed
                + Vector3.forward * PlayerMovementData.playerForwardSpeed, Vector3.up);
        }

        private float BoolToInt(bool b)
        {
            return b ? 1f : 0f;
        }
    }
}