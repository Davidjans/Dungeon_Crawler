using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonVial : Pickups
{
    public override void PickupEffect(Collider other)
    {
        if (other.GetComponent<PlayerStats>())
            other.GetComponent<PlayerStats>().m_PoisonBullets = true;
        base.PickupEffect(other);
    }
}
