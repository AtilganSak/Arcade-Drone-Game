using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PoliceSystem : MonoBehaviour
{
    MovingPolice[] polices = new MovingPolice[0];

    private void Update()
    {
        if (polices.Length > 0)
        {
            for (int i = 0; i < polices.Length; i++)
            {
                if (polices[i] != null)
                {
                    if(polices[i].connectPath)
                        polices[i].OnUpdate();
                }
            }
        }
    }
    public void InjectVehicle(MovingPolice police)
    {
        Array.Resize(ref polices, polices.Length + 1);
        polices[polices.Length - 1] = police;
    }
    public void EjectVehicle(MovingPolice police)
    {
        List<MovingPolice> cars = polices.ToList();
        cars.Remove(police);
        polices = cars.ToArray();
    }
}
