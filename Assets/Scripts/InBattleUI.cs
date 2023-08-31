using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InBattleUI : MonoBehaviour
{
    // Player cups text
    public TextMeshProUGUI playerCupsText;
    
    public TextMeshProUGUI playerAddressText;

    public DataScript dataScript;


    // Start is called before the first frame update
    async void Start()
    {
        // get the DataScript
        dataScript = GameObject.Find("Data").GetComponent<DataScript>();

        // set the player cups text
        playerCupsText.text = dataScript.cups.ToString();

        // get the player address
        dataScript.GetAccountAddress();

        // update the account address text in a format 0x12...56
        string accountAddress = await dataScript.GetAccountAddress();
        playerAddressText.text = accountAddress.Substring(0, 4) + "..." + accountAddress.Substring(accountAddress.Length - 4);
    }

}
