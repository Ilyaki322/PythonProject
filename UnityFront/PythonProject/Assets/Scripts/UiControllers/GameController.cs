using System;
using System.Collections;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class GameController : MonoBehaviour
{
    [SerializeField] private ShopController m_shopController;
    [SerializeField] private CombinedInventoryManager m_inventoryManager;
    [SerializeField] private LoginController m_loginController;

    private Slot[] m_slots;

    private enum status_t {
        Menu,
        PlayerTurn,
        EnemyTurn
    }

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
    private VisualElement m_connectionContainer;

    private VisualElement m_timeFill;
    private VisualElement m_healthFillPlayer1;
    private VisualElement m_healthFillPlayer2;
    private VisualElement m_battleInventoryContainer;

    private Image m_shieldLeft;
    private Image m_shieldRight;

    private Button m_findGameButton;
    private Button m_endGameButton;
    private Button m_ActionButton1;
    private Button m_ActionButton2;
    private Button m_ActionButton3;
    private Button m_ActionButton4;
    private Button m_storeButton;
    private Button m_closeInventory;
    private Button m_logout;

    private Label m_namePlayer1;
    private Label m_namePlayer2;
    private Label m_healthPlayer1;
    private Label m_healthPlayer2;
    private Label m_statusBar;
    private Label m_endGameLabel;
    private Label m_findMatch;
    private Label m_rewardLabel;
    private Label m_connectLoading;
    private Label m_connectError;

    // Character Menu
    private Label m_charLevel;
    private Label m_charMoney;
    private Label m_charName;

    private bool m_inQueue = false;

    private float m_maxTurnTimer = 10f; // was 30 seconds
    private float m_counter = 0f;

    private string m_token;
    private CharacterDTO m_selectedCharacter;
    private status_t m_gameStatus = status_t.Menu;

    private Coroutine m_findMatchCoroutine;
    private Coroutine m_connectionCoroutine;

    static Tooltip m_tooltip;
    static Slot m_hoveredSlot;

    public void SetToken(string token)
    {
        m_token = token;
        m_socketManager.SetToken(token);
    }

    public void SetCharacter(CharacterDTO character) {
        m_selectedCharacter = character;
        m_shopController.SelectedCharacter = character;
        m_inventoryManager.InitInventory();

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
        m_connectionContainer = root.Q<VisualElement>("Connecting");
        m_findMatch = root.Q<Label>("MatchText");
        m_timeFill = root.Q<VisualElement>("TimeFill");
        m_healthFillPlayer1 = root.Q<VisualElement>("HealthFill1");
        m_healthFillPlayer2 = root.Q<VisualElement>("HealthFill2");

        m_rewardLabel = root.Q<Label>("RewardLabel");

        m_shieldLeft = root.Q<Image>("ShieldLeft");
        m_shieldRight = root.Q<Image>("ShieldRight");

        m_endGameButton = root.Q<Button>("ContinueButton");
        m_ActionButton1 = root.Q<Button>("Button1");
        m_ActionButton2 = root.Q<Button>("Button2");
        m_ActionButton3 = root.Q<Button>("Button3");
        m_ActionButton4 = root.Q<Button>("Button4");
        m_logout = root.Q<Button>("Logout");

        m_endGameLabel = root.Q<Label>("ResultLabel");
        m_namePlayer1 = root.Q<Label>("PlayerName1");
        m_namePlayer2 = root.Q<Label>("PlayerName2");
        m_healthPlayer1 = root.Q<Label>("PlayerHealth1");
        m_healthPlayer2 = root.Q<Label>("PlayerHealth2");
        m_statusBar = root.Q<Label>("StatusLabel");
        m_connectLoading = root.Q<Label>("ConnectionLoading");
        m_connectError = root.Q<Label>("ConnectionError");

        m_findGameButton = root.Q<Button>("FindButton");
        m_storeButton = root.Q<Button>("StoreButton");
        m_charLevel = root.Q<Label>("LevelLabel");
        m_charMoney = root.Q<Label>("CoinsLabel");
        m_charName = root.Q<Label>("NameLabel");

        m_closeInventory = root.Q<Button>("Close");

        m_buttonContainer = root.Q<VisualElement>("ButtonContainer");
        m_storeButton.clicked += ShowShop;
        m_findGameButton.clicked += onFind;

        m_ActionButton1.clicked += () =>
        {
            int damage = m_playerController.OnAttack();
            m_socketManager.OnAttack(damage);
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
            OpenInventory();
        };

        m_closeInventory.clicked += () =>
        {
            m_inventoryContainer.style.display = DisplayStyle.None;
            m_combatButtonContainer.style.display = DisplayStyle.Flex;
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

        m_logout.clicked += logout;

        m_container.style.display = DisplayStyle.None;
        m_endGameUI.style.display = DisplayStyle.None;
        m_inventoryContainer.style.display = DisplayStyle.None;
        m_connectionContainer.style.display = DisplayStyle.None;
    }

    private void logout()
    {
        m_container.style.display = DisplayStyle.None;
        m_endGameUI.style.display = DisplayStyle.None;
        m_inventoryContainer.style.display = DisplayStyle.None;

        m_token = null;
        m_selectedCharacter = null;
        m_gameStatus = status_t.Menu;
        m_socketManager.Logout();
        m_loginController.Logout();
}

    private void OpenInventory()
    {
        m_combatButtonContainer.style.display = DisplayStyle.None;
        m_inventoryContainer.style.display = DisplayStyle.Flex;
    }

    private void CloseInventory()
    {
        m_combatButtonContainer.style.display = DisplayStyle.Flex;
        m_inventoryContainer.style.display = DisplayStyle.None;
    }

    private void initInventory()
    {
        var root = m_document.rootVisualElement;
        var container = root.Q<VisualElement>("CombatInventoryContainer");
        m_tooltip = container.CreateChild<Tooltip>("Tooltip");
        var itemsList = m_inventoryManager.userItems();
        m_slots = new Slot[10]; // abstract 10 away

        m_battleInventoryContainer = container.CreateChild("slotsContainer");
        for (int i = 0; i < 10; i++)
        {
            var slot = m_battleInventoryContainer.CreateChild<Slot>("slot");
            slot.focusable = true;
            m_slots[i] = slot;
            slot.OnClick += onItemClick;
            slot.OnHover += OnPointerOver;
            slot.RegisterCallback<PointerLeaveEvent>(e => OnPointerOutSlot(slot, e));
        }

        foreach(var (item,index) in itemsList)
        {
            m_slots[index].Set(item.Id, item.Details.Icon, item.Quantity);
        }
    }

    static void OnPointerOver(Slot slot, PointerOverEvent e)
    {
        if (m_hoveredSlot != slot || m_tooltip.style.visibility == Visibility.Hidden)
        {
            m_hoveredSlot = slot;
        }

        if (m_hoveredSlot.ItemId == SerializableGuid.Empty) return;

        ItemDetails item = ItemDatabase.Instance.GetItemDetailsById(m_hoveredSlot.ItemId);
        m_tooltip.Set(item.Name, "", item.OnUse, item.Price);
        m_tooltip.Show();
        m_tooltip.BringToFront();
        m_tooltip.SetPosition(e.position);
    }

    static void OnPointerOutSlot(Slot slot, PointerLeaveEvent e)
    {
        if (!m_tooltip.worldBound.Contains(e.position))
        {
            m_tooltip.Hide();
            m_hoveredSlot = null;
        }
    }

    private void onItemClick(int index, SerializableGuid guid)
    {
        if (m_slots[index] == null) return;

        m_playerController.OnItemUse(index, guid);
        m_socketManager.OnItemUse(index, guid);
        m_ActionButton3.SetEnabled(false);
        CloseInventory();
    }

    public void RemoveItem(int index)
    {
        if (index < 0 || index >= m_slots.Length)
        {
            Debug.LogError("Index out of bounds for slots array.");
            return;
        }

        m_inventoryManager.RemoveItem(index);
        m_slots[index].Remove();
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
        m_charLevel.text = "Level: " + m_selectedCharacter.CharLevel;
        m_charMoney.text = "Coins: " + m_selectedCharacter.CharMoney;
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
        if (e.PropertyName == nameof(CharacterDTO.CharLevel))
            m_charLevel.text = "Level: " + m_selectedCharacter.CharLevel;
    }

    public void Connect()
    {
        m_gameStatus = status_t.Menu;
        m_connectionContainer.style.display = DisplayStyle.Flex;
        m_connectError.text = "";
        m_connectLoading.text = "";
        m_connectionCoroutine = StartCoroutine(AnimateConnection());


        m_socketManager.Connect(() =>
        {
            m_socketManager.InitSocket(m_enemyController);
            MainThreadDispatcher.Instance.Enqueue(() => {
                m_container.style.display = DisplayStyle.Flex;
                m_connectionContainer.style.display = DisplayStyle.None;
                StopCoroutine(m_connectionCoroutine);
                setCharacterStatus();});
        });
    }

    private void onFind()
    {
        if (!m_inQueue)
        {
            m_inQueue = true;
            m_findGameButton.text = "Cancel";

            m_findMatch.style.display = DisplayStyle.Flex;
            m_findMatchCoroutine = StartCoroutine(AnimateFindingMatch());
            m_socketManager.OnEnterQueue(m_selectedCharacter.id);
            return;
        }

        if (m_inQueue)
        {
            m_inQueue = false;
            m_findGameButton.text = "Find Match";

            if (m_findMatchCoroutine != null)
            {
                StopCoroutine(m_findMatchCoroutine);
                m_findMatchCoroutine = null;
            }
            m_findMatch.style.display = DisplayStyle.None;
            m_socketManager.OnLeaveQueue();
            return;
        }
    }

    private IEnumerator AnimateFindingMatch()
    {
        var baseText = "Finding Match";
        while (true)
        {
            for (int dots = 1; dots <= 3; ++dots)
            {
                m_findMatch.text = baseText + new string('.', dots);
                yield return new WaitForSeconds(0.5f);
            }

            m_findMatch.text = baseText;
            yield return new WaitForSeconds(0.5f);
        }
    }

    private IEnumerator AnimateConnection()
    {
        var baseText = "Connecting";
        while (true)
        {
            for (int dots = 1; dots <= 3; ++dots)
            {
                m_connectLoading.text = baseText + new string('.', dots);
                yield return new WaitForSeconds(0.5f);
            }

            m_connectLoading.text = baseText;
            yield return new WaitForSeconds(0.5f);
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
            CloseInventory();
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

        if (m_findMatchCoroutine != null)
        {
            StopCoroutine(m_findMatchCoroutine);
            m_findMatchCoroutine = null;
        }

        m_findMatch.style.display = DisplayStyle.None;
        m_container.style.display = DisplayStyle.None;
        m_combatUI.style.display = DisplayStyle.Flex;

        m_player.SetActive(true);
        m_playerCreator.Generate(player);
        m_namePlayer1.text = player.name;

        m_enemy.SetActive(true);
        m_enemyCreator.Generate(enemy);
        m_namePlayer2.text = enemy.name;

        initInventory();

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
        m_playerController.SetPlayer(player);
        m_enemyController.Init(m_healthPlayer2, m_healthFillPlayer2, m_playerController, this);
        m_enemyController.SetPlayer(enemy);
    }

    public void OnWin()
    {
        endGameUI("Victory!", "3");
        m_selectedCharacter.CharMoney += 3;
    }

    public void OnLose()
    {
        endGameUI("Defeat!", "1");
        m_selectedCharacter.CharMoney += 1;
    }

    private void endGameUI(string result, string reward)
    {
        m_player.SetActive(false);
        m_enemy.SetActive(false);
        m_combatUI.style.display = DisplayStyle.None;
        m_endGameUI.style.display = DisplayStyle.Flex;
        m_endGameLabel.text = result;
        m_rewardLabel.text = reward;
        m_shieldLeft.RemoveFromClassList("ShieldIcon--enabled");
        m_shieldRight.RemoveFromClassList("ShieldIcon--enabled");

        m_battleInventoryContainer.Clear();
        m_slots = null;
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
