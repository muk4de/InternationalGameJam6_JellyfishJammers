using DG.Tweening;
using System.Collections.Generic;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class MoonPoolManager : MonoBehaviour
{
    public bool EventTrigger = false;
    public UnityEvent StartEvent;

    [SerializeField] List<string> narrationSentences;

    [SerializeField] PlayerController playerController;
    [SerializeField] DialogueController jellyfishDialogue;
    [SerializeField] SoundData endingBgm;
    [SerializeField] CanvasGroup endingPanelGroup;
    [SerializeField] TextMeshProUGUI endingText;

    Tween tween;

    private void Start()
    {
        StartCoroutine(MoonPoolCoroutine());
        endingPanelGroup.gameObject.SetActive(false);
        endingPanelGroup.alpha = 0f;

        endingText.gameObject.SetActive(false);
        endingText.alpha = 0f;
    }

    IEnumerator MoonPoolCoroutine()
    {
        playerController.CanMove = false;
        AudioManager.I.PlayBgm(endingBgm, 2f);
        yield return new WaitForSeconds(1);

        var coroutine = jellyfishDialogue.GetStartDialogue();

        yield return coroutine;

        playerController.CanMove = true;
        endingPanelGroup.gameObject.SetActive(true);
        tween = endingPanelGroup.DOFade(1f, 5f).SetEase(Ease.Linear);

        yield return tween.WaitForCompletion();
        playerController.CanMove = false;

        coroutine = StartCoroutine(NarrationManager.Instance.StartNarration(narrationSentences));

        yield return coroutine;



        endingText.gameObject.SetActive(true);
        tween = endingText.DOFade(1f, 10f).SetEase(Ease.Linear);

        yield return tween.WaitForCompletion();

        yield return new WaitUntil(() => Input.anyKey);

        SceneLoader.I.FadeScene(SceneLoader.TitleSceneName, 5f, 1f, 5f, Color.black);
    }

    public void TriggerEvent()
    {
        EventTrigger = true;
    }

    private void OnDestroy()
    {
        if (tween.IsActive())
        {
            tween.Kill(true);
        }
    }
}
