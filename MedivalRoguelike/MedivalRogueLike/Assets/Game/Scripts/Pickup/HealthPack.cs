using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPack : Pickups {

    public override void PickupEffect(Collider other)
    {
        if (other.GetComponent<PlayerStats>())
            other.GetComponent<PlayerStats>().m_CurrentHealth += Random.Range(20,40);
        base.PickupEffect(other);
    }
}
