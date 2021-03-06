﻿using UnityEngine;

namespace Interaction
{
    public class CameraFollow : MonoBehaviour
    {
        private Transform _target;
        
        private void Start()
        {
            _target = transform.parent;
            transform.parent = null;
        }

        private void Update()
        {
            var rotation = Quaternion.Euler(0, _target.rotation.eulerAngles.y, 0);
            
            transform.position = _target.position + (rotation * new Vector3(0f, 2.28f, -3.73f));
            transform.rotation = Quaternion.Euler(
                27.74f,
                _target.rotation.eulerAngles.y,
                0
            );
        }
    }
}