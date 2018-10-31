using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour {
    [SerializeField] private GameObject m_BotDeath;
    [SerializeField] private int m_MinCashDrop = 10;
    [SerializeField] private int m_MaxCashDrop = 50;
    public float m_MaxHealth = 40;
    public float m_Health;
    private PlayerStats m_PlayerStats;
    private EnemyAnimations m_EnemyAnimations;
    private void Start()
    {
        m_Health = m_MaxHealth;
        m_EnemyAnimations = GetComponent<EnemyAnimations>();
    }
    public void TakeDamage(float Damage)
    {

        Damage *= StatManager.m_Data.m_DamageModifier;
        m_Health = m_Health - Damage;
        if (m_Health <= 0)
        {
            int cashDrop = Random.Range(m_MinCashDrop, m_MaxCashDrop);
            StatManager.m_Data.m_CurrentMoney += cashDrop;
            StatManager.m_Data.m_TotalCashEarned += cashDrop;
            m_EnemyAnimations.SetAnimation(AnimationState.Death);
            StatManager.SaveStats();
            GameObject BotDeath = Instantiate(m_BotDeath, transform.position, new Quaternion(0, 0, 0, 0));
            Destroy(gameObject);
        }
    }

    public void DestroyObject()
    {
        Destroy(gameObject);
        GameObject BotDeath = Instantiate(m_BotDeath, transform.position, new Quaternion(0, 0, 0, 0));
    }
}
