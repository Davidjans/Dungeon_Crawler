using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullets : MonoBehaviour {
    [SerializeField] float m_Damage;
    [SerializeField] GameObject m_HitEffect;
    public string m_Owner;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            other.GetComponent<EnemyHealth>().TakeDamage(m_Damage);
            GameObject HitEffect = Instantiate(m_HitEffect, transform.position, new Quaternion(0, 0, 0, 0));
        }
        else if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerStats>().TakeDamage(m_Damage, m_Owner);
        }
        Destroy (gameObject);
    }
}
