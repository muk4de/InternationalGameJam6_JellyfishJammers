using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Events;

public class TitleStartEvent : MonoBehaviour
{
    public UnityEvent StartEvent;
    public SoundData StartBGMData;
    void Start()
    {
        StartEvent?.Invoke();
        AudioManager.I.PlayBgm(StartBGMData);
    }
}
