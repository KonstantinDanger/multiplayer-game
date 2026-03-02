using UnityEngine;

[CreateAssetMenu(menuName = "Upgrade/Upgrade")]
public class ScriptableUpgrade : ScriptableObject
{
    [field: SerializeField] public string Name { get; private set; }
    [field: SerializeField] public Sprite Sprite { get; private set; }
    [SerializeReference, SubclassSelector] private Upgrade _upgrade;

    public Upgrade GetNew()
        => Utils.GetInstancedCopyOf(_upgrade);
}