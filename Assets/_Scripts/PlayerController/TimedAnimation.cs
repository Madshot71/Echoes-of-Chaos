using UnityEngine;

[CreateAssetMenu(fileName = "TimedAnimation" , menuName = "TimedAnimation")]
public class TimedAnimation : ScriptableObject
{
    public string _name;
    public float time;
    [field : SerializeField] public AnimationClip clip {get; private set;}
}