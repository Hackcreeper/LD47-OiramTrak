using Data;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Interaction.Cars
{
    public class LocalCar : Car
    {
        public Camera mainCamera;
        public TextMeshPro resetWarning;
        
        private PlayerInput _playerInput;
        private float _warningTimer = 2f;
        
        private float _speedAmount;
        private float _breakAmount;
        
        private void Awake()
        {
            _playerInput = GetComponent<PlayerInput>();
        }

        public override void Init(PlayerInfo player, GameObject[] waypoints)
        {
            base.Init(player, waypoints);
            
            _playerInput.SwitchCurrentControlScheme(Player.Device);

            resetWarning.text = player.Type == ControlType.Keyboard
                ? "Press <b>[R]</b> to reset"
                : "Press <b>[Y]</b> to reset";
            
            EnableActiveWaypoint();
        }
        
        // ReSharper disable once UnusedMember.Global
        public void OnMove(InputAction.CallbackContext context)
        {
            Movement = context.ReadValue<Vector2>();
        }

        // ReSharper disable once UnusedMember.Global
        public void OnBrake(InputAction.CallbackContext context)
        {
            _breakAmount = context.ReadValue<float>();
        }

        // ReSharper disable once UnusedMember.Global
        public void OnSpeed(InputAction.CallbackContext context)
        {
            _speedAmount = context.ReadValue<float>();
        }

        // ReSharper disable once UnusedMember.Global
        public void OnReset(InputAction.CallbackContext context)
        {
            if (!context.started)
            {
                return;
            }

            ResetPosition();
        }

        // ReSharper disable once UnusedMember.Global
        public void OnRestart(InputAction.CallbackContext context)
        {
            DiContainer.Instance.GetByName<WinScreen>("Win").OnRestart(context);
        }

        protected override void Update()
        {
            if (Player.Type == ControlType.Controller)
            {
                Movement.y = _speedAmount - _breakAmount;
            }
            
            base.Update();

            if (blocked || Finished)
            {
                return;
            }
            
            HandleWarning();
        }
        
        private void HandleWarning()
        {
            resetWarning.gameObject.SetActive(_warningTimer <= 0f);
        }

        protected override bool CheckTrack()
        {
            var hitTrack = base.CheckTrack();
            
            if (!hitTrack)
            {
                _warningTimer -= UnityEngine.Time.fixedDeltaTime;
            }
            else
            {
                _warningTimer = 2f;
            }

            return hitTrack;
        }
    }
}