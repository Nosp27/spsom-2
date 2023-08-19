using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

[ExecuteAlways]
public class ProceduralBackground : MonoBehaviour
{
    [SerializeField] private Transform camera;
    [SerializeField] private Transform sphere;
    [SerializeField] private GameObject starPrefab;
    [SerializeField][Range(0.01f, 1)] private float starDensity = 0.7f;
    [SerializeField][Range(0.01f, 0.9f)] private float starMinRelativeScale = 0.5f;

    private Camera mainCamera;
    
    private void Start()
    {
        mainCamera = Camera.main;
        if (!Application.isPlaying)
            return;
        
        for (int i = 0; i < starDensity * 10000; i++)
        {
            GameObject star = Instantiate(starPrefab, sphere);
            star.transform.localPosition = Random.onUnitSphere / 2.01f;
            star.transform.localScale *= Random.Range(starMinRelativeScale, 1);
        }
    }

    private void LateUpdate()
    {
        camera.rotation = mainCamera.transform.rotation;
    }
}
