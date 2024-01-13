using System;
using UnityEngine;

namespace MovementPackage.Runtime.Scripts.Parameters
{
    [Serializable]
    public class JumpParameters
    {
        public float jumpHeight = 4f;
        
        [Header("Feature Toggle")]
        [Tooltip("Jump buffering. " +
                 "If you press and hold the jump button a short time before landing," +
                 " you will jump on the exact frame that you land.")]
        public bool jumpBufferingEnabled;

        public bool coyoteEnabled;
        public bool holdJumpEnabled;
        public bool jumpCooldownEnabled;
        public bool doubleJumpEnabled;
        [Header("Hold Jump")]
        public float holdJumpGravity = 0.35f;
        [Header("Coyote")]
        public float coyoteTime = 0.25f;
        [Header("Jump Cooldown")] 
        public float jumpCooldown = 0.1f;
        [Header("Double Jump")]
        public float doubleJumpHeight = 4f;
    }
}