using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPolice : MovingObject
{
    PoliceSystem policeSystem;

    protected override void VirtualOnEnable()
    {
        policeSystem = FindObjectOfType<PoliceSystem>();
    }
    protected override void VirtualStart()
    {
        if (connectPath)
        {
            policeSystem?.InjectVehicle(this);
        }
    }
    protected override void VirtualOnDestroy()
    {
        policeSystem?.EjectVehicle(this);
    }
}
