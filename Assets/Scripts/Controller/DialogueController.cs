using TMPro;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine.Events;
using UnityEngine.InputSystem;
// リストのラッパークラス（修正版）
[System.Serializable]
public class DialogueScenario
{
    // stringのリストではなく、DialogueLineのリストに変更
    public DialogueLines lines;
    public UnityEvent OnStartDialogue;
    public UnityEvent OnEndDialogue;
}

public class DialogueController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float popupDuration = 0.2f;
    [SerializeField] float charInterval = 0.05f;
    [SerializeField] bool stopPlayer = true;
    [SerializeField] InputAction skipAction;

    [Header("Entity References (Self)")]
    [SerializeField] CanvasGroup myDialogueGroup;
    [SerializeField] TextMeshProUGUI myDialogueTmp;

    [Header("Player Reference")]
    // ここにシーン上のPlayerDialogueControllerをアタッチする
    [SerializeField] PlayerDialogueController playerController;

    [Header("General Events")]
    [SerializeField] UnityEvent OnStartDialogue;
    [SerializeField] UnityEvent OnEndDialogue;

    [SerializeField] List<DialogueScenario> sentences = new();

    public bool IsTalking { get; private set; } = false;
    public int sentenceIndex = 0;
    private bool isInputReceived = false;
    private Coroutine currentDialogueCoroutine;

    // 現在アクティブなウィンドウ情報（切り替え用）
    private CanvasGroup currentGroup;
    private TextMeshProUGUI currentTmp;
    private Vector3 currentInitialScale;

    private void OnEnable()
    {
        skipAction.Enable();
    }
    private void OnDisable()
    {
        skipAction.Disable();
    }

    void Awake()
    {
        InitializeWindow(myDialogueGroup);
        if (playerController != null)
        {
            InitializeWindow(playerController.dialogueGroup);
        }
    }

    public void NextSentence()
    {
        sentenceIndex++;
    }

    private void Update()
    {
        if (!IsTalking) return;
        if (skipAction.WasPressedThisFrame())
        {
            isInputReceived = true;
        }
    }

    private void InitializeWindow(CanvasGroup group)
    {
        if (group != null)
        {
            group.gameObject.SetActive(false);
            group.alpha = 0f;
            group.transform.localScale = Vector3.zero;
        }
    }

    public void StartDialogueWithDelay(float delay)
    {
        Invoke("StartDialogue", delay);
    }
    public void StartDialogue()
    {
        if (IsTalking) return;
        if (sentenceIndex < 0 || sentenceIndex >= sentences.Count) return;

        if (currentDialogueCoroutine != null) StopCoroutine(currentDialogueCoroutine);
        currentDialogueCoroutine = StartCoroutine(DialogueRoutine(sentences[sentenceIndex]));
    }

    public Coroutine GetStartDialogue()
    {
        if (IsTalking) return null;
        if (sentenceIndex < 0 || sentenceIndex >= sentences.Count) return null;

        if (currentDialogueCoroutine != null) StopCoroutine(currentDialogueCoroutine);
        currentDialogueCoroutine = StartCoroutine(DialogueRoutine(sentences[sentenceIndex]));
        return currentDialogueCoroutine;
    }


    private IEnumerator DialogueRoutine(DialogueScenario currentData)
    {
        IsTalking = true;
        isInputReceived = false;

        if (stopPlayer && GameManager.Instance != null) GameManager.Instance.SetPlayerMovable(false);

        OnStartDialogue?.Invoke();
        currentData.OnStartDialogue?.Invoke();

        // 前回の話者タイプ（初期値はnull）
        SpeakerType? lastSpeaker = null;

        foreach (var line in currentData.lines.list)
        {
            // --- 話者の切り替え判定 ---

            // 話者が変わった場合、前のウィンドウを閉じる
            if (lastSpeaker != null && lastSpeaker != line.speaker)
            {
                yield return HideWindowRoutine(currentGroup);
            }

            // 現在の操作対象UIをセット
            if (line.speaker == SpeakerType.Player && playerController != null)
            {
                currentGroup = playerController.dialogueGroup;
                currentTmp = playerController.dialogueTmp;
            }
            else // Entity (Self)
            {
                currentGroup = myDialogueGroup;
                currentTmp = myDialogueTmp;
            }

            // 初回、または話者が変わった時だけウィンドウを開く
            if (lastSpeaker != line.speaker)
            {
                // 元のスケールを取得（Vector3.zeroになってる可能性があるので工夫が必要）
                // ※ここでは簡易的に1としていますが、Prefabの設定に合わせる場合は別途保存が必要
                currentInitialScale = Vector3.one;
                yield return ShowWindowRoutine(currentGroup, currentInitialScale);
            }

            lastSpeaker = line.speaker;

            // --- 文字送り処理 (前回と同じロジック) ---

            currentTmp.text = line.text;
            currentTmp.maxVisibleCharacters = 0;

            int totalChars = line.text.Length;
            float duration = totalChars * charInterval;

            Tween typeTween = DOTween.To(
                () => currentTmp.maxVisibleCharacters,
                x => currentTmp.maxVisibleCharacters = x,
                totalChars,
                duration
            ).SetEase(Ease.Linear);

            while (typeTween.IsActive() && typeTween.IsPlaying())
            {
                if (isInputReceived)
                {
                    typeTween.Complete();
                    isInputReceived = false;
                    break;
                }
                yield return null;
            }

            yield return new WaitUntil(() => isInputReceived);
            isInputReceived = false;
        }

        // 最後のアクティブなウィンドウを閉じる
        yield return HideWindowRoutine(currentGroup);

        currentData.OnEndDialogue?.Invoke();
        OnEndDialogue?.Invoke();

        if (stopPlayer && GameManager.Instance != null) GameManager.Instance.SetPlayerMovable(true);

        IsTalking = false;
        currentDialogueCoroutine = null;
    }

    private IEnumerator ShowWindowRoutine(CanvasGroup group, Vector3 targetScale)
    {
        if (group == null) yield break;

        group.gameObject.SetActive(true);
        // テキストはクリアしておく
        if (currentTmp != null) currentTmp.text = "";

        Sequence seq = DOTween.Sequence();
        seq.Append(group.DOFade(1f, popupDuration));
        seq.Join(group.transform.DOScale(targetScale, popupDuration));

        yield return seq.WaitForCompletion();
    }

    private IEnumerator HideWindowRoutine(CanvasGroup group)
    {
        if (group == null || !group.gameObject.activeSelf) yield break;

        Sequence seq = DOTween.Sequence();
        seq.Append(group.DOFade(0f, popupDuration));
        seq.Join(group.transform.DOScale(0f, popupDuration));

        yield return seq.WaitForCompletion();

        group.gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        myDialogueGroup?.DOKill();
        if (playerController != null && playerController.dialogueGroup != null)
            playerController.dialogueGroup.DOKill();
    }
}