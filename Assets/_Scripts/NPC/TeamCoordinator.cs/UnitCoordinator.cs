using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows.Speech;

public class UnitCoordinator : MonoBehaviour 
{
    public List<Unit> troops = new List<Unit>();
    private Transform follow;
    private KeywordRecognizer recognizer;

    private Dictionary<string, Action> keywords = new Dictionary<string, Action>();

    private void Start()
    {
        keywords.Add("Follow", FollowMe);
        keywords.Add("Attack", Attack);
        keywords.Add("Hold", Hold);

        recognizer = new KeywordRecognizer(keywords.Keys.ToArray());
        recognizer.OnPhraseRecognized += OnPhraseRecognized;
        recognizer.Start();
    }


    private void OnPhraseRecognized(PhraseRecognizedEventArgs args)
    {
        Debug.Log("Keyword :" + args);
        keywords[args.text].Invoke();
    }

    public void FollowMe()
    {

    }

    public void Attack()
    {

    }
    
    public void Hold()
    {
        
    }

    public void MoveTo(Vector3 position)
    {
        follow.position = position;

        foreach (Unit unit in troops)
        {
            unit.MoveTo(position);
        }
    }
}