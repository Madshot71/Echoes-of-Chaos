using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneTransition : MonoBehaviour
{
    public static SceneTransition instance; //Singleton pattern

    [Header("UI")]
    [SerializeField] private Image loadingBar;


    public void Awake()
    {
        if (instance == null && instance != this)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public void LoadScene(string name)
    {

    }

    public IEnumerator Load(string name)
    {
        AsyncOperation load = SceneManager.LoadSceneAsync(name);

        load.allowSceneActivation = false;
        WaitForSeconds wait = new WaitForSeconds(0.5f);

        while (load.isDone == false)
        {
            float progress = load.progress;
            loadingBar.fillAmount = progress;

            if(progress > 0.9f)
            {
                load.allowSceneActivation = true;
            }
            yield return wait;
        }
    }

}