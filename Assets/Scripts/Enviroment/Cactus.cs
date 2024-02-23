using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cactus : MonoBehaviour
{
    private List<IDamagable> thingsDoDamage = new List<IDamagable>();
    public int damage;
    public float damageRate;

    private void Start()
    {
        StartCoroutine(DealDamage());
    }

    IEnumerator DealDamage()
    {
        while (true)
        {
            for (int i = 0; i < thingsDoDamage.Count; i++)
            {
                thingsDoDamage[i].TakePhysicalDamage(damage);
            }

            yield return new WaitForSeconds(damageRate);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<IDamagable>() != null)
        {
            thingsDoDamage.Add(collision.gameObject.GetComponent<IDamagable>());
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.GetComponent<IDamagable>() != null)
        {
            thingsDoDamage.Remove(collision.gameObject.GetComponent<IDamagable>());
        }
    }
}
