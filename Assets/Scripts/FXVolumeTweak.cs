using UnityEngine;
using UnityEngine.Rendering;

public class FXVolumeTweak : MonoBehaviour
{
    [SerializeField] private Vector2 effectBounds;
    [SerializeField] private Volume volume;
    void Start()
    {
        
    }
    
    void Update()
    {
        float playerDistance = (GameController.Current.PlayerShip.transform.position - transform.position).magnitude;
        volume.weight = Mathf.InverseLerp(effectBounds.y, effectBounds.x, playerDistance);
    }
}
