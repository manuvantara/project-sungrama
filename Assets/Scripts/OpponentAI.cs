using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpponentAI : MonoBehaviour
{
    [SerializeField] private Vector2 spawnzoneMin;
    [SerializeField] private Vector2 spawnzoneMax;

    public GameObject[] usedUnits;

    [SerializeField] private int maxEnergy = 10;
    [SerializeField] private int currentEnergy = 10;

    [SerializeField] private float spawnCooldownTime = 3f;

    [SerializeField] private float spawnY = 15.26f;

    private float spawnCooldown = 0f;
    private float energyRegenCounter = 0f;

    private void Start()
    {
        currentEnergy = maxEnergy;
    }

    private void Update()
    {
        // spawn a random unit from the usedUnits in the random point of the spawnzone
        // if theres enough energy to spawn it
        int randomUnit = Random.Range(0, usedUnits.Length);

        if ((spawnCooldown <= 0) && (currentEnergy >= usedUnits[randomUnit].GetComponent<UnitAI>().energyRequired))
        {
            SpawnUnit(usedUnits[randomUnit]);
            spawnCooldown = spawnCooldownTime;
            currentEnergy -= usedUnits[1].GetComponent<UnitAI>().energyRequired;
        }
        spawnCooldown -= Time.deltaTime;

        // regenerate energy
        if (energyRegenCounter >= 2f)
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

    // spawn a given unit in a random point of the spawnzone
    private GameObject SpawnUnit(GameObject unit)
    {
        // get a random point in the spawnzone
        Vector3 spawnPoint = new Vector3(Random.Range(spawnzoneMin.x, spawnzoneMax.x), spawnY, Random.Range(spawnzoneMin.y, spawnzoneMax.y));

        // spawn the unit
        GameObject newUnit = Instantiate(unit, spawnPoint, Quaternion.identity);

        // gine the unit the Enemy tag
        newUnit.tag = "Enemy";

        // make the unit red to differentiate it from the player's units
        newUnit.GetComponent<Renderer>().material.color = new Color(1, 0, 0, 1);

        // return the unit
        return newUnit;
    }
}
