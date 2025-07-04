using System;
using System.Collections;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UIElements;

public class LoginController : MonoBehaviour
{
    private UIDocument m_document;
    private VisualElement m_loginElement;
    private VisualElement m_registerElement;
    private VisualElement m_characterCreation;

    private TextField m_loginUsernameField;
    private TextField m_loginPasswordField;

    private TextField m_registerUsernameField;
    private TextField m_registerPasswordField;
    private TextField m_registerEmailField;

    private Label m_loginError;
    private Label m_usernameError;
    private Label m_emailError;
    private Label m_passwordError;
    private Label m_regSuccess;

    private Button m_loginButton;
    private Button m_googleLoginBtn;
    private Button m_goToRegisterButton;
    private Button m_registerButton;
    private Button m_backButton;

    private CharacterApi m_characterApi;
    private InventoryApi m_invetoryApi;

    private CharacterSelectionController m_characterSelectionController;

    private string m_oauthState; // Used to track the OAuth state for Google login

    private void Awake()
    {
        m_characterApi = GetComponent<CharacterApi>();
        m_invetoryApi = GetComponent<InventoryApi>();
        m_characterSelectionController = GetComponent<CharacterSelectionController>();

        m_document = GetComponent<UIDocument>();
        var root = m_document.rootVisualElement;

        // Get main UI sections
        m_loginElement = root.Q<VisualElement>("Login");
        m_registerElement = root.Q<VisualElement>("Register");
        m_characterCreation = root.Q<VisualElement>("CharacterSelectionContainer");

        // Login fields
        m_loginUsernameField = root.Q<TextField>("UsernameField");
        m_loginPasswordField = root.Q<TextField>("PasswordField");
        m_loginError = root.Q<Label>("LoginError");
        m_regSuccess = root.Q<Label>("RegSuccess");

        // Register fields
        m_registerUsernameField = root.Q<TextField>("UserRegisterField");
        m_registerPasswordField = root.Q<TextField>("PasswordRegisterField");
        m_registerEmailField = root.Q<TextField>("EmailRegisterField");

        m_usernameError = root.Q<Label>("UsernameError");
        m_emailError = root.Q<Label>("EmailError");
        m_passwordError = root.Q<Label>("PasswordError");

        // Buttons
        m_loginButton = root.Q<Button>("LoginButton");
        m_googleLoginBtn = root.Q<Button>("GoogleLogin");

        m_registerButton = root.Q<Button>("RegisterButton");
        m_backButton = root.Q<Button>("BackButton");
        m_goToRegisterButton = root.Q<Button>("GoToRegisterButton");

        // Register events
        m_loginButton.clicked += OnLoginClicked;
        m_registerButton.clicked += OnRegisterClicked;
        m_backButton.clicked += OnBackClicked;
        m_goToRegisterButton.clicked += OnToRegister;
        m_googleLoginBtn.clicked += OnGoogleLoginClicked;

        // Initially show login UI
        showLoginUI();
    }

    private void OnGoogleLoginClicked()
    {
        m_oauthState = Guid.NewGuid().ToString();
        Application.OpenURL($"http://localhost:5000/login/google?state={m_oauthState}");

        //Start polling for the JWT
        StartCoroutine(PollForToken(m_oauthState));
    }

    private IEnumerator PollForToken(string state)
    {
        while (true)
        {
            using var req = UnityWebRequest.Get($"http://localhost:5000/login/status?state={state}");
            yield return req.SendWebRequest();

            if (req.result == UnityWebRequest.Result.Success &&
                !string.IsNullOrEmpty(req.downloadHandler.text))
            {
                // got the JWT
                var resp = JsonUtility.FromJson<TokenResponse>(req.downloadHandler.text);

                m_characterApi.setToken(resp.token);
                m_invetoryApi.setToken(resp.token);
                StartCoroutine(m_characterApi.GetCharacters(json => showSelection(json)));
                yield break;  // stop polling
            }

            // wait before next try
            yield return new WaitForSeconds(1f);
        }
    }

    [Serializable]
    private class TokenResponse
    {
        public string token;
    }

    private void showLoginUI(DisplayStyle regSucess = DisplayStyle.None)
    {
        m_loginElement.style.display = DisplayStyle.Flex;
        m_registerElement.style.display = DisplayStyle.None;

        m_loginError.style.display = DisplayStyle.None;
        m_regSuccess.style.display = regSucess;
    }

    private void ShowRegisterUI()
    {
        m_loginElement.style.display = DisplayStyle.None;
        m_registerElement.style.display = DisplayStyle.Flex;
    }

    private void showSelection(string json)
    {
        m_loginElement.style.display = DisplayStyle.None;
        m_registerElement.style.display = DisplayStyle.None;
        m_characterCreation.style.display = DisplayStyle.Flex;
        m_characterSelectionController.LoadCharacters(json);
    }


    #region button events
    private void OnLoginClicked()
    {
        string username = m_loginUsernameField.value;
        string password = m_loginPasswordField.value;

        LoginRequest lg = new();
        StartCoroutine(lg.Login(username, password,
            (string error) =>
            {
                m_loginError.text = "password or username are incorrect";
                m_loginError.style.display = DisplayStyle.Flex;
            },
            (string text) =>
            {
                m_characterApi.setToken(text);
                m_invetoryApi.setToken(text);
                StartCoroutine(m_characterApi.GetCharacters((json) =>
                {
                    showSelection(json);
                }));
            }
        ));
    }

    private void OnRegisterClicked()
    {
        string username = m_registerUsernameField.value;
        string password = m_registerPasswordField.value;
        string email = m_registerEmailField.value;

        m_usernameError.style.display = DisplayStyle.None;
        m_emailError.style.display = DisplayStyle.None;
        m_passwordError.style.display = DisplayStyle.None;

        if (!checkEmpty(username, password, email)) return;
        if (!validate(username, password, email)) return;

        RegisterRequest rr = new();
        StartCoroutine(rr.Register(username, password, email,
            (string error) =>
            {
                RegisterErrorResponse err = JsonUtility.FromJson<RegisterErrorResponse>(error);
                ShowRegisterError(err.field, err.error);
            },
            (string _) =>
            {
                showLoginUI(DisplayStyle.Flex);
            }
        ));
    }

    private void OnBackClicked()
    {
        showLoginUI();
    }

    private void OnToRegister()
    {
        ShowRegisterUI();
    }

    #endregion


    #region Register Errors 

    private bool checkEmpty(string username, string pass, string email)
    {
        bool noError = true;
        if (username == "")
        {
            ShowRegisterError("username", "this field is required");
            noError = false;
        }
        if (email == "")
        {
            ShowRegisterError("email", "this field is required");
            noError = false;
        }
        if (pass == "")
        {
            ShowRegisterError("password", "this field is required");
            noError = false;
        }

        return noError;
    }

    private bool validate(string username, string password, string email)
    {
        bool noError = true;

        // Username: only letters, length between 4 and 10
        if (!Regex.IsMatch(username, @"^[a-zA-Z]{4,10}$"))
        {
            ShowRegisterError("username", "Invalid username. Must be 4-10 letters (a-z, A-Z) only.");
            noError = false;
        }

        // Email: basic email format validation
        if (!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
        {
            ShowRegisterError("email", "Invalid email format.");
            noError = false;
        }

        // Password: only letters and digits, length > 4
        if (!Regex.IsMatch(password, @"^[a-zA-Z0-9]{5,}$"))
        {
            ShowRegisterError("password", "Invalid password. Must be at least 5 characters (letters and digits only).");
            noError = false;
        }

        return noError;
    }

    #endregion
    private void ShowRegisterError(string field, string message)
    {
        m_usernameError.style.display = DisplayStyle.None;
        m_emailError.style.display = DisplayStyle.None;
        m_passwordError.style.display = DisplayStyle.None;

        switch (field)
        {
            case "username":
                m_usernameError.text = message;
                m_usernameError.style.display = DisplayStyle.Flex;
                break;
            case "email":
                m_emailError.text = message;
                m_emailError.style.display = DisplayStyle.Flex;
                break;
            case "password":
                m_passwordError.text = message;
                m_passwordError.style.display = DisplayStyle.Flex;
                break;
        }
    }

    [System.Serializable]
    public class RegisterErrorResponse
    {
        public bool success;
        public string field;
        public string error;
    }
}
