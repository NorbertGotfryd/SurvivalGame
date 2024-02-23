using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class PlayerNeeds : MonoBehaviour, IDamagable
{
    public Need health;
    public Need hunger;
    public Need thirst;
    public Need sleep;
    public UnityEvent onTakeDamage;

    public float noHungerHealthDecay;
    public float noThirstHealthDecay;

    public static PlayerNeeds instance;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        health.currentValue = health.startValue;
        hunger.currentValue = hunger.startValue;
        thirst.currentValue = thirst.startValue;
        sleep.currentValue = sleep.startValue;
    }

    private void Update()
    {
        //decay need over time
        hunger.Subtract(hunger.decayRate * Time.deltaTime);
        thirst.Subtract(thirst.decayRate * Time.deltaTime);
        sleep.Add(sleep.regenRate * Time.deltaTime);

        //decay health over time if hunger or thirst is equal 0
        if (hunger.currentValue == 0.0f)
            health.Subtract(noHungerHealthDecay * Time.deltaTime);

        if (thirst.currentValue == 0.0f)
            health.Subtract(noThirstHealthDecay * Time.deltaTime);

        //check if player is dead
        if (health.currentValue == 0.0f)
            Die();

        //update UI bat
        health.uiBar.fillAmount = health.GetPercentage();
        hunger.uiBar.fillAmount = hunger.GetPercentage();
        thirst.uiBar.fillAmount = thirst.GetPercentage();
        sleep.uiBar.fillAmount = sleep.GetPercentage();
    }

    public void Heal (float amount)
    {
        health.Add(amount);
    }

    public void Eat(float amount)
    {
        hunger.Add(amount);
    }

    public void Drink(float amount)
    {
        thirst.Add(amount);
    }

    public void Sleep(float amount)
    {
        sleep.Subtract(amount);
    }

    public void TakePhysicalDamage(int amount)
    {
        health.Subtract(amount);
        onTakeDamage?.Invoke(); //if onTakeDamage is happend then Invoke if not then do nothing
    }

    public void Die()
    {

    }
}

[System.Serializable]
public class Need
{
    public Image uiBar;
    [HideInInspector]
    public float currentValue;
    public float maxValue;
    public float startValue;
    public float regenRate;
    public float decayRate;

    public void Add (float amount)
    {
        //value is never above max
        currentValue = Mathf.Min(currentValue + amount, maxValue);
    }

    public void Subtract (float amount)
    {
        //value is never belov min
        currentValue = Mathf.Max(currentValue - amount, 0.0f);
    }

    public float GetPercentage()
    {
        return currentValue / maxValue;
    }
}

//jak dobrze rozumiem to interfejs nadaje wasciwosci, w tym przypadku daje mu moliwosc dostawania obrazen
//funkcja w interfejsie musi byc uzywana w obiekcie pod taka sama nazwa oraz z taka sama iloscia takich samych parametrow, tutaj "TakePhysicalDamage"
//nie można przypisać interfejsu do obiektu bez dodania do niej funkcji z interfejsu, czy jakos tak
public interface IDamagable 
{
    void TakePhysicalDamage(int damageAmount);
}
