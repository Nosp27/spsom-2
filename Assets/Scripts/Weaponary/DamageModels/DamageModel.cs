using System;
using GameEventSystem;
using UnityEngine;

public abstract class DamageModel : MonoBehaviour
{
    [Space(20f)] [SerializeField] private GameObject aliveMesh;
    [SerializeField] private GameObject debrisMesh;
    
    [SerializeField] protected int maxHealth;
    [SerializeField] protected int health = 0;

    public GameObject AliveMesh => aliveMesh;
    public GameObject DebrisMesh => debrisMesh;
    
    private void Awake()
    {
        alive = true;
        if (health == 0)
            health = maxHealth;
        EventLibrary.objectReceivesDamage.AddListener(DamageListener);
    }

    private bool alive;
    public bool Alive => alive;

    public int MaxHealth => maxHealth;
    public int Health => health;

    public virtual void Die()
    {
        if (!alive)
            throw new Exception("Invoke Die() on already dead damage model");

        alive = false;
        EventLibrary.objectDestroyed.Invoke(this);
    }

    private void DamageListener(DamageModel dm, BulletHitDTO dto)
    {
        if (dm == this)
        {
            GetDamage(dto);
        }
    }

    public virtual void GetDamage(BulletHitDTO hit)
    {
    }
    
    protected virtual void PlayDebris()
    {
        if (!(aliveMesh && debrisMesh))
            return;

        aliveMesh.SetActive(false);
        debrisMesh.transform.parent = null;
        debrisMesh.SetActive(true);
    }
}