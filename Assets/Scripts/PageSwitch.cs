using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PageSwitch : MonoBehaviour
{
    public GameObject[] pages;

    public void SwitchPage(int page)
    {
        for(int i = 0; i < pages.Length; i++)
        {
            if(i == page)
            {
                pages[i].SetActive(true);
            } else {
                pages[i].SetActive(false);
            }
        }
    }
}
