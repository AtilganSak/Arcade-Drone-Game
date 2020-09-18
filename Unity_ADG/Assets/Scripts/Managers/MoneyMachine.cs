using UnityEngine;

[CreateAssetMenu(menuName = "Money Machine")]
public class MoneyMachine : ScriptableObject
{
    [System.Serializable]
    public struct Money
    {
        public float min;
        public float max;
        public int amount;
        public Color color;
    }

    public Money[] bank;
}
