using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CharacterSelectionController : MonoBehaviour
{
    [SerializeField] VisualTreeAsset m_entry;

    [SerializeField] private UIDocument m_document;
    private VisualElement m_selectionElement;
    private VisualElement m_creationElement;

    [SerializeField] private GameObject m_dummy;
    [SerializeField] private CharacterCreator m_creator;
    [SerializeField] private Inventory m_invetory;

    private ListView m_charList;
    private Button m_playButton;

    private CharacterDTO[] m_characterList = new CharacterDTO[5];

    private int lastClickedIndex = 0;

    private CharacterApi m_characterApi;
    private InventoryApi m_invetoryApi;

    private void Awake()
    {
        m_characterApi = GetComponent<CharacterApi>();
        m_invetoryApi = GetComponent<InventoryApi>();

        m_document = GetComponent<UIDocument>();
        var root = m_document.rootVisualElement;

        m_selectionElement = root.Q<VisualElement>("CharacterSelectionContainer");
        m_creationElement = root.Q<VisualElement>("CharacterCreation");
        m_charList = root.Q<ListView>("CharacterList");
        m_playButton = root.Q<Button>("PlayButton");

        m_dummy.SetActive(false);
        m_charList.selectionChanged += onCharacterClick;
        m_playButton.clicked += OnPlayClicked;
    }

    public void LoadCharacters(string json)
    {
        CharacterListWrapper wrapper = JsonUtility.FromJson<CharacterListWrapper>(json);
        if (wrapper.count > 5) wrapper.count = 5;
        if (wrapper.count > 0)
        {
            m_dummy.SetActive(true);
            m_creator.Generate(wrapper.characters[0]);
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
        m_dummy.SetActive(true);
    }

    private void OnPlayClicked()
    {
        m_invetory.LoadInventory(m_invetoryApi);
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
