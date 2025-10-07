using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Events : MonoBehaviour
{
    [Header("Wave Banner")]
    [SerializeField] CanvasGroup waveBanner;
    [SerializeField] TextMeshProUGUI waveNumText;

    [Header("Boss Incoming")]
    [SerializeField] CanvasGroup bossIncomingBanner;

    [Header("Boss HP Panel")]
    [SerializeField] CanvasGroup bossHpBanner;
    [SerializeField] TextMeshProUGUI bossNameText;
    [SerializeField] Image bossHpFill;

    [Header("Animation")]
    [SerializeField, Min(0f)] float holdTime = 1.0f;
    [SerializeField, Min(0f)] float fadeTime = 0.8f;

    [Header("Optional")]
    [SerializeField] WaveManager waveManager;

    Coroutine waveRoutine;
    Coroutine bossIncomingRoutine;
    Coroutine bossHpFadeRoutine;

    void Awake()
    {
        if (!waveManager) waveManager = GetComponent<WaveManager>();

        SetHidden(waveBanner);
        SetHidden(bossIncomingBanner);
        SetHidden(bossHpBanner);
    }
    void OnEnable()
    {
        if (!waveManager) return;
        waveManager.OnWaveStarted += HandleWaveStarted;
        waveManager.OnBossIncoming += HandleBossIncoming;
    }

    void OnDisable()
    {
        if (!waveManager) return;
        waveManager.OnWaveStarted -= HandleWaveStarted;
        waveManager.OnBossIncoming -= HandleBossIncoming;
    }

    void HandleWaveStarted(int waveIndex, bool isBossWave)
    {
        if (!isBossWave) ShowWave(waveIndex);
    }

    void HandleBossIncoming()
    {
        ShowBossIncoming();
    }

    public void ShowWave(int waveIndex)
    {
        StopAndNull(ref waveRoutine);
        waveRoutine = StartCoroutine(ShowBanner(waveBanner, () =>
        {
            if (waveNumText) waveNumText.text = $"Wave: {waveIndex}";
        }));
    }

    public void ShowBossIncoming()
    {
        StopAndNull(ref bossIncomingRoutine);
        bossIncomingRoutine = StartCoroutine(ShowBanner(bossIncomingBanner, null));
    }

    public void ShowBoss(string name, float hpPercent = 1f)
    {
        if (bossNameText) bossNameText.text = name;
        if (bossHpFill) bossHpFill.fillAmount = Mathf.Clamp01(hpPercent);

        StopAndNull(ref bossHpFadeRoutine);
        Show(bossHpBanner);
    }

    public void BossHP(float current, float max)
    {
        if (!bossHpFill) return;
        float p = (max > 0f) ? current / max : 0f;
        bossHpFill.fillAmount = Mathf.Clamp01(p);
    }

    public void BossDead()
    {
        StopAndNull(ref bossHpFadeRoutine);
        bossHpFadeRoutine = StartCoroutine(FadeOut(bossHpBanner));
    }

    IEnumerator ShowBanner(CanvasGroup cg, System.Action beforeShow)
    {
        if (!cg) yield break;

        beforeShow?.Invoke();
        Show(cg);

        if (holdTime > 0f)
            yield return new WaitForSeconds(holdTime);

        yield return FadeOut(cg);
    }

    IEnumerator FadeOut(CanvasGroup cg)
    {
        if (!cg) yield break;

        float t = 0f;
        float start = cg.alpha;

        while (t < fadeTime)
        {
            t += Time.deltaTime;
            float k = fadeTime > 0f ? Mathf.Clamp01(t / fadeTime) : 1f;
            cg.alpha = Mathf.Lerp(start, 0f, k);
            yield return null;
        }

        SetHidden(cg);
    }

    void Show(CanvasGroup cg)
    {
        cg.gameObject.SetActive(true);
        cg.alpha = 1f;
        cg.interactable = false;
        cg.blocksRaycasts = false;
    }

    void SetHidden(CanvasGroup cg)
    {
        if (!cg) return;
        cg.alpha = 0f;
        cg.gameObject.SetActive(false);
        cg.interactable = false;
        cg.blocksRaycasts = false;
    }

    void StopAndNull(ref Coroutine routine)
    {
        if (routine != null)
        {
            StopCoroutine(routine);
            routine = null;
        }
    }
}
