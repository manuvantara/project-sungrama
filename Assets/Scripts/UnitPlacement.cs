using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitPlacement : MonoBehaviour
{
    private GameObject unitPrefab;
    [SerializeField] private GameObject unitParent;

    [SerializeField] private GameObject unitPrefabPreview;

    [SerializeField] private LayerMask ignoreLayer;

    [SerializeField] private GameObject[] unitPrefabs;

    [SerializeField] private GameObject[] unitPrefabPreviews;

    [SerializeField] private float maxSpawnZ = 0f;

    // UI
    [SerializeField] private Toggle[] unitButtons;
    [SerializeField] private EnergyBar energyBar;

    // energy
    [SerializeField] private int maxEnergy = 10;
    private int currentEnergy = 10;

    [SerializeField] private float energyRegenRate = 2f;
    private float energyRegenCounter = 0f;

    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
    }

    public void SelectTank()
    {
        // skip if the toggle is deselected
        if (!unitButtons[0].isOn)
        {
            return;
        }

        DeselectTogglesExcept(0);

        unitPrefab = unitPrefabs[0];
        // destroy the previous preview if it exists
        if (unitPrefabPreview != null)
        {
            Destroy(unitPrefabPreview);
        }
        unitPrefabPreview = Instantiate(unitPrefabPreviews[0], Vector3.zero, Quaternion.identity);
        // set the layer of preview to Preview
        unitPrefabPreview.layer = LayerMask.NameToLayer("Preview");
        unitPrefabPreview.GetComponent<Renderer>().material.color = new Color(0, 0, 1, 0.3f);
    }

    public void SelectArcher()
    {
        // skip if the toggle is deselected
        if (!unitButtons[1].isOn)
        {
            return;
        }

        DeselectTogglesExcept(1);

        unitPrefab = unitPrefabs[1];
        // destroy the previous preview if it exists
        if (unitPrefabPreview != null)
        {
            Destroy(unitPrefabPreview);
        }
        unitPrefabPreview = Instantiate(unitPrefabPreviews[1], Vector3.zero, Quaternion.identity);
        unitPrefabPreview.layer = LayerMask.NameToLayer("Preview");
        unitPrefabPreview.GetComponent<Renderer>().material.color = new Color(0, 1, 0, 0.3f);
    }

    private void Update()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;

        // show a preview of the unit if the unit is selected
        if (unitPrefab != null)
        {

            if (Physics.Raycast(ray, out hitInfo, Mathf.Infinity, ~ignoreLayer))
            {
                // if it hits anything except the ground or preview osr theres not enough energy or its beyond max z, change the preview color to red
                if (hitInfo.collider.gameObject.layer != LayerMask.NameToLayer("Ground") || currentEnergy < unitPrefab.GetComponent<UnitAI>().energyRequired
                    || hitInfo.point.z > maxSpawnZ)
                {
                    unitPrefabPreview.GetComponent<Renderer>().material.color = new Color(1, 0, 0, 0.3f);
                } else {
                    unitPrefabPreview.GetComponent<Renderer>().material.color = new Color(0, 1, 0, 0.3f);
                }
                
                Vector3 spawnPosition = hitInfo.point;
                spawnPosition.y += 0.25f;
                // change the x and z as a multiple of 1f
                spawnPosition.x = Mathf.Round(spawnPosition.x);
                spawnPosition.z = Mathf.Round(spawnPosition.z);
                unitPrefabPreview.transform.position = spawnPosition;
            }
        }

        // spawning the unit if the unit is selected and the player clicks
        if (Input.GetMouseButtonDown(0) && unitPrefab != null && currentEnergy >= unitPrefab.GetComponent<UnitAI>().energyRequired && !IsMouseOverUI())
        {
            ray = mainCamera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hitInfo, Mathf.Infinity, ~ignoreLayer))
            {
                // if it hits anything except the ground, return
                if (hitInfo.collider.gameObject.layer != LayerMask.NameToLayer("Ground"))
                {
                    return;
                }

                Vector3 spawnPosition = hitInfo.point;
                spawnPosition.y += 0.25f;
                // change the x and z as a multiple of 1f
                spawnPosition.x = Mathf.Round(spawnPosition.x);
                spawnPosition.z = Mathf.Round(spawnPosition.z);
                GameObject unit = Instantiate(unitPrefab, spawnPosition, Quaternion.identity, unitParent.transform);

                // gine the unit Player tag
                unit.tag = "Player";

                // deduct the energy cost of the unit
                currentEnergy -= unitPrefab.GetComponent<UnitAI>().energyRequired;

                // deselect the unit and destroy the preview
                Destroy(unitPrefabPreview);
                unitPrefab = null;

                // deselect the toggle
                DeselectToggles();
            }
        }

        // regenerate energy
        RegenEnergy();

        // update the energy bar
        energyBar.UpdateEnergy(currentEnergy);
    }

    private void RegenEnergy()
    {
       if (energyRegenCounter >= energyRegenRate)
        {
            currentEnergy += 1;
            if(currentEnergy > maxEnergy)
            {
                currentEnergy = maxEnergy;
            }
            energyRegenCounter = 0f;
        } else {
            energyRegenCounter += Time.deltaTime;
        }
    }

    private void SelectToggle(int index)
    {
        for (int i = 0; i < unitButtons.Length; i++)
        {
            if (i == index)
            {
                unitButtons[i].isOn = true;
            } else {
                unitButtons[i].isOn = false;
            }
        }
    }

    private void DeselectToggles()
    {
        for (int i = 0; i < unitButtons.Length; i++)
        {
            unitButtons[i].isOn = false;
        }
    }

    private void DeselectTogglesExcept(int index)
    {
        for (int i = 0; i < unitButtons.Length; i++)
        {
            if (i != index)
            {
                unitButtons[i].isOn = false;
            }
        }
    }

    private bool IsMouseOverUI()
    {
        return UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject();
    }
}
