using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealthScript : MonoBehaviour
{
    public float _Hp;
    [SerializeField]
    private GameObject _healthBar;

    private void Start()
    {
        _Hp = 100;
    }
    private void Update()
    {
        _healthBar.transform.localScale = new Vector3((_Hp / 100), _healthBar.transform.localScale.y, _healthBar.transform.localScale.z);
        if (_Hp <= 0)
        {
            this.gameObject.SetActive(false);
        }
    }
    public void TakeDamage(float damage)
    {
        _Hp -= damage;
    }
}
