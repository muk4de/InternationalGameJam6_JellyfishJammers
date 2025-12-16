using System.Collections;
using UnityEngine;

public abstract class GameEventBase : ScriptableObject
{
    public abstract IEnumerator Execute();
}