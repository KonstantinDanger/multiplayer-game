using UnityEngine;
using UnityEngine.UI;

public class RegistrationWindow
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
        //string email = _emailField.text;
        //string pwd = _passwordField.text;
        ////string name = Steam.GetPersonaName;

        //string response = await BackendApi.Register(email, pwd, name);
    }
}