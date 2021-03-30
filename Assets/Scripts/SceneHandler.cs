using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneHandler : MonoBehaviour
{
    public static SceneHandler Instance { get; private set; }
    private Camera mainCamera;

    public Image fadeImage;

    public float fadeOutTime;
    public float fadeInTime;

    private void Awake()
    {
        Instance = this;

        mainCamera = Camera.main;

        fadeImage.enabled = false;
    }

    private void Start()
    {
        Scene currentScene = SceneManager.GetActiveScene();

        if (currentScene.name == "Game")
        {
            UIHandler.Instance.ShowFailedScreen(false);
        }

        StartCoroutine(FadeImageRoutine(true));
    }


    //onclick
    public void LoadGame()
    {
        fadeImage.enabled = false;
        StartCoroutine(LoadGameRoutine());
    }

    private IEnumerator LoadGameRoutine()
    {
        yield return StartCoroutine(FadeImageRoutine(false));
        SceneManager.LoadScene(1);
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }

    public IEnumerator FadeImageRoutine(bool fadeAway)
    {
        fadeImage.enabled = true;

        //fade in
        if (fadeAway)
        {
            fadeImage.color = new Color(0, 0, 0, 1);

            for (float i = fadeOutTime; i >= 0; i -= Time.deltaTime)
            {
                fadeImage.color = new Color(0, 0, 0, i);
                yield return null;
            }

            fadeImage.enabled = false;
        }
        //fade out
        else
        {
            fadeImage.color = new Color(0, 0, 0, 0);

            for (float i = 0; i <= fadeInTime; i += Time.deltaTime)
            {
                fadeImage.color = new Color(0, 0, 0, i);
                yield return null;
            }


        }
    }

    //onclick
    public void QuitGame()
    {
        Application.Quit();
    }
}
