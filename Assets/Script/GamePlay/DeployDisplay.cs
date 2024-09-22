using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeployDisplay : MonoBehaviour
{
    public Sprite[] deployNumber;
    public SpriteRenderer deployDisplay;

    private void Awake()
    {
        deployDisplay = GetComponent<SpriteRenderer>();
    }
    public void UpdateNumber(int number)
    {
        if(number > 8)
        {
            deployDisplay.sprite = deployNumber[8];
        }
        deployDisplay.sprite = deployNumber[number];
    }
}
