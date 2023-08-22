using Cinemachine;
using FMODUnity;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    private float endTimestamp;
    private bool isShaking;
    private CinemachineVirtualCamera vcam;
    private CinemachineBasicMultiChannelPerlin perlin;
    private void Start()
    {
        GameController.Current.OnShipChange.AddListener(OnPlayerShipChange);
        vcam = GetComponent<CinemachineVirtualCamera>();
        perlin = vcam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }

    void OnPlayerShipChange(Ship _old, Ship _new)
    {
        if (_old != null)
        {
            _old.OnWeaponFire.RemoveListener(ShakeListener);
        }
        _new.OnWeaponFire.AddListener(ShakeListener);
    }

    public void ShakeListener()
    {
        Shake(0.7f, 0.2f);
    }
    
    public void Shake(float intensity, float duration)
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
