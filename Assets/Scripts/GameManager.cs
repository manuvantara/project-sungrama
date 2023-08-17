using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // static instance of the GameManager which allows it to be accessed by any other script
    public static GameManager instance = null;

    public float energyRegenRate = 1f;

    public GameObject[] enemyTowers;
    public GameObject[] playerTowers;

    private int playerScore = 0;
    private int enemyScore = 0;

    public DataScript dataScript;

    [SerializeField] private GameObject playerWinsScreen;
    [SerializeField] private GameObject enemyWinsScreen;
    [SerializeField] private GameObject drawScreen;

    private void Awake()
    {
        // get the DataScript
        dataScript = GameObject.Find("Data").GetComponent<DataScript>();

        // check if instance already exists
        if (instance == null)
        {
            // if not, set instance to this
            instance = this;
        }
        // if instance already exists and it's not this:
        else if (instance != this)
        {
            // then destroy this. this enforces our singleton pattern, meaning there can only ever be one instance of a GameManager
            Destroy(gameObject);
        }
        // sets this to not be destroyed when reloading scene
        DontDestroyOnLoad(gameObject);

        instance.enemyTowers = enemyTowers;
        instance.playerTowers = playerTowers;
    }

    public void AddScore(GameObject tower)
    {
        // if the tower is in the enemy tower array, add score to the player
        if (ArrayContainsTower(tower, enemyTowers))
        {
            playerScore++;
            // if the tower is the main tower, player wins
            if (tower.tag == "MainTower")
            {
                PlayerWins();
            }
        } else {
            enemyScore++;
            // if the tower is the main tower, enemy wins
            if (tower.tag == "MainTower")
            {
                EnemyWins();
            }
        }
    }

    public bool ArrayContainsTower(GameObject tower, GameObject[] towerArray)
    {
        foreach (GameObject towerInArray in towerArray)
        {
            if (towerInArray == tower)
            {
                return true;
            }
        }

        return false;
    }

    public void EndGame()
    {
        if (playerScore > enemyScore)
        {
            PlayerWins();
        }
        else if (enemyScore > playerScore)
        {
            EnemyWins();
        }
        else
        {
            Draw();
        }
    }

    public void PlayerWins()
    {
        playerWinsScreen.SetActive(true);
        // stop the game time
        Time.timeScale = 0;

        // update the player's cups
        dataScript.cups += 14;
    }

    public void EnemyWins()
    {
        enemyWinsScreen.SetActive(true);
        // stop the game time
        Time.timeScale = 0;

        // update the player's cups
        dataScript.cups -= 14;
        if (dataScript.cups < 0)
        {
            dataScript.cups = 0;
        }
    }

    public void Draw()
    {
        drawScreen.SetActive(true);
        // stop the game time
        Time.timeScale = 0;
    }

    public void ReturnToMenu()
    {
        // reset the game time
        Time.timeScale = 1;
        // load the menu scene
        UnityEngine.SceneManagement.SceneManager.LoadScene("Menu");
    }
}
