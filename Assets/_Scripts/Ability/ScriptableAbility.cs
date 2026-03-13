using UnityEngine;

[CreateAssetMenu(menuName = "Ability/Ability")]
public class ScriptableAbility : ScriptableObject
{
    [SerializeReference, SubclassSelector] private Ability _ability;

    public Ability GetNew()
        => Utils.GetInstancedCopyOf(_ability);
}
