using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "NewBgmEvent", menuName = "GameEvent/NewBgmEvent")]
public class BgmEvent : GameEventBase
{
    public SoundData BgmSoundData;
    public float FadeDuration = 0f;
    public override IEnumerator Execute()
    {
        AudioManager.I.PlayBgm(BgmSoundData, FadeDuration);
        yield return null;
    }
}
