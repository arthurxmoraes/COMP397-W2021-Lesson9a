using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public enum CryptoState
{
    IDLE,
    RUN,
    JUMP,
    ATTACK
}

public class CryptoBehaviour : MonoBehaviour
{
    [Header("Line of sight")]
    public bool hasLOS;
    public GameObject player;

    private NavMeshAgent agent;
    private Animator animator;

    [Header("Attack")]
    public float attackDistance;
    public PlayerBehaviour playerBehaviour;
    public float damageDelay = 1.0f;
    public bool isAttacking = false;
    public float attackForce = 0.01f;
    public float distanceToPlayer;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        playerBehaviour = FindObjectOfType<PlayerBehaviour>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
        if (hasLOS)
        {
            agent.SetDestination(player.transform.position);
            distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
        }

        if (hasLOS && distanceToPlayer < attackDistance && !isAttacking)
        {
            //could be attack
            animator.SetInteger("AnimState", (int)CryptoState.ATTACK);
            transform.LookAt(transform.position - player.transform.forward);

            DoDamage();
            isAttacking = true;

            if (agent.isOnOffMeshLink)
            {
                animator.SetInteger("AnimState", (int)CryptoState.JUMP);
            }

        }
        else if (hasLOS && distanceToPlayer > attackDistance)
        {
            animator.SetInteger("AnimState", (int)CryptoState.RUN);
            isAttacking = false;
        }
        else
        {
            animator.SetInteger("AnimState", (int)CryptoState.IDLE);
        }    
    }

    void OnTriggerEnter(Collider other)
    {
        //Debug.Log(other.gameObject.name);

        if (other.gameObject.CompareTag("Player"))
        {
            hasLOS = true;
            player = other.transform.gameObject;
        }        
    }

    private void DoDamage()
    {
        playerBehaviour.TakeDamage(20);
        StartCoroutine(AttackBack());
    }

    private IEnumerator AttackBack()
    {
        yield return new WaitForSeconds(0.6f);
        
        var direction = Vector3.Normalize(player.transform.position - transform.position);
        playerBehaviour.controller.SimpleMove(direction * attackForce);
        StopCoroutine(AttackBack());
    }
}
