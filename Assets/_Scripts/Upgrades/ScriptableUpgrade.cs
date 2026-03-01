using UnityEngine;

[CreateAssetMenu(menuName = "Upgrade/Upgrade")]
public class ScriptableUpgrade : ScriptableObject
{
    [SerializeReference, SubclassSelector] private Upgrade _upgrade;

    public Upgrade GetNew()
        => Utils.GetInstancedCopyOf(_upgrade);
}