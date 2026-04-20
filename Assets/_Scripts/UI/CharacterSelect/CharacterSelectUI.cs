using UnityEngine;

public class CharacterSelectUI : MonoBehaviour
{
    private CharacterClassList _classes;

    public void Initialize(CharacterClassList list)
    {
        _classes = list;

        DrawClasses(_classes);
    }

    private void DrawClasses(CharacterClassList list)
    {
        var classes = list.Classes;

        foreach (var c in classes)
        {

        }
    }
}
