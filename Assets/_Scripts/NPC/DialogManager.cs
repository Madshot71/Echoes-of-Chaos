using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using System;
using System.Linq;

public class DialogManager : MonoBehaviour
{
    public static DialogManager instance;

    public DialogOption prefab;
    public RectTransform content;

    private const int Max = 16;
    private DialogOption[] pool = new DialogOption[Max];

    private List<DialogOption> active = new List<DialogOption>();
    private event Action onClose;

    private void Awake()
    {
        if (instance == null && instance != this)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        CreatePool();
    }

    private void CreatePool()
    {
        for (int i = 0; i < Max; i++)
        {
            pool[i] ??= Instantiate(prefab , content);
            pool[i].gameObject.SetActive(false);
        }
    }

    public void Close()
    {
        ClearActiveList();
        onClose?.Invoke();
    }


    private void ClearActiveList()
    {
        foreach (var item in active)
        {
            item.gameObject.SetActive(false);
        }
        active.Clear();
    }

    public void ShowOptions(Dictionary<string , Action> pairs , Action OnClose)
    {
        string[] keys = pairs.Keys.ToArray();

        for (int i = 0; i < keys.Count(); i++)
        {
            DialogOption option = GetOption();
            if (option == null)
            {
                break;
            }
            option.SetUp(pairs[keys[i]], keys[i]);
        }

        this.onClose += OnClose;
    }
    
    public DialogOption GetOption()
    {
        foreach (var item in pool)
        {
            if (item.gameObject.activeInHierarchy)
            {
                continue;
            }

            return item;
        }

        return null;
    }
    
}