using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    public int m_TimesPlayed;
    private GameObject m_DeathMenu;
    private PlayerStats m_PlayerStats;
    private GameObject m_UIOverlay;
    public GameObject m_PauseMenu;

    public bool m_CurrentlyPauzed;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }
    void Start () {
        m_TimesPlayed += 1;
        GameObject MainMenu = GameObject.Find("MainMenu");

        m_DeathMenu = GameObject.Find("DeathMenu");
        m_PauseMenu = GameObject.Find("PauseMenu");
        m_PlayerStats = GameObject.Find("Player").GetComponent<PlayerStats>();
        m_UIOverlay = GameObject.Find("UIOverlay");
        if(MainMenu != null)
        {
            m_UIOverlay.SetActive(false);
            m_PlayerStats.gameObject.SetActive(false);
        }
        
        m_PauseMenu.SetActive(false);
        m_DeathMenu.SetActive(false);
    }
	
	// Update is called once per frame
	void Update () {
        
        if (Input.GetKeyDown(KeyCode.Escape) && m_PlayerStats.m_CurrentHealth >0)
        {
            if(m_CurrentlyPauzed == false)
            {
                StatManager.SaveStats();
                m_PauseMenu.SetActive(true);
                m_CurrentlyPauzed = true;
            }
            else if(m_CurrentlyPauzed == true)
            {
                m_PauseMenu.SetActive(false);
                m_CurrentlyPauzed = false;
            }
        }

        if(m_PlayerStats.m_CurrentHealth <= 0)
        {
            m_DeathMenu.SetActive(true);
        }
	}
}
