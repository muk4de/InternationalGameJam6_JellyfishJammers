using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class MemoController : MonoBehaviour
{
    [SerializeField] RectTransform memoRect;
    [SerializeField] Button exitButton;

    [Header("Panel Animation")]
    [SerializeField] float slideDuration = 1f;
    [SerializeField] float slideLength = 1000f;

    Vector3 memoStartPos;
    Tween tween;
    public bool IsOpening = false;


    private void Start()
    {
        memoRect.gameObject.SetActive(false);
        memoStartPos = memoRect.anchoredPosition;
        memoRect.anchoredPosition = memoStartPos - new Vector3(0, slideLength);
    }

    public void OpenMemo()
    {
        if (IsOpening) return;   
        
        IsOpening = true;

        if (tween.IsActive()) tween.Kill(true);

        exitButton.interactable = true;
        memoRect.gameObject.SetActive(true);
        tween = memoRect.DOAnchorPosY(memoStartPos.y, slideDuration).SetEase(Ease.OutCubic);
    }

    public void CloseMemo()
    {
        if (!IsOpening) return;
        IsOpening = false;

        if (tween.IsActive()) tween.Kill(true);

        exitButton.interactable = false;
        tween = memoRect.DOAnchorPosY(memoStartPos.y - slideLength, slideDuration).SetEase(Ease.OutCubic);
        tween.OnComplete(() =>
        {
            memoRect.gameObject.SetActive(false);
        });
    }

    private void OnDestroy()
    {
        if (tween != null && tween.IsActive())
        {
            tween.Kill(true);
        }
    }
}
