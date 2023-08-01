using UnityEngine;
using Managers;
using TMPro;

public class login_handler : MonoBehaviour
{
    [Header("Login UI")]
    public TextMeshProUGUI emailField;
    public TextMeshProUGUI passwordField;

    [Header("Signup UI")]
    public TextMeshProUGUI signupNameField;
    public TextMeshProUGUI signupEmailField;
    public TextMeshProUGUI signupPasswordField;
    public TextMeshProUGUI signupConfirmPasswordField;

    AuthManager authManager;
    // Start is called before the first frame update
    void Start()
    {
        authManager = MainManager.Instance.authManager;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoginButton()
    {
        StartCoroutine(authManager.Login(emailField.text, passwordField.text));
    }

    public void SignupButton()
    {
        if (signupPasswordField.text != signupConfirmPasswordField.text || signupPasswordField.text.Length < 6)
        {
            return;
        }
        StartCoroutine(authManager.SignUp(signupEmailField.text, signupPasswordField.text, signupNameField.text));
    }
}
