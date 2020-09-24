using UnityEngine;

public class MoneyManager : MonoBehaviour
{
    public MoneyMachine moneyMachine;

    public MoneyMachine.Money? GiveMoney(float distance)
    {
        if (moneyMachine.bank.Length > 0)
        {
            for (int i = 0; i < moneyMachine.bank.Length; i++)
            {
                if (distance >= moneyMachine.bank[i].min && distance <= moneyMachine.bank[i].max)
                {
                    return moneyMachine.bank[i];
                }
            }
        }
        return null;
    }
}
