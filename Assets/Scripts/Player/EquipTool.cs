using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipTool : Equip
{
    public float attackRate;
    public float attackDistance;

    private bool isAttacking;

    [Header("Resource Gahtering")]
    public bool doesGatherResources;

    [Header("Combat")]
    public bool doesDealDamage;
    public int damageAmount;

    //components
    private Animator anim;
    private Camera cam;

    private void Awake()
    {
        //get out components
        anim = GetComponent<Animator>();
        cam = Camera.main;
    }

    public override void OnAttackInput()
    {
        if (!isAttacking)
        {
            isAttacking = true;
            anim.SetTrigger("Attack");
            Invoke("OnCanAttack", attackRate); //call function in time delay
        }

        //base.OnAttackInput();
    }

    public override void OnAltAttackInput()
    {
        //base.OnAltAttackInput();
    }

    public void OnCanAttack()
    {
        isAttacking = false;
    }

    public void OnHit()
    {
        Ray ray = cam.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, attackDistance))
        {
            if (doesGatherResources && hit.collider.GetComponent<Resource>())
            {
                hit.collider.GetComponent<Resource>().Gather(hit.point, hit.normal);
            }
            if (doesDealDamage && hit.collider.GetComponent<IDamagable>() != null)
            {
                hit.collider.GetComponent<IDamagable>().TakePhysicalDamage(damageAmount);
            }
        }
    }
}
