using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class TutorialController : MonoBehaviour
{
    public static TutorialController instance;

    public int currentStep = 0;
    public bool stepPassed;
    public bool tutorialMoving;
    public Image TutorialPanel;
    public Text TutorialText;
    public Image SkipButton;
    public Text SkipText;
    [Space]
    [SerializeField] private Transform FirstStep;
    [SerializeField] private SpriteRenderer[] SawmillDots;
    [SerializeField] private Transform SecondStep;
    [SerializeField] private SpriteRenderer[] QuarryDots;
    [Space]
    [SerializeField] private SpriteRenderer[] StorageDots1;
    [SerializeField] private SpriteRenderer[] StorageDots2;
    [SerializeField] private Transform ThirdStep;
    [SerializeField] private SpriteRenderer[] Sawmill2Dots;
    [SerializeField] private Transform FourthStep;

    private Coroutine DotsCoroutine;
    private float delay = 0;

    private void Start()
    {
        instance = this;
        
        if (PlayerPrefs.GetInt("Tutorial") == 0)
            PlayerPrefs.SetInt("Tutorial", 1);
        Debug.Log(PlayerPrefs.GetInt("Tutorial"));

        currentStep = PlayerPrefs.GetInt("Tutorial");
        if (currentStep <= 7)
        {
            tutorialMoving = true;
            TutorialPanel.gameObject.SetActive(true);
            TutorialPanel.DOFade(0.375f, DOTweenManager.instance.time);
        }
    }

    public IEnumerator CheckTutorial()
    {
        switch (currentStep)
        {
            case 1:
                StartCoroutine(StepRoutine(SawmillDots, FirstStep.position, 10, "Drag and drop line from the Sawmills Out to the In of the Storage"));
                break;
            case 2:
                StartCoroutine(StepRoutine(QuarryDots, SecondStep.position, 10, "Do the same with the Quarry"));
                break;
            case 3:
                StartCoroutine(StepRoutine(StorageDots1, ThirdStep.position, 10, "Extend the Tree line between Storage and Sawmill"));
                break;
            case 4:
                StartCoroutine(StepRoutine(StorageDots2, ThirdStep.position, 10, "Now connect the Stone Out and Stone In"));
                break;
            case 5:
                StartCoroutine(CameraMover.instance.MoveAndZoomCamera(ThirdStep.position, 10));
                yield return StartCoroutine(ChangeText());

                currentStep += 3;
                PlayerPrefs.SetInt("Tutorial", currentStep);
                StartCoroutine(CheckTutorial());
                break;
            case 6:
                //StartCoroutine(StepRoutine(Sawmill2Dots, ThirdStep.position, 10, "Draw a line from one sawmill to another"));
                break;
            case 7:
                //StartCoroutine(Step7());
                break;
            case 8:
                StartCoroutine(CameraMover.instance.MoveAndZoomCamera(FourthStep.position, 20));
                tutorialMoving = false;
                currentStep++;
                PlayerPrefs.SetInt("Tutorial", currentStep);
                break;
        }
    }

    private IEnumerator FadeDots(SpriteRenderer[] sprites)
    {
        foreach (SpriteRenderer sprite in sprites)
            yield return sprite.DOFade(1, 2 / (float)sprites.Length).WaitForCompletion();

        yield return new WaitForSeconds(1f);
        
        foreach (SpriteRenderer sprite in sprites)
            sprite.DOFade(0, DOTweenManager.instance.time);

        yield return new WaitForSeconds(DOTweenManager.instance.time);
    }

    private IEnumerator StepRoutine(SpriteRenderer[] currentDots, Vector3 cameraPosition, float zoom, string text)
    {
        foreach (SpriteRenderer sprite in currentDots)
            sprite.gameObject.SetActive(true);
        TutorialText.gameObject.SetActive(true);
        TutorialText.text = text;
        
        TutorialText.DOFade(1, DOTweenManager.instance.time);
        StartCoroutine(CameraMover.instance.MoveAndZoomCamera(cameraPosition, zoom));
        
        do
        {
            DotsCoroutine = StartCoroutine(FadeDots(currentDots));
            yield return new WaitForSeconds(4f);
        } while (!stepPassed);

        stepPassed = false;
        
        foreach (SpriteRenderer sprite in currentDots)
            sprite.gameObject.SetActive(false);
        
        StopCoroutine(DotsCoroutine);
        yield return TutorialText.DOFade(0, DOTweenManager.instance.time).WaitForCompletion();

        currentStep++;
        PlayerPrefs.SetInt("Tutorial", currentStep);
        StartCoroutine(CheckTutorial());
    }

    private IEnumerator ChangeText()
    {
        SkipButton.gameObject.SetActive(true);
        TutorialText.DOFade(1, DOTweenManager.instance.time);
        SkipButton.DOFade(1, DOTweenManager.instance.time);
        SkipText.DOFade(1, DOTweenManager.instance.time);
        
        TutorialText.text = "There are different buildings in the game that can produce different resources";
        yield return StartCoroutine(Delay());
        yield return TutorialText.DOFade(0, DOTweenManager.instance.time).WaitForCompletion();
        TutorialText.DOFade(1, DOTweenManager.instance.time);
        TutorialText.text = "Also there are factories in the game that combine several resources into one";
        yield return StartCoroutine(Delay());
        yield return TutorialText.DOFade(0, DOTweenManager.instance.time).WaitForCompletion();
        TutorialText.DOFade(1, DOTweenManager.instance.time);
        TutorialText.text = "You can connect multiple buildings, but you can not cross mountains, water and forests";
        yield return StartCoroutine(Delay());
        yield return TutorialText.DOFade(0, DOTweenManager.instance.time).WaitForCompletion();
        TutorialText.DOFade(1, DOTweenManager.instance.time);
        TutorialText.text = "To move to the next location, you should build a road";
        yield return StartCoroutine(Delay());
        yield return TutorialText.DOFade(0, DOTweenManager.instance.time).WaitForCompletion();
        TutorialText.DOFade(1, DOTweenManager.instance.time);
        TutorialText.text = "Double-click the line will delete it";
        
        
        do
            yield return new WaitForEndOfFrame();
        while (!stepPassed && delay != 7);
        
        SkipButton.DOFade(0, DOTweenManager.instance.time);
        SkipText.DOFade(0, DOTweenManager.instance.time);
        TutorialPanel.DOFade(0, DOTweenManager.instance.time);
        yield return TutorialText.DOFade(0, DOTweenManager.instance.time).WaitForCompletion();
        
        TutorialPanel.gameObject.SetActive(false);
    }

    private IEnumerator Step7()
    {
        TutorialText.DOFade(1, DOTweenManager.instance.time);
        TutorialText.text = "Double-click the line on the first sawmill to delete it";

        do
        {
            yield return new WaitForEndOfFrame();
        } while (!stepPassed);

        TutorialPanel.DOFade(0, DOTweenManager.instance.time);
        yield return TutorialText.DOFade(0, DOTweenManager.instance.time).WaitForCompletion();
        TutorialPanel.gameObject.SetActive(false);
        currentStep++;
        PlayerPrefs.SetInt("Tutorial", currentStep);
        yield return new WaitForSeconds(1);
        StartCoroutine(CheckTutorial());
    }

    private IEnumerator Delay()
    {
        do
        {
            yield return new WaitForSeconds(0.1f);
            delay += 0.1f;
        } while (delay <= 7);

        delay = 0;
    }

    public void SkipButtonChange()
    {
        delay = 7;
    }
}
