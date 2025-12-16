using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(fileName = "NewSoundData", menuName = "Audio/Sound Data")]
public class SoundData : ScriptableObject
{
    public AudioClip clip;
    public AudioMixerGroup outputGroup;

    [Range(0f, 1f)]
    public float volume = 1f;

    [Range(0.1f, 3f)]
    public float pitch = 1f;

    public bool loop = false;

    [Range(0f, 1f)]
    public float spatialBlend = 0f; // 0.0 = 2D, 1.0 = 3D
}