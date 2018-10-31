using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyMovement : MonoBehaviour {
    [SerializeField] private List<Transform> m_WayPoints;
    [SerializeField] private Transform m_PlayerTransform;
    [SerializeField] [Range(0f, 360f)] private float m_FOVAngle;
    [SerializeField] [Range(0f, 50f)] private float m_SightRadius;
    

    private Transform m_GuardTransform;
    private int m_CurrentWaypoint;
    private Ray ray;
    private RaycastHit hit;
    private Vector3 m_DirectionToPlayer;
    private Vector3 m_DirectionToWaypoint;
    private Vector3 m_WayPointDirection;
    private NavMeshAgent m_NavMesh;
    private EnemyAnimations m_EnemyAnimations;

    // Use this for initialization
    void Start() {
        m_NavMesh = GetComponent<NavMeshAgent>();
        m_GuardTransform = GetComponent<Transform>();
        m_EnemyAnimations = GetComponent<EnemyAnimations>();
        m_CurrentWaypoint = Random.Range(0, m_WayPoints.Count);
        m_NavMesh.SetDestination(m_WayPoints[m_CurrentWaypoint].position);
    }
	


	// Update is called once per frame
	void Update () {
        if (m_EnemyAnimations.m_IsDead == false || m_EnemyAnimations == null)
        {
            m_DirectionToPlayer = (m_PlayerTransform.position - m_GuardTransform.position).normalized;
            m_DirectionToWaypoint = (m_WayPoints[m_CurrentWaypoint].position - m_GuardTransform.position).normalized;

            Physics.Raycast(m_GuardTransform.position, m_DirectionToWaypoint, out hit);
            Debug.DrawLine(m_GuardTransform.position, m_WayPoints[m_CurrentWaypoint].position, Color.grey);
            if (hit.distance <= 1)
            {
                m_CurrentWaypoint = Random.Range(0, m_WayPoints.Count);
                m_NavMesh.SetDestination(m_WayPoints[m_CurrentWaypoint].position);
            }
            if (Physics.Raycast(m_GuardTransform.position, m_DirectionToPlayer, out hit, m_SightRadius))
            {
                Debug.Log(hit.transform.tag);
                if (hit.transform.CompareTag("Player"))
                {
                    Debug.DrawLine(m_GuardTransform.position, m_PlayerTransform.position, Color.red);
                }
                else if (hit.transform.tag != "Player")
                {
                    Debug.DrawLine(m_GuardTransform.position, m_PlayerTransform.position, Color.grey);
                }

                if (hit.transform.CompareTag("Player"))
                {
                    m_NavMesh.SetDestination(m_PlayerTransform.position);
                }
                else
                {
                    m_NavMesh.SetDestination(m_WayPoints[m_CurrentWaypoint].position);
                }
            }
            else
            {
                Debug.DrawLine(m_GuardTransform.position, m_PlayerTransform.position, Color.green);
            }
            
        }
    }
}
