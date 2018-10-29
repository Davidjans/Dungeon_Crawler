using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour {
    [SerializeField] private Text m_TotalTimePlayedText;
    [SerializeField] private Text m_TotalCashEarnedText;
    [SerializeField] private Text m_TimesPlayedText;
	// Update is called once per frame
	void Update () {
        Display();
	}

    public void Display()
    {
        if (m_TotalTimePlayedText != null)
        {
            m_TotalTimePlayedText.text = StatManager.m_Data.m_HoursPlayed + " Hours, " + StatManager.m_Data.m_MinutesPlayed + " Minutes, " + Mathf.Round(StatManager.m_Data.m_SecondsPlayed) + " Seconds.";
        }
        if (m_TimesPlayedText != null)
        {
            m_TimesPlayedText.text = "You have played " + StatManager.m_Data.m_TimesPlayed + " times.";
        }
        if (m_TotalCashEarnedText != null)
        {
            m_TotalCashEarnedText.text = "You have earned " + StatManager.m_Data.m_TotalCashEarned + "$.";
        }
    }

    public void PlayingNow()
    {
        StatManager.SaveStats();
        StatManager.m_Data.m_TimesPlayed += 1;
    }
}
