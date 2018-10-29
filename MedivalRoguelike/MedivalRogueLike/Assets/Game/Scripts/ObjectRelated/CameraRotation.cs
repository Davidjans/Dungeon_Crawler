using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotation : MonoBehaviour {

    [SerializeField] private Vector3 m_Rotation = new Vector3();
    [SerializeField] private Transform m_CameraTransform;
    [SerializeField] private float m_FramesNeeded;

    private void Awake()
    {
        m_Rotation.x = m_Rotation.x / m_FramesNeeded;
        m_Rotation.y = m_Rotation.y / m_FramesNeeded;
        m_Rotation.z = m_Rotation.z / m_FramesNeeded;
    }

    private void Update()
    {
        if(m_FramesNeeded >= 1)
        {
            m_CameraTransform.Rotate(m_Rotation);
            m_FramesNeeded -= 1;
        }
        if(m_FramesNeeded == 0)
        {
            //m_CameraTransform.rotation = new Quaternion(m_Rotation.x, m_Rotation.y, m_Rotation.z, 0);
            this.gameObject.SetActive(false);
        }


    }

    /*
	public void RotateCamera()
    {
        
        m_CameraTransform.Rotate(m_Rotation);
    }*/
}
