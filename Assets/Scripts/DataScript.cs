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

    void Start()
    {
        // dont destroy this object when loading a new scene
        DontDestroyOnLoad(this.gameObject);

        // get the UIController script
        uiController = GameObject.Find("UI").GetComponent<UIController>();
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
}
