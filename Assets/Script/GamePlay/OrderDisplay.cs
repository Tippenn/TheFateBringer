using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrderDisplay : MonoBehaviour
{
    public Sprite[] orderNumber;
    public SpriteRenderer orderDisplay;
    private void Awake()
    {
        orderDisplay = GetComponent<SpriteRenderer>();
    }
    public void UpdateNumber(int number)
    {
        orderDisplay.sprite = orderNumber[number - 1];
    }
}
