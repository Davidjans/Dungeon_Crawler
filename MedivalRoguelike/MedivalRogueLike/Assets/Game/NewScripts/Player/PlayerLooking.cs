using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLooking : MonoBehaviour {
    [SerializeField] private Transform m_Transform;
    [SerializeField] private Transform m_CameraTransform;
    [SerializeField] private float m_MinLookY;
    [SerializeField] private float m_MaxLookY;
    [SerializeField] private float m_YSensetivity;
    [SerializeField] private float m_XSensetivity;
    private float YRotation;
    private float XRotation;

    private float m_RotationY;
    private float m_RotationX;


    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        
        YRotation += ((m_YSensetivity * Input.GetAxis("Mouse Y") ) * Time.deltaTime) * -1;
        XRotation += (m_XSensetivity * Input.GetAxis("Mouse X")) * Time.deltaTime;

        YRotation = Mathf.Clamp(YRotation, m_MinLookY, m_MaxLookY);
        if (XRotation < 0)
        {
            XRotation += 360;
        }
        else if ( XRotation >= 360)
        {
            XRotation -= 360;
        }
        m_Transform.eulerAngles = new Vector3 (0, XRotation, 0);
        m_CameraTransform.eulerAngles = new Vector3(YRotation, XRotation, 0);
    }
}
