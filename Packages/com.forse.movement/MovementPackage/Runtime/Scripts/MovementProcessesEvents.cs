using System;
using UnityEngine;
using UnityEngine.Events;

namespace MovementPackage.Runtime.Scripts
{
    public class MovementProcessesEvents
    {
        public UnityEvent<GameObject> HookFocus = new();
        public UnityEvent<GameObject> HookLostFocus = new();
        public UnityEvent<Unit> Jumped = new();

    }
}

public class Unit
{
}