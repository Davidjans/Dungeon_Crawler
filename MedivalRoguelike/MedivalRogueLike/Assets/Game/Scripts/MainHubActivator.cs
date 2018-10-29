using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainHubActivator : MonoBehaviour {
    [SerializeField] public GameObject m_MainMenuCamera;
    [SerializeField] public GameObject m_PlayerCamera;
    [SerializeField] public GameObject m_Player;
    [SerializeField] public GameObject UI;

    public void PlayNow()
    {
        m_Player.SetActive(true);
        m_PlayerCamera.SetActive(true);
        UI.SetActive(true);
        m_MainMenuCamera.SetActive(false);
        
    }
}
