using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SingleShotFire : MonoBehaviour {
    [SerializeField] private float m_InitialForce = 1500;
    [SerializeField] private Transform m_Origin;
    [SerializeField] private Rigidbody m_Projectile;
    [SerializeField] private float m_OriginalFireRate;
    [SerializeField] private float m_OriginalReloadTime;
    [SerializeField] private float m_MagSize;

    public float m_CurrentReloadTime;
    public float m_FireRate;
    public float m_CurrentMagSize;
    public bool m_CurrentlyReloading;

    private Text m_AmmoCount;
    private void Start()
    {
        m_AmmoCount = GameObject.Find("AmmoCount").GetComponent<Text>();
        m_CurrentMagSize = m_MagSize;
        m_FireRate = m_OriginalFireRate;
        m_AmmoCount.text = m_CurrentMagSize + "/" + m_MagSize;
    }

    private void FixedUpdate()
    {
        m_Origin = transform;
        m_FireRate -= Time.fixedDeltaTime;
        if (Input.GetKeyDown(KeyCode.R))
        {
            m_CurrentlyReloading = true;
        }
        if (m_CurrentlyReloading == true)
        {
            m_CurrentReloadTime -= Time.deltaTime;
            if(m_CurrentReloadTime <= 0)
            {
                m_CurrentMagSize = m_MagSize;
                m_CurrentReloadTime = m_OriginalReloadTime *= StatManager.m_Data.m_ReloadModifier;
                m_CurrentlyReloading = false;
                m_AmmoCount.text = m_CurrentMagSize + "/" + m_MagSize;
            }
        }
        if (Input.GetMouseButton(0) && m_FireRate <= 0 && m_CurrentlyReloading == false && m_CurrentMagSize >0)
        {
            m_CurrentMagSize -= 1;
            Rigidbody newProjectile = Instantiate(m_Projectile, m_Origin.position, m_Origin.rotation);
            newProjectile.GetComponent<Renderer>().material.color = GetComponentInParent<PlayerStats>().m_PoisonBullets ? Color.green : Color.white; 
            newProjectile.AddForce(transform.forward * m_InitialForce, ForceMode.Impulse);
            m_FireRate = m_OriginalFireRate;

            m_AmmoCount.text = m_CurrentMagSize + "/" + m_MagSize;
        }

        
    }
}
