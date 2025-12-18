using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [SerializeField] PlayerController playerController;

    [Header("Director")]
    public PlayableDirector HelpJellyfish;

    public bool EventTrigger = false;
    [Header("Reference")]
    public DialogueController BlueJellyfishDialogue;

    public List<GameObject> HelpJellyfishSceneObj = new();
    public List<GameObject> EelSceneObj = new();

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

    public void DisActiveHelpJellyfishSceneObj()
    {
        DisActiveSceneObj(HelpJellyfishSceneObj);
    }
    public void DisActiveSceneObj(List<GameObject> objects)
    {
        foreach(var obj in objects)
        {
            if (obj == null) continue;
            obj.SetActive(false);
        }
    }

    public IEnumerator PlayTimeline(PlayableDirector director)
    {
        director.Play();

        yield return new WaitWhile(() => director.state == PlayState.Playing);
    }

    public void TriggerEventYield()
    {
        EventTrigger = true;
    }
}
