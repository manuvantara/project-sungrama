using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthScript : MonoBehaviour
{
    public float initialHP = 100f;
    [SerializeField] private float currentHP = 100f;

    [SerializeField] private HealthBar healthBar;
    [SerializeField] private HealthBar healthBarAux;

    public GameObject[] deathEffects;
    public ParticleSystem hitEffect;

    // Start is called before the first frame update
    void Start()
    {
        currentHP = initialHP;

        // if on the enemy or player unit, get the health bar in the children
        if (this.gameObject.tag == "Enemy" || this.gameObject.tag == "Player")
        {
            healthBar = GetComponentInChildren<HealthBar>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(currentHP <= 0)
        {
            Death();   
        }
    }

    // Get damage from enemy
    public void GetDamage(float damage)
    {
        if(hitEffect != null)
        {
            // play the hit effect
            hitEffect.Play();
        }

        currentHP -= damage;

        healthBar.SetHealth(currentHP, initialHP);
        if(healthBarAux != null)
        {
            healthBarAux.SetHealth(currentHP, initialHP);
        }
    }

    public void Death() {
        // if its a tower, add score to the player
        if (this.gameObject.tag == "Tower" || this.gameObject.tag == "MainTower")
        {
            GameManager.instance.AddScore(this.gameObject);
        }

        if(deathEffects.Length != 0)
        {
            // instantiate the death effects
            foreach (GameObject deathEffect in deathEffects)
            {
                Instantiate(deathEffect, transform.position, transform.rotation);
            }
        }

        Destroy(gameObject);
    }
}
