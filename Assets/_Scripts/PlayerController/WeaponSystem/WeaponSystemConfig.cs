using UnityEngine;

[CreateAssetMenu(fileName = "WeaponSystemConfig", menuName = "WeaponSystemConfig", order = 0)]
public class WeaponSystemConfig : ScriptableObject 
{

    [Header("Settings")]
    [SerializeField] public float lerpSpeed;
    public float maxTurnAngle;

    [Header("Gun")]
    [SerializeField] public float ads_camera_offset;


    [Header("Peak")]
    [SerializeField][Range(0f , 25f)] internal float peakAngle = 15f;

    [Header("Spine")]
    [SerializeField][Range(0 , 90)] internal float spineAngle = 15f;

}