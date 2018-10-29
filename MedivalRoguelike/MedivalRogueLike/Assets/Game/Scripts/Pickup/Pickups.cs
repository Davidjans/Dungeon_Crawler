using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickups : MonoBehaviour {
    [SerializeField] float m_BobbingMinimum;
    [SerializeField] float m_BobbingMaximum;
    [SerializeField] private bool m_IsShop;
    [SerializeField] private float m_ItemCost;
    private PlayerStats m_PlayerStats;
    private Transform m_ItemTransform;
    private float changeNumber = 0.1f;
    private void Start()
    {
        m_ItemTransform = GetComponent<Transform>();
        m_PlayerStats = GameObject.Find("Player").GetComponent<PlayerStats>();
    }
    private void Update()
    {
        ItemRotation();
        ItemBobbing();
    }

    private void ItemRotation()
    {
        m_ItemTransform.Rotate( new Vector3(Vector3.up.x, Vector3.up.y + 20, Vector3.up.z)  *  Time.deltaTime );
        this.transform.rotation = m_ItemTransform.rotation;
    }

    private void ItemBobbing()
    {
        
        if (m_ItemTransform.position.y >= m_BobbingMaximum) {
            changeNumber *= -1;
            m_ItemTransform.position = (new Vector3(m_ItemTransform.position.x, m_BobbingMaximum - 0.01f, m_ItemTransform.position.z));
        }
        else if (m_ItemTransform.position.y <= m_BobbingMinimum)
        {
            changeNumber *= -1;
            m_ItemTransform.position = (new Vector3(m_ItemTransform.position.x, m_BobbingMinimum + 0.01f, m_ItemTransform.position.z));
        }

        m_ItemTransform.position = (new Vector3(m_ItemTransform.position.x, m_ItemTransform.position.y + changeNumber * Time.deltaTime, m_ItemTransform.position.z));
        this.transform.position = m_ItemTransform.position;
    }

    public virtual void PickupEffect(Collider other)
    {
    }

    private void OnTriggerStay(Collider other)
    {
        if (Input.GetKeyDown(KeyCode.E) == true && other.CompareTag("Player"))
        {
            if (m_IsShop == true && m_ItemCost < StatManager.m_Data.m_CurrentMoney)
            {
                StatManager.m_Data.m_CurrentMoney -= m_ItemCost;
                PickupEffect(other);
                Destroy(gameObject);
            }
            else if (m_IsShop == true && m_ItemCost > StatManager.m_Data.m_CurrentMoney){
                // input not having enough money here
            }
            else
            {
                PickupEffect(other);
                Destroy(gameObject);
            }
            
            
        }
    }
}
