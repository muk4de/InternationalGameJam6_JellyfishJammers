using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // Sliderを使うために必要
using DG.Tweening;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader I { get; private set; }

    [Header("Loading Screen")]
    [SerializeField] private CanvasGroup loadingCanvasGroup;
    [SerializeField] private Slider progressBar;

    public static readonly string TitleSceneName = "TitleScene";
    public static readonly string CitySceneName = "CityScene";
    public static readonly string MoonPoolSceneName = "MoonPoolScene";

    Tween tween;

    void Awake()
    {
        // シングルトン＆DontDestroyOnLoad
        if (I != null && I != this)
        {
            Destroy(gameObject);
            return;
        }
        I = this;
        DontDestroyOnLoad(gameObject);

        Debug.Assert(loadingCanvasGroup != null);
        Debug.Assert(progressBar != null);

        // 最初はロード画面を非表示にしておく
        loadingCanvasGroup.gameObject.SetActive(false);
        loadingCanvasGroup.blocksRaycasts = false;
    }

    /// <summary>
    /// 指定した名前のシーンを非同期で読み込みます。
    /// </summary>
    /// <param name="sceneName">シーンビルド設定の名前</param>
    public void LoadScene(string sceneName, float fadeInDur = 0.25f, float minLoadDur = 0.25f, float fadeOutDur = 0.25f)
    {
        // 他のコルーチンが動いていれば停止（念のため）
        StopAllCoroutines();
        StartCoroutine(LoadAsyncRoutine(sceneName, fadeInDur, minLoadDur, fadeOutDur));
    }

    /// <summary>
    /// 非同期でシーンを読み込むコルーチン
    /// </summary>
    private IEnumerator LoadAsyncRoutine(string sceneName, float fadeInDur, float minLoadDur, float fadeOutDur)
    {
        // 1. ロード画面を表示する
        if (loadingCanvasGroup != null)
        {
            loadingCanvasGroup.gameObject.SetActive(true);
        }
        if (progressBar != null)
        {
            progressBar.value = 0f; // スライダーをリセット
        }

        // 2. 非同期でシーンの読み込みを開始

        loadingCanvasGroup.alpha = 0f;
        tween = loadingCanvasGroup.DOFade(1f, fadeInDur).SetUpdate(true);
        yield return new WaitForSecondsRealtime(fadeInDur);

        loadingCanvasGroup.blocksRaycasts = true;

        // 3. 読み込みが終わるまでループ
        Time.timeScale = 0f;
        var time = 0f;
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        while (!operation.isDone || time < minLoadDur)
        {
            time += Time.unscaledDeltaTime;
            // operation.progress は 0.0 から 0.9 までの値をとる
            // 0.9 = 読み込み完了、1.0 = シーン有効化（isDone）

            // 0.9で完了とみなすため、0.9で割って0.0〜1.0の範囲に変換する
            float operationProg = Mathf.Clamp01(operation.progress / 0.9f);
            float timeProg = time / minLoadDur;
            if (progressBar != null)
            {
                progressBar.value = Mathf.Min(operationProg, timeProg); // スライダーに進捗を反映
            }

            yield return null; // 1フレーム待つ
        }
        loadingCanvasGroup.blocksRaycasts = false;
        Time.timeScale = 1f;

        tween = loadingCanvasGroup.DOFade(0f, fadeOutDur).SetUpdate(true);
        yield return new WaitForSecondsRealtime(fadeOutDur);

        if (loadingCanvasGroup != null)
        {
            loadingCanvasGroup.gameObject.SetActive(false);
        }

        tween.Kill(true);
        tween = null;
    }

    private void OnDestroy()
    {
        if (tween != null && tween.IsActive() && tween.IsPlaying()) tween.Kill(true);
    }
}