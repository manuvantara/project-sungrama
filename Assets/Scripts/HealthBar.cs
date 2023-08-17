using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Image fillImage;

    private void Start()
    {
        mainCamera = Camera.main;
        fillImage.fillAmount = 1;
    }

    private void Update()
    {
        // if the helath bar is on the enemy or player unit, make it face the camera
        if (this.gameObject.tag == "Unit")
        {
            transform.LookAt(transform.position + mainCamera.transform.rotation * Vector3.forward,
            mainCamera.transform.rotation * Vector3.up);

            // if the health is full, hide the health bar
            if (fillImage.fillAmount == 1)
            {
                this.gameObject.GetComponent<Canvas>().enabled = false;
            }
            else
            {
                this.gameObject.GetComponent<Canvas>().enabled = true;
            }
        }
    }

    public void SetHealth(float health, float maxHealth)
    {
        fillImage.fillAmount = health / maxHealth;
    }
}
