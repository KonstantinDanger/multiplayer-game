using UnityEngine;
using UnityEngine.UI;

public class ClassSelectCell : MonoBehaviour
{
    [SerializeField] private Button _selectButton;
    [SerializeField] private Image _classPreviewImage;

    private ScriptableCharacterClass _class;
    private Currency _cost;
    private Player _player;

    public void Initialize(ScriptableCharacterClass charClass, Currency cost, Player player)
    {
        _class = charClass;
        _cost = cost;
        _player = player;

        _selectButton.onClick.AddListener(HandleCharacterSelect);
    }

    private void OnDestroy()
        => _selectButton.onClick.RemoveListener(HandleCharacterSelect);

    private void HandleCharacterSelect()
    {
        if (_player.CharacterClass == _class)
        {
            _selectButton.enabled = false;

            return;
        }

        _selectButton.enabled = true;
        _player.SetCharacterClass(_class);
    }
}
