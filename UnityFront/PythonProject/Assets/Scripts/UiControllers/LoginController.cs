using System;
using System.Collections;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UIElements;

public class LoginController : MonoBehaviour
{
    [SerializeField] private GameController m_gameController;

    private UIDocument m_document;
    private VisualElement m_loginElement;
    private VisualElement m_registerElement;
    private VisualElement m_characterSelection;
    private VisualElement m_errorContainer;

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
    private Label m_goToRegisterLink;
    private Button m_registerButton;
    private Button m_backButton;

    // Loading overlay elements for spinner when oauth2 is called
    private VisualElement m_loadingOverlay;
    private VisualElement m_spinner;
    private Button m_overlayCancel;
    private bool m_cancelPolling;
    private Coroutine m_pollingCoroutine;
    private float m_spinnerAngle = 0f;
    private bool m_isSpinning = false;

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

        initializeQuerys();
        SubscribeToEvents();
        // Initially show login UI
        showLoginUI();
    }

    void initializeQuerys()
    {
        var root = m_document.rootVisualElement;

        // Get main UI sections
        m_loginElement = root.Q<VisualElement>("Login");
        m_registerElement = root.Q<VisualElement>("Register");
        m_characterSelection = root.Q<VisualElement>("CharacterSelection");

        // Login fields
        m_loginUsernameField = root.Q<TextField>("UsernameField");
        m_loginPasswordField = root.Q<TextField>("PasswordField");
        m_errorContainer = root.Q<VisualElement>("ErrorContainer");
        m_loginError = root.Q<Label>("LoginError");
        m_regSuccess = root.Q<Label>("RegSuccess");

        // Loading overlay elements
        m_loadingOverlay = root.Q<VisualElement>("LoadingOverlay");
        m_spinner = root.Q<VisualElement>("Spinner");
        m_overlayCancel = root.Q<Button>("OverlayCancel");

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
        m_goToRegisterLink = root.Q<Label>("GoToRegisterButton");

    }

    void SubscribeToEvents()
    {
        m_overlayCancel.clicked += AbortLogin;

        // Register events
        m_loginButton.clicked += OnLoginClicked;
        m_registerButton.clicked += OnRegisterClicked;
        m_backButton.clicked += OnBackClicked;
        m_goToRegisterLink.RegisterCallback<ClickEvent>(evt => OnToRegister());
        m_googleLoginBtn.clicked += OnGoogleLoginClicked;
    }


    private void Update()
    {
        if (m_isSpinning)
        {
            m_spinnerAngle += 360f * Time.deltaTime;
            m_spinner.style.rotate = new Rotate(new Angle(m_spinnerAngle % 360f, AngleUnit.Degree));
        }
    }

    #region Google Login
    private void OnGoogleLoginClicked()
    {
        var lr = new LoginRequest();
        m_oauthState = lr.InitiateGoogleLogin();

        ShowLoadingOverlay();

        m_pollingCoroutine = StartCoroutine(lr.PollForToken(m_oauthState,
                shouldCancel: () => m_cancelPolling,
                onError: HandleGoogleLoginError,
                onSuccess: HandleGoogleLoginSuccess));
    }

    private void ShowLoadingOverlay()
    {
        m_cancelPolling = false;
        m_loadingOverlay.style.display = DisplayStyle.Flex;
        m_isSpinning = true;
    }

    private void HideLoadingOverlay()
    {
        m_loadingOverlay.style.display = DisplayStyle.None;
        m_isSpinning = false;
    }

    private void HandleGoogleLoginError(string error)
    {
        HideLoadingOverlay();
        m_errorContainer.style.display = DisplayStyle.Flex;
        m_loginError.text = error;
        m_loginError.style.display = DisplayStyle.Flex;
    }

    private void HandleGoogleLoginSuccess(string token)
    {
        HideLoadingOverlay();

        // stash the token & fetch characters
        m_characterApi.setToken(token);
        m_invetoryApi.setToken(token);
        m_gameController.SetToken(token);
        StartCoroutine(m_characterApi.GetCharacters(json => showSelection(json)));
    }

    private void AbortLogin()
    {
        m_cancelPolling = true;
        if (m_pollingCoroutine != null) StopCoroutine(m_pollingCoroutine);
        m_oauthState = null;
        LoginErrorReset();

        m_loadingOverlay.style.display = DisplayStyle.None;
        m_errorContainer.style.display = DisplayStyle.Flex;
        m_loginError.text = "Connection aborted. Please try again.";
        m_loginError.style.display = DisplayStyle.Flex;
    }

    #endregion

    private void showLoginUI(DisplayStyle regSucess = DisplayStyle.None)
    {


        if (m_loginElement != null) m_loginElement.style.display = DisplayStyle.Flex;
        if (m_registerElement != null) m_registerElement.style.display = DisplayStyle.None;

        m_loginError.style.display = DisplayStyle.None;

        m_errorContainer.style.display = regSucess;
        m_regSuccess.style.display = regSucess;

        ClearRegisterForm();
    }

    private void ShowRegisterUI()
    {
        if (m_loginElement != null) m_loginElement.style.display = DisplayStyle.None;
        if (m_registerElement != null) m_registerElement.style.display = DisplayStyle.Flex;
        
        // Clear login errors
        m_errorContainer.style.display = DisplayStyle.None;
        m_loginError.style.display = DisplayStyle.None;

        // Clear register form and errors
        ClearRegisterForm();
    }

    private void ClearRegisterForm()
    {
        // Clear input fields
        if (m_registerUsernameField != null) m_registerUsernameField.value = "";
        if (m_registerPasswordField != null) m_registerPasswordField.value = "";
        if (m_registerEmailField != null) m_registerEmailField.value = "";

        // Hide all error messages
        if (m_usernameError != null) m_usernameError.style.display = DisplayStyle.None;
        if (m_emailError != null) m_emailError.style.display = DisplayStyle.None;
        if (m_passwordError != null) m_passwordError.style.display = DisplayStyle.None;
    }

    //to fix. /* =========================================================================== */
    private void showSelection(string json)
    {
        m_loginElement.style.display = DisplayStyle.None;
        if (m_characterSelection != null) m_characterSelection.style.display = DisplayStyle.Flex;

        m_characterSelectionController.LoadCharacters(json);
    }

    /* =========================================================================== */

    #region button events
    private void OnLoginClicked()
    {
        string username = m_loginUsernameField.value;
        string password = m_loginPasswordField.value;

        LoginRequest lg = new();
        StartCoroutine(lg.Login(username, password,
            (string error) =>
            {
                m_errorContainer.style.display = DisplayStyle.Flex;
                m_loginError.text = "password or username are incorrect";
                m_loginError.style.display = DisplayStyle.Flex;
            },
            (string text) =>
            {
                m_characterApi.setToken(text);
                m_invetoryApi.setToken(text);
                m_gameController.SetToken(text);
                StartCoroutine(m_characterApi.GetCharacters((json) =>
                {
                    showSelection(json);
                }));
            }
        ));
    }

    private void LoginErrorReset()
    {
        m_errorContainer.style.display = DisplayStyle.None;
        m_loginError.style.display = DisplayStyle.None;
        m_regSuccess.style.display = DisplayStyle.None;
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
        ClearErrorFields();
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

    private void ShowRegisterError(string field, string message)
    {
        switch (field)
        {
            case "username":
                if (m_usernameError != null)
                {
                    m_usernameError.text = message;
                    m_usernameError.style.display = DisplayStyle.Flex;
                }
                break;
            case "email":
                if (m_emailError != null)
                {
                    m_emailError.text = message;
                    m_emailError.style.display = DisplayStyle.Flex;
                }
                break;
            case "password":
                if (m_passwordError != null)
                {
                    m_passwordError.text = message;
                    m_passwordError.style.display = DisplayStyle.Flex;
                }
                break;
        }
    }

    private void ClearErrorFields()
    {
        // First hide all errors
        if (m_usernameError != null) m_usernameError.style.display = DisplayStyle.None;
        if (m_emailError != null) m_emailError.style.display = DisplayStyle.None;
        if (m_passwordError != null) m_passwordError.style.display = DisplayStyle.None;
    }


    #endregion

    [System.Serializable]
    public class RegisterErrorResponse
    {
        public bool success;
        public string field;
        public string error;
    }
}
