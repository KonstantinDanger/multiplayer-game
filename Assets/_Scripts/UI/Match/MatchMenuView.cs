using UnityEngine;
using UnityEngine.UI;

public class MatchMenuView : MonoBehaviour
{
    [SerializeField] private Button _leaveButton;

    //Dialog window to cancel match
    //Dialog window to surrender

    private void OnEnable()
        => _leaveButton.onClick.AddListener(HandleLeave);

    private void OnDisable()
        => _leaveButton.onClick.RemoveListener(HandleLeave);

    private void HandleLeave()
    {

    }
}
