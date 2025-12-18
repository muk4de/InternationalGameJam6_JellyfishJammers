using UnityEngine;
using UnityEngine.Events;

public class EventTrigger : MonoBehaviour
{
    public string Tag = "Player";

    public UnityEvent OnEnterEvent;
    public UnityEvent OnExitEvent;

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
