using System;
using UnityEngine;
using UnityEngine.Events;

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

    }

    public enum HookDropOffEnum
    {
        ContinueDirection,
        PredefinedPositions,
    }
}