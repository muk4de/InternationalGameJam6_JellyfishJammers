using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class TitleUIController : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] Image smokeBG;
    [SerializeField] RectTransform mainMenu;
    [SerializeField] RectTransform settingsMenu;
    [SerializeField] RectTransform creditMenu;

    [Header("Animation settings")]
    [SerializeField] float slideDuration;
    [SerializeField] float mainMenuSlideLength;
    [SerializeField] float settingsSlideLength;
    [SerializeField] float creditSlideLength;


    [Header("Scene load settings")]
    [SerializeField] float fadeInDur = 0.5f;
    [SerializeField] float minLoadDur = 1f;
    [SerializeField] float fadeOutDur = 0.5f;


    Sequence sequence;

    Vector2 mainMenuStartPos;
    Vector2 settingsStartPos;
    Vector2 creditStartPos;
    float smokeAlpha;

    private void Start()
    {
        Debug.Assert(canvasGroup != null, "Canvas Group is not found");
        Debug.Assert(smokeBG != null, "Smoke Background is not found");
        Debug.Assert(settingsMenu != null, "Setting Menu is not found");
        Debug.Assert(mainMenu != null, "Start Buttons is not found");



        var col = smokeBG.color;
        smokeAlpha = col.a;
        smokeBG.color = new Color(col.r, col.g, col.b, 0f);
        smokeBG.gameObject.SetActive(false);

        settingsMenu.gameObject.SetActive(false);
        settingsStartPos = settingsMenu.anchoredPosition;
        settingsMenu.anchoredPosition = settingsStartPos - new Vector2(0, settingsSlideLength);

        creditMenu.gameObject.SetActive(false);
        creditStartPos = creditMenu.anchoredPosition;
        creditMenu.anchoredPosition = creditStartPos - new Vector2(0, creditSlideLength);
    }

    public void OnSettingsButton()
    {
        SequenceSetup(Ease.OutCubic);

        sequence.AppendCallback(() => smokeBG.gameObject.SetActive(true));
        sequence.AppendCallback(() => settingsMenu.gameObject.SetActive(true));

        sequence.Append(mainMenu.DOAnchorPos(mainMenuStartPos - new Vector2(mainMenuSlideLength, 0f), slideDuration));

        sequence.Join(smokeBG.DOFade(smokeAlpha, slideDuration));
        sequence.Join(settingsMenu.DOAnchorPos(settingsStartPos, slideDuration));

        sequence.AppendCallback(() =>
        {
            mainMenu.gameObject.SetActive(false);
        });
    }

    public void OnSettingsReturnButton()
    {
        SequenceSetup(Ease.OutCubic);

        sequence.AppendCallback(() => mainMenu.gameObject.SetActive(true));

        sequence.Append(smokeBG.DOFade(0f, slideDuration));
        sequence.Join(settingsMenu.DOAnchorPos(settingsStartPos - new Vector2(0, settingsSlideLength), slideDuration));

        sequence.Join(mainMenu.DOAnchorPos(mainMenuStartPos, slideDuration));

        sequence.AppendCallback(() =>
        {
            smokeBG.gameObject.SetActive(false);
            settingsMenu.gameObject.SetActive(false);
        });
    }

    public void OnCreditButton()
    {
        SequenceSetup(Ease.OutCubic);

        sequence.AppendCallback(() => smokeBG.gameObject.SetActive(true));
        sequence.AppendCallback(() => creditMenu.gameObject.SetActive(true));

        sequence.Append(mainMenu.DOAnchorPos(mainMenuStartPos - new Vector2(mainMenuSlideLength, 0f), slideDuration));

        sequence.Join(smokeBG.DOFade(smokeAlpha, slideDuration));
        sequence.Join(creditMenu.DOAnchorPos(creditStartPos, slideDuration));

        sequence.AppendCallback(() =>
        {
            mainMenu.gameObject.SetActive(false);
        });
    }

    public void OnCreditReturnButton()
    {
        SequenceSetup(Ease.OutCubic);

        sequence.AppendCallback(() => mainMenu.gameObject.SetActive(true));

        sequence.Append(smokeBG.DOFade(0f, slideDuration));
        sequence.Join(creditMenu.DOAnchorPos(creditStartPos - new Vector2(0, creditSlideLength), slideDuration));

        sequence.Join(mainMenu.DOAnchorPos(mainMenuStartPos, slideDuration));

        sequence.AppendCallback(() =>
        {
            smokeBG.gameObject.SetActive(false);
            creditMenu.gameObject.SetActive(false);
        });
    }

    public void HideTitleButtons()
    {
        sequence.Append(mainMenu.DOAnchorPos(mainMenuStartPos - new Vector2(mainMenuSlideLength, 0f), slideDuration));
        sequence.AppendCallback(() =>
        {
            mainMenu.gameObject.SetActive(false);
        });
    }

    public void OnPlayButton()
    {
        LockInteract();
        SequenceSetup(Ease.InCubic);

        HideTitleButtons();
        sequence.AppendCallback(() => SceneLoader.I.LoadScene(SceneLoader.GameSceneName, fadeInDur, minLoadDur, fadeOutDur));
    }

    private void SequenceSetup(Ease ease)
    {
        if (sequence.IsActive()) sequence.Kill(true);
        sequence = DOTween.Sequence();
        sequence.SetEase(ease);
    }

    private void LockInteract()
    {
        canvasGroup.interactable = false;
    }

    private void UnlockInteract()
    {
        canvasGroup.interactable = true;
    }

    private void OnDestroy()
    {
        if (sequence != null && sequence.IsActive()) sequence.Kill(true);
    }
}
