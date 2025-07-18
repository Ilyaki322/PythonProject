using System.ComponentModel;
using UnityEngine;
using UnityEngine.UIElements;

public class GameController : MonoBehaviour
{
    [SerializeField] private ShopController m_shopController;
    private Slot[] m_slots;

    private enum status_t {
        Menu,
        PlayerTurn,
        EnemyTurn
    }

    [SerializeField] private GameObject m_inventoryTest;
    [SerializeField] private UIDocument m_document;
    [SerializeField] private SocketManager m_socketManager;

    [SerializeField] private GameObject m_player;
    [SerializeField] private GameObject m_enemy;
    [SerializeField] private CharacterCreator m_playerCreator;
    [SerializeField] private CharacterCreator m_enemyCreator;
    [SerializeField] private CharacterCombatController m_playerController;
    [SerializeField] private CharacterCombatController m_enemyController;

    private VisualElement m_container;
    private VisualElement m_combatUI;
    private VisualElement m_endGameUI;
    private VisualElement m_buttonContainer;
    private VisualElement m_inventoryContainer;
    private VisualElement m_combatButtonContainer;

    private VisualElement m_timeFill;
    private VisualElement m_healthFillPlayer1;
    private VisualElement m_healthFillPlayer2;

    private Image m_shieldLeft;
    private Image m_shieldRight;

    private Button m_findGameButton;
    private Button m_endGameButton;
    private Button m_ActionButton1;
    private Button m_ActionButton2;
    private Button m_ActionButton3;
    private Button m_ActionButton4;
    private Button m_storeButton;

    private Label m_namePlayer1;
    private Label m_namePlayer2;
    private Label m_healthPlayer1;
    private Label m_healthPlayer2;
    private Label m_statusBar;
    private Label m_endGameLabel;

    // Character Menu
    private Label m_charLevel;
    private Label m_charMoney;
    private Label m_charName;

    private bool m_inQueue = false;

    private float m_maxTurnTimer = 30f;
    private float m_counter = 0f;

    private string m_token;
    private CharacterDTO m_selectedCharacter;
    
    private status_t m_gameStatus = status_t.Menu;

    public void SetToken(string token)
    {
        m_token = token;
        m_socketManager.SetToken(token);
    }

    public void SetCharacter(CharacterDTO character) {
        character.m_money += 10; // Initial money for testing
        m_selectedCharacter = character;
        m_shopController.SelectedCharacter = character;

        m_selectedCharacter.OnMoneyChanged += UpdateMoneyLabel;
    }

    public void ShowMenu()
    {
        m_buttonContainer.style.display = DisplayStyle.Flex;
    }

    public void ShowShop()
    {
        m_buttonContainer.style.display = DisplayStyle.None;
        m_shopController.ShowShop();

    }

    private void Start()
    {
        Application.runInBackground = true;

        var root = m_document.rootVisualElement;
        m_container = root.Q<VisualElement>("Container");
        m_combatUI = root.Q<VisualElement>("CombatUI");
        m_endGameUI = root.Q<VisualElement>("EndGameContainer");
        m_inventoryContainer = root.Q<VisualElement>("CombatInventoryContainer");
        m_combatButtonContainer = root.Q<VisualElement>("CombatButtonContainer");

        m_timeFill = root.Q<VisualElement>("TimeFill");
        m_healthFillPlayer1 = root.Q<VisualElement>("HealthFill1");
        m_healthFillPlayer2 = root.Q<VisualElement>("HealthFill2");

        m_shieldLeft = root.Q<Image>("ShieldLeft");
        m_shieldRight = root.Q<Image>("ShieldRight");

        m_endGameButton = root.Q<Button>("ContinueButton");
        m_ActionButton1 = root.Q<Button>("Button1");
        m_ActionButton2 = root.Q<Button>("Button2");
        m_ActionButton3 = root.Q<Button>("Button3");
        m_ActionButton4 = root.Q<Button>("Button4");

        m_endGameLabel = root.Q<Label>("ResultLabel");
        m_namePlayer1 = root.Q<Label>("PlayerName1");
        m_namePlayer2 = root.Q<Label>("PlayerName2");
        m_healthPlayer1 = root.Q<Label>("PlayerHealth1");
        m_healthPlayer2 = root.Q<Label>("PlayerHealth2");
        m_statusBar = root.Q<Label>("StatusLabel");

        m_findGameButton = root.Q<Button>("FindButton");
        m_storeButton = root.Q<Button>("StoreButton");
        m_charLevel = root.Q<Label>("LevelLabel");
        m_charMoney = root.Q<Label>("CoinsLabel");
        m_charName = root.Q<Label>("NameLabel");

        m_buttonContainer = root.Q<VisualElement>("ButtonContainer");
        m_storeButton.clicked += ShowShop;
        m_findGameButton.clicked += onFind;

        m_ActionButton1.clicked += () =>
        {
            m_playerController.OnAttack();
            m_socketManager.OnAttack();
            NextTurn();
        };

        m_ActionButton2.clicked += () =>
        {
            m_playerController.OnDefend();
            m_socketManager.OnDefend();
            NextTurn();
        };

        m_ActionButton3.clicked += () =>
        {
            //m_playerController.OnItemUse();
            //m_socketManager.OnItemUse();
            //NextTurn();
            m_combatButtonContainer.style.display = DisplayStyle.None;
            m_inventoryContainer.style.display = DisplayStyle.Flex;
        };

        m_ActionButton4.clicked += () =>
        {
            m_playerController.OnFourthButton();
            m_socketManager.OnFourthButton();
            NextTurn();
        };

        m_endGameButton.clicked += () =>
        {
            m_endGameUI.style.display = DisplayStyle.None;
            m_combatUI.style.display = DisplayStyle.None;
            m_container.style.display = DisplayStyle.Flex;
        };

        m_container.style.display = DisplayStyle.None;
        m_endGameUI.style.display = DisplayStyle.None;
        m_inventoryContainer.style.display = DisplayStyle.None;

        initInventory();
    }

    private void initInventory()
    {
        var root = m_document.rootVisualElement;
        var container = root.Q<VisualElement>("CombatInventoryContainer");

        m_slots = new Slot[10]; // abstract 10 away

        var slotsContainer = container.CreateChild("slotsContainer");
        for (int i = 0; i < 10; i++) // same
        {
            var slot = slotsContainer.CreateChild<Slot>("slot");
            slot.focusable = true;
            m_slots[i] = slot;
            slot.OnClick += onItemClick;
        }
    }

    private void onItemClick(int index, SerializableGuid guid)
    {
        Debug.Log($"Item clicked at: {index} with guid: {guid}");
    }

    private void Update()
    {
        if (m_gameStatus != status_t.Menu)
        {
            m_counter -= Time.deltaTime;
            float timerRatio = m_counter / m_maxTurnTimer;
            float timerPercent = Mathf.Lerp(0, 100, timerRatio);
            m_timeFill.style.width = Length.Percent(timerPercent);
        }

        if (m_gameStatus == status_t.PlayerTurn && m_counter < 0)
        {
            NextTurn();
            m_socketManager.OnSkipTurn();
        }
    }

    private void setCharacterStatus()
    {
        m_charLevel.text = "Level: " + m_selectedCharacter.level;
        m_charMoney.text = "Coins: " + m_selectedCharacter.money;
        m_charName.text = m_selectedCharacter.name;
        m_selectedCharacter.PropertyChanged += OnCharacterPropertyChanged;
    }

    private void UpdateMoneyLabel(int newMoney)
    {
        m_charMoney.text = "Coins: " + newMoney;
    }

    private void OnCharacterPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        // Only care about level changes here
        if (e.PropertyName == nameof(CharacterDTO.level))
            m_charLevel.text = "Level: " + m_selectedCharacter.level;
    }

    public void Connect()
    {
        m_gameStatus = status_t.Menu;
        
        m_socketManager.Connect(() =>
        {
            m_socketManager.InitSocket(m_enemyController);
            MainThreadDispatcher.Instance.Enqueue(() => {
                m_container.style.display = DisplayStyle.Flex;
                setCharacterStatus();});
        });
    }

    private void onFind()
    {
        if (!m_inQueue)
        {
            m_inQueue = true;
            m_findGameButton.text = "Cancel";
            m_socketManager.OnEnterQueue(m_selectedCharacter.id);
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

    private void activateButtons(bool active)
    {
        m_ActionButton1.SetEnabled(active);
        m_ActionButton2.SetEnabled(active);
        m_ActionButton3.SetEnabled(active);
        m_ActionButton4.SetEnabled(active);
    }

    public void NextTurn()
    {
        m_counter = m_maxTurnTimer;
        if (m_gameStatus == status_t.PlayerTurn)
        {
            m_gameStatus = status_t.EnemyTurn;
            m_statusBar.text = "Enemy Turn";
            activateButtons(false);
        }
        else
        {
            m_gameStatus = status_t.PlayerTurn;
            m_statusBar.text = "Your Turn";
            activateButtons(true);
        }
    }

    public void OnMatchFound(CharacterDTO player, CharacterDTO enemy, bool isStarting)
    {
        m_counter = m_maxTurnTimer;

        m_inQueue = false;
        m_findGameButton.text = "Find Match";

        m_container.style.display = DisplayStyle.None;
        m_combatUI.style.display = DisplayStyle.Flex;

        m_player.SetActive(true);
        m_playerCreator.Generate(player);
        m_namePlayer1.text = player.name;

        m_enemy.SetActive(true);
        m_enemyCreator.Generate(enemy);
        m_namePlayer2.text = enemy.name;

        if (isStarting)
        {
            m_gameStatus = status_t.PlayerTurn;
            m_statusBar.text = "Your Turn";
            activateButtons(true);
        } 
        else
        {
            m_gameStatus = status_t.EnemyTurn;
            m_statusBar.text = "Enemy Turn";
            activateButtons(false);
        }

        m_playerController.Init(m_healthPlayer1, m_healthFillPlayer1, m_enemyController, this);
        m_enemyController.Init(m_healthPlayer2, m_healthFillPlayer2, m_playerController, this);
    }

    public void OnWin()
    {
        endGameUI("Victory!");

    }

    public void OnLose()
    {
        endGameUI("Defeat!");
    }

    private void endGameUI(string result)
    {
        m_player.SetActive(false);
        m_enemy.SetActive(false);
        m_combatUI.style.display = DisplayStyle.None;
        m_endGameUI.style.display = DisplayStyle.Flex;
        m_endGameLabel.text = result;
        m_shieldLeft.RemoveFromClassList("ShieldIcon--enabled");
        m_shieldRight.RemoveFromClassList("ShieldIcon--enabled");
    }

    public void OnDead()
    {
        if (m_gameStatus == status_t.EnemyTurn)
        {
            m_socketManager.OnFourthButton();
        }
    }

    public void TurnShieldOn()
    {
        if (m_gameStatus == status_t.PlayerTurn)
        {
            m_shieldLeft.AddToClassList("ShieldIcon--enabled");
        }
        else
        {
            m_shieldRight.AddToClassList("ShieldIcon--enabled");
        }
    }

    public void TurnShieldOff()
    {
        if (m_gameStatus == status_t.PlayerTurn) {
            m_shieldRight.RemoveFromClassList("ShieldIcon--enabled");
        }
        else
        {
            m_shieldLeft.RemoveFromClassList("ShieldIcon--enabled");
        }
    }
}
