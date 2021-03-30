using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputDetection : MonoBehaviour
{
    [HideInInspector] public static InputDetection instance { get; private set; }

    public bool isGamepadEnabled = false;

    public bool isJoystickControlsForMobileEnabled = true;

    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        if (Input.GetJoystickNames().Length > 0)
        {
            isGamepadEnabled = true;
        }
        else
        {
            isGamepadEnabled = false;
        }

        if (Application.platform == RuntimePlatform.Android && Application.platform == RuntimePlatform.IPhonePlayer)
        {
            isJoystickControlsForMobileEnabled = true;
        }
        // else
        // {
        //     isMobile = false;
        // }

    }
#if UNITY_EDITOR
    public void EnableKeyboard()
    {
        isJoystickControlsForMobileEnabled = false;
        UIHandler.Instance.ShowJoysticks(false);
    }
    public void EnableJoystick()
    {
        isJoystickControlsForMobileEnabled = true;
        UIHandler.Instance.ShowJoysticks(true);
    }
#endif


}