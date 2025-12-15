using UnityEngine;

[CreateAssetMenu(menuName = "Stat Parameter")]
public class ScriptableStatParameter : ScriptableObject
{
    [SerializeField] private StatParameter _statParameter;

    public StatParameter GetNew()
        => _statParameter.Clone();
}
