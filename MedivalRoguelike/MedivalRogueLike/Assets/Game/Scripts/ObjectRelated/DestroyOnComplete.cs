using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOnComplete : MonoBehaviour {
    [SerializeField] private float m_DestroyInThisTime;
	// Update is called once per frame
	void Update () {
        m_DestroyInThisTime -= Time.deltaTime;
        if (m_DestroyInThisTime <= 0)
        {
            Destroy(this.gameObject);
        }
	}
}
