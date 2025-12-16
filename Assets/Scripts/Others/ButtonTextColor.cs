using UnityEngine;
using UnityEngine.EventSystems; // イベント検知に必要
using TMPro;                    // TextMeshProに必要
using DG.Tweening;              // DOTweenに必要

public class ButtonTextColor : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [Header("Settings")]
    [SerializeField] TextMeshProUGUI targetText;
    [SerializeField] float duration = 0.1f; // 色が変わる速さ

    [Header("Colors")]
    [SerializeField] Color normalColor = Color.white;
    [SerializeField] Color highlightColor = Color.yellow;
    [SerializeField] Color pressedColor = Color.gray;

    // 現在のアニメーションを管理する変数
    private Tween currentTween;

    void Start()
    {
        // 自動で取得（アタッチし忘れた場合用）
        if (targetText == null) targetText = GetComponentInChildren<TextMeshProUGUI>();

        // 初期色を設定
        if (targetText != null) targetText.color = normalColor;
    }

    private void OnEnable()
    {
        ChangeColor(normalColor, true);
    }

    // ハイライト（マウスが乗った / 選択された）
    public void OnPointerEnter(PointerEventData eventData)
    {
        ChangeColor(highlightColor);
    }

    // ノーマル（マウスが離れた）
    public void OnPointerExit(PointerEventData eventData)
    {
        ChangeColor(normalColor);
    }

    // プレス（押した瞬間）
    public void OnPointerDown(PointerEventData eventData)
    {
        ChangeColor(pressedColor);
    }

    // アップ（離した瞬間＝ハイライトに戻るのが一般的）
    public void OnPointerUp(PointerEventData eventData)
    {
        ChangeColor(highlightColor);
    }

    // DOTweenで色を変える処理
    void ChangeColor(Color color, bool immidiate = false)
    {
        if (targetText == null) return;
        if(immidiate)
        {
            targetText.color = color;
            return;
        }

        // 前のアニメーションをキャンセルして新しい色へ
        if (currentTween != null) currentTween.Kill();
        currentTween = targetText.DOColor(color, duration);
    }

    // オブジェクト破棄時の処理
    void OnDestroy()
    {
        if (currentTween != null) currentTween.Kill();
    }
}