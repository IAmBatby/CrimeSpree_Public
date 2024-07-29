using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : Equipment
{
    public override void Selected()
    {
        Debug.Log("I am a gun hehehehe");
    }

    public override void EquipmentUpdate()
    {
        if (Input.GetMouseButtonDown(0))
        {
            inventory.player.Shoot(inventory.player.gunMask);
        }
    }
}