using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuCanvas : MonoBehaviour
{
    [SerializeField] private Button ContinueButton;
    [SerializeField] private Button NewStoryButton;
    private AsyncOperation op;

    // Start is called before the first frame update
    void Start()
    {
        int savedPage = PlayerPrefs.GetInt("Page");
        ContinueButton.gameObject.SetActive(savedPage != 0);

       op =  SceneManager.LoadSceneAsync("BookScene",LoadSceneMode.Single);
        op.allowSceneActivation = false;

        NewStoryButton.onClick.AddListener(startNewStory);
    }

    private void OnDisable()
    {
        NewStoryButton.onClick.RemoveListener(startNewStory);
    }

    private void startNewStory()
    {
        PlayerPrefs.SetInt("Page", 1);

        op.allowSceneActivation = true;

    }
}
