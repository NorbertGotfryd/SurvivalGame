using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

public enum AiType
{
    Passive,
    Scared,
    Aggresive
}

public enum AiState
{
    Idle,
    Wandering,
    Attacking,
    Fleeing
}
public class NPC : MonoBehaviour, IDamagable
{
    public NPCData data;

    [Header("Stats")]
    public int maxHealth;
    public float walkSpeed;
    public float runSpeed;
    public ItemData[] dropOnDeath;

    [Header("AI")]
    public AiType aiType;
    public AiState aiState;
    public float detectDistance;
    public float safeDistance;

    [Header("Wandering")]
    public float minWanderDistance;
    public float maxWanderDistance;
    public float minWanderWaitTime;
    public float maxWanderWaitTime;

    [Header("Combat")]
    public int damage;
    public float attackrate;
    public float attackDistance;

    private float lastAttackTime;
    private float playerDistance;

    //components
    [HideInInspector]
    public NavMeshAgent agent;

    private Animator anim;
    private SkinnedMeshRenderer[] meshRenderers;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();

        meshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();
    }

    private void Start()
    {
        SetState(AiState.Wandering);
    }

    private void Update()
    {
        //get the player distance
        playerDistance = Vector3.Distance(transform.position, PlayerController.instance.transform.position);

        anim.SetBool("Moving", aiState != AiState.Idle);

        switch (aiState)
        {
            case AiState.Idle:
                PassiveUpdate();
                break;
            case AiState.Wandering:
                PassiveUpdate();
                break;
            case AiState.Attacking:
                AttackingUpdate();
                break;
            case AiState.Fleeing:
                FleeingUpdate();
                break;
            default:
                break;
        }
    }

    private void PassiveUpdate()
    {
        if (aiState == AiState.Wandering && agent.remainingDistance < 0.1f)
        {
            SetState(AiState.Idle);
            Invoke("WanderToNewLocation", Random.Range(minWanderWaitTime, maxWanderWaitTime));
        }

        if(aiType == AiType.Aggresive && playerDistance < detectDistance)
            SetState(AiState.Attacking);
        else if (aiType == AiType.Scared && playerDistance < detectDistance)
        {
            SetState(AiState.Fleeing);
            agent.SetDestination(GetFleeLocation());
        }
    }

    private void AttackingUpdate()
    {
        if(playerDistance > attackDistance)
        {
            agent.isStopped = false;
            agent.SetDestination(PlayerController.instance.transform.position);
        }
        else
        {
            agent.isStopped = true;

            if(Time.time - lastAttackTime > lastAttackTime)
            {
                lastAttackTime = Time.time;
                PlayerController.instance.GetComponent<IDamagable>().TakePhysicalDamage(damage);
                anim.SetTrigger("Attack");
            }
        }
    }

    private void FleeingUpdate()
    {
        if(playerDistance < safeDistance && agent.remainingDistance < 0.1f)
            agent.SetDestination(GetFleeLocation()); 
        else if(playerDistance > safeDistance)
            SetState(AiState.Wandering);
    }

    private void SetState(AiState newState)
    {
        aiState = newState;

        switch (aiState)
        {
            case AiState.Idle:
                {
                    agent.speed = walkSpeed;
                    agent.isStopped = true;
                    break;
                }
            case AiState.Wandering:
                {
                    agent.speed = walkSpeed;
                    agent.isStopped = false;
                    break;
                }
            case AiState.Attacking:
                {
                    agent.speed = runSpeed;
                    agent.isStopped = false;
                    break;
                }
            case AiState.Fleeing:
                {
                    agent.speed = runSpeed;
                    agent.isStopped = false;
                    break;
                }
            default:
                break;
        }
    }

    private void WanderToNewLocation()
    {
        if (aiState != AiState.Idle)
            return;

        SetState(AiState.Wandering);
        agent.SetDestination(GetWanderLocation());
    }

    private Vector3 GetWanderLocation()
    {
        NavMeshHit hit;
        NavMesh.SamplePosition(transform.position + (Random.onUnitSphere * Random.Range(minWanderDistance, maxWanderDistance)), out hit ,maxWanderDistance, NavMesh.AllAreas);

        int i = 0;

        while (Vector3.Distance(transform.position, hit.position) < detectDistance)
        {
            NavMesh.SamplePosition(transform.position + (Random.onUnitSphere * Random.Range(minWanderDistance, maxWanderDistance)), out hit, maxWanderDistance, NavMesh.AllAreas);


            i++;

            if (i == 30)
                break;
        }

        return hit.position;
    }

    private Vector3 GetFleeLocation()
    {
        NavMeshHit hit;
        NavMesh.SamplePosition(transform.position + (Random.onUnitSphere * safeDistance), out hit, safeDistance, NavMesh.AllAreas);

        int i = 0;

        while(GetDestinationAngle(hit.position) > 90 || playerDistance < safeDistance)
        {
            NavMesh.SamplePosition(transform.position + (Random.onUnitSphere * safeDistance), out hit, safeDistance, NavMesh.AllAreas);
            
            i++;

            if (i == 30)
                break;
        }

        return hit.position;
    }

    private float GetDestinationAngle(Vector3 targetPosition)
    {
        return Vector3.Angle(transform.position - PlayerController.instance.transform.position, transform.position + targetPosition);
    }

    public void TakePhysicalDamage(int damageAmount)
    {
        maxHealth -= damageAmount;

        if (maxHealth <= 0)
            Die();

        StartCoroutine(DamageFlash());

        if (aiType == AiType.Passive)
            SetState(AiState.Fleeing);
    }

    private void Die()
    {
        for (int x = 0; x < dropOnDeath.Length; x++)
        {
            Instantiate(dropOnDeath[x].dropPrefab, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }

    private IEnumerator DamageFlash()
    {
        for (int i = 0; i < meshRenderers.Length; i++)
            meshRenderers[i].material.color = new Color(1.0f, 0.6f, 0.6f);

        yield return new WaitForSeconds(0.1f);

        for (int i = 0; i < meshRenderers.Length; i++)
            meshRenderers[i].material.color = Color.white;
    }
}
