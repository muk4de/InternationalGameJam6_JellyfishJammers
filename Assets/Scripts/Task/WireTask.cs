using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class WireTask : MonoBehaviour
{
    [Header("Configuration")]
    public List<Color> availableColors; // 使用する色のリスト（赤、青、黄色、ピンクなど）
    public GameObject taskPanel;        // タスク画面全体のパネル
    public Canvas parentCanvas;         // 親キャンバス
    public Button exitButton;

    [Header("Panel Animation")]
    [SerializeField] float slideDuration = 1f;
    [SerializeField] float slideLength = 1000f;

    [Header("Wire Objects")]
    public List<Wire> leftWires;        // 左側のワイヤー（Wireスクリプト付き）
    public List<Image> rightWires;      // 右側のワイヤー（ただのImageでOK）

    [Header("Events")]
    public UnityEvent OnTaskCompleted;  // タスク完了時に実行したいイベント

    private List<int> currentIndices;   // 色の割り当て用インデックス

    public bool IsCompleted = false;
    Vector3 panelStartPos;
    RectTransform panelRect;
    Sequence sequence;

    public bool IsOpening = false;

    void Start()
    {
        // 最初は非表示
        taskPanel.SetActive(false);
        panelRect = taskPanel.GetComponent<RectTransform>();
        panelStartPos = panelRect.anchoredPosition;
        panelRect.anchoredPosition = panelStartPos - new Vector3(0, slideLength);

        GenerateTask();
    }

    // 既存のInteractionEventスクリプトから呼ぶ関数

    [ContextMenu("OpenTask")]
    public void OpenTask()
    {
        if (IsOpening) return;
        IsOpening = true;
        for(int i = 0; i < leftWires.Count; i++)
        {
            leftWires[i].ResetLine();
        }
        GameManager.Instance.SetPlayerMovable(false);
        SequenceSetup(Ease.OutCubic);
        ShowTaskPanel(sequence);
    }

    public void CloseTask()
    {
        if (!IsOpening) return;
        IsOpening = false;
        GameManager.Instance.SetPlayerMovable(true);
        foreach(var wire in leftWires)
        {
            wire.isConnected = false;
        }
        SequenceSetup(Ease.OutCubic);
        HideTaskPanel(sequence);
    }

    void SequenceSetup(Ease ease)
    {
        if (sequence.IsActive()) sequence.Kill(true);
        sequence = DOTween.Sequence();
        sequence.SetEase(ease);
    }

    void ShowTaskPanel(Sequence seq)
    {
        seq.AppendCallback(() => taskPanel.SetActive(true));
        seq.Append(panelRect.DOAnchorPosY(panelStartPos.y, slideDuration));
    }

    void HideTaskPanel(Sequence seq)
    {
        seq.Append(panelRect.DOAnchorPosY(panelStartPos.y - slideLength, slideDuration));
        seq.AppendCallback(() => taskPanel.SetActive(false));
    }

    // ワイヤーの色と正解ペアをランダム生成
    private void GenerateTask()
    {
        // 1. 色のリストを作成（0, 1, 2, 3...）
        List<int> indices = new List<int>();
        for (int i = 0; i < leftWires.Count; i++) indices.Add(i);

        // 2. 左側はそのまま、右側をシャッフルする
        List<int> leftIndices = new List<int>(indices);
        List<int> rightIndices = indices.OrderBy(x => Random.value).ToList();

        // 3. それぞれに適用
        for (int i = 0; i < leftWires.Count; i++)
        {
            // 左側の設定
            Color color = availableColors[leftIndices[i]];
            Wire leftWire = leftWires[i];

            // 右側の正解を見つける（右側のリストから同じ色のインデックスを持つものを探す）
            int targetIndexInRightList = rightIndices.IndexOf(leftIndices[i]);
            Image targetRightWire = rightWires[targetIndexInRightList];

            // 色を適用
            targetRightWire.color = color;

            // Wireスクリプトに情報を渡す
            leftWire.Setup(color, this, parentCanvas);
            leftWire.successTarget = targetRightWire.rectTransform;

            // 右側ワイヤーにもRaycastTargetが必要（当たり判定用）
            targetRightWire.raycastTarget = true;
        }
    }

    // ワイヤーがつながるたびに呼ばれる
    public void CheckTaskCompletion()
    {
        bool allConnected = true;
        foreach (var wire in leftWires)
        {
            if (!wire.isConnected)
            {
                allConnected = false;
                break;
            }
        }

        if (allConnected)
        {
            Debug.Log("タスク完了！");
            IsCompleted = true;
            OnTaskCompleted.Invoke();
            exitButton.interactable = false;
            // 少し待ってから閉じるなどの演出を入れる場合はコルーチン推奨
            Invoke(nameof(CloseTask), 1.0f);
        }
    }
}