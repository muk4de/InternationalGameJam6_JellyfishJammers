using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [SerializeField] PlayerController playerController;

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void SetPlayerMovable(bool movable)
    {
        playerController.CanMove = movable;
    }
}
