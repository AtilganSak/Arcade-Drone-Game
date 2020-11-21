using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Damage Table")]
public class DamageTable : ScriptableObject
{
    [System.Serializable]
    public struct Slot
    {
        public float amount;
        public float minThresh;
        public float maxThresh;
    }

    public Slot[] slots;
}
