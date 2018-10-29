using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShooting : MonoBehaviour {
    [SerializeField] private float m_ShotRange;
    [SerializeField] private float m_OriginalFireRate;
    [SerializeField] private Rigidbody m_Projectile;
    [SerializeField] private float m_InitialForce;
    [SerializeField] private Transform m_PlayerTransform;
    private Transform m_Origin;
    private Ray ray;
    private RaycastHit hit;
    private Vector3 m_Direction;
    private float m_FireRate;
    private string m_EnemyName;
    [SerializeField] private EnemyAnimations m_EnemyAnimations;
    // Use this for initialization
    void Start () {
        m_Origin = GetComponent<Transform>();
        m_EnemyName = transform.parent.name;
        m_EnemyName = m_EnemyName.Replace(@"_", " ");
    }
	
	// Update is called once per frame
	void Update () {
        if (m_EnemyAnimations.m_IsDead == false || m_EnemyAnimations == null)
        {
            m_Direction = (m_PlayerTransform.position - m_Origin.position).normalized;
            m_FireRate -= Time.deltaTime;
            if (Physics.Raycast(m_Origin.position, m_Direction, out hit, m_ShotRange))
            {
                if (hit.transform.CompareTag("Player") && m_FireRate <= 0)
                {
                    Debug.Log("aoowfjaklsdjflk");
                    m_EnemyAnimations.SetAnimation(AnimationState.ShootTwoHanded);
                    Rigidbody enemyProjectile = Instantiate(m_Projectile, m_Origin.position, m_Origin.rotation);
                    enemyProjectile.AddForce(m_Origin.forward * m_InitialForce, ForceMode.Impulse);
                    enemyProjectile.GetComponent<Bullets>().m_Owner = m_EnemyName;
                    m_FireRate = m_OriginalFireRate;
                }
                else
                {
                    m_EnemyAnimations.SetAnimation(AnimationState.MoveTwoHanded);
                }
            }
        }
    }
}
