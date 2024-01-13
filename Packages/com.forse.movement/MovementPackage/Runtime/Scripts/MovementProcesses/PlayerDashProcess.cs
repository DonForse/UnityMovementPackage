using MovementPackage.Runtime.Scripts.Parameters;

namespace MovementPackage.Runtime.Scripts.MovementProcesses
{
    public class PlayerDashProcess : IMovementProcess
    {
        private DashParameters _dashParameters;
        private PlayerMovementData _playerMovementData;
        private PlayerMovementInputDataSo _playerMovementInputDataSo;

        public void Initialize(PlayerMovementData playerMovementData,
            PlayerMovementInputDataSo playerMovementInputDataSo, DashParameters dashParameters)
        {
            _playerMovementData = playerMovementData;
            _playerMovementInputDataSo = playerMovementInputDataSo;
            _dashParameters = dashParameters;
        }

        public void ProcessFixedUpdate()
        {
            if (!_playerMovementInputDataSo.dashPressed) return;
            _playerMovementInputDataSo.dashPressed = false;
            if (_playerMovementInputDataSo.horizontalPressed > 0) _playerMovementData.playerHorizontalSpeed = 5f;
            if (_playerMovementInputDataSo.horizontalPressed < 0) _playerMovementData.playerHorizontalSpeed = -5f;
            if (_playerMovementInputDataSo.verticalPressed > 0) _playerMovementData.playerVerticalSpeed = 5f;
            if (_playerMovementInputDataSo.verticalPressed < 0) _playerMovementData.playerVerticalSpeed = -5f;
        }
    }
}