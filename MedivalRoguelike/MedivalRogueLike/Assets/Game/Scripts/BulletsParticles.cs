using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletsParticles : MonoBehaviour {
    [SerializeField] private float m_Damage;
    [SerializeField] private float m_BulletLifetime;
    private void OnParticleCollision(GameObject other)
    {
        if (other.CompareTag("Enemy"))
        {
            other.GetComponent<EnemyHealth>().TakeDamage(m_Damage);
        }
    }

    private void Update()
    {
        m_BulletLifetime -= Time.deltaTime;
        if (m_BulletLifetime <= 0)
        {
            Destroy(gameObject);
        }
    }
}
