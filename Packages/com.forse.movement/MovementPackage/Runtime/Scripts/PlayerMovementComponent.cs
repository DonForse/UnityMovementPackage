using System;
using System.Collections.Generic;
using UnityEngine;

namespace MovementPackage.Runtime.Scripts
{
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(PlayerMovementCollision))]
    [RequireComponent(typeof(PlayerJumpComponent))]
    [RequireComponent(typeof(PlayerWalkComponent))]
    [RequireComponent(typeof(PlayerGravityComponent))]
    [RequireComponent(typeof(PlayerWallJumpComponent))]
    [RequireComponent(typeof(PlayerWallGrabComponent))]
    [RequireComponent(typeof(PlayerCrouchComponent))]
    public class PlayerMovementComponent : MonoBehaviour
    {
        public event EventHandler Jumped;
        public event EventHandler<float> Moving;
        public event EventHandler<bool> Grabbing;
        public event EventHandler<bool> Crouching;
        
        [SerializeField] private bool gravityEnabled = true;
        [SerializeField] private bool walkEnabled = true;
        [SerializeField] private bool jumpEnabled = true;
        [SerializeField] private bool wallJumpEnabled = false;
        [SerializeField] private bool crouchEnabled = false;
        [SerializeField] private bool wallGrabEnabled = false;

        private PlayerMovementInputData _playerMovementInputData;
        private PlayerMovementData _playerMovementData;

        private PlayerGravityComponent _playerGravityComponent;
        private PlayerMovementCollision _playerMovementCollision;
        private CharacterController _characterController;
        private PlayerJumpComponent _playerJumpComponent;
        private PlayerWalkComponent _playerWalkComponent;
        private PlayerWallJumpComponent _playerWallJumpComponent;
        private PlayerWallGrabComponent _playerWallGrabComponent;
        private PlayerCrouchComponent _playerCrouchComponent;
        private List<IMovementComponent> _actions;

        private void OnEnable()
        {
            _actions = new List<IMovementComponent>();

            _playerMovementData = new PlayerMovementData();
            _playerMovementInputData = new PlayerMovementInputData();
            _characterController = GetComponent<CharacterController>();

            _playerMovementCollision = GetComponent<PlayerMovementCollision>();
            _playerMovementCollision.Initialize(_playerMovementData);
            _actions.Add(_playerMovementCollision);

            if (gravityEnabled)
            {
                _playerGravityComponent = GetComponent<PlayerGravityComponent>();
                _playerGravityComponent.Initialize(_playerMovementData);
                _actions.Add(_playerGravityComponent);
            }

            if (crouchEnabled)
            {
                _playerCrouchComponent = GetComponent<PlayerCrouchComponent>();
                _playerCrouchComponent.Initialize(_playerMovementData, _playerMovementInputData, _characterController);
                _actions.Add(_playerCrouchComponent);
            }

            if (jumpEnabled)
            {
                _playerJumpComponent = GetComponent<PlayerJumpComponent>();
                _playerJumpComponent.Initialize(_playerMovementData, _playerMovementInputData);
                _playerJumpComponent.Jumped.AddListener(OnJump);
                _actions.Add(_playerJumpComponent);
            }

            if (walkEnabled)
            {
                _playerWalkComponent = GetComponent<PlayerWalkComponent>();
                _playerWalkComponent.Initialize(_playerMovementInputData, _playerMovementData);
                _actions.Add(_playerWalkComponent);
            }

            if (wallGrabEnabled)
            {
                _playerWallGrabComponent = GetComponent<PlayerWallGrabComponent>();    
                _playerWallGrabComponent.Initialize(_playerMovementData, _playerMovementInputData);
                _actions.Add(_playerWallGrabComponent);
            }

            if (wallJumpEnabled)
            {
                _playerWallJumpComponent = GetComponent<PlayerWallJumpComponent>(); 
                _playerWallJumpComponent.Initialize(_playerMovementData, _playerMovementInputData);
                _actions.Add(_playerWallJumpComponent);
            }
        }

        private void OnDisable()
        {
            _playerJumpComponent.Jumped.RemoveListener(OnJump);
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

        private void OnJump() => Jumped?.Invoke(this, null);

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