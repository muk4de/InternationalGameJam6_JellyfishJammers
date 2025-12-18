using UnityEngine;
using TMPro;

public class PlayerDialogueController : MonoBehaviour
{
    [Header("UI References")]
    // プレイヤーの頭上に表示するCanvasGroup
    public CanvasGroup dialogueGroup;
    // その中のテキスト
    public TextMeshProUGUI dialogueTmp;

    private void Start()
    {
        // 初期化：隠しておく
        if (dialogueGroup != null)
        {
            dialogueGroup.alpha = 0f;
            dialogueGroup.gameObject.SetActive(false);
            dialogueGroup.transform.localScale = Vector3.zero;
        }
    }

    // 必要に応じて、プレイヤーの向きに合わせて吹き出しを反転させる処理などをここに書く
}