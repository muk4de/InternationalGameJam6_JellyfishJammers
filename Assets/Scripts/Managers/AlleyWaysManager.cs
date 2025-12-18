using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class AlleyWaysManager : MonoBehaviour
{
    public enum Choice
    {
        None,
        Right,
        Left
    }

    public bool EventTrigger = false;
    public UnityEvent StartEvent;

    [SerializeField] DialogueController jellyfishDialogue;
    [SerializeField] CanvasGroup choiceButtonGroup;
    [SerializeField] CanvasGroup fadePanelGroup;
    [SerializeField] CanvasGroup gameOverGroup;
    [SerializeField] TextMeshProUGUI gameOverText;
    [SerializeField] Image BackgroundImage;

    public Choice CurrentChoice = Choice.None;
    public List<Choice> CorrectAnswers = new List<Choice>();
    public List<Sprite> BackgroundSprites = new List<Sprite>();

    Tween tween;

    private void Start()
    {
        BackgroundImage.sprite = BackgroundSprites[0];
        choiceButtonGroup.gameObject.SetActive(false);
        choiceButtonGroup.interactable = false;
        choiceButtonGroup.alpha = 0f;

        fadePanelGroup.gameObject.SetActive(false);
        fadePanelGroup.alpha = 0f;

        gameOverGroup.gameObject.SetActive(false);
        gameOverGroup.alpha = 0f;

        StartCoroutine(StartAlleyChoice());

    }

    IEnumerator StartAlleyChoice()
    {
        yield return new WaitForSeconds(1f);

        bool clearFlg = true;

        for(int i = 0; i < CorrectAnswers.Count; i++)
        {
            CurrentChoice = Choice.None ;
            if(i != 0)
            {
                BackgroundImage.sprite = BackgroundSprites[i];

                tween = fadePanelGroup.DOFade(0f, 1f);

                yield return tween.WaitForCompletion();

                fadePanelGroup.gameObject.SetActive(false);
            }

            jellyfishDialogue.sentenceIndex = i;
            var coroutine = jellyfishDialogue.GetStartDialogue();

            yield return coroutine;

            choiceButtonGroup.gameObject.SetActive(true);
            tween = choiceButtonGroup.DOFade(1f, 1f);

            yield return tween.WaitForCompletion();
            choiceButtonGroup.interactable = true;

            yield return new WaitWhile(() => CurrentChoice == Choice.None);

            choiceButtonGroup.interactable = false;
            tween = choiceButtonGroup.DOFade(0f, 1f);

            yield return tween.WaitForCompletion();
            choiceButtonGroup.gameObject.SetActive(false);

            if (CurrentChoice == Choice.Right)
            {
                jellyfishDialogue.sentenceIndex = 3;
            }
            else
            {
                jellyfishDialogue.sentenceIndex = 4;
            }

            coroutine = jellyfishDialogue.GetStartDialogue();

            yield return coroutine;

            fadePanelGroup.gameObject.SetActive(true);
            tween = fadePanelGroup.DOFade(1f, 1f);

            yield return tween.WaitForCompletion();
            yield return new WaitForSeconds(0.5f);

            if(CurrentChoice != CorrectAnswers[i])
            {
                clearFlg = false;
            }
        }

        Debug.Log($"clearFlg = {clearFlg}");

        if (clearFlg)
        {
            SceneLoader.I.LoadScene(SceneLoader.MoonPoolSceneName, 2f, 1f, 2f);
        }
        else
        {
            jellyfishDialogue.sentenceIndex = 5;
            var coroutine = jellyfishDialogue.GetStartDialogue();

            yield return coroutine;

            gameOverGroup.gameObject.SetActive(true);
            tween = gameOverGroup.DOFade(1f, 1f);
            tween = DOTween.To(() => 0f, (x) => gameOverText.characterSpacing = x, 30f, 3f);
            yield return tween.WaitForCompletion();

            yield return new WaitUntil(() => Input.anyKey);

            SceneLoader.I.LoadScene(SceneLoader.TitleSceneName, 2f, 1f, 2f);
        }
    }

    public void TriggerEvent()
    {
        EventTrigger = true;
    }

    public void ChoiceRight()
    {
        CurrentChoice = Choice.Right;
    }

    public void ChoiceLeft()
    {
        CurrentChoice = Choice.Left;
    }
}