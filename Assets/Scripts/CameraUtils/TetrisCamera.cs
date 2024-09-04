using System;
using UnityEngine;

namespace CustomCamera
{
    public class TetrisCamera : MonoBehaviour
    {
        [SerializeField] private Transform _character;
        [SerializeField] private Transform _larva;
        [SerializeField] private float _yOffset;

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
            _targetPosition.y = Mathf.Max(_character.position.y + _yOffset, _larva.position.y + _yOffset);
            transform.position = _targetPosition;
        }

    }
}