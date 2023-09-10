using DG.Tweening;
using GameEventSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private DamageModel attachedDamageModel;
    [SerializeField] private bool trackPlayerShip;
    [SerializeField] private TextMeshProUGUI HP;
    [SerializeField] private Gradient colorShade;

    [SerializeField] private Image circle;

    private void Awake()
    {
        EventLibrary.objectReceivesDamage.AddListener(ReactOnHealthChange);
        if (trackPlayerShip)
            EventLibrary.switchPlayerShip.AddListener(
                (x, y) => ReactOnHealthChange(y.damageModel, null)
            );
    }

    private void ReactOnHealthChange(DamageModel damageModel, BulletHitDTO bulletHitDto)
    {
        bool isTrackingPlayerShip =
            attachedDamageModel == null && trackPlayerShip &&
            damageModel == GameController.Current.PlayerShip.damageModel;
        bool isTrackingAttachedShip = damageModel != null && damageModel == attachedDamageModel;
        if (!isTrackingPlayerShip && !isTrackingAttachedShip)
            return;
        SetPieAngle(361 * damageModel.Health / damageModel.MaxHealth);
        if (HP != null)
            HP.text = damageModel.Health.ToString();
    }

    private void SetPieAngle(int pieAngle)
    {
        pieAngle = Mathf.Clamp(pieAngle, 0, 361);
        circle.DOFillAmount(pieAngle / 361f, .1f);
        circle.color = colorShade.Evaluate(pieAngle / 361f);
    }
}