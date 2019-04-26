using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordScript : MonoBehaviour
{
    private float _damage;
    private void Start()
    {
        _damage = 10f;
    }
    private void OnTriggerEnter(Collider other)
    {
        _damage = Random.Range(10f, 20f);
        if (other.CompareTag("Enemy"))
        {
            other.gameObject.GetComponent<EnemyHealthScript>().TakeDamage(_damage);
        }
    }
}
