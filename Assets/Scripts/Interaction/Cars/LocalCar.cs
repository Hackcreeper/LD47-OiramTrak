using System.Collections.Generic;
using Data;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Interaction.Cars
{
    public class LocalCar : Car
    {
        public List<AxleInfo> axleInfos;
        public float maxMotorTorque = 800;
        public float maxBrakeTorque = 1600;
        public float maxSteeringAngle = 30;
        public float maxSpeed = 80;
        public Camera mainCamera;

        private Rigidbody _rigidbody;
        private PlayerInput _playerInput;
        private Vector2 _moveVector;
        private float _speedAmount;
        private bool _reverse;
        private bool _braking;
        private PlayerInfo _player;

        private void Awake()
        {
            // DiContainer.Instance.Register("main_car", this);

            _rigidbody = GetComponent<Rigidbody>();
            _playerInput = GetComponent<PlayerInput>();
        }

        public void Init(PlayerInfo player)
        {
            _player = player;
            
            // Keyboard&Mouse
            
            
            _playerInput.SwitchCurrentControlScheme(
                _player.Device
                // _controlType == ControlType.Keyboard ? "Keyboard&Mouse" : "Gamepad"
            );
        }
        
        // ReSharper disable once UnusedMember.Global
        public void OnMove(InputAction.CallbackContext context)
        {
            _moveVector = context.ReadValue<Vector2>();
        }

        // ReSharper disable once UnusedMember.Global
        public void OnBrake(InputAction.CallbackContext context)
        {
            _braking = context.ReadValueAsButton();
        }

        // ReSharper disable once UnusedMember.Global
        public void OnSpeed(InputAction.CallbackContext context)
        {
            _speedAmount = context.ReadValue<float>();
        }

        private float GetMotorSpeed()
        {
            if (_reverse)
            {
                return maxMotorTorque * -1;
            }

            if (_player.Type == ControlType.Keyboard)
            {
                return maxMotorTorque * _moveVector.y;
            }

            return maxMotorTorque * _speedAmount;
        }

        private float GetSteeringAngle()
        {
            return maxSteeringAngle * _moveVector.x;
        }

        private void HandleBrake(AxleInfo axleInfo)
        {
            if (_braking)
            {
                if (!_reverse)
                {
                    axleInfo.leftWheel.brakeTorque = maxBrakeTorque;
                    axleInfo.rightWheel.brakeTorque = maxBrakeTorque;
                }

                if (_rigidbody.velocity.sqrMagnitude < 0.8f)
                {
                    _reverse = true;
                    axleInfo.leftWheel.brakeTorque = 0;
                    axleInfo.rightWheel.brakeTorque = 0;   
                }

                return;
            }

            axleInfo.leftWheel.brakeTorque = 0;
            axleInfo.rightWheel.brakeTorque = 0;
            _reverse = false;
        }

        private void FixedUpdate()
        {
            var steering = GetSteeringAngle();

            var speed = _rigidbody.velocity.sqrMagnitude;

            foreach (var axleInfo in axleInfos)
            {
                if (axleInfo.steering)
                {
                    axleInfo.leftWheel.steerAngle = steering;
                    axleInfo.rightWheel.steerAngle = steering;
                }

                HandleBrake(axleInfo);

                var motor = GetMotorSpeed();
                Debug.Log(motor);

                if (axleInfo.motor && speed < maxSpeed && motor >= 0.1f)
                {
                    axleInfo.leftWheel.motorTorque = motor;
                    axleInfo.rightWheel.motorTorque = motor;
                }
                else if (motor <= 0.01f)
                {
                    Debug.Log("Kill");
                    axleInfo.leftWheel.motorTorque = 0;
                    axleInfo.rightWheel.motorTorque = 0;
                    axleInfo.leftWheel.brakeTorque = maxMotorTorque * 2f;
                    axleInfo.rightWheel.brakeTorque = maxMotorTorque * 2f;
                }
                else if (speed >= maxSpeed)
                {
                    axleInfo.leftWheel.motorTorque = 0;
                    axleInfo.rightWheel.motorTorque = 0;
                }
            }
        }
    }
}