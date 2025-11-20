using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEngine;

public class WorldCharacterManagment : MonoBehaviour 
{
    public static WorldCharacterManagment instance;
    internal List<CharacterBase> Characters = new();

    private void Awake()
    {
        if(instance == null && instance != this)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        //Characters = FindAllObjectsOfType<CharacterBase>().ToList();
    }
}