using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EngineBalancer : MonoBehaviour
{
    private EngineRenderer[] Engines;

    public float[] engineRotationMultipliers;
    public float[] engineDriveMultipliers;

    private void Start()
    {
        Engines = GetComponentsInChildren<EngineRenderer>();
        initEngines();
    }

    void Die()
    {
        foreach (var e in Engines)
        {
            e.SendMessage("Die");
        }
    }

    private void initEngines()
    {
        if (Engines == null || Engines.Length == 0)
            return;
        engineDriveMultipliers = new float[Engines.Length];
        engineRotationMultipliers = new float[Engines.Length];
        int i = 0;
        foreach (var engine in Engines)
        {
            Vector3 distance = engine.transform.position - transform.position;
            Vector3 moment = distance.normalized - engine.transform.forward;
            float directionMultiplier = LinUtils.Projection(transform.forward, engine.transform.forward);
            // float sideMultiplier = LinUtils.Projection(transform.right, distance) *
            //                        Mathf.Sign(LinUtils.Projection(transform.right, -engine.transform.forward));
            Vector3 momentumCross = Vector3.Cross(distance, Vector3.up);
            float sideMultiplier = LinUtils.Projection(
                engine.transform.forward, momentumCross.normalized
            );
            moment.y = 0;

            if (Mathf.Abs(sideMultiplier) > 0.1)
            {
                engineRotationMultipliers[i] = 1 * sideMultiplier;
            }
            else
            {
                engineRotationMultipliers[i] = 0;
            }

            if (directionMultiplier > 0.1)
            {
                engineDriveMultipliers[i] = directionMultiplier;
            }
            else
            {
                engineDriveMultipliers[i] = 0;
            }
            
            i++;
            
            if (engine.debug)
            {
                Debug.DrawRay(transform.position, distance, Color.white);
                Debug.DrawRay(engine.transform.position, Vector3.up, Color.white);
                Debug.DrawRay(engine.transform.position, momentumCross.normalized, Color.green);
                Debug.DrawRay(engine.transform.position, engine.transform.forward * Mathf.Abs(sideMultiplier), Color.red);
            }
        }

        foreach (var engineMultipliers in new[] {engineRotationMultipliers,})
        {
            float minMultiplier = engineMultipliers.Min();
            float maxMultiplier = engineMultipliers.Max();
            float shift = -minMultiplier;
            float deviation = maxMultiplier - minMultiplier;
            deviation = deviation == 0 ? 1 : deviation;
            float shrink = 200 / deviation;

            i = 0;
            foreach (var engine in Engines)
            {
                engineMultipliers[i] = ((engineMultipliers[i] + shift) * shrink) - 100;
                i++;
            }
        }
    }

    public void BalanceEnginePower(float currentThrottle, Vector3 point, float angle)
    {
        if (Engines == null || Engines.Length == 0)
            return;
        float linearPower = currentThrottle * 70f;

        float angularPower = 0f;

        if (angle >= 1)
        {
            float proj = LinUtils.Projection(point - transform.position, transform.right);
            float totalEngineMultiplier = proj > 0 ? -1f : 1f;
            angularPower = totalEngineMultiplier * angle / 180f;
        }

        for (int i = 0; i < Engines.Length; ++i)
        {
            float thrust =
                (int) Mathf.Clamp(angularPower * engineRotationMultipliers[i] + linearPower * engineDriveMultipliers[i],
                    0, 100);
            Engines[i].SetThrust((int) thrust);
        }
    }
}