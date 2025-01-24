using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;


public enum MonsterState
{
    Idle,
    Persue,
    Attack
}

public class MonsterAI : MonoBehaviour
{
    [SerializeField] private Transform movePosition;
    [SerializeField] private int distanceToDetectPlayer = 20;
    [SerializeField] private int distanceToAttackPlayer = 1;
    [SerializeField] private float radiusForRandomPath = 20;
    [SerializeField] private float durationToWait = 10f;
    [SerializeField] private float durationToWaitBeforeAttack = 1f;
    [SerializeField] private float moveTargetDistance;
    [SerializeField] private float velocity;
    [SerializeField] private float timer = 0f;
    private MonsterState _state;


    private NavMeshAgent _navMeshAgent;
    private Animator _animator;
    private void Awake()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
        _state = MonsterState.Idle;
    }

    private void Update()
    {
        moveTargetDistance = Vector3.Distance(movePosition.position, transform.position);

        velocity = _navMeshAgent.velocity.magnitude;
        _animator.SetFloat("Distance", moveTargetDistance);
        _animator.SetFloat("Velocity", velocity);
        //_navMeshAgent.speed = ((score - minScore) /(maxScore - minScore)) * (maxValue - minValue) + minValue
        //This will calculate speed based on score from min value to max value
        switch (_state)
        {
            case MonsterState.Idle:
                //Changing state to persuing od attacking
                if (moveTargetDistance <= distanceToDetectPlayer && moveTargetDistance > distanceToAttackPlayer)
                {
                    timer = 0f;
                    _state = MonsterState.Persue;
                }
                else if (moveTargetDistance <= distanceToAttackPlayer)
                {
                    timer = 0f;
                    _state = MonsterState.Attack;
                }
                //waits before choosing random spot on navmesh
                timer += Time.deltaTime;
                if (timer >= durationToWait)
                {
                    Vector3 randomDirection = Random.insideUnitSphere * radiusForRandomPath;
                    randomDirection += transform.position;
                    NavMeshHit hit;
                    Vector3 destination = Vector3.zero;
                    if (NavMesh.SamplePosition(randomDirection, out hit, radiusForRandomPath, 1))
                    {
                        destination = hit.position;
                    }
                    _navMeshAgent.destination = destination;
                    timer = 0f;
                }
                break;

            case MonsterState.Persue:
                //Moving to player
                _navMeshAgent.destination = movePosition.position;
                //Changing state to idle and attack
                if (moveTargetDistance > distanceToDetectPlayer)
                {
                    //before changing to idle, stope the movement
                    _navMeshAgent.destination = transform.position;
                    _state = MonsterState.Idle;
                }
                else if (moveTargetDistance <= distanceToAttackPlayer)
                {
                    timer = 0f;
                    _state = MonsterState.Attack;
                }
                break;
            //Attack state
            case MonsterState.Attack:
                if (moveTargetDistance > distanceToAttackPlayer)
                {
                    timer = 0f;
                    _state = MonsterState.Idle;
                }
                //Waits before doing attack function
                timer += Time.deltaTime;
                if (timer >= durationToWaitBeforeAttack)
                {
                    //Attack code
                    timer = 0f;
                }
                break;
        }

    }

}
