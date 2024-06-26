using UnityEngine;

namespace MovementPackage.Runtime.Scripts
{
    public class PlayerMovementInput : MonoBehaviour
    {
        [SerializeField] private PlayerMovementInputDataSo playerMovementInputDataSo;

        private void Awake()
        {
            playerMovementInputDataSo.inputBlocked = false;
        }

        void Update()
        {
            if (playerMovementInputDataSo.inputBlocked) return;
            playerMovementInputDataSo.jumpHold = Input.GetButton("Jump");
            playerMovementInputDataSo.jumpPressed =
                playerMovementInputDataSo.jumpPressed || Input.GetButtonDown("Jump");
            playerMovementInputDataSo.jumpReleased =
                playerMovementInputDataSo.jumpReleased || Input.GetButtonUp("Jump");
            playerMovementInputDataSo.horizontalPressed = Input.GetAxis("Horizontal");
            playerMovementInputDataSo.verticalPressed = Input.GetAxis("Vertical");
            playerMovementInputDataSo.crouchPressed = Input.GetKey(KeyCode.LeftControl);
            playerMovementInputDataSo.hookPressed =playerMovementInputDataSo.hookPressed|| Input.GetKeyDown(KeyCode.LeftControl);
            playerMovementInputDataSo.dashPressed = playerMovementInputDataSo.dashPressed || Input.GetKeyDown(KeyCode.LeftShift);

        }
    }
}