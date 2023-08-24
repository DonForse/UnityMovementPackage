using System;

namespace MovementPackage.Runtime.Scripts
{
    [Serializable]
    public class  PlayerMovementInputData
    {
        public bool jumpHold;
        public bool jumpPressed;
        public bool jumpReleased;
        public float horizontalPressed;
        public float verticalPressed;
        public bool crouchPressed;
    }
}