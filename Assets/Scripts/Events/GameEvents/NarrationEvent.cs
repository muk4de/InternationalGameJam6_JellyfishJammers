using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewNarrationEvent", menuName = "GameEvent/NewNarrationEvent")]
public class NarrationEvent : GameEventBase
{
    public List<string> Sentences = new List<string>();
    public bool ShowInstant = false;

    public override IEnumerator Execute()
    {
        GameManager.Instance.SetPlayerMovable(false);

        var narration = NarrationManager.Instance.StartCoroutine(NarrationManager.Instance.StartNarration(Sentences, ShowInstant));
        yield return narration;

        GameManager.Instance.SetPlayerMovable(true);
        yield return null;
    }
}
