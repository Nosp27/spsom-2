using UnityEngine;

public class KeepLookVector : MonoBehaviour
{
    [SerializeField] private bool mainCameraAsSounrce;
    [SerializeField] private Vector3 lookVector;

    private Transform mainCameraTransform;
    
    private void Start()
    {
        mainCameraTransform = Camera.main.transform;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (mainCameraAsSounrce && mainCameraTransform != null)
        {
            transform.rotation = Quaternion.LookRotation(transform.position - mainCameraTransform.position);
        }
        else
        {
            transform.rotation = Quaternion.LookRotation(lookVector);   
        }
    }
}
