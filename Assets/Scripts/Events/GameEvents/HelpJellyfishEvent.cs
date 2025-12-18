using System.Collections;
using UnityEngine;
using UnityEngine.Playables;

[CreateAssetMenu(fileName = "NewHelpJellyfishEvent", menuName = "GameEvent/NewHelpJellyfishEvent")]
public class HelpJellyfishEvent : GameEventBase
{
    public PlayableAsset Timeline;

    public override IEnumerator Execute()
    {
        var gm = GameManager.Instance;


        yield return new WaitWhile(() => !gm.EventTrigger);
        gm.EventTrigger = false;

        yield return new WaitForSeconds(1f);

        var ienumerator = gm.PlayTimeline(gm.HelpJellyfish);
        gm.BlueJellyfishDialogue.sentenceIndex = 1;
        yield return ienumerator;


        yield return null;
    }
}
