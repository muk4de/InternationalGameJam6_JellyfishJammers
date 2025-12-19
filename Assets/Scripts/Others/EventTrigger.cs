using UnityEngine;
using UnityEngine.Events;

public class EventTrigger : MonoBehaviour
{
    public string Tag = "Player";

    public UnityEvent OnEnterEvent;
    public UnityEvent OnExitEvent;
    public Collider2D Col;

    private void Start()
    {
        Col = GetComponent<Collider2D>();
    }

    public void TurnOffCollider()
    {
        Col.enabled = false;
    }

    public void TurnOnCollider()
    {
        Col.enabled = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(Tag))
        {
            OnEnterEvent.Invoke();
        }
    }


    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag(Tag))
        {
            OnExitEvent.Invoke();
        }
    }
}
