using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIToggler : MonoBehaviour
{
    public GameObject[] objects;

    public void ToggleObjects()
    {
        for(int i = 0; i < objects.Length; i++)
        {
            objects[i].SetActive(!objects[i].activeSelf);
        }
    }

    public void ToggleObjects(bool state)
    {
        for(int i = 0; i < objects.Length; i++)
        {
            objects[i].SetActive(state);
        }
    }
}
