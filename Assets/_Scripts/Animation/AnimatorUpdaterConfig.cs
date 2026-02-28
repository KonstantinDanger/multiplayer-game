using UnityEngine;

[CreateAssetMenu(menuName = "Config/EntitiesAnimatorUpdater")]
public class AnimatorUpdaterConfig : ScriptableObject
{
    [field: SerializeField] public float StartingUpdateDistance { get; private set; }
    [field: SerializeField] public float EndingUpdateDistance { get; private set; }
    [field: SerializeField] public AnimationCurve DelayOverDistance { get; private set; }
    [field: SerializeField] public float MaxUpdateDelay { get; private set; }
}