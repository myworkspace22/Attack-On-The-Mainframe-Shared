using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public int startMoney = 400;
    public int startLives = 20;

    public static int Rounds;

    private int lives;
    private int money;

    public int Lives { get { return lives; } }
    public int Money { get { return money; } }

    private void Start()
    {
        Rounds = 0;

        lives = startLives;
    }
    public void takeDamage()
    {
        lives--;
    }
    public void changeMoney(int amount)
    {
        money += amount;
    }
}