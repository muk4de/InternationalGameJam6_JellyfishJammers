using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class InteractionEvent : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] GameObject visualCue;
    [SerializeField] float popupDuration = 0.1f;
    Vector3 visualCueScale;

    [SerializeField] InputAction interactAction;
    private bool isPlayerInRange;

    [Header("Interact Event")]
    public UnityEvent OnInteract;


    Tween tween;

    void Start()
    {
        if (visualCue != null)
        {
            visualCue.SetActive(false);
            visualCueScale = visualCue.transform.localScale;
            visualCue.transform.position = new Vector3(visualCue.transform.position.x, visualCue.transform.position.y, -5);
            visualCue.transform.localScale = Vector3.zero;
        }
    }

    void Update()
    {
        if (isPlayerInRange && interactAction.IsPressed())
        {
            Debug.Log("interacted");
            OnInteract.Invoke();
        }
    }


    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInRange = true;
            if (visualCue != null)
            {
                tween.Kill(true);
                visualCue.SetActive(true);
                tween = visualCue.transform.DOScale(visualCueScale, popupDuration);
            }
            if(interactAction != null)
            {
                interactAction.Enable();
            }
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInRange = false;
            if (visualCue != null)
            {
                tween.Kill(true);
                tween = visualCue.transform.DOScale(0f, popupDuration).OnKill(() => visualCue.SetActive(false));
            }
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
}