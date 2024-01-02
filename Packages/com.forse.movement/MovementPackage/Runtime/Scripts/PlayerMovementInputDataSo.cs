using System;
using UnityEngine;

namespace MovementPackage.Runtime.Scripts
{
    [Serializable]
    [CreateAssetMenu(fileName = "PlayerMovementInputData", menuName = "Movement/PlayerMovementInputData", order = 1)]
    public class  PlayerMovementInputDataSo : ScriptableObject
    {
        public bool jumpHold;
        public bool jumpPressed;
        public bool jumpReleased;
        public float horizontalPressed;
        public float verticalPressed;
        public bool crouchPressed;
        public bool hookPressed;
        public bool inputBlocked;
    }
}