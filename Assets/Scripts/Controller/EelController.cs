using UnityEngine;

public class EelController : MonoBehaviour
{
    Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    [ContextMenu("BrokenWireAnim")]
    public void BrokenWireAnim()
    {
        animator.SetBool("BrokenWire", true);
        animator.SetBool("Swim", false);
    }

    [ContextMenu("Swim")]
    public void SwimAnim()
    {
        animator.SetBool("BrokenWire", false);
        animator.SetBool("Swim", true);
    }

    public void IdleAnimWithDelay(float delay)
    {
        Invoke("IdleAnim", delay);
    }

    [ContextMenu("Idle")]
    public void IdleAnim()
    {
        animator.SetBool("BrokenWire", false);
        animator.SetBool("Swim", false);
    }
}
