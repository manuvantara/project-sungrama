using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TokenImage : MonoBehaviour
{
    // graphic to change
    public Image image;

    // text to change
    public TextMeshProUGUI text;

    public void ChangeImage(Sprite sprite)
    {
        image.sprite = sprite;
    }

    public void ChangeText(string newText)
    {
        text.text = newText;
    }
}
