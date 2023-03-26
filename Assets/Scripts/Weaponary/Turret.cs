using System;
using Unity.Mathematics;
using UnityEngine;

public class Turret : MonoBehaviour
{
    [SerializeField] float VerticalSpeed;
    [SerializeField] float HorizontalSpeed;

    [SerializeField] Transform HorizontalDrive;
    [SerializeField] Transform VerticalDrive;

    [SerializeField] float[] HorizontalLimits;
    [SerializeField] float[] VerticalLimits;

    [SerializeField] private Transform aimingPivot; 

    public GameObject AimingRay;

    //For Debug
    [SerializeField] GameObject target;

    public void Aim(Vector3 point)
    {
        Vector3 lookVectorHorizontal = aimingPivot.InverseTransformPoint(point);
        lookVectorHorizontal.y = 0;
        
        Vector3 lookVectorVertical = aimingPivot.InverseTransformPoint(point);
        lookVectorVertical.x = 0;

        Quaternion targetRotationHorizontal = Quaternion.LookRotation(lookVectorHorizontal);
        Quaternion targetRotationVertical = Quaternion.LookRotation(lookVectorVertical);

        Quaternion rotationStepHorizontal = Quaternion.RotateTowards(HorizontalDrive.localRotation, targetRotationHorizontal,
            Time.deltaTime * HorizontalSpeed);

        Quaternion rotationStepVertical = Quaternion.RotateTowards(VerticalDrive.localRotation, targetRotationVertical,
            Time.deltaTime * VerticalSpeed);
        rotationStepVertical = Quaternion.Euler(rotationStepVertical.eulerAngles.x, 0, 0);

        Quaternion staticPartRotation = aimingPivot.localRotation;
        float finalHorizontalDeviation = Quaternion.Angle(staticPartRotation, rotationStepHorizontal);
        float finalVerticalDeviation = Quaternion.Angle(staticPartRotation, rotationStepVertical);

        if (finalHorizontalDeviation > HorizontalLimits[0] && finalHorizontalDeviation < HorizontalLimits[1])
            HorizontalDrive.localRotation = rotationStepHorizontal;
        
        if (finalVerticalDeviation > -VerticalLimits[0] && finalVerticalDeviation < -VerticalLimits[1])
            VerticalDrive.localRotation = rotationStepVertical;
    }

    public bool Aimed(Vector3 point)
    {
        Vector3 lookVector = point - VerticalDrive.position;
        return Vector3.Angle(VerticalDrive.transform.forward, lookVector) < 5f;
    }

    void Die()
    {
        if (AimingRay != null)
            AimingRay.SetActive(false);
    }
}