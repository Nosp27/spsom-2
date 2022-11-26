using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleWithEngine : MonoBehaviour
{
    [SerializeField] AnimationCurve scaleCurve = AnimationCurve.EaseInOut(0, 0.1f, 100, 3);
    private Vector3 startScale;
    private Engine attachedEngine;
    
    // Start is called before the first frame update
    void Start()
    {
        attachedEngine = GetComponentInParent<Engine>();
        startScale = transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 scale = startScale;
        scale.z = scaleCurve.Evaluate(attachedEngine.thrust);
        transform.localScale = scale;
    }
}
