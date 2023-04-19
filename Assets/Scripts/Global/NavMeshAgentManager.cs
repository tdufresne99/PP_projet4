using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class NavMeshAgentManager : MonoBehaviour
{
    [SerializeField] private NavMeshAgent _agent;
    [SerializeField] private Transform _destination;

    void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
    }

    public void ChangeDestination(Vector3 newDestination)
    {
        _agent.destination = newDestination;
    }
    public void ChangeDestination()
    {
        _agent.destination = _destination.position;
    }

    public void ChangeAgentSpeed(float newSpeed)
    {
        _agent.speed = newSpeed;
    }

    public void ToggleNavMeshAgent(bool enable)
    {
        _agent.enabled = enable;
    }

    public NavMeshAgent agent => _agent;
}
