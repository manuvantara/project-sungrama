using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BtnClose : MonoBehaviour
{
    public void DisableObject(GameObject obj)
    {
        obj.SetActive(false);
    }
}
