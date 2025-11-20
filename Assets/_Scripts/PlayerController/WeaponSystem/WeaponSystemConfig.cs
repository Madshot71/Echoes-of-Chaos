using UnityEngine;

[CreateAssetMenu(fileName = "WeaponSystemConfig", menuName = "WeaponSystemConfig", order = 0)]
public class WeaponSystemConfig : ScriptableObject 
{

    [Header("Gun")]
    [SerializeField] public float ads_camera_offset;

    public Vector3 aimPointPosition; 
    public Vector3 HipPointPosition;

    [Header("Peak")]
    [SerializeField][Range(0f , 25f)] internal float peakAngle = 15f;
}