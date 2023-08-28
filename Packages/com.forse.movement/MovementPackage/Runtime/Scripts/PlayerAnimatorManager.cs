using System;
using JetBrains.Annotations;
using UnityEngine;

namespace MovementPackage.Runtime.Scripts
{
    public class PlayerAnimatorManager : MonoBehaviour
    {
        [SerializeField] private PlayerMovementComponent movementComponent;
        [SerializeField] private Animator animator;
        [SerializeField] private ParticleSystem stepPartycleSystem;
        private static readonly int MoveSpeedKey = Animator.StringToHash("moveSpeed");
        private static readonly int Jump = Animator.StringToHash("jump");
        private static readonly int Grab = Animator.StringToHash("grab");
        private static readonly int Crouch = Animator.StringToHash("crouch");

        private void OnEnable()
        {
            movementComponent.Jumped += SetJump;
            movementComponent.Grabbing += SetGrab;
            movementComponent.Moving += SetSpeed;
            movementComponent.Crouching += SetCrouch;
        }

        private void SetSpeed(object sender, float speed) => animator.SetFloat(MoveSpeedKey, speed);

        private void SetCrouch(object sender, bool value) => animator.SetBool(Crouch, value);

        private void SetGrab(object sender, bool value) => animator.SetBool(Grab, value);

        private void SetJump(object sender, EventArgs e) => animator.SetTrigger(Jump);

        [UsedImplicitly]
        public void TriggerParticleStep() => stepPartycleSystem?.Play();
    }
}