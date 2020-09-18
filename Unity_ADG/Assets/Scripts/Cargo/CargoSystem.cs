using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CargoSystem : MonoBehaviour
{
    public bool showDistances;

    public GameObject[] cargoPrefabs;
    public int deliveredCargoCount { get; private set; }
    public Cargo receivedCargoNow { get; private set; }

    CargoUI cargoUI;
    TransportModule transportModule;

    Cargo[] cargos;
    DeliveryPlace[] deliveryPlaces;
    List<DeliveryPlace> emptyDeliveryPlaces;

    private void OnValidate()
    {
        if (showDistances)
        {
            if (cargos.Length == 0)
            {
                cargos = FindObjectsOfType<Cargo>();
            }
        }
    }
    private void OnEnable()
    {
        cargoUI = FindObjectOfType<CargoUI>();

        cargos = FindObjectsOfType<Cargo>();
        if (cargos.Length == 0) enabled = false;

        deliveryPlaces = FindObjectsOfType<DeliveryPlace>();
        if (deliveryPlaces.Length == 0) enabled = false;

        transportModule = FindObjectOfType<TransportModule>();
        if (transportModule == null) enabled = false;

        for (int i = 0; i < deliveryPlaces.Length; i++)
            deliveryPlaces[i].listIndex = i;
        for (int i = 0; i < cargos.Length; i++)
            cargos[i].listIndex = i;

        emptyDeliveryPlaces = new List<DeliveryPlace>(deliveryPlaces.Length);

        cargoUI?.UpdateDeliveredCargoCount(deliveredCargoCount);

        transportModule.OnReceived += ReceivedCargo;
        transportModule.OnDelivered += DeliveredCargo;

        GC.Collect();
    }
    private void OnDestroy()
    {
        transportModule.OnReceived -= ReceivedCargo;
        transportModule.OnDelivered -= DeliveredCargo;
    }

    public void AddEmptyDeliveryPlace(DeliveryPlace deliveryPlace)
    {
        deliveryPlaces[deliveryPlace.listIndex] = null;
        emptyDeliveryPlaces.Add(deliveryPlace);
    }

    void ActivateCargos()
    {
        for (int i = 0; i < cargos.Length; i++)
        {
            if(!cargos[i].isCarrying && !cargos[i].spawning)
                cargos[i].DoReceivable();
        }
    }
    void DeactivateCargos()
    {
        for (int i = 0; i < cargos.Length; i++)
        {
            if (!cargos[i].isCarrying && !cargos[i].spawning)
                cargos[i].DoUnreachable();
        }
    }
    void DeliveredCargo(Cargo cargo)
    {
        deliveredCargoCount++;
        cargoUI?.UpdateDeliveredCargoCount(deliveredCargoCount);

        Lebug.Log("Delivered Cargo Count", deliveredCargoCount, "Cargo System");

        SpawnRandomCargo(cargo.startPoint);

        Destroy(receivedCargoNow.upPivot.gameObject);
        Destroy(receivedCargoNow.downPivot.gameObject);

        receivedCargoNow = null;

        ActivateCargos();
    }
    void ReceivedCargo(Cargo cargo)
    {
        receivedCargoNow = cargo;

        DeactivateCargos();

        Lebug.Log("Received Cargo Now", receivedCargoNow, "Cargo System");
    }
    void SpawnRandomCargo(CargoPoint cargoPoint)
    {
        Cargo newCargo = null;
        if (cargoPrefabs.Length > 1)
        {
            newCargo = Instantiate(cargoPrefabs[UnityEngine.Random.Range(0, cargoPrefabs.Length)], cargoPoint.transform.position, cargoPoint.transform.rotation).GetComponentInChildren<Cargo>();
        }
        else if(cargoPrefabs.Length == 1)
        {
            newCargo = Instantiate(cargoPrefabs[0], cargoPoint.transform.position, cargoPoint.transform.rotation).GetComponentInChildren<Cargo>();
        }
        if (newCargo)
        {
            cargos[receivedCargoNow.listIndex] = newCargo;
            cargoPoint.cargo = newCargo;
            newCargo.listIndex = receivedCargoNow.listIndex;
            newCargo.startPoint = cargoPoint;

            if (emptyDeliveryPlaces.Count > 0)
            {
                newCargo.deliveryPlace = emptyDeliveryPlaces[UnityEngine.Random.Range(0, emptyDeliveryPlaces.Count)];
                newCargo.CalculateDistance();
                newCargo.deliveryPlace.connectedCargo = true;
                emptyDeliveryPlaces.Remove(newCargo.deliveryPlace);
                deliveryPlaces[newCargo.deliveryPlace.listIndex] = newCargo.deliveryPlace;
            }

            newCargo.upPivot.gameObject.FixNameForClone();
            newCargo.upPivot.position = cargoPoint.transform.position;
            newCargo.DoReceivableWithTime(5);
        }
    }
    float nearDistance = 9999999F;
    float farDistance = 0F;
    private void OnDrawGizmos()
    {
        if (showDistances)
        {
            if (cargos.Length > 0)
            {
                for (int i = 0; i < cargos.Length; i++)
                {
                    Gizmos.color = Color.white;
                    Handles.color = Color.white;
                    if (Vector3.Distance(cargos[i].transform.position, cargos[i].deliveryPlace.transform.position) <= nearDistance)
                    {
                        nearDistance = Vector3.Distance(cargos[i].transform.position, cargos[i].deliveryPlace.transform.position);
                        Gizmos.color = Color.green;
                    }
                    else if(Vector3.Distance(cargos[i].transform.position, cargos[i].deliveryPlace.transform.position) >= farDistance)
                    {
                        farDistance = Vector3.Distance(cargos[i].transform.position, cargos[i].deliveryPlace.transform.position);
                        Gizmos.color = Color.red;
                    }

                    Gizmos.DrawLine(cargos[i].transform.position, cargos[i].deliveryPlace.transform.position);

                    Handles.Label((cargos[i].transform.position + cargos[i].deliveryPlace.transform.position) / 2, Mathf.RoundToInt(Vector3.Distance(cargos[i].transform.position, cargos[i].deliveryPlace.transform.position)).ToString());
                }
            }
        }
    }
}
