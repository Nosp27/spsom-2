using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawRelativeAxes : MonoBehaviour
{
    private Transform From => GameController.Current.PlayerShip.transform;
    private Transform To => transform;

    private LineRenderer directionRenderer;

    // Start is called before the first frame update
    void Start()
    {
        directionRenderer = GetComponent<LineRenderer>();
        directionRenderer.useWorldSpace = true;
    }

    // Update is called once per frame
    void Update()
    {
        RenderDirection();
    }

    void RenderDirection()
    {
        directionRenderer.SetPosition(0, From.position);
        directionRenderer.SetPosition(1, To.position);
        directionRenderer.positionCount = 2;
    }
}
