using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FacilityGUI : MonoBehaviour
{
    private Camera cam;
    private LineRenderer lr;
    private TextMesh tm;
    private Collider coll;
    private MeshRenderer gizmo;
    private Color baseGizmoColor;

    [SerializeField] public GameObject CanvasPrefab; 

    public float scale;

    public string FacName;

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
        lr = GetComponentInChildren<LineRenderer>();
        tm = GetComponentInChildren<TextMesh>();
        coll = GetComponentInChildren<Collider>();
        gizmo = coll.GetComponent<MeshRenderer>();
        baseGizmoColor = gizmo.material.color;

        Vector3[] points = new Vector3[3];
        points[0] = Vector3.zero;
        points[1] = (Vector3.right * 2 + Vector3.up);
        points[2] = points[1] + (Vector3.right * 2);

        lr.transform.position = coll.ClosestPointOnBounds(cam.transform.right * 1000 + transform.position);
        tm.transform.localPosition = points[1];
        tm.text = FacName;
        lr.SetPositions(points);
        lr.positionCount = 3;
        lr.useWorldSpace = false;
    }

    // Update is called once per frame
    void Update()
    {
        lr.transform.rotation = Quaternion.LookRotation(
            Vector3.ProjectOnPlane(transform.position - cam.transform.position, Vector3.up), Vector3.up
        );

        Ship playerShip = GameController.Current.PlayerShip;
        if (playerShip)
            lr.transform.localScale = Vector3.one * (
                0.2f * scale * (playerShip.transform.position - cam.transform.position)
                .magnitude
            );
    }

    public void Hover()
    {
        gizmo.material.color = baseGizmoColor + new Color(0, 0, 0, 0.2f);
    }

    public void Unhover()
    {
        gizmo.material.color = baseGizmoColor;
    }
}