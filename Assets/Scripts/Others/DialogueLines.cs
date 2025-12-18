using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

// 複数のクラスで使うため、namespace外に出すか、このファイルに定義します
public enum SpeakerType
{
    Entity, // NPCや看板など（自分自身）
    Player  // プレイヤー
}

[System.Serializable]
public class DialogueData
{
    public SpeakerType speaker;
    [TextArea(3, 10)]
    public string text;
}

[CreateAssetMenu(fileName = "NewScenario", menuName = "Dialogue/Scenario")]
public class DialogueLines : ScriptableObject
{
    public string title;

    public List<DialogueData> list = new List<DialogueData>();

}