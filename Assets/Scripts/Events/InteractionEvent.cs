using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class InteractionEvent : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] CanvasGroup visualCueGroup;
    [SerializeField] float popupDuration = 0.1f;
    Vector3 visualCueScale;

    [SerializeField] InputAction interactAction;
    [SerializeField] private bool isPlayerInRange;

    [Header("Interact Event")]
    public UnityEvent OnEnterTrigger;
    public UnityEvent OnExitTrigger;
    public UnityEvent OnInteract;

    public bool CanInteract = true;

    Tween tween;

    void Start()
    {
        if (visualCueGroup != null)
        {
            visualCueGroup.gameObject.SetActive(false);
            visualCueGroup.alpha = 0f;
            visualCueScale = visualCueGroup.transform.localScale;
            visualCueGroup.transform.position = new Vector3(visualCueGroup.transform.position.x, visualCueGroup.transform.position.y, -5);
            visualCueGroup.transform.localScale = Vector3.zero;
        }
    }

    void Update()
    {
        if (!CanInteract) return;
        if (isPlayerInRange && interactAction.WasPressedThisFrame())
        {
            Debug.Log("interacted");
            OnInteract.Invoke();
        }
    }


    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!CanInteract) return;
        if (collision.CompareTag("Player"))
        {
            isPlayerInRange = true;
            OnEnterTrigger.Invoke();
            if (interactAction != null)
            {
                interactAction.Enable();
            }
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (!CanInteract) return;
        if (collision.CompareTag("Player"))
        {
            isPlayerInRange = false;
            OnExitTrigger.Invoke();
            if (interactAction != null)
            {
                interactAction.Disable();
            }
        }
    }

    private void OnDestroy()
    {
        if (tween != null && tween.IsActive())
        {
            tween.Kill(true);
        }
    }

    public void ShowVisualCue()
    {
        if (visualCueGroup != null)
        {
            tween.Kill(true);
            visualCueGroup.gameObject.SetActive(true);
            tween = visualCueGroup.DOFade(1f, popupDuration);
            tween = visualCueGroup.transform.DOScale(visualCueScale, popupDuration);
        }
    }

    public void HideVisualCue()
    {
        if (visualCueGroup != null)
        {
            tween.Kill(true);
            tween = visualCueGroup.DOFade(0f, popupDuration);
            tween = visualCueGroup.transform.DOScale(0f, popupDuration);
            tween.OnKill(() => visualCueGroup.gameObject.SetActive(false));
        }
    }
    public void EnableInteraction()
    {
        CanInteract = true;
    }

    public void DisableInteraction()
    {
        CanInteract = false;
        isPlayerInRange = false;
        HideVisualCue();
    }
}