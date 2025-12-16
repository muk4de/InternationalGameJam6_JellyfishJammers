using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventSequencer : MonoBehaviour
{
    public List<GameEventBase> eventList;

    public void StartSequence()
    {
        StartCoroutine(RunSequence());
    }

    private IEnumerator RunSequence()
    {
        foreach (var gameEvent in eventList)
        {
            yield return gameEvent.Execute();
        }

        Debug.Log("finished all event");
    }

    void Start()
    {
        StartSequence();
    }
}