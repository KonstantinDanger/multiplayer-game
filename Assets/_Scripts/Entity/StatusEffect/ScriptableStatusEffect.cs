using UnityEngine;

[CreateAssetMenu(menuName = "StatusEffect/StatusEffect")]
public class ScriptableStatusEffect : ScriptableObject
{
    [SerializeReference, SubclassSelector] private StatusEffect _statusEffect;

    public StatusEffect GetNew()
        => Utils.GetInstancedCopyOf(_statusEffect);
}


