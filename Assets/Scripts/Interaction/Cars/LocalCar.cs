using System.Collections.Generic;
using Data;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Interaction.Cars
{
    public class LocalCar : Car
    {
        public List<AxleInfo> axleInfos;
        public ControlType controlType = ControlType.Keyboard;
        public float maxMotorTorque = 800;
        public float maxBrakeTorque = 1600;
        public float maxSteeringAngle = 30;
        public float maxSpeed = 80;

        private Rigidbody _rigidbody;
        private PlayerInput _playerInput;
        private Vector2 _moveVector;
        private float _speedAmount;
        private bool _reverse;
        private bool _braking;

        private void Awake()
        {
            DiContainer.Instance.Register("main_car", this);

            _rigidbody = GetComponent<Rigidbody>();
            _playerInput = GetComponent<PlayerInput>();

            // Keyboard&Mouse
            _playerInput.SwitchCurrentControlScheme(
                controlType == ControlType.Keyboard ? "Keyboard&Mouse" : "Gamepad"
            );
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            _moveVector = context.ReadValue<Vector2>();
        }

        public void OnBrake(InputAction.CallbackContext context)
        {
            _braking = context.ReadValueAsButton();
        }

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

            if (controlType == ControlType.Keyboard)
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

                if (axleInfo.motor && speed < maxSpeed)
                {
                    axleInfo.leftWheel.motorTorque = motor;
                    axleInfo.rightWheel.motorTorque = motor;
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