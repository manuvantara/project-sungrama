using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;


public class DataScript : MonoBehaviour
{
    // account address
    private string accountAddress = "0x1TEST";

    public int cups = 0;

    private UIController uiController;

    public List<int> inventory = new List<int>();

    public int[] inventoryDropIds; 

    void Start()
    {
        // dont destroy this object when loading a new scene
        DontDestroyOnLoad(this.gameObject);

        // get the UIController script
        uiController = GameObject.Find("UI").GetComponent<UIController>();

        // shards: 1001
        // gears: 1002
        // crystals: 1003
        // prism: 1004
        // essence: 1005
        // sparks: 1006
        // cores: 1007
        // plates: 1008
        // dust: 1009
        // runes: 1010
        inventoryDropIds = new int[] { 1001, 1002, 1003, 1004, 1005, 1006, 1007, 1008,
                                       1009, 1010 };

        // load the data
        loadData();
    }

    public void CopyAddressInClipboard()
    {
        // show the pending popup
        uiController.ShowPending();

        TextEditor te = new TextEditor();
        te.text = accountAddress;
        te.SelectAll();
        te.Copy();

        // show the success popup
        uiController.ShowSuccess();
    }

    public async Task<string> GetAccountAddress()
    {
        accountAddress = await ThirdwebManager.Instance.SDK.wallet.GetAddress();
        return accountAddress;
    }

    public void saveData() {
        PlayerPrefs.SetInt("cups", cups);
        PlayerPrefs.SetString("accountAddress", accountAddress);
        PlayerPrefs.Save();
    }

    public void loadData() {
        cups = PlayerPrefs.GetInt("cups", 0);
        if(accountAddress == "0x1TEST") {
            accountAddress = PlayerPrefs.GetString("accountAddress", "0x1TEST");
        }
    }
}
