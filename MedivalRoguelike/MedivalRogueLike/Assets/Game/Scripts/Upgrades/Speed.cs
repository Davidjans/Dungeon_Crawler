using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Speed : MonoBehaviour {
    [SerializeField] private float m_UpgradeCost;
    [SerializeField] private int m_MaxLevel;
    [SerializeField] private Text m_UpgradeText;
    private void Start()
    {
        LevelDisplay();
    }
    private void OnTriggerStay(Collider other)
    {
        if (Input.GetKeyDown(KeyCode.E) && other.CompareTag("Player"))
        {
            if (m_UpgradeCost * StatManager.m_Data.m_SpeedLevel <= StatManager.m_Data.m_CurrentMoney && StatManager.m_Data.m_SpeedLevel <= m_MaxLevel)
            {
                StatManager.m_Data.m_CurrentMoney -= m_UpgradeCost * StatManager.m_Data.m_SpeedLevel;
                UpgradeEffect(other);
                StatManager.SaveStats();
            }
            else if (m_UpgradeCost > StatManager.m_Data.m_CurrentMoney * StatManager.m_Data.m_SpeedLevel)
            {
                // input not having enough money here
            }
            LevelDisplay();
        }
    }
    private void UpgradeEffect(Collider other)
    {
        StatManager.m_Data.m_SpeedModifier += 0.1f;
        StatManager.m_Data.m_SpeedLevel += 1;
    }
    private void LevelDisplay()
    {
        if (m_UpgradeText != null)
        {
            m_UpgradeText.text = StatManager.m_Data.m_SpeedLevel + " / " + m_MaxLevel;
        }
    }
}
