using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace MovementPackage.Runtime.Scripts.Parameters
{
    [Serializable]
    public class HookParameters
    {
        public bool blockInput;
        public LayerMask hookLayerMask;
        public HookDropOffEnum continueHookDirection;
        public float hookJumpInSpeed;
        public float hookJumpOffSpeed;
        public float hookJumpOffHeight;
        public float hookRangeDistance;
        public float detectionAngle;
        
        [FormerlySerializedAs("hookJumpCooldown")] public float hookCooldown = 0.2f;
        public float hookInStopDistance = 1f;
    }

    public enum HookDropOffEnum
    {
        ContinueDirection,
        PredefinedPositions,
    }
}