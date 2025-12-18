using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class JellyfishController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float swimInterval = 1f;
    [SerializeField] float swimPower = 1f;

    //public
    public Transform Player;
    public bool IsFollow;

    // private 
    bool isPlayerInRange = false;
    float swimTimer = 0f;
    Rigidbody2D rb;
    Animator animator;

    void Start()
    {
        swimTimer = Random.value * swimInterval;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    public void SetFollowPlayer(bool follow)
    {
        IsFollow = follow;
    }

    void Update()
    {
        if (!IsFollow) return;
        swimTimer += Time.deltaTime;
        if (swimTimer > swimInterval)
        {
            swimTimer = 0f;
            Swim();
        }
    }

    void Swim()
    {
        if (Player == null) return;

        if (isPlayerInRange)
        {
            float targetAngle = 0f;
            float angleDiff = Mathf.DeltaAngle(rb.rotation, targetAngle);
            rb.DORotate(rb.rotation + angleDiff, swimInterval).SetEase(Ease.OutCubic);
            return;
        }
        else
        {
            if (animator != null)
            {
                animator.SetTrigger("Swim");
            }
            var diff = Player.position - transform.position;
            diff.z = 0f;

            float targetAngle = Mathf.Rad2Deg * Mathf.Atan2(diff.y, diff.x) - 90f;

            float angleDiff = Mathf.DeltaAngle(rb.rotation, targetAngle);

            rb.DORotate(rb.rotation + angleDiff, swimInterval).SetEase(Ease.OutCubic);
            rb.AddForce(diff.normalized * swimPower, ForceMode2D.Impulse);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInRange = true;
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInRange = false;
        }
    }
}
