using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ButtonFont : MonoBehaviour
{
    private TextMeshProUGUI text;

    void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
        // make the text black
        text.color = Color.black;
    }

    public void ChangeColorOn()
    {
        // make the text yellow
        text.color = Color.yellow;
    }

    public void ChangeColorOff()
    {
        // make the text black
        text.color = Color.black;
    }
}
