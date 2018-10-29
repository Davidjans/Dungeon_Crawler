using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Luck : MonoBehaviour {
    [SerializeField] private float m_UpgradeCost;
    [SerializeField] private int m_MaxLevel;
    [SerializeField] private Text m_UpgradeText;
    private void Start()
    {
        LevelDisplay();
    }
    private void OnTriggerStay(Collider other)
    {
        if (Input.GetKeyDown(KeyCode.E) == true && other.CompareTag("Player"))
        {
            if (m_UpgradeCost * StatManager.m_Data.m_LuckLevel <= StatManager.m_Data.m_CurrentMoney && StatManager.m_Data.m_DamageLevel <= m_MaxLevel)
            {
                StatManager.m_Data.m_CurrentMoney -= m_UpgradeCost * StatManager.m_Data.m_LuckLevel;
                UpgradeEffect(other);
                StatManager.SaveStats();
            }
            else if (m_UpgradeCost > StatManager.m_Data.m_CurrentMoney * StatManager.m_Data.m_LuckLevel)
            {
                // input not having enough money here
            }
            LevelDisplay();
        }
    }
    private void UpgradeEffect(Collider other)
    {
        StatManager.m_Data.m_LuckModifier += 1f;
        StatManager.m_Data.m_LuckLevel += 1;
    }
    private void LevelDisplay()
    {
        if (m_UpgradeText != null)
        {
            m_UpgradeText.text = StatManager.m_Data.m_LuckLevel + " / " + m_MaxLevel;
        }
    }
}
