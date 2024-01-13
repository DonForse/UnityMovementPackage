using System;
using UnityEngine;
using UnityEngine.Events;

namespace MovementPackage.Runtime.Scripts
{
    public class MovementProcessesEvents
    {
        public UnityEvent Jump = new();
        public UnityEvent DoubleJump = new();
        public UnityEvent<GameObject> HookLostFocus = new();
        public UnityEvent<GameObject> HookFocus = new();
        public UnityEvent<GameObject> HookStart = new();
        public UnityEvent<GameObject> HookWindUpComplete = new();
        public UnityEvent<GameObject> HookReachPosition = new();
        public UnityEvent<GameObject> HookEnd = new();
        public UnityEvent DashStart = new ();
        public UnityEvent DashFinish = new();
    }
}