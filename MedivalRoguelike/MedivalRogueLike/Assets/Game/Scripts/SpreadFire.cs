using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpreadFire : MonoBehaviour
{
    [SerializeField] private Transform m_Origin;
    [SerializeField] private Rigidbody m_Projectile;
    [SerializeField] private float m_OriginalTimeForReload;
    [SerializeField] private float m_BulletSpeed;
    [SerializeField] private int m_ShotCount;
    [SerializeField] private float m_Spread;

    private float m_TimeForReload;

    private void Start()
    {
        m_TimeForReload = m_OriginalTimeForReload;
    }

    private void Update()
    {
        m_Origin = this.transform;
        m_TimeForReload -= Time.deltaTime;
        if (Input.GetMouseButtonDown(0) && m_TimeForReload <= 0)
        {
            for (int i = 0; i < m_ShotCount; i++)
            {
                Quaternion shotRotation = transform.rotation;
                shotRotation.x = Random.Range(-m_Spread, m_Spread);
                shotRotation.y = Random.Range(-m_Spread, m_Spread);
                m_Projectile = Instantiate(m_Projectile, transform.position, shotRotation);

                m_Projectile.AddForce(this.transform.forward * m_BulletSpeed, ForceMode.Impulse);
                m_TimeForReload = m_OriginalTimeForReload;  
            }
        }
    }
}
    /*
    private void Update()
    {
        m_Origin = this.transform;
        m_TimeForReload -= Time.deltaTime;
        if (Input.GetMouseButtonDown(0) && m_TimeForReload <= 0)
        {
            Rigidbody BulletOne = Instantiate(m_Projectile, m_Origin.position, m_Origin.rotation);
            BulletOne.AddForce(this.transform.forward * m_InitialForce);
            
            m_TimeForReload = m_OriginalTimeForReload;
        }
    }*/


