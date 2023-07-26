using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SoundSettings : MonoBehaviour
{
    public float Background;
    public float Touch;
    public float UI;
    public float sentence;
    [SerializeField] private Slider BackgroundSlider;
    [SerializeField] private Slider UISlider;
    [SerializeField] private Slider SentenceSlider;
    [SerializeField] private Slider TouchSlider;

    [SerializeField] private AudioMixer Mixer;

    private const string BACKGROUNDVOL = "BackgoundVol";
    private const string UIVOL = "UIVol";
    private const string SENTVOL = "SentVol";
    private const string TOUCHVOL = "TouchVol";
    private void Awake()
    {
        Background = PlayerPrefs.GetFloat(BACKGROUNDVOL);
        UI = PlayerPrefs.GetFloat(UIVOL);
        sentence = PlayerPrefs.GetFloat(SENTVOL);
        Touch = PlayerPrefs.GetFloat(TOUCHVOL);
    }
    private void OnEnable()
    {
        BackgroundSlider.onValueChanged.AddListener(val => HandleBackgrounSliderChange(val, BACKGROUNDVOL));
        UISlider.onValueChanged.AddListener(val => HandleUISliderChange(val, UIVOL));
        SentenceSlider.onValueChanged.AddListener(val => HandleSentenceSliderChange(val, SENTVOL));
        TouchSlider.onValueChanged.AddListener(val => HandleTouchSliderChange(val, TOUCHVOL));
    }

    private void Start()
    {
        BackgroundSlider.value = Background;
        UISlider.value = UI;
        SentenceSlider.value = sentence;
        TouchSlider.value = Touch;
    }

    private void OnDisable()
    {
        BackgroundSlider.onValueChanged.RemoveListener(val => HandleBackgrounSliderChange(val, BACKGROUNDVOL));
        UISlider.onValueChanged.RemoveListener(val => HandleUISliderChange(val, UIVOL));
        SentenceSlider.onValueChanged.RemoveListener(val => HandleSentenceSliderChange(val, SENTVOL));
        TouchSlider.onValueChanged.RemoveListener(val => HandleTouchSliderChange(val, TOUCHVOL));
    }
    public void HandleTouchSliderChange(float val, string mixerName)
    {
        Touch = val;
        Mixer.SetFloat(mixerName, Touch);
    }
    public void HandleBackgrounSliderChange(float val, string mixerName)
    {
        Background = val;
        Mixer.SetFloat(mixerName, Background);
    }
    public void HandleUISliderChange(float val, string mixerName)
    {
        UI = val;
        Mixer.SetFloat(mixerName, UI);
    }
    public void HandleSentenceSliderChange(float val, string mixerName)
    {
        sentence = val;
        Mixer.SetFloat(mixerName, sentence);
    }

    public void SaveSettings()
    {
        PlayerPrefs.SetFloat(BACKGROUNDVOL, Background);
        PlayerPrefs.SetFloat(UIVOL, UI);
        PlayerPrefs.SetFloat(SENTVOL, sentence);
        PlayerPrefs.SetFloat(TOUCHVOL, Touch);
    }
}
