using UnityEngine;

public class Shield : MonoBehaviour
{
    private Material shieldMat;
    private float power = 100;
    private static readonly int TotalPower = Shader.PropertyToID("TotalPower");

    private void Start()
    {
        shieldMat = GetComponent<MeshRenderer>().material;
    }

    void GetDamage(BulletHitDTO hit)
    {
        power -= 5;
        if (power <= 0)
            Destroy(gameObject, 0.1f);
        shieldMat.SetFloat("TotalPower", power);
    }
}
