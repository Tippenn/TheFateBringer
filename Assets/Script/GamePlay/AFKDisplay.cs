using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AFKDisplay : MonoBehaviour
{
    public Sprite[] afkNumber;
    public SpriteRenderer afkDisplay;
    private void Awake()
    {
        afkDisplay = GetComponent<SpriteRenderer>();
    }
    public void UpdateNumber(int number)
    {
        if(number == 0)
        {
            afkDisplay.enabled = false;
        }
        else
        {
            afkDisplay.enabled = true;
            afkDisplay.sprite = afkNumber[number - 1];
        }
        
    }
}
