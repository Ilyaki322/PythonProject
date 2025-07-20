using UnityEngine;
using UnityEngine.UIElements;

public class CharacterCreationController : MonoBehaviour
{
    [SerializeField] private CharacterCreator m_generator;
    private UIDocument m_document;

    private Button m_helmLeft;
    private Button m_helmRight;

    private Button m_hairLeft;
    private Button m_hairRight;

    private Button m_beardLeft;
    private Button m_beardRight;

    private Button m_armorLeft;
    private Button m_armorRight;

    private Button m_pantsLeft;
    private Button m_pantsRight;

    private Button m_weaponLeft;
    private Button m_weaponRight;

    private Button m_backLeft;
    private Button m_backRight;

    private Button m_create;
    private TextField m_name;

    private void Awake()
    {
        m_document = GetComponent<UIDocument>();
        var root = m_document.rootVisualElement;

        m_helmLeft = root.Q<Button>("HelmetLeftButton");
        m_helmRight = root.Q<Button>("HelmetRightButton");

        m_hairLeft = root.Q<Button>("HairLeftButton");
        m_hairRight = root.Q<Button>("HairRightButton");

        m_beardLeft = root.Q<Button>("BeardLeftButton");
        m_beardRight = root.Q<Button>("BeardRightButton");

        m_armorLeft = root.Q<Button>("ArmorLeftButton");
        m_armorRight = root.Q<Button>("ArmorRightButton");

        m_pantsLeft = root.Q<Button>("PantsLeftButton");
        m_pantsRight = root.Q<Button>("PantsRightButton");

        m_weaponLeft = root.Q<Button>("WeaponLeftButton");
        m_weaponRight = root.Q<Button>("WeaponRightButton");

        m_backLeft = root.Q<Button>("BackLeftButton");
        m_backRight = root.Q<Button>("BackRightButton");

        m_create = root.Q<Button>("CreateButton");
        m_name = root.Q<TextField>("CharacterNameField");
        m_name.value = "";

        m_create.clicked += () => { m_generator.save(m_name.value); };

        m_helmLeft.clicked += changeHelmetLeft;
        m_helmRight.clicked += changeHelmetRight;

        m_hairLeft.clicked += changeHairLeft;
        m_hairRight.clicked += changeHairRight;

        m_beardLeft.clicked += changeBeardLeft;
        m_beardRight.clicked += changeBeardRight;

        m_armorLeft.clicked += changeArmorLeft;
        m_armorRight.clicked += changeArmorRight;

        m_pantsLeft.clicked += changePantsLeft;
        m_pantsRight.clicked += changePantsRight;

        m_weaponLeft.clicked += changeWeaponLeft;
        m_weaponRight.clicked += changeWeaponRight;

        m_backLeft.clicked += changeBackLeft;
        m_backRight.clicked += changeBackRight;
    }

    private void changeHelmetLeft() => m_generator.ChangeHelm(1);
    private void changeHelmetRight() => m_generator.ChangeHelm(-1);
    private void changeHairLeft() => m_generator.ChangeHair(1);
    private void changeHairRight() => m_generator.ChangeHair(-1);
    private void changeBeardLeft() => m_generator.ChangeBeard(1);
    private void changeBeardRight() => m_generator.ChangeBeard(-1);
    private void changeArmorLeft() => m_generator.ChangeArmor(1);
    private void changeArmorRight() => m_generator.ChangeArmor(-1);
    private void changePantsLeft() => m_generator.ChangePants(1);
    private void changePantsRight() => m_generator.ChangePants(-1);
    private void changeWeaponLeft() => m_generator.ChangeWeapon1(1);
    private void changeWeaponRight() => m_generator.ChangeWeapon1(-1);
    private void changeBackLeft() => m_generator.ChangeBack(1);
    private void changeBackRight() => m_generator.ChangeBack(-1);
}
