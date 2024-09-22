using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackGroundHandler : MonoBehaviour
{
    public Sprite backGroundAct1;
    public Sprite backGroundAct2;
    public Sprite backGroundAct3;
    public Image backGroundImage;
    private void Awake()
    {
        backGroundImage = GetComponent<Image>();
    }
    public void SetBackground(int act)
    {
        if(act == 0)
        {
            backGroundImage.sprite = backGroundAct1;
        }
        else if(act == 1)
        {
            backGroundImage.sprite = backGroundAct2;
        }
        else
        {
            backGroundImage.sprite = backGroundAct3;
        }
    }
}
