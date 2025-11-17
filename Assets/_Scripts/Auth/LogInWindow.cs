using UnityEngine;
using UnityEngine.UI;

public class LogInWindow
{
    [SerializeField] private InputField _emailField;
    [SerializeField] private InputField _passwordField; //pwd should be -> ******
    [SerializeField] private Button _submitButton;

    private void OnEnable()
        => _submitButton.onClick.AddListener(HandleSubmit);

    private void OnDisable()
        => _submitButton.onClick.RemoveListener(HandleSubmit);

    private async void HandleSubmit()
    {
        //string email = _emailField.value;
        //string pwd = _passwordField.value;
        ////string name = Steam.GetPersonaName;

        //string response = await BackendApi.LogIn(email, pwd, name);
    }
}