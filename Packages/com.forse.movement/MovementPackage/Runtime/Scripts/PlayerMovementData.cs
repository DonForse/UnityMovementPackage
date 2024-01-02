using System;

namespace MovementPackage.Runtime.Scripts
{
    [Serializable]
    public class PlayerMovementData
    {
        public float playerVerticalSpeed;
        public float gravityMultiplier;
        public bool collidingGround;
        public float playerHorizontalSpeed;
        public float playerForwardSpeed;
        public bool collidingRightWall;
        public bool collidingLeftWall;
        public bool grabbedToRightWall;
        public bool grabbedToLeftWall;
        public bool grabbedToForwardWall;
        public bool grabbedToBackWall;
        public bool wallJumped;
        public bool closeRightWall;
        public bool closeLeftWall;
        public bool crouching;
        public float movementSpeedMultiplier;
        public bool collidingForwardWall;
        public bool collidingBackWall;
        public bool closeForwardWall;
        public bool closeBackWall;
        public bool closeGround;

        public bool IsGrabbedToWall()
        {
            return grabbedToRightWall || grabbedToLeftWall || grabbedToBackWall || grabbedToForwardWall;
        }
    }
}