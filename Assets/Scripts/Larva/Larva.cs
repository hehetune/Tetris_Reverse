using System.Collections;
using UnityEngine;

namespace Larva
{
public class Larva : MonoBehaviour
    {
        [Tooltip("Distance to move upward each time.")]
        public float riseDistance = 1f;

        [Tooltip("Time in seconds to move upward by riseDistance.")]
        public float riseDuration = 1f;

        [Tooltip("Initial time in seconds between each rise.")]
        public float initialIntervalBetweenRises = 2f;

        [Tooltip("Minimum time in seconds between each rise.")]
        public float minIntervalBetweenRises = 0.5f;

        [Tooltip("Amount by which the interval is reduced each time.")]
        public float intervalReductionAmount = 0.1f;

        [Tooltip("Time in seconds after which the interval is reduced.")]
        public float timeToReduceInterval = 5f;

        [Tooltip("Time in seconds before the larva starts rising.")]
        public float delayBeforeRising = 10f;

        private bool _isRising;
        private float _delayTimer = 0f;
        private float _targetYPosition;
        private float _currentInterval;
        private float _intervalReductionTimer;

        private Coroutine _riseCoroutine;

        private void Start()
        {
            StartRising();
        }

        public void StartRising()
        {
            this._isRising = true;
            this._delayTimer = this.delayBeforeRising;
            this._currentInterval = this.initialIntervalBetweenRises;
            this._intervalReductionTimer = this.timeToReduceInterval;

            if (_riseCoroutine != null)
                StopCoroutine(_riseCoroutine);
            _riseCoroutine = StartCoroutine(RiseCoroutine());
        }

        public void StopRising()
        {
            this._isRising = false;
            if (_riseCoroutine == null) return;
            StopCoroutine(_riseCoroutine);
            _riseCoroutine = null;
        }

        private IEnumerator RiseCoroutine()
        {
            yield return new WaitForSeconds(_delayTimer);

            while (_isRising)
            {
                // Calculate the target position for this rise
                _targetYPosition = transform.position.y + riseDistance;

                // Smoothly move the larva to the target position over riseDuration
                float elapsedTime = 0f;
                Vector3 startPosition = transform.position;

                while (elapsedTime < riseDuration)
                {
                    transform.position = Vector3.Lerp(startPosition,
                        new Vector3(transform.position.x, _targetYPosition, transform.position.z),
                        elapsedTime / riseDuration);
                    elapsedTime += Time.deltaTime;
                    yield return null;
                }

                // Ensure the final position is exact
                transform.position = new Vector3(transform.position.x, _targetYPosition, transform.position.z);

                // Wait for the current interval before rising again
                yield return new WaitForSeconds(_currentInterval);

                // Reduce the interval if enough time has passed
                _intervalReductionTimer -= _currentInterval;
                if (_intervalReductionTimer <= 0f)
                {
                    _currentInterval = Mathf.Max(_currentInterval - intervalReductionAmount, minIntervalBetweenRises);
                    _intervalReductionTimer = timeToReduceInterval;
                }
            }
        }
    }
}