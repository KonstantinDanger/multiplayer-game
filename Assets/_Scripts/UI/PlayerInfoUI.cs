using UnityEngine;
using UnityEngine.UI;

public class PlayerInfoUI : UI
{
    [SerializeField] private VerticalLayoutGroup _container;

    public VerticalLayoutGroup Container => _container;
}
