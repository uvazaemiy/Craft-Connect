using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class DOTweenManager : MonoBehaviour
{
    public static DOTweenManager instance;

    public float time = 0.5f;
    [SerializeField] private Transform MusicButton;
    [SerializeField] private Transform ResetButton;
    [SerializeField] private Transform SettingsButton;
    [SerializeField] private float ySettingsMoving;
    [Space]
    [SerializeField] private Image ResetPanel;
    [SerializeField] private Text ResetText;
    [SerializeField] private Image ResetButtonImage;
    [SerializeField] private Text ResetButtonText;
    [SerializeField] private Image CancelButtonImage;
    [SerializeField] private Text CancelButtonText;
    [SerializeField] private Image FadeBack;

    private Image MusicImage;
    private Image ResetImage;
    private bool isMoving;
    private bool stateOfSettings;
    private Camera camera;

    private void Start()
    {
        instance = this;
        
        MusicImage = MusicButton.GetComponent<Image>();
        ResetImage = ResetButton.GetComponent<Image>();
        camera = Camera.main;
    }

    public void MoveSettingsButtons()
    {
        if (!isMoving)
            StartCoroutine(MoveButtonsRoutine());
    }
    
    private IEnumerator MoveButtonsRoutine()
    {
        float yOffset = CameraMover.instance.zoomOutMax / camera.orthographicSize;
        
        isMoving = true;

        if (!stateOfSettings)
        {
            MusicButton.gameObject.SetActive(true);
            ResetButton.gameObject.SetActive(true);

            MusicImage.DOFade(1, time);
            ResetImage.DOFade(1, time);
            
            ResetButton.DOMoveY(SettingsButton.position.y - (ySettingsMoving / yOffset) * 2, time);
            yield return MusicButton.DOMoveY(SettingsButton.position.y - ySettingsMoving / yOffset, time).WaitForCompletion();
        }
        else
        {
            MusicImage.DOFade(0, time);
            ResetImage.DOFade(0, time);

            ResetButton.DOMoveY(SettingsButton.position.y, time);
            yield return MusicButton.DOMoveY(SettingsButton.position.y, time).WaitForCompletion();
            
            MusicButton.gameObject.SetActive(false);
            ResetButton.gameObject.SetActive(false);
        }
        
        stateOfSettings = !stateOfSettings;
        isMoving = false;
    }

    public void ChangeMusic()
    {
        SoundController.instance.ChangeMusicVolume(MusicButton.GetComponent<SoundButtonData>());
    }

    public IEnumerator FadeBackground()
    {
        yield return FadeBack.DOFade(0, time).WaitForCompletion();
        FadeBack.gameObject.SetActive(false);
    }

    public void OpenAcceptWindow()
    {
        ResetPanel.gameObject.SetActive(true);

        ResetPanel.DOFade(1, time);
        ResetText.DOFade(1, time);
        ResetButtonImage.DOFade(1, time);
        ResetButtonText.DOFade(1, time);
        CancelButtonImage.DOFade(1, time);
        CancelButtonText.DOFade(1, time);
    }

    public void CloseAcceptWindow()
    {
        StartCoroutine(CloseAcceptWindowRoutine());
    }

    private IEnumerator CloseAcceptWindowRoutine()
    {
        ResetPanel.DOFade(0, time);
        ResetText.DOFade(0, time);
        ResetButtonImage.DOFade(0, time);
        ResetButtonText.DOFade(0, time);
        CancelButtonImage.DOFade(0, time);
        yield return CancelButtonText.DOFade(0, time).WaitForCompletion();
        
        ResetPanel.gameObject.SetActive(false);
    }
}
