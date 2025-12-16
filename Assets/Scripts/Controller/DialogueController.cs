using TMPro;
using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class DialogueController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float popupDuration = 0.1f;
    [SerializeField] float charInterval = 0.05f;

    [Header("Reference")]
    [SerializeField] CanvasGroup dialogueGroup;
    [SerializeField] TextMeshProUGUI dialogueTmp;

    [Header("Events")]
    [SerializeField] UnityEvent OnStartDialogue;
    [SerializeField] UnityEvent OnEndDialogue;

    [SerializeField] List<string> sentences = new List<string>();
    private Queue<string> sentenceQueue = new Queue<string>();

    public bool IsTalking = false;
    Vector3 dialogueGroupScale;
    Sequence seq;

    void Start()
    {
        if (dialogueGroup != null)
        {
            dialogueGroup.gameObject.SetActive(false);
            dialogueGroupScale = dialogueGroup.transform.localScale;
            dialogueGroup.transform.localScale = Vector3.zero;
        }
    }

    public void ExecuteDialogue()
    {
        CreateNewSequence();

        if (IsTalking)
        {
            if (sentenceQueue.Count == 0)
            {
                OnEndDialogue.Invoke();
                HideDialogue(true);
                return;
            }
            HideDialogue(false);
            ShowDialogue();
            DisplayNextSentence();
        }
        else
        {
            IsTalking = true;
            OnStartDialogue.Invoke();
            sentenceQueue.Clear();
            foreach (var sentence in sentences)
            {
                sentenceQueue.Enqueue(sentence);
            }

            ShowDialogue();
            DisplayNextSentence();
        }
    }

    public void ShowDialogue()
    {
        if (dialogueGroup != null)
        {
            seq.AppendCallback(() =>
            {
                dialogueTmp.text = "";
            });
            seq.AppendCallback(() => dialogueGroup.gameObject.SetActive(true));

            seq.Append(dialogueGroup.DOFade(1f, popupDuration));
            seq.Join(dialogueGroup.transform.DOScale(dialogueGroupScale, popupDuration));
        }
    }

    public void HideDialogue(bool isFinished)
    {
        if (!IsTalking) return;
        if (isFinished)
        {
            IsTalking = false;
            CreateNewSequence();
        }

        if (dialogueGroup != null)
        {
            seq.Append(dialogueGroup.DOFade(0f, popupDuration));
            seq.Join(dialogueGroup.transform.DOScale(0f, popupDuration));

            seq.AppendCallback(() => dialogueGroup.gameObject.SetActive(false));
        }
    }

    public void DisplayNextSentence()
    {
        var text = sentenceQueue.Dequeue();
        seq.AppendCallback(() =>
        {
            dialogueTmp.text = text;
            dialogueTmp.maxVisibleCharacters = 0;
        });

        var charNum = text.Length;
        var dur = text.Length * charInterval;
        seq.Append(DOTween.To(() => 0, (x) => dialogueTmp.maxVisibleCharacters = x, charNum, dur));
    }

    void CreateNewSequence()
    {
        if (seq != null && seq.IsActive())
        {
            seq.Kill(true);
        }
        seq = DOTween.Sequence();
    }

    private void OnDestroy()
    {
        if (seq != null && seq.IsActive())
        {
            seq.Kill(true);
        }
    }
}
