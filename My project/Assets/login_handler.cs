using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Managers;
using TMPro;

public class login_handler : MonoBehaviour
{
    public TextMeshProUGUI emailField;
    public TextMeshProUGUI passwordField;

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
}
