using System;
using System.Collections;
using UnityEngine;

namespace MovementPackage.Runtime.Scripts.MovementProcesses
{
    public class CoroutineHelper : MonoBehaviour
    {
        public void StartCoroutineFrom(Func<IEnumerator> coroutine)
        {
            StartCoroutine(coroutine.Invoke());
        }
    }
}