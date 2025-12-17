using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Wire : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    [Header("References")]
    public Image image;             // ワイヤーの根元の画像
    public LineRenderer lineRenderer; // 線を描画するコンポーネント
    public Canvas canvas;           // 親のCanvas

    [HideInInspector] public Color customColor; // このワイヤーの色
    [HideInInspector] public RectTransform successTarget; // つなぐべき正解の右側ワイヤー
    [HideInInspector] public bool isConnected = false;

    private RectTransform rectTransform;
    private WireTask wireTask;
    private Vector3 startPos;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    // 初期化処理（WireTaskから呼ばれる）
    public void Setup(Color color, WireTask task, Canvas parentCanvas)
    {
        this.customColor = color;
        this.wireTask = task;
        this.canvas = parentCanvas;

        image.color = customColor;
        lineRenderer.startColor = customColor;
        lineRenderer.endColor = customColor;

        // LineRendererの初期化（根元の位置に点を打つ）
        isConnected = false;
        lineRenderer.positionCount = 2;
    }

    // 更新処理：UI座標をLineRenderer用のワールド座標に変換してセット
    private void UpdateLinePositions(Vector3 endPosition)
    {
        // UIのアンカー位置をワールド座標に変換
        Vector3 startWorldPos = transform.InverseTransformPoint(rectTransform.position);
        startWorldPos.z = 0; // 2D用にZを固定

        Vector3 endWorldPos = transform.InverseTransformPoint(endPosition); ;
        endWorldPos.z = 0;

        lineRenderer.SetPosition(0, startWorldPos);
        lineRenderer.SetPosition(1, endWorldPos);
    }

    // --- ドラッグイベント ---

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (isConnected) return; // 既に成功していたら動かせない

        // ドラッグ開始時に音を鳴らすなどはここ
        UpdateLinePositions(GetWorldPositionFromMouse(eventData));
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isConnected) return;

        // マウス位置に線の先端を追従させる
        UpdateLinePositions(GetWorldPositionFromMouse(eventData));
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (isConnected) return;

        // ドロップした場所に正解のオブジェクトがあるか判定
        if (IsOverSuccessTarget(eventData))
        {
            // 成功：吸着させる
            isConnected = true;
            UpdateLinePositions(successTarget.position);
            wireTask.CheckTaskCompletion(); // タスク完了チェック
        }
        else
        {
                // 失敗：線をリセット（見た目上、長さを0にする）
                ResetLine();
        }
    }

    public void ResetLine()
    { 
        lineRenderer.SetPosition(0, transform.InverseTransformPoint(Vector3.up * 100));
        lineRenderer.SetPosition(1, transform.InverseTransformPoint(Vector3.up * 100));
    }

    // マウス位置をワールド座標に変換
    private Vector3 GetWorldPositionFromMouse(PointerEventData eventData)
    {
        Vector3 worldPoint;
        RectTransformUtility.ScreenPointToWorldPointInRectangle(
            rectTransform, eventData.position, canvas.worldCamera, out worldPoint);
        return worldPoint;
    }

    // マウスの下に正解のターゲットがあるかチェック
    private bool IsOverSuccessTarget(PointerEventData eventData)
    {
        // UI要素の重なり判定
        if (eventData.pointerEnter != null)
        {
            // ヒットしたオブジェクトが、正解の右側ワイヤー(のImage等)か確認
            return eventData.pointerEnter.transform.parent == successTarget ||
                   eventData.pointerEnter.transform == successTarget;
        }
        return false;
    }


}