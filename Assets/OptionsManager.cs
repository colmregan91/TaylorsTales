using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionsManager : MonoBehaviour
{
    [SerializeField] private Animator anim;
    private int triggerHashClick = Animator.StringToHash("onclick");
    private int triggerHashUnclick = Animator.StringToHash("onunclick");

    [SerializeField] private CanvasGroup optionsPanel;
    [SerializeField] private CanvasGroup mainPanel;
    [SerializeField] private CanvasGroup audioPanel;
    [SerializeField] private CanvasGroup coloursPanel;
    [SerializeField] private GameObject quitButton;
    private CanvasGroup currentPanel;

    private bool isActive;

    public static SoundSettings curSoundSettings;

    public static Action onOptionsShown;
    public static Action onOptionsHidden;

    private void Awake()
    {
        ResetPanel(optionsPanel);

        ResetPanel(audioPanel);
        currentPanel = mainPanel;
        SetCanvas(currentPanel);

        quitButton.SetActive(false);
    }

    private void Start()
    {
        BookManager.OnPageChanged += checkTitlePage;
    }

    private void OnDisable()
    {
        BookManager.OnPageChanged -= checkTitlePage;
    }

    private void checkTitlePage(int number, PageContents contents)
    {
        quitButton.SetActive(!BookManager.isTitlePage);
    }
    public void HandleOptionsClicked()
    {
        if (isActive) return;

        AudioMAnager.instance.PlayUIpop();
        anim.SetTrigger(triggerHashClick);
        isActive = true;
        onOptionsShown?.Invoke();
    }



    private void ResetPanel(CanvasGroup group)
    {
        group.blocksRaycasts = false;
        group.interactable = false;
        group.alpha = 0;
    }
    private void ActivatePanel(CanvasGroup group)
    {
        group.alpha = 1;
        group.interactable = true;
        group.blocksRaycasts = true;
    }

    public void Quit()
    {
        AudioMAnager.instance.PlayUIpop();

    }

    public void Resume()
    {
        if (!isActive) return;

        AudioMAnager.instance.PlayUIpop();
        anim.SetTrigger(triggerHashUnclick);
        isActive = false;
        onOptionsHidden?.Invoke();
    }

    public void SetCanvas(CanvasGroup group)
    {
        ResetPanel(currentPanel);
        currentPanel = group;
        ActivatePanel(currentPanel);

    }
    public void ColoursPanel()
    {
        AudioMAnager.instance.PlayUIpop();
        SetCanvas(coloursPanel);
    }
    public void AudioPanel()
    {
        AudioMAnager.instance.PlayUIpop();
        SetCanvas(audioPanel);
    }
    public void OptionsPanel()
    {
        AudioMAnager.instance.PlayUIpop();
        SetCanvas(optionsPanel);
    }
    public void MainPanel()
    {
        AudioMAnager.instance.PlayUIpop();
        SetCanvas(mainPanel);
    }
}
