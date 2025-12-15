using Unity.Cinemachine;
using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    [Header("Settings")]
    [Range(0f, 1f)]
    public float parallaxEffect;

    public bool loopX = false;
    public bool loopY = false;

    public const float farMax = 50;

    private Vector2 size;
    private Vector3 startpos;

    void Awake()
    {
        startpos = transform.position;
        size = GetComponent<SpriteRenderer>().bounds.size;
    }
    void OnEnable()
    {
        CinemachineCore.CameraUpdatedEvent.AddListener(OnCameraUpdated);
    }
    void OnDisable()
    {
        CinemachineCore.CameraUpdatedEvent.RemoveListener(OnCameraUpdated);
    }

    void OnCameraUpdated(CinemachineBrain brain)
    {
        var cam = Camera.main;

        Vector3 dist = cam.transform.position * parallaxEffect;

        Vector3 temp = cam.transform.position * (1 - parallaxEffect);

        transform.position = new Vector3(startpos.x + dist.x, startpos.y + dist.y, transform.position.z);

        if (loopX)
        {
            if (temp.x > startpos.x + size.x)
            {
                startpos.x += size.x;
            }
            else if (temp.x < startpos.x - size.x)
            {
                startpos.x -= size.x;
            }
        }

        if (loopY)
        {
            if (temp.y > startpos.y + size.y)
            {
                startpos.y += size.y;
            }
            else if (temp.y < startpos.y - size.y)
            {
                startpos.y -= size.y;
            }
        }
    }

    private void OnValidate()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, farMax * parallaxEffect);
    }
}