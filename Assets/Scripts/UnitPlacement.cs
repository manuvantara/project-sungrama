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

    [SerializeField] private GameObject[] unitToggles;

    private int currentToggle = 0;
    private bool[] displayedToggles = new bool[4]{true, true, false, false};
    private int[] availableToggles = new int[4]{0, 1, 2, 3};

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
        unitPrefab = unitPrefabs[0];
        // destroy the previous preview if it exists
        if (unitPrefabPreview != null)
        {
            Destroy(unitPrefabPreview);
        }
        unitPrefabPreview = Instantiate(unitPrefabPreviews[0], Vector3.zero, Quaternion.identity);
        // set the layer of preview to Preview
        unitPrefabPreview.layer = LayerMask.NameToLayer("Preview");
        
        unitPrefabPreview.transform.GetChild(1).gameObject.SetActive(true);
        unitPrefabPreview.transform.GetChild(0).gameObject.SetActive(false);

        currentToggle = 0;
    }

    public void SelectArcher()
    {
        unitPrefab = unitPrefabs[1];
        // destroy the previous preview if it exists
        if (unitPrefabPreview != null)
        {
            Destroy(unitPrefabPreview);
        }
        unitPrefabPreview = Instantiate(unitPrefabPreviews[1], Vector3.zero, Quaternion.identity);
        unitPrefabPreview.layer = LayerMask.NameToLayer("Preview");
        
        unitPrefabPreview.transform.GetChild(1).gameObject.SetActive(true);
        unitPrefabPreview.transform.GetChild(0).gameObject.SetActive(false);

        currentToggle = 1;
    }

    public void SelectMaus()
    {
        unitPrefab = unitPrefabs[2];
        // destroy the previous preview if it exists
        if (unitPrefabPreview != null)
        {
            Destroy(unitPrefabPreview);
        }
        unitPrefabPreview = Instantiate(unitPrefabPreviews[2], Vector3.zero, Quaternion.identity);
        unitPrefabPreview.layer = LayerMask.NameToLayer("Preview");

        unitPrefabPreview.transform.GetChild(1).gameObject.SetActive(true);
        unitPrefabPreview.transform.GetChild(0).gameObject.SetActive(false);

        currentToggle = 2;
    }

    public void SelectKarlsson()
    {
        unitPrefab = unitPrefabs[3];
        // destroy the previous preview if it exists
        if (unitPrefabPreview != null)
        {
            Destroy(unitPrefabPreview);
        }
        unitPrefabPreview = Instantiate(unitPrefabPreviews[3], Vector3.zero, Quaternion.identity);
        unitPrefabPreview.layer = LayerMask.NameToLayer("Preview");

        unitPrefabPreview.transform.GetChild(1).gameObject.SetActive(true);
        unitPrefabPreview.transform.GetChild(0).gameObject.SetActive(false);

        currentToggle = 3;
    }

    // swaps the selected toggle with the random toggle from the ones that are not displayed
    public void SwapToggle()
    {
        // if there are no available toggles, return
        if (availableToggles.Length <= 2)
        {
            return;
        }

        // get a random toggle from the available toggles that is not displayed
        int randomToggle = availableToggles[Random.Range(0, availableToggles.Length)];

        // if the random toggle is already displayed, run the function again
        if (displayedToggles[randomToggle])
        {
            SwapToggle();
            return;
        }

        // disable the current toggle
        unitToggles[currentToggle].SetActive(false);
        displayedToggles[currentToggle] = false;

        // enable the random toggle
        unitToggles[randomToggle].SetActive(true);
        displayedToggles[randomToggle] = true;

        // make the position of the old toggle the position of the new toggle
        unitToggles[randomToggle].transform.position = unitToggles[currentToggle].transform.position;
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
                    unitPrefabPreview.transform.GetChild(0).gameObject.SetActive(true);
                    unitPrefabPreview.transform.GetChild(1).gameObject.SetActive(false);
                } else {
                    unitPrefabPreview.transform.GetChild(1).gameObject.SetActive(true);
                    unitPrefabPreview.transform.GetChild(0).gameObject.SetActive(false);
                }
                
                Vector3 spawnPosition = hitInfo.point;
                spawnPosition.y += 0.25f;
                // change the x and z as a multiple of 1f
                spawnPosition.x = Mathf.Round(spawnPosition.x) + 0.5f;
                spawnPosition.z = Mathf.Round(spawnPosition.z) + 0.5f;
                unitPrefabPreview.transform.position = spawnPosition;
            }
        }

        // spawning the unit if the unit is selected and the player clicks
        if (Input.GetMouseButtonDown(0) && unitPrefab != null && currentEnergy >= unitPrefab.GetComponent<UnitAI>().energyRequired && !IsMouseOverUI())
        {
            ray = mainCamera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hitInfo, Mathf.Infinity, ~ignoreLayer))
            {
                // if it hits anything except the ground, or the point is beyond min z return
                if (hitInfo.collider.gameObject.layer != LayerMask.NameToLayer("Ground") || hitInfo.point.z > maxSpawnZ)
                {
                    return;
                }

                Vector3 spawnPosition = hitInfo.point;
                spawnPosition.y += 0.25f;
                // change the x and z as a multiple of 1f
                spawnPosition.x = Mathf.Round(spawnPosition.x) + 0.5f;
                spawnPosition.z = Mathf.Round(spawnPosition.z) + 0.5f;
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

                // swap the toggle
                SwapToggle();
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
