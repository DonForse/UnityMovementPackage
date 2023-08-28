using System;
using UnityEngine;

namespace MovementPackage.Runtime.Scripts.Parameters
{
    [Serializable]
    public class CrouchParameters
    {
        public float movementSpeedMultiplier = 0.5f;
        public bool changeHeightOnCrouch = true;
        public float crouchHeight =1f;
        public Vector3 characterControllerCenterOnCrouch = Vector3.up * -0.5f;
    }
}