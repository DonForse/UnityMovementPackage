using System;
using UnityEngine;

namespace MovementPackage.Runtime.Scripts.Parameters
{
    [Serializable]
    public class WallJumpParameters
    {
        [Header("Remember to enable Close Wall Detection in Player Movement Collision.")]
        [Space]
        [Space]
        public float jumpHeight = 4f;
        public float jumpOppositeSpeed = 4f;
    }
}