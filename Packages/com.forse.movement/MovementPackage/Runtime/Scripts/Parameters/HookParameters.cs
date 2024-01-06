using System;
using UnityEngine;

namespace MovementPackage.Runtime.Scripts.Parameters
{
    [Serializable]
    public class HookParameters
    {
        public LayerMask hookLayerMask;
        public float detectionAngle;
        public float hookRangeDistance;
        
        public bool blockInput;
        public bool turnOffGravity;
        
        public bool windUp;
        public float windUpTime;
        
        [Min(0.001f)]public float hookInTime;
        public AnimationCurve hookInCurve;
        [Min(0.1f)]public float hookInStopDistance = 1f;

        public bool continueForce;
        [Min(0.001f)]public float hookOffTime;
        public AnimationCurve hookOutCurve;
        [Min(0f)]public float hookOutDistance;
    }
}