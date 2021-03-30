using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class CameraPostProcess : MonoBehaviour
{
    public static CameraPostProcess Instance { get; private set; }

    private PostProcessVolume postProcessVolume;
    private ColorGrading colorGrading;

    public FloatParameter greenIntensity;

    private void Awake()
    {
        Instance = this;
        postProcessVolume = GetComponent<PostProcessVolume>();
    }

    void Start()
    {
        postProcessVolume.profile.TryGetSettings(out colorGrading);

    }

    public void Poisoned(bool status)
    {
        if (status)
        {
            colorGrading.tint.value = greenIntensity;
        }
        else
        {
            colorGrading.tint.value = 0;
        }
    }
}
