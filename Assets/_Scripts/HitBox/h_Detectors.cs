using UnityEngine;

public class h_Detectors : MonoBehaviour
{
    private HitBox main;

    private void Awake()
    {
        if (main != null)
        {
            return;
        }

        if (transform.root.TryGetComponent<HitBox>(out main) == false)
        {
            // Turn off the component if main can't be found
            this.enabled = false;
            return;
        }
    }

    public HitBox PassRef()
    {
        return main;
    }

}