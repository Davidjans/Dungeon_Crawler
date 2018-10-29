using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CircleHealthBar : MonoBehaviour {
    [SerializeField] private Image m_Bar;
    //public RectTransform m_Button; red knob 

    private float m_HealthValue;
    private float m_MaxHealth;

    private bool m_IsEnemy;

    private void Start() 
    {
        if (transform.parent.CompareTag("Enemy"))
        {
            m_IsEnemy = true;
            m_MaxHealth = GetComponentInParent<EnemyHealth>().m_MaxHealth;
        }
        if (transform.parent.CompareTag("Player"))
        {
            m_IsEnemy = false;
            m_MaxHealth = GetComponentInParent<PlayerStats>().m_MaxHealth;
            m_HealthValue = m_MaxHealth;
        }
    }

    void Update () {
        HealthChange(m_HealthValue);
	}

    private void HealthChange(float healthvalue)
    {
        if (m_IsEnemy == true)
        {
            m_HealthValue = GetComponentInParent<EnemyHealth>().m_Health;
        }
        if (m_IsEnemy == false)
        {
            m_HealthValue = GetComponentInParent<PlayerStats>().m_CurrentHealth;
        }

        float amount = (healthvalue / m_MaxHealth);
        if(amount > 1)
        {
            amount = 1;
        }
        m_Bar.fillAmount = amount / 2f;
    }
}
