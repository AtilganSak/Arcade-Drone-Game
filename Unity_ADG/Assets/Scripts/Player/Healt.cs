using UnityEngine;
using UnityEngine.Events;

public class Healt : MonoBehaviour
{
    [SerializeField]float healt = 100;

    public UnityEvent OnDead;

    HealtUI healtUI;

    private void OnEnable()
    {
        healt = healt < 0 ? 0 : healt;

        healtUI = FindObjectOfType<HealtUI>();
    }
    private void Start()
    {
        UpdateUI();
    }
    public void Heal(float amount)
    {
        if (healt + amount < 100)
        {
            healt += amount;
        }
        else
        {
            healt = 100;
        }
        UpdateUI();
    }
    public void TakeDamage(float amount)
    {
        if (healt - amount > 0)
        {
            healt -= amount;
        }
        else
        {
            healt = 0;
        }
        UpdateUI();

        if (healt == 0)
        {
            OnDead.Invoke();
        }
    }
    void UpdateUI()
    {
        if (healtUI != null)
        {
            healtUI.SetValue(healt / 100);
        }
    }
}
