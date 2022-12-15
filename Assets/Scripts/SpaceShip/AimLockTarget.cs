using UnityEngine;

public class AimLockTarget : MonoBehaviour
{
    public Ship AttachedShip { get; private set; } 
    
    // Start is called before the first frame update
    void Start()
    {
        AttachedShip = GetComponentInParent<Ship>();
        MeshRenderer[] meshes = AttachedShip.GetComponentsInChildren<MeshRenderer>();
        Bounds bounds = new Bounds();
        bool setup = false;
        foreach (var mesh in meshes)
        {
            if (!mesh.enabled)
                continue;

            if (!setup)
            {
                bounds = mesh.bounds;
                setup = true;
            }
            else
            {
                bounds.Encapsulate(mesh.bounds);
            }
        }

        BoxCollider boundingBox = gameObject.AddComponent<BoxCollider>();
        boundingBox.isTrigger = true;
        gameObject.transform.localScale = bounds.size;
        gameObject.transform.position = bounds.center;
        gameObject.transform.rotation = transform.rotation;
        gameObject.name = "BoundingBox";
    }
}
