using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{
    
    private int playerPoints = 0;
    private int enemyPoints = 0;
    public GameObject ball;
    public GameObject blankBall;
    public static Manager current;
    public NetworkManager networkManager;
    private bool ballExist = false;

    public event Action onBallPointTriggerEnterEnemy;
    public event Action onBallPointTriggerEnterPlayer;
    public void BallPointTriggerEnterEnemy()
    {
        if (onBallPointTriggerEnterEnemy != null && networkManager.isHost == 1)
        { 
            //Debug.Log("Wylecia³o");
            Instantiate(ball);
            playerPoints += 1;
            Debug.LogFormat("Player: {0} - Enemy {1}", playerPoints, enemyPoints);
        }
    }
    public void BallPointTriggerEnterPlayer()
    {
        if (onBallPointTriggerEnterPlayer != null && networkManager.isHost == 1)
        {
            Debug.Log("Wylecia³o");
            Instantiate(ball);
            enemyPoints += 1;
            Debug.LogFormat("Player: {0} - Enemy {1}", playerPoints, enemyPoints);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        //Instantiate(ball);
    }


    private void Awake()
    {
        current = this;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        checkBall();
    }

    void checkBall()
    {
        if (!ballExist)
        {
            if (networkManager.isHost == -1)
            {
                Instantiate(blankBall);
                ballExist = true;
            }
            if (networkManager.isHost == 1 && networkManager.gameState.EnemyPosX != 0)
            {
                Instantiate(ball);
                ballExist = true;
            }
        }
        
    }
}
