using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RogueLike {
	public class PlayerMovement : MonoBehaviour {

		[SerializeField] private float m_Speed;

		private Transform m_PlayerTransform;
		private Rigidbody m_PlayerRigidbody;
		private Vector3 m_Velocity = Vector3.zero;

		// Use this for initialization
		void Start () {
			m_PlayerRigidbody = GetComponent<Rigidbody>();
			m_PlayerTransform = GetComponent<Transform>();
		}
	
		// Update is called once per frame
		void Update () {
			float xMovement = Input.GetAxisRaw("Horizontal");
			float zMovement = Input.GetAxisRaw("Vertical");
			float yMovement = Input.GetAxisRaw("Jump");

			Vector3 moveHorizontal = transform.right * xMovement;
			Vector3 moveVertical = transform.forward * zMovement;
			Vector3 moveUp = transform.up * yMovement;

			m_Velocity = (moveHorizontal + moveUp + moveVertical);
		}
		private void FixedUpdate()
		{
			if(m_Velocity != Vector3.zero)
			{
				m_PlayerRigidbody.MovePosition(m_PlayerRigidbody.position + m_Velocity * (m_Speed * Time.deltaTime));
			}
		}
	}
}
