using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerState
{
	Idle,
	Attacking
}

public class PlayerController : MonoBehaviour {
	public PlayerState m_PlayerState;

	[SerializeField] private Animator m_PlayerAnimator;

	private void Start()
	{
	}


	private void Update()
	{
		m_PlayerState = PlayerState.Idle;
		if (Input.GetMouseButton(0))
		{
			m_PlayerState = PlayerState.Attacking;
		}

		m_PlayerAnimator.SetInteger("PlayerState", (int)m_PlayerState);
	}

	public void SetAnimation(PlayerState playerState)
	{
		m_PlayerState = playerState;
	}
}
