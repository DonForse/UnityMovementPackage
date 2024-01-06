using MovementPackage.Runtime.Scripts;
using UnityEngine;

public class DemoHook : MonoBehaviour
{
    [SerializeField] private PlayerMovementComponent playerMovementComponent;
    private bool _drawing = false;
    private LineRenderer _lr = null;

    void OnEnable()
    {
        playerMovementComponent.Events.HookFocus.AddListener(OnHookFocus);
        playerMovementComponent.Events.HookLostFocus.AddListener(OnHookFocusLost);
        playerMovementComponent.Events.HookStart.AddListener(OnHookStart);
        playerMovementComponent.Events.HookReachPosition.AddListener(StopDrawingLine);
    }

    private void StopDrawingLine(GameObject arg0)
    {
        _drawing = false;
        _lr = null;
    }

    void Update()
    {
        UpdateLineRenderer();
    }

    private void UpdateLineRenderer()
    {
        if (!_drawing || _lr == null) return;
        _lr.SetPosition(0, playerMovementComponent.transform.position);
    }

    private void OnHookStart(GameObject arg0)
    {
        var hook = arg0.GetComponent<Hook>();
        if (hook != null)
        {
            _lr = hook.GetComponent<LineRenderer>();
            _lr.enabled = true;
            _drawing = true;
            _lr.SetPosition(0, playerMovementComponent.transform.position);
            _lr.SetPosition(1, arg0.transform.position);
        }
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
}