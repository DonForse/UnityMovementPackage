using UnityEngine;
using UnityEngine.Serialization;

namespace MovementPackage.Runtime.Scripts
{
    public class PlayerMovementInput : MonoBehaviour
    {
        [SerializeField]private PlayerMovementInputDataSo playerMovementInputDataSo; 
        void Update()
        {
            playerMovementInputDataSo.jumpHold = Input.GetButton("Jump");
            playerMovementInputDataSo.jumpPressed = playerMovementInputDataSo.jumpPressed || Input.GetButtonDown("Jump");
            playerMovementInputDataSo.jumpReleased = playerMovementInputDataSo.jumpReleased || Input.GetButtonUp("Jump");
            playerMovementInputDataSo.horizontalPressed = Input.GetAxis("Horizontal");
            playerMovementInputDataSo.verticalPressed = Input.GetAxis("Vertical");
            playerMovementInputDataSo.crouchPressed = Input.GetKey(KeyCode.LeftControl);
        }
    }
}