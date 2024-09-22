using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthDisplay : MonoBehaviour
{
    public Sprite[] healthNumber;
    public SpriteRenderer healthDisplay;
    private void Awake()
    {
        healthDisplay = GetComponent<SpriteRenderer>();
    }
    public void UpdateNumber(int number)
    {
        if(number-1 >= 0)
        {
            healthDisplay.sprite = healthNumber[number - 1];
        }
        
    }
}
