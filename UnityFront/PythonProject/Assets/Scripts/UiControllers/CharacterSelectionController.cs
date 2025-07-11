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

    [SerializeField] private GameObject m_dummy;
    [SerializeField] private GameController m_gameController;
    [SerializeField] private CharacterCreator m_creator;
    [SerializeField] private Inventory m_invetory;

    private ListView m_charList;
    private Button m_playButton;
    private Button m_deleteButton;
    private Button m_logoutButton;
    private Button m_cancelButton;

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
        m_charList = root.Q<ListView>("CharacterList");
        m_playButton = root.Q<Button>("PlayButton");
        m_deleteButton = root.Q<Button>("DeleteButton");
        m_logoutButton = root.Q<Button>("LogoutButton");
        m_cancelButton = root.Q<Button>("CancelButton");

        m_dummy.SetActive(false);
        m_charList.selectionChanged += onCharacterClick;
        m_playButton.clicked += OnPlayClicked;
        m_cancelButton.clicked += onCancelClick;
        m_logoutButton.clicked += onLogoutclick;
    }

    public void LoadCharacters(string json)
    {
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

        testInit();
    }

    private void testInit()
    {
        var root = m_document.rootVisualElement;
        m_charList = root.Q<ListView>("CharacterList");
        m_charList.fixedItemHeight = 80;

        m_charList.itemsSource = m_characterList;

        m_charList.makeItem = () =>
        {
            return m_entry.Instantiate();
        };

        m_charList.bindItem = (element, i) =>
        {
            var label = element.Q<Label>("NameLabel");
            if (m_characterList[i] == null) label.text = "Create New";
            else label.text = m_characterList[i].name;

            var level = element.Q<Label>("LevelLabel");
            level.text = "";
        };

        
        m_charList.selectionType = SelectionType.Single;
    }

    private void onCancelClick()
    {
        m_selectionElement.style.display = DisplayStyle.Flex;
        m_creationElement.style.display = DisplayStyle.None;
        m_dummy.SetActive(true);
        m_creator.Generate(m_characterList[lastClickedIndex]);
    }

    private void onLogoutclick()
    {
        m_characterApi.Logout();
        m_loginElement.style.display = DisplayStyle.Flex;
        m_selectionElement.style.display = DisplayStyle.None;
    }

    private void onCharacterClick(IEnumerable<object> selectedItems)
    {
        var selectedChar = m_charList.selectedItem as CharacterDTO;
        
        if (selectedChar == null)
        {
            lastClickedIndex = m_charList.selectedIndex;
            m_selectionElement.style.display = DisplayStyle.None;
            m_creationElement.style.display = DisplayStyle.Flex;
        }
        else
        {
            m_invetoryApi.setCharID(selectedChar.id);
            m_creator.Generate(selectedChar);
        }

        m_selectedCharID = selectedChar.id;
        m_dummy.SetActive(true);
    }

    private void OnPlayClicked()
    {
        // hide current ui
        m_selectionElement.style.display = DisplayStyle.None;
        m_creationElement.style.display = DisplayStyle.None;
        m_dummy.SetActive(false);

        //m_invetory.LoadInventory(m_invetoryApi);
        m_gameController.SetCharacter(m_selectedCharID);
        m_gameController.Connect();

    }

    public void Save(CharacterDTO c)
    {
        StartCoroutine(m_characterApi.AddCharacter(c));
        m_characterList[lastClickedIndex] = c;

        m_selectionElement.style.display = DisplayStyle.Flex;
        m_creationElement.style.display = DisplayStyle.None;
        m_dummy.SetActive(true);
        m_creator.Generate(m_characterList[lastClickedIndex]);
        m_charList.RefreshItems();
    }

    [System.Serializable]
    public class CharacterListWrapper
    {
        public int count;
        public List<CharacterDTO> characters;
    }
}
