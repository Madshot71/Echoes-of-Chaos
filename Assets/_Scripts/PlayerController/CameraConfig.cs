using UnityEngine;

[CreateAssetMenu(fileName = "CameraConfig", menuName = "Player/CameraConfig", order = 0)]
public class CameraConfig : ScriptableObject 
{
    [field: SerializeField] public float rotateSpeed { get; private set; } = 5f;
    public float lerpSpeed = 3f;
    [Range(0f , 1f)]public float Sensitivity = 1f;
    
}