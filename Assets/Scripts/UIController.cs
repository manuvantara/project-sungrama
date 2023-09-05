using System.Collections;
using Game.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public DataScript dataScript;

    public GameObject statsPage;
    public GameObject levelUpPage;

    public Slider damageSlider;
    public Slider armourSlider;
    public Slider energySlider;

    public float maxDamage;
    public float maxArmour;
    public float maxEnergy;

    public TextMeshProUGUI damageText;
    public TextMeshProUGUI armourText;
    public TextMeshProUGUI energyText;

    public GameObject[] units;
    public GameObject[] unitToggles;

    public GameObject popupSuccess;
    private static GameObject _popupSuccess { get; set; }
    public GameObject popupPending;
    private static GameObject _popupPending { get; set; }
    public GameObject popupFail;
    private static GameObject _popupFail { get; set; }

    public TextMeshProUGUI accountAddressText;

    public TextMeshProUGUI cupsText;

    public TextMeshProUGUI balanceText;

    private void Awake()
    {
        _popupSuccess = popupSuccess;
        _popupPending = popupPending;
        _popupFail = popupFail;
    }

    private async void Start()
    {
        // get the DataScript
        dataScript = GameObject.Find("Data").GetComponent<DataScript>();

        statsPage.SetActive(true);
        levelUpPage.SetActive(false);
        damageSlider.maxValue = maxDamage;
        armourSlider.maxValue = maxArmour;
        energySlider.maxValue = maxEnergy;

        // update the cups text
        cupsText.text = dataScript.cups.ToString();

        // update the account address text in a format 0x12...56
        string accountAddress = await dataScript.GetAccountAddress();
        accountAddressText.text =
            accountAddress.Substring(0, 4) + "..." + accountAddress.Substring(accountAddress.Length - 4);

        // update the balance text
        UpdateBalance();
    }

    private void OnEnable()
    {
        EventManager.OnTransactionConfirmed += UpdateBalance;
    }

    private void OnDisable()
    {
        EventManager.OnTransactionConfirmed -= UpdateBalance;
    }

    public void StartGame()
    {
        // load the Main Scene
        SceneManager.LoadScene("MainScene");
    }

    public void ShowStats()
    {
        statsPage.SetActive(true);
        levelUpPage.SetActive(false);
    }

    public void ShowLevelUp()
    {
        statsPage.SetActive(false);
        levelUpPage.SetActive(true);
    }

    public void SelectUnit(int unit)
    {
        damageSlider.value = units[unit].GetComponent<UnitAI>().attackDamage;
        armourSlider.value = units[unit].GetComponent<HealthScript>().initialHP;
        energySlider.value = units[unit].GetComponent<UnitAI>().energyRequired;

        damageText.text = units[unit].GetComponent<UnitAI>().attackDamage.ToString();
        armourText.text = units[unit].GetComponent<HealthScript>().initialHP.ToString();
        energyText.text = units[unit].GetComponent<UnitAI>().energyRequired.ToString();
    }

    public static void ShowSuccess()
    {
        _popupSuccess.SetActive(true);
        _popupPending.SetActive(false);
        _popupFail.SetActive(false);

        HidePopup();
    }

    public static void ShowPending()
    {
        _popupSuccess.SetActive(false);
        _popupPending.SetActive(true);
        _popupFail.SetActive(false);

        HidePopup();
    }

    public static void ShowFail()
    {
        _popupSuccess.SetActive(false);
        _popupPending.SetActive(false);
        _popupFail.SetActive(true);

        HidePopup();
    }

    public async void UpdateBalance()
    {
        // get the balance from the SDK
        var value = await ThirdwebManager.Instance.SDK.wallet.GetBalance();

        // display the balance capped to 2 decimal places
        balanceText.text = value.displayValue;
    }

    // timer for the popup
    // Won't work in static context, but there's no time to fix it
    static IEnumerator HidePopup()
    {
        yield return new WaitForSeconds(5);
        _popupSuccess.SetActive(false);
        _popupPending.SetActive(false);
        _popupFail.SetActive(false);
    }
}