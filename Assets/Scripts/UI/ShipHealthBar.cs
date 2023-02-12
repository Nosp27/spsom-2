using UnityEngine;
using UnityEngine.UI;

public class ShipHealthBar : MonoBehaviour
{
    private DamageModel damageModel;
    [SerializeField] private Image healthBarImage;

    private void Start()
    {
        damageModel = GetComponentInParent<DamageModel>();
    }

    void Update()
    {
        healthBarImage.fillAmount = 1.0f * damageModel.Health / damageModel.MaxHealth;
    }
}
