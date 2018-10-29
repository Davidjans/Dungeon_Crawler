using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{
    [SerializeField] private Text m_MoneyCount;
    [SerializeField] private Text m_KilledByText;
    [SerializeField] private Animator m_ShieldAnimator;
    public bool m_PoisonBullets = false;

    public float m_MaxHealth = 100;
    public float m_CurrentHealth;
    public int m_CurrentKeyCards;
    public bool m_ArmourActive = false;

    
    public bool ArmorActive
    {
        set
        {
            m_ArmourActive = value;
            if (value == true)
                m_ShieldAnimator.SetTrigger("Activate");
            else
                m_ShieldAnimator.SetTrigger("Deactivate");
        }
    }

    private void Start()
    {
        m_MoneyCount = GameObject.Find("MoneyCount").GetComponent<Text>();
        m_KilledByText = GameObject.Find("KilledBy").GetComponent<Text>();
    }

    private void Update()
    {
        TakeDamage(0, "Debugging");
        if (Input.GetKeyDown(KeyCode.U))
        {
            m_CurrentHealth = 9999999999;
        }
        if (Input.GetKeyDown(KeyCode.M))
        {
            StatManager.m_Data.m_CurrentMoney = 9999999999;
        }
        m_MoneyCount.text = StatManager.m_Data.m_CurrentMoney.ToString();
    }

    public void TakeDamage(float Damage, string Owner)
    {
        if (m_ArmourActive == false)
        {
            m_CurrentHealth = m_CurrentHealth - Damage;
        }
        if (Damage > 0 && m_ArmourActive == true)
        {
            ArmorActive = false;
        }


        if (m_CurrentHealth <= 0)
        {
            if(m_KilledByText != null)
            {
                m_KilledByText.text = "Killed by: " + Owner;
            }
            StatManager.SaveStats();
            Destroy(gameObject);
        }
    }
}
