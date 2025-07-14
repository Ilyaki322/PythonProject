using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CharacterSelectionController : MonoBehaviour
{
    [SerializeField] VisualTreeAsset m_entry;

    [SerializeField] private UIDocument m_document;

    private VisualElement m_loginElement;
    private VisualElement m_selectionElement;
    private VisualElement m_creationElement;
    private VisualElement m_charList;
    private VisualElement m_alertPopup;

    [SerializeField] private GameObject m_dummy;
    [SerializeField] private GameController m_gameController;
    [SerializeField] private CharacterCreator m_creator;
    [SerializeField] private Inventory m_invetory;

    private Label m_nameError;
    private Label m_alertText;

    private Button m_playButton;
    private Button m_deleteButton;
    private Button m_logoutButton;
    private Button m_cancelButton;
    private Button m_alertYesButton;
    private Button m_alertNoButton;

    private CharacterDTO[] m_characterList = new CharacterDTO[5];

    private int lastClickedIndex = 0;
    private int m_selectedCharID;

    private CharacterApi m_characterApi;
    private InventoryApi m_invetoryApi;

    private void Awake()
    {
        m_characterApi = GetComponent<CharacterApi>();
        m_invetoryApi = GetComponent<InventoryApi>();

        m_document = GetComponent<UIDocument>();
        var root = m_document.rootVisualElement;

        m_loginElement = root.Q<VisualElement>("Login");
        m_selectionElement = root.Q<VisualElement>("CharacterSelection");
        m_creationElement = root.Q<VisualElement>("CharacterCreation");
        m_charList = root.Q<VisualElement>("CharacterList");
        m_alertPopup = root.Q<VisualElement>("DeleteAlert");

        m_nameError = root.Q<Label>("CreationError");
        m_alertText = root.Q<Label>("AlertText");
        m_playButton = root.Q<Button>("PlayButton");
        m_deleteButton = root.Q<Button>("DeleteButton");
        m_logoutButton = root.Q<Button>("LogoutButton");
        m_cancelButton = root.Q<Button>("CancelButton");
        m_alertYesButton = root.Q<Button>("AlertYes");
        m_alertNoButton = root.Q<Button>("AlertNo");

        m_dummy.SetActive(false);

        m_playButton.clicked += OnPlayClicked;
        m_cancelButton.clicked += onCancelClick;
        m_logoutButton.clicked += onLogoutclick;
        m_deleteButton.clicked += DeleteCharacter;
        m_alertNoButton.clicked += () => { m_alertPopup.style.display = DisplayStyle.None; };
        m_alertYesButton.clicked += ConfirmDeleteChar;
    }

    public void LoadCharacters(string json)
    {
        m_characterList = new CharacterDTO[5];
        CharacterListWrapper wrapper = JsonUtility.FromJson<CharacterListWrapper>(json);
        if (wrapper.count > 5) wrapper.count = 5;
        if (wrapper.count > 0)
        {
            m_dummy.SetActive(true);
            m_creator.Generate(wrapper.characters[0]);
            m_selectedCharID = wrapper.characters[0].id;
        }
        for (int i = 0; i < wrapper.count; i++)
        {
            m_characterList[i] = wrapper.characters[i];
        }

        initializeChars();
    }

    private void initializeChars()
    {
        m_charList.Clear();
        for (int i = 0; i < m_characterList.Length; i++)
        {
            int index = i; // Capture the current index for the lambda
            VisualElement entry = m_entry.CloneTree();
            Button button = entry.Q<Button>("CharButton");
            button.text = m_characterList[i] == null ? "Create New" : m_characterList[i].name;
            button.clicked += () => { onCharacterClick(index); };
            m_charList.Add(entry);
        }
    }

    private void onCancelClick()
    {
        m_dummy.SetActive(false);
        m_selectionElement.style.display = DisplayStyle.Flex;
        m_creationElement.style.display = DisplayStyle.None;
        m_nameError.style.display = DisplayStyle.None;
    }

    private void onLogoutclick()
    {
        m_dummy.SetActive(false);
        m_characterApi.Logout();
        m_loginElement.style.display = DisplayStyle.Flex;
        m_selectionElement.style.display = DisplayStyle.None;
    }

    private void onCharacterClick(int index)
    {
        var selectedChar = m_characterList[index];
        lastClickedIndex = index;
        
        if (selectedChar == null)
        {
            m_creator.Generate(new CharacterDTO());
            m_selectionElement.style.display = DisplayStyle.None;
            m_creationElement.style.display = DisplayStyle.Flex;
        }
        else
        {
            m_selectedCharID = selectedChar.id;
            m_invetoryApi.setCharID(selectedChar.id);
            m_creator.Generate(selectedChar);
        }
        m_dummy.SetActive(true);
    }

    private void OnPlayClicked()
    {
        // hide current ui
        m_selectionElement.style.display = DisplayStyle.None;
        m_creationElement.style.display = DisplayStyle.None;
        m_dummy.SetActive(false);

        //m_invetory.LoadInventory(m_invetoryApi);
        m_gameController.SetCharacter(m_characterList[lastClickedIndex]);
        m_gameController.Connect();

    }

    private void DeleteCharacter()
    {
        Debug.Log("Delete Character Clicked lastIndex " + lastClickedIndex);
        if(m_characterList[lastClickedIndex] == null) return;

        m_alertPopup.style.display = DisplayStyle.Flex;
        string alertText = "Are you sure you want to Delete";
        m_alertText.text = alertText + " " + m_characterList[lastClickedIndex].name + "?";
    }

    private void ConfirmDeleteChar()
    {
        StartCoroutine(m_characterApi.DeleteCharacter(m_characterList[lastClickedIndex].id, (success) => {
            if (success)
            {
                m_characterList[lastClickedIndex] = null;
                m_dummy.SetActive(true);
                StartCoroutine(m_characterApi.GetCharacters((json) => {LoadCharacters(json);}));
                m_creator.Generate(new CharacterDTO());
            }
        }));

        m_alertPopup.style.display = DisplayStyle.None;
    }

    public void Save(CharacterDTO c)
    {
        if (!ValidateCreatedChar(c))return;

        StartCoroutine(m_characterApi.AddCharacter(c, success =>
        {
            if(success)
            {
                StartCoroutine(m_characterApi.GetCharacters((json) =>
                {
                    LoadCharacters(json);
                    m_selectionElement.style.display = DisplayStyle.Flex;
                    m_creationElement.style.display = DisplayStyle.None;
                    m_dummy.SetActive(true);
                }));
            }
        }));  
    }

    private bool ValidateCreatedChar(CharacterDTO c)
    {
        if (string.IsNullOrEmpty(c.name))
        {
            m_nameError.text = "Name cannot be empty!";
            m_nameError.style.display = DisplayStyle.Flex;
            return false;
        }

        m_nameError.style.display = DisplayStyle.None;
        return true;
    }

    [System.Serializable]
    public class CharacterListWrapper
    {
        public int count;
        public List<CharacterDTO> characters;
    }
}
