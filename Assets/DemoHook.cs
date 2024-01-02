using System;
using System.Collections;
using System.Collections.Generic;
using MovementPackage.Runtime.Scripts;
using UnityEngine;
using UnityEngine.Events;

public class DemoHook : MonoBehaviour
{
    [SerializeField]private PlayerMovementComponent playerMovementComponent;
    void OnEnable()
    {
        playerMovementComponent.Events.HookFocus.AddListener(OnHookFocus);
        playerMovementComponent.Events.HookLostFocus.AddListener(OnHookFocusLost);
    }

    private void OnHookFocusLost(GameObject arg0)
    {
        var hook = arg0.GetComponent<Hook>();
        if (hook != null)
            hook.ToggleFocus(false);
    }

    private void OnHookFocus(GameObject arg0)
    {
        var hook = arg0.GetComponent<Hook>();
        if (hook != null)
            hook.ToggleFocus(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}