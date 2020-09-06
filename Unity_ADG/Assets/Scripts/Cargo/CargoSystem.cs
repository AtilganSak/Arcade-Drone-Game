﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CargoSystem : MonoBehaviour
{
    public int deliveredCargoCount { get; private set; }
    public int cargosCount { get; private set; }
    public int remaningCargoCount
    {
        get
        {
            if (deliveredCargoCount > cargosCount)
                return 0;
            else
                return cargosCount - deliveredCargoCount;
        }
    }
    public Cargo receivedCargoNow { get; private set; }

    public Action deliveryIsCompleted;

    CargoUI cargoUI;

    Cargo[] cargos;
    
    DeliveryPlace[] deliveryPlaces;

    TransportModule transportModule;

    private void OnEnable()
    {
        cargoUI = FindObjectOfType<CargoUI>();

        cargos = FindObjectsOfType<Cargo>();
        if (cargos.Length == 0) enabled = false;

        deliveryPlaces = FindObjectsOfType<DeliveryPlace>();
        if (deliveryPlaces.Length == 0) enabled = false;

        transportModule = FindObjectOfType<TransportModule>();
        if (transportModule == null) enabled = false;

        cargosCount = cargos.Length;
        cargoUI?.UpdateTotalCargoCount(cargos.Length);
        cargoUI?.UpdateRemaningCargoCount(remaningCargoCount);
        cargoUI?.UpdateDeliveredCargoCount(deliveredCargoCount);

        Lebug.Log("Cargos Count", cargosCount, "Cargo System");
        Lebug.Log("Remaning Cargo Count", remaningCargoCount, "Cargo System");

        transportModule.OnReceived += ReceivedCargo;
        transportModule.OnDelivered += DeliveredCargo;
    }
    private void OnDestroy()
    {
        transportModule.OnReceived -= ReceivedCargo;
        transportModule.OnDelivered -= DeliveredCargo;
    }
    void DeliveredCargo(Cargo cargo)
    {
        if(deliveredCargoCount + 1 <= cargosCount)
        {
            deliveredCargoCount++;
            cargoUI?.UpdateDeliveredCargoCount(deliveredCargoCount);
            cargoUI?.UpdateRemaningCargoCount(remaningCargoCount);
        }
        else
        {
            deliveredCargoCount = cargosCount;

            if (deliveryIsCompleted != null)
                deliveryIsCompleted.Invoke();
        }

        Lebug.Log("Delivered Cargo Count", deliveredCargoCount, "Cargo System");
        Lebug.Log("Remaning Cargo Count", remaningCargoCount, "Cargo System");

        cargoUI?.UpdateDeliveredCargoCount(deliveredCargoCount);
        cargoUI?.UpdateRemaningCargoCount(remaningCargoCount);

        receivedCargoNow = null;
    }
    void ReceivedCargo(Cargo cargo)
    {
        receivedCargoNow = cargo;
        Lebug.Log("Received Cargo Now", receivedCargoNow, "Cargo System");
    }
}
