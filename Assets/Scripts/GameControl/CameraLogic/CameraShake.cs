using Cinemachine;
using FMODUnity;
using GameEventSystem;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    private float endTimestamp;
    private bool isShaking;
    private CinemachineVirtualCamera vcam;
    private CinemachineBasicMultiChannelPerlin perlin;
    private void Start()
    {
        EventLibrary.shipShoots.AddListener(ShakeListener);
        vcam = GetComponent<CinemachineVirtualCamera>();
        perlin = vcam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }

    private void ShakeListener(Ship ship)
    {
        if (ship.isPlayerShip)
            Shake(0.7f, 0.2f);
    }

    private void Shake(float intensity, float duration)
    {
        isShaking = true;
        perlin.m_AmplitudeGain = intensity;
        endTimestamp = Time.time + duration;
    }
    
    private void StopShaking()
    {
        isShaking = false;
        perlin.m_AmplitudeGain = 0;
    }

    private void Update()
    {
        if (isShaking && Time.time > endTimestamp)
        {
            StopShaking();
        }
    }
}
