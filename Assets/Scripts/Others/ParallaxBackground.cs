using Unity.Cinemachine;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class ParallaxBackground : MonoBehaviour
{
    [Header("Settings")]
    [Range(-0.1f, 1f)]
    public float parallaxEffect;

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
    }

    private void OnValidate()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, farMax * parallaxEffect);
    }
}