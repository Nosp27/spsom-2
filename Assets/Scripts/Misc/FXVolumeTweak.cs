using GameControl.StateMachine;
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
        Ship playerShip = GameController.Current.PlayerShip;
        if (playerShip == null)
            return;
        float playerDistance = (playerShip.transform.position - transform.position).magnitude;
        volume.weight = Mathf.InverseLerp(effectBounds.y, effectBounds.x, playerDistance);
    }
}
