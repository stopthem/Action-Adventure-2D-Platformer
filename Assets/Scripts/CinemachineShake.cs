using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CinemachineShake : MonoBehaviour
{
    public static CinemachineShake Instance {get; private set;}

    private CinemachineVirtualCamera m_cinemachine;
    private CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin;
    private float m_amplitude;

    private void Awake()
    {
        Instance = this;
        m_cinemachine = GetComponent<CinemachineVirtualCamera>();
    }

    private void Start()
    {
        cinemachineBasicMultiChannelPerlin = m_cinemachine.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = 0;
    }

    public void ShakeCamera(float power, float time)
    {
        StartCoroutine(ShakeRoutine(power,time));
    }

    private IEnumerator ShakeRoutine(float power, float time)
    {
        cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = power;

        yield return new WaitForSeconds(time);

        cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = 0;
    }
}
