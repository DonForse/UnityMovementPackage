using JetBrains.Annotations;
using UnityEngine;

namespace Features.Game.Animations
{
    public class PlayerAnimatorManager : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        [SerializeField] private ParticleSystem _stepPartycleSystem;
        private static readonly int MoveSpeedKey = Animator.StringToHash("moveSpeed");
        private static readonly int Jump = Animator.StringToHash("jump");
        private static readonly int Grab = Animator.StringToHash("grab");
        private static readonly int Crouch = Animator.StringToHash("crouch");

        public void SetSpeed(float speed)
        {
            animator.SetFloat(MoveSpeedKey, speed);
        }

        public void SetJump()
        {
            animator.SetTrigger(Jump);
        }

        public void SetGrab(bool value)
        {
            animator.SetBool(Grab, value);
        }

        [UsedImplicitly]
        public void TriggerParticleStep()
        {
            _stepPartycleSystem?.Play();
        }

        public void SetCrouch(bool value)
        {
            animator.SetBool(Crouch, value);
        }
    }
}