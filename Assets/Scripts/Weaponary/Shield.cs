using System.Collections;
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

    void Die()
    {
        StartCoroutine(AnimateDeath());
    }

    void GetDamage(BulletHitDTO hit)
    {
        power -= 5;
        if (power <= 0)
            Destroy(gameObject, 0.1f);
        shieldMat.SetFloat("TotalPower", power);
    }

    IEnumerator AnimateDeath()
    {
        while (power > 0)
        {
            GetDamage(new BulletHitDTO(1, Vector3.zero, Vector3.zero));
            yield return new WaitForSeconds(0.1f);
        }
        Destroy(gameObject);
    }
}
