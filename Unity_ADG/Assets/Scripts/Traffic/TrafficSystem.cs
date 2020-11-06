using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TrafficSystem : MonoBehaviour
{
    MovingCar[] vehicles = new MovingCar[0];

    private void Update()
    {
        if (vehicles.Length > 0)
        {
            for (int i = 0; i < vehicles.Length; i++)
            {
                if(vehicles[i] != null)
                    vehicles[i].OnUpdate();
            }
        }
    }
    public void InjectVehicle(MovingCar car)
    {
        Array.Resize(ref vehicles, vehicles.Length + 1);
        vehicles[vehicles.Length - 1] = car;
    }
    public void EjectVehicle(MovingCar car)
    {
        List<MovingCar> cars = vehicles.ToList();
        cars.Remove(car);
        vehicles = cars.ToArray();
    }
}
