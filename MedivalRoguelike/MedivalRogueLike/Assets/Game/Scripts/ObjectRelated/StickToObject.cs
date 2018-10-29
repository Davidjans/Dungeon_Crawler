using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickToObject : MonoBehaviour {
    [SerializeField] private Transform m_PlayerPosition;
    [SerializeField] private Vector3 m_AdjustableValues;


    void Update()
    {
        transform.position = new Vector3(m_PlayerPosition.position.x + m_AdjustableValues.x, m_PlayerPosition.position.y + m_AdjustableValues.y, m_PlayerPosition.position.z + m_AdjustableValues.z);
    }
}
