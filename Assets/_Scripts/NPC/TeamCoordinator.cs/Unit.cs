using UnityEngine;

[RequireComponent(typeof(NPC))]
public class Unit : MonoBehaviour
{
    public bool isLeader = false;
    public NPC npc { get; private set; }

    private void Awake()
    {
        npc ??= GetComponent<NPC>();
    }

    public void MoveTo(Vector3 position)
    {
        npc.MoveTo(position);
    }

}