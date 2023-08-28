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
        
        public bool gravityEnabled = true;
        public bool walkEnabled = true;
        public bool jumpEnabled = true;
        public bool wallJumpEnabled = false;
        public bool crouchEnabled = false;
        public bool wallGrabEnabled = false;

        [TabMenu("Walk")] [SerializeField] private WalkParameters walkParameters;
        [TabMenu("Gravity")][SerializeField] private GravityParameters gravityParameters;
        [TabMenu("Jump")][SerializeField] private JumpParameters jumpParameters;
        [TabMenu("Crouch")][SerializeField] private CrouchParameters crouchParameters;
        [TabMenu("Wall Grab")][SerializeField] private WallGrabParameters wallGrabParameters;
        [TabMenu("Wall Jump")][SerializeField] private WallJumpParameters wallJumpParameters;

        private PlayerMovementInputData _playerMovementInputData;
        private PlayerMovementData _playerMovementData;

        private PlayerGravityProcess _playerGravityProcess;
        private PlayerMovementCollision _playerMovementCollision;
        private CharacterController _characterController;
        private PlayerJumpProcess _playerJumpProcess;
        private PlayerWalkProcess _playerWalkProcess;
        private PlayerWallJumpProcess _playerWallJumpProcess;
        private PlayerWallGrabProcess _playerWallGrabProcess;
        private PlayerCrouchProcess _playerCrouchProcess;
        private List<IMovementProcess> _actions;

        private void OnEnable()
        {
            _actions = new List<IMovementProcess>();

            _playerMovementData = new PlayerMovementData();
            _playerMovementInputData = new PlayerMovementInputData();
            _characterController = GetComponent<CharacterController>();

            _playerMovementCollision = GetComponent<PlayerMovementCollision>();
            _playerMovementCollision.Initialize(_playerMovementData);
            _actions.Add(_playerMovementCollision);

            if (gravityEnabled)
            {
                _playerGravityProcess = new PlayerGravityProcess();
                _playerGravityProcess.Initialize(_playerMovementData, gravityParameters);
                _actions.Add(_playerGravityProcess);
            }

            if (crouchEnabled)
            {
                _playerCrouchProcess = new PlayerCrouchProcess();
                _playerCrouchProcess.Initialize(_playerMovementData, _playerMovementInputData, _characterController, crouchParameters);
                _actions.Add(_playerCrouchProcess);
            }

            if (jumpEnabled)
            {
                _playerJumpProcess = new PlayerJumpProcess();
                _playerJumpProcess.Initialize(_playerMovementData, _playerMovementInputData, jumpParameters, GetComponent<CoroutineHelper>());
                _playerJumpProcess.Jumped += OnJump;
                _actions.Add(_playerJumpProcess);
                
                if (wallGrabEnabled)
                {
                    _playerWallGrabProcess = GetComponent<PlayerWallGrabProcess>();    
                    _playerWallGrabProcess.Initialize(_playerMovementData, _playerMovementInputData, wallGrabParameters);
                    _actions.Add(_playerWallGrabProcess);
                }

                if (wallJumpEnabled)
                {
                    _playerWallJumpProcess = GetComponent<PlayerWallJumpProcess>(); 
                    _playerWallJumpProcess.Initialize(_playerMovementData, _playerMovementInputData, wallJumpParameters);
                    _actions.Add(_playerWallJumpProcess);
                }
            }

            if (walkEnabled)
            {
                _playerWalkProcess = new PlayerWalkProcess();
                _playerWalkProcess.Initialize(_playerMovementInputData, _playerMovementData, walkParameters);
                _actions.Add(_playerWalkProcess);
            }
        }

        private void OnDisable()
        {
            _playerJumpProcess.Jumped -= OnJump;
        }

        private void FixedUpdate()
        {
            foreach (var action in _actions)
                action.ProcessFixedUpdate();
            
            LookAtDirection();
            ExecuteMovement();
            SendAnimationEvents();

            _playerMovementInputData.jumpPressed = false;
            _playerMovementInputData.jumpReleased = false;
        }

        private void OnJump(object sender, EventArgs args) => Jumped?.Invoke(this, null);

        private void SendAnimationEvents()
        {
            Moving?.Invoke(this,Mathf.Abs(_playerMovementInputData.horizontalPressed) +
                          Mathf.Abs(_playerMovementInputData.verticalPressed));

            if (wallGrabEnabled)
            {
                Grabbing?.Invoke(this,_playerMovementData.grabbedToRightWall ||
                                      _playerMovementData.grabbedToLeftWall || 
                                      _playerMovementData.grabbedToBackWall ||
                                      _playerMovementData.grabbedToForwardWall);
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

        void Update()
        {
            _playerMovementInputData.jumpHold = Input.GetButton("Jump");
            _playerMovementInputData.jumpPressed = _playerMovementInputData.jumpPressed || Input.GetButtonDown("Jump");
            _playerMovementInputData.jumpReleased = _playerMovementInputData.jumpReleased || Input.GetButtonUp("Jump");
            _playerMovementInputData.horizontalPressed = Input.GetAxis("Horizontal");
            _playerMovementInputData.verticalPressed = Input.GetAxis("Vertical");
            _playerMovementInputData.crouchPressed = Input.GetKey(KeyCode.LeftControl);
        }
    }
}