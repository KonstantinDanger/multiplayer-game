using UnityEditor;

[CustomEditor(typeof(GameMatchConfig))]
public class GameMatchConfigEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GameMatchConfig config = (GameMatchConfig)target;

        float totalMatchTime = config.MatchTime + config.DeathmatchTime;

        EditorGUILayout.LabelField($"Total match time: {totalMatchTime} minutes");
    }
}
