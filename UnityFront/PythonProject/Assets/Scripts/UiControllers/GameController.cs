using UnityEngine;
using UnityEngine.UIElements;

public class GameController : MonoBehaviour
{
    [SerializeField] private UIDocument m_document;
    [SerializeField] private SocketManager m_socketManager;

    [SerializeField] private GameObject m_player;
    [SerializeField] private GameObject m_enemy;
    [SerializeField] private CharacterCreator m_playerCreator;
    [SerializeField] private CharacterCreator m_enemyCreator;

    private VisualElement m_container;

    private Button m_findGameButton;

    bool m_inQueue = false;

    private string m_token;
    private int m_selectedCharacterID;

    public void SetToken(string token)
    {
        m_token = token;
        m_socketManager.SetToken(token);
    }

    public void SetCharacter(int id) => m_selectedCharacterID = id;

    private void Start()
    {
        var root = m_document.rootVisualElement;
        m_container = root.Q<VisualElement>("Container");
        m_findGameButton = root.Q<Button>("FindButton");
        m_findGameButton.clicked += onFind;

        m_container.style.visibility = Visibility.Hidden;
    }

    public void Connect()
    {
        m_socketManager.Connect(() =>
        {
            MainThreadDispatcher.Instance.Enqueue(() => m_container.style.visibility = Visibility.Visible);
        });
    }

    private void onFind()
    {
        if (!m_inQueue)
        {
            m_inQueue = true;
            m_findGameButton.text = "Cancel";
            m_socketManager.OnEnterQueue(m_selectedCharacterID);
            return;
        }

        if (m_inQueue)
        {
            m_inQueue = false;
            m_findGameButton.text = "Find Match";
            m_socketManager.OnLeaveQueue();
            return;
        }
    }

    public void OnMatchFound(CharacterDTO player, CharacterDTO enemy)
    {
        m_player.SetActive(true);
        m_playerCreator.Generate(player);

        m_enemy.SetActive(true);
        m_enemyCreator.Generate(enemy);
    }
}
