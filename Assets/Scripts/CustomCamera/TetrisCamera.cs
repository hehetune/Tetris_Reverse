using System;
using UnityEngine;

namespace CustomCamera
{
    public class TetrisCamera : MonoBehaviour
    {
        [SerializeField] private GameObject _character;
        [SerializeField] private float _minYValue;

        private Vector3 _targetPosition;

        private void Awake()
        {
            _targetPosition = transform.position;
        }

        private void Update()
        {
            UpdateTargetPosition();
        }

        private void UpdateTargetPosition()
        {
            _targetPosition.y = Mathf.Max(_character.transform.position.y, _minYValue);
            transform.position = _targetPosition;
        }

    }
}