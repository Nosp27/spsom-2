using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

public class HealthBar: MonoBehaviour
{
    public TextMesh HP;
    public float Radius;
    public Gradient colorShade;

    public int Health;
    public int MaxHealth = 1;
    private Vector3[] allCirclePoints;

    private LineRenderer lr;
    public Vector3 camOffset;

    private float Width, Height;

    private void Start()
    {
        Health = MaxHealth;
        lr = GetComponent<LineRenderer>();
        DrawPie();
        StartCoroutine(AnimatePieAngle());
    }

    public void DrawPie()
    {
        allCirclePoints = new Vector3[361];

        Vector3 center = Vector3.zero;
        for (int i = 0; i < 361; i++)
        {
            float angle = Mathf.Deg2Rad * i;
            allCirclePoints[i] = new Vector3(center.x + Mathf.Cos(angle) * Radius, center.y + Mathf.Sin(angle) * Radius, center.z);
        }
        lr.SetPositions(allCirclePoints);
    }

    public void SetPieAngle(int pieAngle)
    {
        pieAngle = Mathf.Clamp(pieAngle, 0, 361);
        lr.startColor = colorShade.Evaluate(pieAngle/361f);
        lr.endColor = colorShade.Evaluate(pieAngle/361f);
        lr.positionCount = pieAngle;
        lr.SetPositions(allCirclePoints);
    }

    public IEnumerator AnimatePieAngle()
    {
        while (true)
        {
            transform.position = Camera.main.ScreenToWorldPoint(camOffset);
            int delta = lr.positionCount - (361 * Health / MaxHealth);
            delta = (int)(Mathf.Min(Mathf.Abs(delta), 8) * Mathf.Sign(delta));
            HP.text = Health.ToString();
            if (delta != 0)
                SetPieAngle(lr.positionCount - delta);

            yield return new WaitForSeconds(.02f);
        }
    }
}
