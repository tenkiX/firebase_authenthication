using UnityEngine;
using UnityEngine.UI;

public class LoginTest : MonoBehaviour
{
    [SerializeField] FirebaseManager manager;
    [SerializeField] InputField emailInputField;
    [SerializeField] InputField passwordInputField;

    #region Register
    public void OnRegisterButtonClicked()
    {
        manager.RegisterUserAsync(emailInputField.text, passwordInputField.text, OnRegistrationSuccess, OnRegistrationFailed);
    }

    void OnRegistrationSuccess()
    {
        Debug.Log("Registration success");
    }

    void OnRegistrationFailed(string errorMessage)
    {
        Debug.LogError(errorMessage);
    }
    #endregion

    #region Login
    public void OnLoginButtonClicked()
    {
        manager.LogIn(emailInputField.text, passwordInputField.text, OnLoginSuccess, OnLoginFailed);
    }

    void OnLoginSuccess()
    {
        Debug.Log("Login success");
    }

    void OnLoginFailed(string errorMessage)
    {
        Debug.LogError(errorMessage);
    }
    #endregion

    #region Logout
    public void OnLogoutButtonClicked()
    {
        manager.LogOut();
    }
    #endregion
}

