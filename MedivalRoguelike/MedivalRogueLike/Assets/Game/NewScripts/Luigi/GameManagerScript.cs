using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerScript : MonoBehaviour
{
    [SerializeField]
    private GameObject _enemy;

    private float _respawnTimer;

    private void Start()
    {
        _respawnTimer = 5f;
    }
    private void Update()
    {
        if (_enemy.activeSelf == false)
        {
            StartCoroutine(Respawn());
            print("he dieded");
        }
    }

    IEnumerator Respawn()
    {
        yield return new WaitForSeconds(_respawnTimer);
        _enemy.gameObject.SetActive(true);
        _enemy.GetComponent<EnemyHealthScript>()._Hp = 100f;
    }
}
