using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class UIHandler : MonoBehaviour
{
    public static UIHandler Instance { get; private set; }
    [Header("Screens")]
    public GameObject gameScreen;
    public GameObject menuScreen;
    public GameObject failedScreen;

    [Header("Chest")]
    public TextMeshProUGUI coinText;
    public TextMeshProUGUI chestText;

    public GameObject leftJoystick;

    [Header("Only For Editor/Testing")]
    public Button joystickTestButton;
    public Button keyboardTestButton;


    [Header("Interact Button")]
    public Button interactButton;
    private Image interactButtonImage;
    private TextMeshProUGUI interactTextMesh;

    [Header("Dash Button")]
    public Button dashButton;
    private Image dashButtonImage;
    private TextMeshProUGUI dashTextMesh;

    [Header("Jump Button")]
    public Button jumpJoystick;
    private Image jumpImage;
    private TextMeshProUGUI jumpTextMesh;

    [Header("Attack Button")]
    public Button attackJoystick;
    private Image attackJoystickImage;
    private TextMeshProUGUI attackJoystickTextMesh;

    private Button buttonChange;
    private Image buttonChangeImage;
    private TextMeshProUGUI buttonChangeTextMesh;

    private void Awake()
    {
        Instance = this;
        interactTextMesh = interactButton.GetComponentInChildren<TextMeshProUGUI>();
        interactButtonImage = interactButton.GetComponent<Image>();

        dashTextMesh = dashButton.GetComponentInChildren<TextMeshProUGUI>();
        dashButtonImage = dashButton.GetComponent<Image>();

        attackJoystickTextMesh = attackJoystick.GetComponentInChildren<TextMeshProUGUI>();
        attackJoystickImage = attackJoystick.GetComponent<Image>();

        jumpTextMesh = jumpJoystick.GetComponentInChildren<TextMeshProUGUI>();
        jumpImage = jumpJoystick.GetComponent<Image>();
    }

    private void Start()
    {
        if (!InputDetection.instance.isJoystickControlsForMobileEnabled)
        {
            ShowJoysticks(false);
        }
    }

    private void Update()
    {
        CheckPlayer();

#if UNITY_EDITOR
        ShowTestButtons();
#endif
    }

    //test code
    private void ShowTestButtons()
    {
        joystickTestButton.gameObject.SetActive(true);

        if (InputDetection.instance.isJoystickControlsForMobileEnabled)
        {
            joystickTestButton.GetComponent<Image>().color = Color.red;
        }
        else
        {
            joystickTestButton.GetComponent<Image>().color = Color.white;
        }

        keyboardTestButton.gameObject.SetActive(true);

        if (!InputDetection.instance.isJoystickControlsForMobileEnabled)
        {
            keyboardTestButton.GetComponent<Image>().color = Color.red;
        }
        else
        {
            keyboardTestButton.GetComponent<Image>().color = Color.white;
        }
    }

    private void CheckPlayer()
    {
        if (PlayerController.Instance.isAttacking)
        {
            MobileButtonHandler(false, true, false, false);
        }
        else
        {
            MobileButtonHandler(true, true, false, false);
        }

        if (PlayerMovement.Instance.isJumping)
        {
            MobileButtonHandler(false, false, true, false);
        }
        else
        {
            MobileButtonHandler(true, false, true, false);
        }
    }

    public void ShowJoysticks(bool status)
    {
        leftJoystick.SetActive(status);
        attackJoystick.gameObject.SetActive(status);
        jumpJoystick.gameObject.SetActive(status);
        interactButton.gameObject.SetActive(status);
        dashButton.gameObject.SetActive(status);
    }

    public void UpdateCoinText(float number)
    {
        coinText.text = number.ToString();
    }

    public void ShowChestText(bool status)
    {
        chestText.gameObject.SetActive(status);
        if (InputDetection.instance.isJoystickControlsForMobileEnabled)
        {
            chestText.text = "Tap 'Interact' to open chest";
        }
        else
        {
            chestText.text = "Tap 'E' to open chest";
        }
    }

    //handles attacking,jumping and interact buttons for mobile
    public void MobileButtonHandler(bool status, bool isAttacking, bool isJumping, bool isInteracting)
    {
        if (isAttacking)
        {
            buttonChange = attackJoystick;
            buttonChangeTextMesh = attackJoystickTextMesh;
            buttonChangeImage = attackJoystickImage;
        }
        else if (isJumping)
        {
            buttonChange = jumpJoystick;
            buttonChangeTextMesh = jumpTextMesh;
            buttonChangeImage = jumpImage;
        }
        else if (isInteracting)
        {
            buttonChange = interactButton;
            buttonChangeTextMesh = interactTextMesh;
            buttonChangeImage = interactButtonImage;
        }

        if (status)
        {
            buttonChange.interactable = true;
            buttonChangeTextMesh.color = Color.white;
            buttonChangeImage.color = Color.red;
        }
        else
        {
            buttonChange.interactable = false;
            buttonChangeTextMesh.color = Color.red;
            buttonChangeImage.color = Color.white;
        }
    }

    public void DashButton(bool status, bool isInteractable)
    {
        if (status && PlayerMovement.Instance.isGrounded)
        {
            dashButton.interactable = isInteractable;
            dashTextMesh.color = Color.white;
            dashTextMesh.text = "Dash Attack!";
            dashButtonImage.color = Color.red;
        }
        else
        {
            dashButton.interactable = isInteractable;
            dashTextMesh.color = Color.red;
            dashTextMesh.text = "DASH";
            dashButtonImage.color = Color.white;
        }
    }

    public void ShowMenu(bool status)
    {
        if (status)
        {
            gameScreen.SetActive(false);
            menuScreen.SetActive(true);
        }
        else
        {
            gameScreen.SetActive(true);
            menuScreen.SetActive(false);
        }
    }

    public void ShowFailedScreen(bool status)
    {
        if (status)
        {
            gameScreen.SetActive(false);
            failedScreen.SetActive(true);
        }
        else
        {
            failedScreen.SetActive(false);
            gameScreen.SetActive(true);
        }
    }
}
