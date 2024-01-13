using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace MovementPackage.Runtime.Scripts.Parameters
{
    [Serializable]
    public class DashParameters
    {
        public float dashTime;
        public float dashDistance;
        public AnimationCurve dashMovement;
        public bool blockInput;
        public bool enableDashDirectionX;
        // public bool enableDashDirectionY;
        public bool enableDashDirectionZ;
    }
}