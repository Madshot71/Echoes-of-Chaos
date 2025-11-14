using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class GameSceneManager : MonoBehaviour
{
    public static GameSceneManager instance;
    private SceneTransition transition => SceneTransition.instance;

    public bool isPaused { get; private set; } = false;

    private void Awake()
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

    public void Pause()
    {
        if (!isPaused)
        {
            isPaused = true;
            Time.timeScale = 0;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            isPaused = false;
            Time.timeScale = 1f;
            Time.fixedDeltaTime = 0.02f * Time.timeScale;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    public void Load()
    {

    }

    public void Save()
    {

    }

    public void ReturnToMenu()
    {
        if(transition == null){
            return;
        }

        transition.LoadScene("MainMenu");
    }
    
}

public class SceneData
{
    //public List<CharacterBase> characters = new List<>();
}