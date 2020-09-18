using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CargoPoint : MonoBehaviour
{
    public Cargo cargo;

    private void Start()
    {
        if (cargo != null)
            cargo.startPoint = this;
    }
}
