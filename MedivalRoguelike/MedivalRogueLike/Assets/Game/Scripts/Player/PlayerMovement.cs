using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {
    public float m_Speed;

    private Vector3 m_Movement;
    private Rigidbody m_PlayerRigidBody;
    private int m_Floormask;
    private float m_CamRayLength = 1000f;

    private void Awake()
    {
        m_Floormask = LayerMask.GetMask("Floor");
        m_PlayerRigidBody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        MovePlayer(h, v);
        TurnPlayer();

    }

    private void MovePlayer(float h, float v)
    {
        m_Movement.Set(h, 0f, v);
        m_Movement = m_Movement.normalized * (m_Speed * StatManager.m_Data.m_SpeedModifier) * Time.deltaTime;

        m_PlayerRigidBody.MovePosition(transform.position + m_Movement);
    }

    private void TurnPlayer()
    {
        Ray camRay = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit floorHit;

        if (Physics.Raycast(camRay, out floorHit, m_CamRayLength, m_Floormask))
        {
            Vector3 playerToMouse = floorHit.point - transform.position;
            playerToMouse.y = 0f;

            Quaternion newRotation = Quaternion.LookRotation(playerToMouse);
            m_PlayerRigidBody.MoveRotation(newRotation);
        }
    }
}
