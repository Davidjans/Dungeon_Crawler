using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyCard : Pickups {

    public override void PickupEffect(Collider other)
    {
        if (other.GetComponent<PlayerStats>())
            other.GetComponent<PlayerStats>().m_CurrentKeyCards += 1;
        base.PickupEffect(other);
    }
}
