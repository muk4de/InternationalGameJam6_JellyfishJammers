using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class NarrationManager : MonoBehaviour
{
    public static NarrationManager Instance;

    [Header("Settings")]
    [SerializeField] float panelFadeDuration = 1.0f;
    [SerializeField] float textFadeDuration = 0.5f;
    [SerializeField] float charInterval = 0.05f; // 1文字あたりの表示時間
    [SerializeField] InputAction skipAction;

    [Header("References")]
    [SerializeField] CanvasGroup panelGroup;
    [SerializeField] TextMeshProUGUI tmp;

    [Header("Events")]
    public UnityEvent OnNarrationFinished; // ナレーション終了時に呼ばれるイベント


    private Tween currentTween;
    private bool isSkipInput = false;
    private bool isPlaying = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void OnEnable()
    {
        skipAction.Enable();
    }

    private void OnDisable()
    {
        skipAction.Disable();
    }

    void Update()
    {
        if (!isPlaying) return;
        if (skipAction.WasPressedThisFrame())
        {
            isSkipInput = true;
        }
    }

    // 外部からこのコルーチンを開始する
    public IEnumerator StartNarration(List<string> sentences, bool showInstant = false)
    {
        // 1. パネルを表示
        yield return ShowPanelRoutine(showInstant);

        Time.timeScale = 0f;

        // 少し待機 (Unscaled)
        yield return WaitUnscaled(2.0f);


        isPlaying = true;
        // 2. 文章を順番に表示
        foreach (var sentence in sentences)
        {
            // 文章のセットアップ
            tmp.text = sentence;
            tmp.maxVisibleCharacters = 0;
            tmp.alpha = 1f;

            // A. フェードインしながら文字送り（DOTween）
            float duration = sentence.Length * charInterval;

            currentTween = DOTween.To(
                () => 0,
                x => tmp.maxVisibleCharacters = x,
                sentence.Length,
                duration
            )
            .SetEase(Ease.Linear)
            .SetUpdate(true);

            currentTween.OnUpdate(() =>
            {
                if (isSkipInput)
                {
                    isSkipInput = false;
                    currentTween.Kill(true);
                }
            });
            yield return currentTween.WaitForCompletion();

            while (!isSkipInput)
            {
                yield return new WaitForEndOfFrame();
            }
            isSkipInput = false;

            // C. 文字をフェードアウト
            currentTween = tmp.DOFade(0f, textFadeDuration)
                .SetUpdate(true); 

            yield return currentTween.WaitForCompletion();
        }

        isPlaying = false;

        Time.timeScale = 1f;

        // 3. 全て終わったらパネルを消す
        yield return HidePanelRoutine();

        // 4. 終了イベント発火
        OnNarrationFinished.Invoke();
    }

    // ★重要：Time.timeScale=0 の環境下で、早送り倍率(currentSpeed)を反映して待機する関数
    private IEnumerator WaitUnscaled(float time)
    {
        float timer = 0f;
        while (timer < time)
        {
            // UnscaledDeltaTimeを使うことで停止中も進む
            // currentSpeedを掛けることで早送りを実現
            timer += Time.unscaledDeltaTime;
            yield return null;
        }
    }

    // パネル表示用コルーチン
    private IEnumerator ShowPanelRoutine(bool showInstant)
    {
        panelGroup.gameObject.SetActive(true);

        if (showInstant)
        {
            panelGroup.alpha = 1f;
            yield break;
        }
        else
        {
            panelGroup.alpha = 0f;
            currentTween = panelGroup.DOFade(1f, panelFadeDuration)
                .SetUpdate(true);
            yield return currentTween.WaitForCompletion();
        }
    }

    // パネル非表示用コルーチン
    private IEnumerator HidePanelRoutine()
    {
        currentTween = panelGroup.DOFade(0f, panelFadeDuration)
            .SetUpdate(true);
        yield return currentTween.WaitForCompletion();
        panelGroup.gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        if (currentTween != null || currentTween.IsActive()) currentTween.Kill(true);
    }
}