using UnityEngine;
using UnityEngine.UI;

public class WeaponUISlot : MonoBehaviour
{
    [SerializeField] private Image cooldownImage;
    [SerializeField] private Image weaponImage;

    private Weapon m_AttachedWeapon;

    public void AttachWeapon(Weapon w)
    {
        m_AttachedWeapon = w;
        weaponImage.sprite = w.weaponSprite;
    }
    
    private void Update()
    {
        if (m_AttachedWeapon == null)
            return;
        float cooldownRatio = m_AttachedWeapon.cooldown / m_AttachedWeapon.maxCooldown;
        bool doCooldown = cooldownRatio > 0;
        cooldownImage.enabled = doCooldown;
        if (doCooldown)
        {
            cooldownImage.fillAmount = cooldownRatio;
        }
    }
}
