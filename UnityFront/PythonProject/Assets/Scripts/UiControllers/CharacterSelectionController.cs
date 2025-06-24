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

    private ListView m_charList;

    private CharacterDTO[] m_characterList = new CharacterDTO[5];

    private int lastClickedIndex = 0;

    private void Awake()
    {
        m_document = GetComponent<UIDocument>();
        var root = m_document.rootVisualElement;

        m_selectionElement = root.Q<VisualElement>("CharacterSelectionContainer");
        m_creationElement = root.Q<VisualElement>("CharacterCreation");
        m_charList = root.Q<ListView>("CharacterList");

        m_dummy.SetActive(false);

        testInit();
        m_charList.selectionChanged += onCharacterClick;
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
            m_creator.Generate(selectedChar);
        }
        m_dummy.SetActive(true);
    }

    public void Save(CharacterDTO c)
    {
        m_characterList[lastClickedIndex] = c;

        m_selectionElement.style.display = DisplayStyle.Flex;
        m_creationElement.style.display = DisplayStyle.None;
        m_dummy.SetActive(true);
        m_creator.Generate(m_characterList[lastClickedIndex]);
        m_charList.RefreshItems();
    }
}
