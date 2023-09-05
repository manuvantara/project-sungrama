using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;


public class DataScript : MonoBehaviour
{
    public static DataScript instance = null;

    // account address
    private string accountAddress = "0x1TEST";

    public int cups = 0;
    
    // inventory with resources
    public List<Resource> inventory = new List<Resource>();

    // available resources to drop
    public ResourceList availableResources;

    public Sprite[] resourceImages;

    void Awake()
    {
        // check if instance already exists
        if (instance == null)
        {
            // if not, set instance to this
            instance = this;
        }
        // if instance already exists and it's not this:
        else if (instance != this)
        {
            // then destroy this. this enforces our singleton pattern, meaning there can only ever be one instance of a GameManager
            Destroy(gameObject);
        }
        // sets this to not be destroyed when reloading scene
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        // load the available resources from the json file in Resources/Game Data folder
        string json = Resources.Load<TextAsset>("Game Data/data").text;
        availableResources = ResourceList.CreateFromJSON(json);

        // load the data
        loadData();
    }

    public void CopyAddressInClipboard()
    {
        // show the pending popup
        UIController.ShowPending();

        TextEditor te = new TextEditor();
        te.text = accountAddress;
        te.SelectAll();
        te.Copy();

        // show the success popup
        UIController.ShowSuccess();
    }

    public async Task<string> GetAccountAddress()
    {
        accountAddress = await ThirdwebManager.Instance.SDK.wallet.GetAddress();
        return accountAddress;
    }

    public void saveData() {
        PlayerPrefs.SetInt("cups", cups);
        PlayerPrefs.Save();
    }

    public void loadData() {
        cups = PlayerPrefs.GetInt("cups", 0);
    }
}
