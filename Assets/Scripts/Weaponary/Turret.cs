using System;
using UnityEngine;

public class Turret : MonoBehaviour
{
    public float RotationSpeed;

    [SerializeField] Transform HorizontalDrive;
    [SerializeField] Transform VerticalDrive;

    [SerializeField] float[] HorizontalLimits;
    [SerializeField] float[] VerticalLimits;
    [SerializeField] float VerticalSpeed;
    [SerializeField] float HorizontalSpeed;

    public GameObject AimingRay;
    [SerializeField] private float _TestLookVectorAngle;

    //For Debug
    [SerializeField] GameObject target;

    public void Aim(Vector3 point)
    {
        Transform sourceTransform = VerticalDrive;
        Vector3 lookVector = point - sourceTransform.position;
        Quaternion targetRotation = Quaternion.LookRotation(lookVector);

        float horAngle = targetRotation.eulerAngles.y - transform.rotation.eulerAngles.y;
        float verAngle = targetRotation.eulerAngles.x - sourceTransform.rotation.eulerAngles.x;

        horAngle = NormalizeAngle(horAngle);
        verAngle = NormalizeAngle(verAngle);

        horAngle = Mathf.Clamp(horAngle, HorizontalLimits[0], HorizontalLimits[1]);
        verAngle = Mathf.Clamp(verAngle, -VerticalLimits[0], -VerticalLimits[1]);

        Quaternion hTargetRotation = Quaternion.Euler(0, horAngle, 0);
        Quaternion vTargetRotation = Quaternion.Euler(verAngle, 0, 0);
        HorizontalDrive.localRotation = Quaternion.RotateTowards(HorizontalDrive.localRotation, hTargetRotation,
            Time.deltaTime * HorizontalSpeed);
        VerticalDrive.localRotation =
            Quaternion.RotateTowards(VerticalDrive.localRotation, vTargetRotation, VerticalSpeed * Time.deltaTime);
    }

    public bool Aimed(Vector3 point)
    {
        Vector3 lookVector = point - VerticalDrive.position;
        _TestLookVectorAngle = Vector3.Angle(VerticalDrive.transform.forward, lookVector);
        return Vector3.Angle(VerticalDrive.transform.forward, lookVector) < 5f;
    }

    void Die()
    {
        if (AimingRay != null)
            AimingRay.SetActive(false);
    }

    float NormalizeAngle(float angle)
    {
        angle -= Mathf.Floor(angle / 360f) * 360;
        return angle > 180 ? angle - 360 : angle;
    }
}