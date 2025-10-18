using UnityEngine;

[CreateAssetMenu(fileName = "CameraConfig", menuName = "Player/CameraConfig", order = 0)]
public class CameraConfig : ScriptableObject 
{
    [field : SerializeField] public float rotateSpeed {get; private set;} = 5f;
    
}