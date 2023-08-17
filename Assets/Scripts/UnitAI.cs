using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitAI : MonoBehaviour
{
    private GameObject[] enemyTowers;

    private GameObject[] enemyUnits;

    private UnityEngine.AI.NavMeshAgent agent;
    private GameObject target;

    [SerializeField] private float detectionRadius = 10f;
    [SerializeField] private float attackRadius = 2f;
    [SerializeField] private float attackTimeout = 1f;
    public float attackDamage = 10f;
    public int energyRequired = 3;    

    [SerializeField] private Transform aimingPart;

    public ParticleSystem attackEffect;

    // sounds
    public AudioClip spawnSound;

    private float attackCooldown = 0f;

    private bool isTower = false;

    private void Start()
    {
        // if the unit is not a tower, set the aimingPart to itself
        if(aimingPart == null)
        {
            aimingPart = transform;
        }

        // play the spawn sound
        if(spawnSound != null)
        {
            AudioSource.PlayClipAtPoint(spawnSound, transform.position);
        }

        // check if the unit is a tower
        if (this.gameObject.tag == "Tower" || this.gameObject.tag == "MainTower")
        {
            isTower = true;
        }

        attackCooldown = attackTimeout;

        // get the enemy towers from the GameManager
        if(this.gameObject.tag == "Enemy")
        {
            enemyTowers = GameManager.instance.playerTowers;
        } else {
            enemyTowers = GameManager.instance.enemyTowers;
        }

        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        if (enemyTowers.Length == 0)
        {
            Debug.LogError("No enemy towers assigned for " + gameObject.name);
        }

        // if spawened by the enemy, automatically face the player side
        if (this.gameObject.tag == "Enemy")
        {
            transform.LookAt(GameManager.instance.playerTowers[1].transform.position);
        }
    }

    private void Update()
    {
        // update the enemy towers
        if(this.gameObject.tag == "Enemy")
        {
            enemyTowers = GameManager.instance.playerTowers;
        } else {
            enemyTowers = GameManager.instance.enemyTowers;
        }

        // find the nearest enemy unit if there is no target
        if (target == null)
        {
            target = FindNearestEnemyUnit();
            attackCooldown = attackTimeout;
            
            // if theres no enemy units in the detection radius, go to the nearest tower
            if (target == null && !isTower)
            {
                if (enemyTowers.Length > 0)
                {
                    // find the nearest tower
                    target = FindNearestTower();
                    // set the destination to the nearest tower
                    agent.SetDestination(target.transform.position);
                }
                agent.SetDestination(target.transform.position);
            }

        } else if (DistanceToTarget() < attackRadius)
        {
            // make the Y rotation face the target
            aimingPart.LookAt(target.transform);
            
            if(!isTower) {    
                // stop moving if the target is in the attack radius
                agent.SetDestination(transform.position);
            } else {
                // counter the wrong rotation of lookat
                aimingPart.Rotate(0, -90, 0);
            }

            // if the target is in the attack radius, attack the target
            if (attackCooldown <= 0)
            {
                // play the attack effect
                if(attackEffect != null)
                {
                    attackEffect.Play();
                }

                attackCooldown = attackTimeout;
                target.GetComponent<HealthScript>().GetDamage(attackDamage);
            } else {
                attackCooldown -= Time.deltaTime;
            }
        } else {
            if(!isTower) {
                // if there is a target, go to the target
                agent.SetDestination(target.transform.position);
            }
        }
    }

    // finding the nearest tower
    public GameObject FindNearestTower()
    {
        GameObject nearestTower = null;
        float nearestDistance = Mathf.Infinity;
        foreach (GameObject enemyTower in enemyTowers)
        {
            if (enemyTower == null)
            {
                continue;
            }

            float distance = Vector3.Distance(transform.position, enemyTower.transform.position);
            if (distance < nearestDistance)
            {
                nearestTower = enemyTower;
                nearestDistance = distance;
            }
        }
        return nearestTower;
    }

    // finding the nearest enemy unit in the detection radius
    public GameObject FindNearestEnemyUnit()
    {
        GameObject nearestEnemyUnit = null;
        float nearestDistance = Mathf.Infinity;

        if (this.gameObject.tag == "Enemy" || transform.parent.tag == "EnemyBase")
        {
            enemyUnits = GameObject.FindGameObjectsWithTag("Player");
        } else {
            enemyUnits = GameObject.FindGameObjectsWithTag("Enemy");
        }
        
        foreach (GameObject enemyUnit in enemyUnits)
        {
            float distance = Vector3.Distance(transform.position, enemyUnit.transform.position);
            if (distance < nearestDistance && distance < detectionRadius)
            {
                nearestEnemyUnit = enemyUnit;
                nearestDistance = distance;
            }
        }
        return nearestEnemyUnit;
    }

    // calculate the distance between the unit and the target
    public float DistanceToTarget()
    {
        return Vector3.Distance(transform.position, target.transform.position);
    }
}
