using System.Collections;
using Cinemachine;
using UnityCommunity.UnitySingleton;
using UnityEngine;

namespace CustomCamera
{
    public class CameraShake : MonoSingleton<CameraShake>
    {
        private CinemachineVirtualCamera _cinemachineVirtualCamera;

        private void Awake()
        {
            _cinemachineVirtualCamera = GetComponent<CinemachineVirtualCamera>();
        }

        public void ShakeCamera(float intensity, float time)
        {
            CinemachineBasicMultiChannelPerlin _cinemachineBasicMultiChannelPerlin =
                _cinemachineVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

            _cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = intensity;

            if (_shakeCameraCoroutine != null) StopCoroutine(_shakeCameraCoroutine);
            _shakeCameraCoroutine = StartCoroutine(ShakeCameraCoroutine(time));
        }

        private Coroutine _shakeCameraCoroutine;

        private IEnumerator ShakeCameraCoroutine(float time)
        {
            yield return time.Wait();

            //Timer over
            CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin =
                _cinemachineVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

            cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = 0f;
        }
    }
}