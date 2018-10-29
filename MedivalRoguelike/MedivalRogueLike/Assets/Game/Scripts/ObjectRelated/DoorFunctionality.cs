using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorFunctionality : MonoBehaviour
{
    [SerializeField] private bool m_IsLocked;
    private bool m_DoorIsOpen = false;
    private Animator m_DoorAnimator;
    private PlayerStats m_PlayerStats;
    public bool DoorIsOpen
    {
        set
        {
            m_DoorIsOpen = value;
            if (value == true)
                m_DoorAnimator.SetTrigger("Activate");
            else
                m_DoorAnimator.SetTrigger("Deactivate");
        }
    }
    void Start()
    {
        m_DoorAnimator = GetComponent<Animator>();
        
    }
    private void OnTriggerStay(Collider other)
    {
        if (Input.GetKeyDown(KeyCode.E) == true && other.CompareTag("Player"))
        {
            m_PlayerStats = GameObject.Find("Player").GetComponent<PlayerStats>();
            if (m_IsLocked == true && m_PlayerStats.m_CurrentKeyCards > 0)
            {
                m_PlayerStats.m_CurrentKeyCards -= 1;
                m_IsLocked = false;
                if (m_DoorIsOpen == false)
                {
                    DoorIsOpen = true;
                    m_DoorIsOpen = true;
                }
                else if (m_DoorIsOpen == true)
                {
                    DoorIsOpen = false;
                    m_DoorIsOpen = false;
                }
            }
            else if (m_IsLocked == false)
            {
                if (m_DoorIsOpen == false)
                {
                    DoorIsOpen = true;
                    m_DoorIsOpen = true;
                }
                else if (m_DoorIsOpen == true)
                {
                    DoorIsOpen = false;
                    m_DoorIsOpen = false;
                }
            }

        }
    }
}
