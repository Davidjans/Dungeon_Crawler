using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartElevator : MonoBehaviour {
    [SerializeField] private Animator m_DoorAnimator;
    private float m_Goal;
    private Transform m_ElevatorTransform;
    private bool m_GoingUp = true;
    private bool m_DoorIsOpen = false;
    
    public bool DoorIsOpen
    {
        set
        {
            m_DoorIsOpen = value;
            if (value)
                m_DoorAnimator.SetTrigger("Activate");
            else
                m_DoorAnimator.SetTrigger("Deactivate");
        }
    }
    void Start () {
        m_Goal = transform.position.y;
        m_ElevatorTransform = GetComponent<Transform>();
        m_ElevatorTransform.position = new Vector3(m_ElevatorTransform.position.x, m_ElevatorTransform.position.y - 6 , m_ElevatorTransform.position.z) ;
    }
	
	// Update is called once per frame
	void Update () {
        if (transform.position.y <= m_Goal)
        {
            m_ElevatorTransform.position = new Vector3(m_ElevatorTransform.position.x, m_ElevatorTransform.position.y + 2 * Time.deltaTime, m_ElevatorTransform.position.z);
        }
        else
        {
            DoorIsOpen = true;
        }
    }
}
