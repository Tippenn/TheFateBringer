using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    public Animator animator;
    public bool animate;
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    private void Start()
    {
        if(animate == true)
        {
            animator.SetTrigger("Loading");
            animator.SetTrigger("End");
        }
    }
    
    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadLevel(sceneName));
    }

    public IEnumerator LoadLevel(string sceneName)
    {
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
        animator.SetTrigger("Start");
        while (!op.isDone)
        {            
            animator.SetTrigger("Loading");
            yield return new WaitForSeconds(1.5f);
        }
    }
}
