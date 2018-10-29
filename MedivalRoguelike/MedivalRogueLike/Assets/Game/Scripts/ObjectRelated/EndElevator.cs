using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndElevator : MonoBehaviour
{
    [SerializeField] private float m_ElevatorGoal;
    [SerializeField] private Animator m_DoorAnimator;
    private Transform m_ElevatorTransform;
    private bool m_GoingUp = false;
    private bool m_DoorIsOpen = true;
    
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
    private void Start()
    {
        m_ElevatorTransform = GetComponent<Transform>();
        DoorIsOpen = true;
        m_DoorIsOpen = true;
    }

    private void Update()
    {
        if (transform.position.y <= m_ElevatorGoal && m_GoingUp == true) {
            m_ElevatorTransform.position = new Vector3(m_ElevatorTransform.position.x, m_ElevatorTransform.position.y + 2 * Time.deltaTime, m_ElevatorTransform.position.z);
        }
        else
        {
            m_GoingUp = false;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (Input.GetKeyDown(KeyCode.E) == true && other.CompareTag("Player"))
        {
            m_GoingUp = true;
            DoorIsOpen = false;
            m_DoorIsOpen = false;
        }
    }
}
