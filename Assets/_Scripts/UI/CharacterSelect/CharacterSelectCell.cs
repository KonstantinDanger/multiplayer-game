using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectCell : MonoBehaviour
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

        Setup(_class.PreviewSprite);
    }

    private void OnDestroy()
        => _selectButton.onClick.RemoveListener(HandleCharacterSelect);

    private void Setup(Sprite previewSprite)
    {
        _classPreviewImage.sprite = previewSprite;

        _selectButton.enabled = true;
    }

    private void HandleCharacterSelect()
    {
        if (_player.CharacterClass == _class)
        {
            return;
        }

        _player.SetCharacterClass(_class);
    }
}
