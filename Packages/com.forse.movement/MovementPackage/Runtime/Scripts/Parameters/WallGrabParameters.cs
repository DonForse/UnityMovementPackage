using System;

namespace MovementPackage.Runtime.Scripts.Parameters
{
    [Serializable]
    public class WallGrabParameters
    {
        public bool canMoveInOtherAxisWhileGrabbing = false;
        public bool canMoveInSameAxisWhileGrabbing = false;
        public bool slideOff = true;
        public float slideOffStartAtTime = 0.5f;
        public float slideOffSpeed = -0.25f;
    }
}