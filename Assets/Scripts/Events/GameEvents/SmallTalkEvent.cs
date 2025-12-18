using System.Collections;
using UnityEngine;
using UnityEngine.Playables;

[CreateAssetMenu(fileName = "NewSmallTalkEvent", menuName = "GameEvent/NewSmallTalkEvent")]
public class SmallTalkEvent : GameEventBase
{
    public override IEnumerator Execute()
    {
        var gm = GameManager.Instance;

        yield return new WaitForSeconds(1);

        Debug.Log("1");
        gm.EventTrigger = false;
        yield return new WaitWhile(() => !gm.EventTrigger);
        gm.EventTrigger = false;
        Debug.Log("2");
        gm.SetPlayerMovable(false);

        gm.BlueJellyfishDialogue.sentenceIndex = 2;
        gm.BlueJellyfishDialogue.StartDialogue();

        yield return new WaitWhile(() => !gm.EventTrigger);
        Debug.Log("3");
        gm.SetPlayerMovable(false);

        yield return null;
    }
}
