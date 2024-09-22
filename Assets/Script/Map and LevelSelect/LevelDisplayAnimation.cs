using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelDisplayAnimation : MonoBehaviour
{
    public Image LevelImage;
    public Image LevelBorder;
    public Button LevelClick;
    public Animator animator;

    private void Awake()
    {
        LevelImage = GameObject.Find("LevelImage").GetComponent<Image>();
        LevelBorder = GameObject.Find("Border").GetComponent<Image>();
        LevelClick = GameObject.Find("Border").GetComponent<Button>();
        animator = GetComponent<Animator>();
    }

    public void UpdateLevelDisplayView()
    {
        if (LevelClick.interactable == false)
        {
            animator.enabled = true;
            animator.SetBool("LevelLocked", true);
        }
        else
        {
            animator.enabled = false;
            animator.SetBool("LevelLocked", false);
        }
    }

    //void OnDisable()
    //{
    //    // Stop the Animator
    //    if (animator != null)
    //    {
    //        animator.enabled = false;
    //    }

    //    // Reset the sprite to the default
    //    if (LevelImage != null)
    //    {
    //        LevelImage.sprite = null;
    //    }
    //}

    //void OnEnable()
    //{
    //    // Re-enable the Animator when the GameObject is enabled
    //    if (animator != null)
    //    {
    //        animator.enabled = true;
    //    }

    //    // Optionally reset to the idle state or any other initial state
    //    if (animator != null)
    //    {
    //        animator.Play("Idle");
    //    }
    //}
}
