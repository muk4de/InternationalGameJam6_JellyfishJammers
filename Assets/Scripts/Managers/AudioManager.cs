using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager I { get; private set; }

    [Header("Mixer")]
    [SerializeField] private AudioMixer mainMixer;

    [Header("BGM Settings")]
    [SerializeField] private AudioSource bgmSourceA;
    [SerializeField] private AudioSource bgmSourceB;
    private bool isSourceA_Active = true;
    private Coroutine _bgmFadeCoroutine;

    [Header("SFX Pool Settings")]
    [SerializeField] private int sfxPoolSize = 20;
    [SerializeField] private AudioMixerGroup sfxMixerGroup; // デフォルトのSFXグループ

    private Queue<AudioSource> _sfxPool;
    private List<AudioSource> _activeSfxSources; // プールに戻すためにアクティブなものを追跡

    void Awake()
    {
        // 定番のシングルトン＆DontDestroyOnLoad
        if (I != null && I != this)
        {
            Destroy(gameObject);
            return;
        }
        I = this;
        DontDestroyOnLoad(gameObject);

        // BGMソースの初期設定
        ConfigureAudioSource(bgmSourceA);
        ConfigureAudioSource(bgmSourceB);

        // SFXプールの初期化
        InitializeSfxPool();
    }

    // SFXプールを初期化
    private void InitializeSfxPool()
    {
        _sfxPool = new Queue<AudioSource>(sfxPoolSize);
        _activeSfxSources = new List<AudioSource>(sfxPoolSize);

        for (int i = 0; i < sfxPoolSize; i++)
        {
            GameObject sfxObject = new GameObject($"SFX_Source_{i}");
            sfxObject.transform.SetParent(transform);
            AudioSource source = sfxObject.AddComponent<AudioSource>();

            // デフォルト設定
            source.outputAudioMixerGroup = sfxMixerGroup;
            source.playOnAwake = false;

            sfxObject.SetActive(false); // プールでは非アクティブに
            _sfxPool.Enqueue(source);
        }
    }

    // BGMソースの共通設定
    private void ConfigureAudioSource(AudioSource source)
    {
        if (source == null)
        {
            Debug.LogError("BGM AudioSourceがアタッチされていません。");
            return;
        }
        source.playOnAwake = false;
        source.loop = true;
    }

    #region BGM Control

    // BGMをクロスフェードで再生
    public void PlayBgm(SoundData bgmData, float fadeDuration = 1.0f)
    {
        if (bgmData == null || bgmData.clip == null)
        {
            Debug.LogWarning("BGMのSoundDataまたはClipがありません。");
            return;
        }

        if (_bgmFadeCoroutine != null)
        {
            StopCoroutine(_bgmFadeCoroutine);
        }
        _bgmFadeCoroutine = StartCoroutine(FadeBgmRoutine(bgmData, fadeDuration));
    }

    private IEnumerator FadeBgmRoutine(SoundData bgmData, float duration)
    {
        AudioSource activeSource = isSourceA_Active ? bgmSourceA : bgmSourceB;
        AudioSource newSource = isSourceA_Active ? bgmSourceB : bgmSourceA;

        // 新しいソースを設定
        ApplySoundData(newSource, bgmData);
        newSource.volume = 0f;
        newSource.Play();

        float timer = 0f;

        // 現在のBGMのボリューム（フェードアウト用）
        float startVolume = activeSource.volume;
        // 新しいBGMのターゲットボリューム（SoundDataから）
        float targetVolume = bgmData.volume;

        while (timer < duration)
        {
            timer += Time.unscaledDeltaTime;
            float progress = Mathf.Clamp01(timer / duration);

            activeSource.volume = Mathf.Lerp(startVolume, 0f, progress);
            newSource.volume = Mathf.Lerp(0f, targetVolume, progress);

            yield return null;
        }

        activeSource.Stop();
        activeSource.volume = 0f; // 念のため
        newSource.volume = targetVolume; // 最終ボリュームを確定

        isSourceA_Active = !isSourceA_Active; // アクティブなソースを切り替え
        _bgmFadeCoroutine = null;
    }

    #endregion

    #region SFX Control

    // 2DのSFXを再生（UIクリックなど）
    public void PlaySfx(SoundData sfxData)
    {
        if (sfxData == null || sfxData.clip == null) return;

        AudioSource source = GetAvailableSfxSource();
        if (source == null)
        {
            Debug.LogWarning("SFXプールが枯渇しています。");
            return;
        }

        ApplySoundData(source, sfxData);
        source.spatialBlend = 0f; // 2Dを強制

        source.gameObject.SetActive(true);
        source.Play();
        StartCoroutine(ReturnToPoolAfterPlay(source, sfxData.clip.length));
    }

    // 3DのSFXを再生（爆発音など）
    public void PlaySfxAtPosition(SoundData sfxData, Vector3 position)
    {
        if (sfxData == null || sfxData.clip == null) return;

        AudioSource source = GetAvailableSfxSource();
        if (source == null) return;

        source.transform.position = position;
        ApplySoundData(source, sfxData);
        source.spatialBlend = sfxData.spatialBlend > 0 ? sfxData.spatialBlend : 1.0f; // 3Dを強制

        source.gameObject.SetActive(true);
        source.Play();
        StartCoroutine(ReturnToPoolAfterPlay(source, sfxData.clip.length));
    }

    // プールから利用可能なAudioSourceを取得
    private AudioSource GetAvailableSfxSource()
    {
        if (_sfxPool.Count == 0)
        {
            // プールが足りない場合、動的に拡張する（が、警告は出す）
            Debug.LogWarning("SFX Pool is empty. Expanding pool.");
            // ここでは簡易的にnullを返すが、本来はプール拡張処理
            return null;
        }

        AudioSource source = _sfxPool.Dequeue();
        _activeSfxSources.Add(source);
        return source;
    }

    // 再生終了後にプールに戻す
    private IEnumerator ReturnToPoolAfterPlay(AudioSource source, float duration)
    {
        yield return new WaitForSeconds(duration);

        source.Stop();
        source.gameObject.SetActive(false);
        _activeSfxSources.Remove(source);
        _sfxPool.Enqueue(source);
    }

    #endregion

    #region Utility

    // SoundDataをAudioSourceに適用するヘルパーメソッド
    private void ApplySoundData(AudioSource source, SoundData data)
    {
        source.clip = data.clip;
        source.outputAudioMixerGroup = data.outputGroup;
        source.volume = data.volume;
        source.pitch = data.pitch;
        source.loop = data.loop;
        source.spatialBlend = data.spatialBlend;
    }

    // AudioMixerのパラメータを設定（設定画面用）
    public void SetMasterVolume(float volume)
    {
        // 0から1の値を-80dBから0dBのデシベルに変換
        mainMixer.SetFloat("master", Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20f);
    }
    public void SetBgmVolume(float volume)
    {
        mainMixer.SetFloat("bgm", Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20f);
    }
    public void SetSfxVolume(float volume)
    {
        mainMixer.SetFloat("sfx", Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20f);
    }

    public float GetMasterVolume()
    {
        mainMixer.GetFloat("master", out var raw);
        return Mathf.Pow(10f, raw / 20f);
    }

    public float GetBgmVolume()
    {
        mainMixer.GetFloat("bgm", out var raw);
        return Mathf.Pow(10f, raw / 20f);
    }

    public float GetSfxVolume()
    {
        mainMixer.GetFloat("sfx", out var raw);
        return Mathf.Pow(10f, raw / 20f);
    }
    #endregion
}