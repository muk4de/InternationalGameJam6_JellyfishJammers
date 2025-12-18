using UnityEngine;
using DG.Tweening;
using System.Collections;

public class JellyfishAnimController : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator animator;
    [SerializeField] private float swimInterval = 2f; // 泳ぐ間隔
    [SerializeField] private float swimPower = 5f;    // 泳ぐ力

    private void Start()
    {
        Swim();
        // 起動時に泳ぎのループを開始
        StartCoroutine(SwimLoop());
    }

    private IEnumerator SwimLoop()
    {
        while (true)
        {
            // 間隔 + ランダムなゆらぎを持たせて待機
            float randomDelay = Random.Range(0f, 1.0f);
            yield return new WaitForSeconds(swimInterval + randomDelay);

            Swim();
        }
    }

    void Swim()
    {
        if (animator != null)
        {
            animator.SetTrigger("Swim");
        }

        // --- 変更点: ランダムな方向を決定 ---
        // ランダムな方向ベクトルを取得 (正規化して長さを1にする)
        Vector2 randomDir = Random.insideUnitCircle.normalized;

        // Z成分は0にする (2D用)
        Vector3 diff = new Vector3(randomDir.x, randomDir.y, 0);

        // --- 既存の回転ロジックを流用 ---
        // 進行方向に向く角度を計算 (-90fはスプライトの向きに合わせて調整してください)
        float targetAngle = Mathf.Rad2Deg * Mathf.Atan2(diff.y, diff.x) - 90f;

        // 現在の角度との差分を計算
        float angleDiff = Mathf.DeltaAngle(rb.rotation, targetAngle);

        // 回転実行
        rb.DORotate(rb.rotation + angleDiff, swimInterval * 0.5f).SetEase(Ease.OutCubic);

        // --- 力を加える ---
        // 回転がある程度進んでから力を加える方が自然な場合は、ここで遅延を入れるか
        // DOVirtual.DelayedCallを使う手もありますが、まずは同時に実行します。
        rb.AddForce(randomDir * swimPower, ForceMode2D.Impulse);
    }
}