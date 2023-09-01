using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TokenPopulator : MonoBehaviour
{
    public GameObject tokenPrefab;

    private DataScript dataScript;

    public GameObject tokenR;

    void Awake()
    {
        Debug.Log("TokenPopulator Awake");
        
        // get the DataScript
        dataScript = GameObject.Find("Data").GetComponent<DataScript>();

        // go through the inventory and create a token for each resource
        foreach (Resource resource in dataScript.inventory)
        {
            Debug.Log("TokenPopulator Awake: " + resource.name);

            // create a token
            GameObject token = Instantiate(tokenPrefab, transform);
            token.transform.localScale = new Vector3( 1.0f, 1.0f, 1.0f );

            tokenR = token;

            // // set the token image
            // token.GetComponent<TokenImage>().ChangeImage(dataScript.resourceImages[resource.image_id]);

            // // set the token text
            // token.GetComponent<TokenImage>().ChangeText(resource.name);

            Debug.Log("awaked");
        }
    }
}
