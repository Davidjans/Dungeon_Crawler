using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance;
    public bool IsPaused;

    private List<ParticleSystem> m_ParticleSystems;
    private List<Rigidbody> m_Rigidbodies;
    private List<RigidbodyData> m_RigidbodyData;

    private void Awake()
    {
        Instance = this;

        m_ParticleSystems = new List<ParticleSystem>();
        m_Rigidbodies = new List<Rigidbody>();
        m_RigidbodyData = new List<RigidbodyData>();
    }
                                                          

    public void Add(Rigidbody _rigidbody)
    {
        m_Rigidbodies.Add(_rigidbody);
    }
    public void Add(ParticleSystem _ps)
    {
    }

    public void Pause()
    {
        for (int r = 0; r < m_Rigidbodies.Count; r++)
        {
            // Store backup data
            RigidbodyData _data = new RigidbodyData()
            {
                InstanceID = m_Rigidbodies[r].GetInstanceID(),
                Velocity = m_Rigidbodies[r].velocity,
                Drag = m_Rigidbodies[r].drag,
                AngularDrag = m_Rigidbodies[r].angularDrag
            };
            m_RigidbodyData.Add(_data);

            // Reset values
            m_Rigidbodies[r].velocity = Vector3.zero;
            m_Rigidbodies[r].drag = 0;
            m_Rigidbodies[r].angularDrag = 0;

            // Paused
            m_Rigidbodies[r].isKinematic = true;
            m_Rigidbodies[r].Sleep();
        }

        IsPaused = true;
    }

    public void Resume()
    {
        for (int r = 0; r < m_Rigidbodies.Count; r++)     
        {
            // Fetch data
            RigidbodyData _data = m_RigidbodyData.Find(i => i.InstanceID == m_Rigidbodies[r].GetInstanceID());

            // Set data back
            m_Rigidbodies[r].velocity = _data.Velocity;
            m_Rigidbodies[r].drag = _data.Drag;
            m_Rigidbodies[r].angularDrag = _data.AngularDrag;

            // Resume physics
            m_Rigidbodies[r].isKinematic = false;
            m_Rigidbodies[r].WakeUp();
        }

        m_RigidbodyData.Clear();
        IsPaused = false;
    }

    public struct RigidbodyData
    {
        public int InstanceID;
        public Vector3 Velocity;
        public float Drag;
        public float AngularDrag;
    }
}
