using UnityEngine;

[CreateAssetMenu(fileName = "CameraConfig", menuName = "Player/CameraConfig", order = 0)]
public class CameraConfig : ScriptableObject 
{
    [field: SerializeField] public float yawSpeed { get; private set; } = 5f;
    [field: SerializeField] public float pitchSpeed { get; private set; } = 3f;
    public float lerpSpeed = 3f;
    [Range(0f , 1f)]public float Sensitivity = 1f;

    [Header("Clamps")]
    [Range(20f, 45f)] public float maxPivot = 35f;
    [Range(-20f , -45f)] public float minPivot = -35f;    

    [Header("First-Person")]
    [SerializeField][Range(0f, 1f)] public float fp_sensitivity = 1f;
    [SerializeField][Range(0f, 1f)] public float fp_aim_sensitivity = 1f;

    [Header("Third-Person")]
    [SerializeField][Range(0f , 1f)] public float tp_sensitivity = 1f;
    [SerializeField][Range(0f , 1f)] public float tp_aim_sensitivity = 1f;
}