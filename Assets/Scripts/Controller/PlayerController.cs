using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    // inspector
    [Header("Player Movement")]
    [SerializeField] float moveSpeed = 10f;
    [SerializeField] float accelerationTime = 0.5f;
    [SerializeField] float decelerationTime = 0.5f;

    [Header("Input system")]
    [SerializeField] InputAction moveAction;

    // public 
    public bool CanMove = true;

    // private
    Rigidbody2D rb;
    Vector2 moveInput;

    private void OnEnable()
    {
        moveAction.Enable();
    }

    private void OnDisable()
    {
        moveAction.Disable();
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        moveInput = moveAction.ReadValue<Vector2>();

    }

    void FixedUpdate()
    {
        if (CanMove)
        {
            Flip();
            Swim();
        }
    }

    void Swim()
    {
        Vector2 targetVelocity = moveInput * moveSpeed;

        float accelerationRate;

        if (moveInput.sqrMagnitude > 0.01f)
        {
            accelerationRate = moveSpeed / Mathf.Max(accelerationTime, 0.01f);
        }
        else
        {
            accelerationRate = moveSpeed / Mathf.Max(decelerationTime, 0.01f);
        }

        rb.linearVelocity = Vector2.MoveTowards(
            rb.linearVelocity,
            targetVelocity,
            accelerationRate * Time.fixedDeltaTime
        );
    }

    void Flip()
    {
        if (moveInput.x > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        else if (moveInput.x < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
    }
}