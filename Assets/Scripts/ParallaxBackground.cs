using Unity.Cinemachine;
using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    [Header("Settings")]
    public GameObject cam;

    [RangeAttribute(0f, 1f)]public float parallaxEffect;

    public const float farMax = 50;

    private float length;
    private float startpos;

    void Start()
    {
        startpos = transform.position.x;
        length = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    void Update()
    { 
        float dist = (cam.transform.position.x * parallaxEffect);
        float temp = (cam.transform.position.x * (1 - parallaxEffect));

        transform.position = new Vector3(startpos + dist, transform.position.y, transform.position.z);

        if (temp > startpos + length)
        {
            startpos += length;
        }
        else if (temp < startpos - length)
        {
            startpos -= length;
        }
    }

    private void OnValidate()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, farMax * parallaxEffect);
    }
}