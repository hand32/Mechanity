using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System;
namespace roguelike{
public class CharacterPhysics : MonoBehaviour {
	
	/*
	[Serializable]
	public class CrouchNotifyer : UnityEvent<bool> {}

	public CrouchNotifyer crouchListeners;
	void CrouchNotify()
	{
		crouchListeners.Invoke(isCrouching);
	}
	public void AddCrouchListener(UnityAction<bool> _listener) //call by value 인지 class pointer인지 잘 봐보자.
	{
		crouchListeners.AddListener(_listener);
	}
	public void RemoveCrouchListener(UnityAction<bool> _listener)
	{
		crouchListeners.RemoveListener(_listener);
	}
	*/

	public static UnityEvent DeathListener = new UnityEvent();

	public static bool stop;
	public float laydistance;
	public LayerMask platformLayerMask;
	public bool onGround;
	public bool onWall;
	public bool isCrouching;
	public bool isDashing;
	CharacterCollisionState2D collision2D;

	public bool death;
	public bool canJump;
	public int m_jumpCount;
	public float jumpSpeed; // unit/second
	public float moveSpeed; // unit/second
	public float jumpMoveSpeed;
	public bool fixMove;
	public float dashSpeed;
	public float dashTime;
	public float onWallSlidingSpeed;
	public float meleeAtackDamage;

	int currentJumpCount;
	float m_moveSpeed;


	CapsuleCollider2D m_capsuleCollider2D;
	Rigidbody2D m_rigidbody2D;
	Animator m_Animator;
	Vector2 m_colliderSize;
	Vector3 m_position;

	public DialogueSystem dialogueSystem;



	void OnEnable()
	{
		stop = false;
		m_capsuleCollider2D = GetComponent<CapsuleCollider2D>();
		m_rigidbody2D = GetComponent<Rigidbody2D>();
		m_Animator = GetComponent<Animator>();
		laydistance = m_capsuleCollider2D.size.y /2 - m_capsuleCollider2D.offset.y + 0.05f;
		m_colliderSize = m_capsuleCollider2D.size;
		m_moveSpeed = moveSpeed;

		GetComponent<StatusManagement>().getDamageListeners.AddListener(Damaged);
		dialogueSystem = FindObjectOfType<DialogueSystem>();

		Class_Weapon.meleeAttackDamage = meleeAtackDamage;

	}
	

	void Move(int inputHorizontal)
	{
		if(death || stop)
		{
			return;
		}
		m_Animator.SetBool("Walk", false);
		if(isDashing)
			return;


		//onWall Checking
		Debug.DrawRay(transform.position, Vector2.right * (m_colliderSize.x/2 + 0.05f));
		Debug.DrawRay(transform.position, Vector2.left * (m_colliderSize.x/2 + 0.05f));
		if(!onGround && m_rigidbody2D.velocity.y <0 && ((Physics2D.Raycast(transform.position, Vector2.right, m_colliderSize.x/2 + 0.05f, platformLayerMask) && inputHorizontal == 1f) ||
							(Physics2D.Raycast(transform.position, Vector2.left, m_colliderSize.x/2 + 0.05f, platformLayerMask) && inputHorizontal == -1f)))
		{
			OnWall(inputHorizontal);
		}
		else
		{
			if(onWall)
				transform.rotation = Quaternion.Euler(0f, Mathf.Round(transform.rotation.eulerAngles.y) == 0f ? 180f : 0f, 0f);
			m_Animator.SetBool("OnWall", false);
			onWall = false;
			m_rigidbody2D.gravityScale = 2f;
		}
		if(onWall)
			return;

		RotatePlayer(inputHorizontal);

		/*
		//fixMove
		if(fixMove)
		{
			if(inputHorizontal == 1f)
				transform.rotation = Quaternion.Euler(0f, 0f, 0f);

			else if(inputHorizontal == -1f)
				transform.rotation = Quaternion.Euler(0f, 180f, 0f);


			return;
		}
		*/

		//m_rigidbody2D.velocity.y != 0 || 
		if(!onGround)
		{
			m_rigidbody2D.AddForce(new Vector2(inputHorizontal * jumpMoveSpeed * Time.deltaTime, 0));
		}
		else{
			m_rigidbody2D.velocity = new Vector2(inputHorizontal * moveSpeed, m_rigidbody2D.velocity.y);
			
			if(inputHorizontal != 0)
				m_Animator.SetBool("Walk", true);
		}
	}
	
	void RotatePlayer(float _inputHorizontal)
	{
		if(death || stop)
		{
			return;
		}
		if(_inputHorizontal == 1){
			transform.rotation = Quaternion.Euler(0, 0, 0);
			
		}
		else if(_inputHorizontal == -1){
			transform.rotation = Quaternion.Euler(0, 180f, 0);
		}
		Class_Weapon weapon = GetComponentInChildren<Class_Weapon>();
		if(weapon && weapon.isFixed)
			weapon.transform.rotation = weapon.preRotate;

		
	}


	void Jump(int jumpKeyInput) // Down = 0, Held = 1, Release = 2
	{ 
		if(death || stop)
		{
			return;
		}
		if(!canJump || dialogueSystem.isShowing)
			return;
		
		switch (jumpKeyInput)
		{
			case 0: //Down  && jumpStartTime checking
				if(currentJumpCount > 0)
				{
					if(onWall)
					{
						m_rigidbody2D.AddForce(new Vector2(Mathf.Round(transform.rotation.eulerAngles.y) == 0f ? 500f : -500f, 0f));
					}
					else
					{
						if(isDashing)
							DashStop();
						if(onGround)
							m_Animator.SetBool("JumpUp", true);
					}
					m_rigidbody2D.velocity = new Vector2(m_rigidbody2D.velocity.x, jumpSpeed);
				}
				break;

			case 1: //Pushing / change rigidboy.velocity.y to jumpSpeed
				
				

					//if(jumpHeight < jumpSpeed * (Time.time - jumpStartTime)) //if less than jumpheight
					//	m_rigidbody2D.velocity = new Vector2(m_rigidbody2D.velocity.x, 0f);
					//else if(m_rigidbody2D.velocity.y > 0) //quick stop at jumpHeight
					//	m_rigidbody2D.velocity = new Vector2(m_rigidbody2D.velocity.x, 0f);
				
				break;

			case 2: //Release / stop jump before jumpHeight
				//if(m_rigidbody2D.velocity.y > 0) //quick stop
				//	m_rigidbody2D.velocity = new Vector2(m_rigidbody2D.velocity.x, 0f);
				currentJumpCount--;
				break;

			default:
				break;
		}
	}
	/*
	void Crouch(KeyState _keyState)
	{
		if(isDashing)
			return;

		if((_keyState == KeyState.Down || _keyState == KeyState.Held) && onGround)
		{
			if(!isCrouching)
			{
				isCrouching = true;
				CrouchNotify();
				// 앉는 애니메이션 재생.
				m_capsuleCollider2D.size = new Vector2(m_colliderSize.x, m_colliderSize.y / 2f);
				m_capsuleCollider2D.offset -= new Vector2(0f, m_colliderSize.y /4f);
				moveSpeed = m_moveSpeed * 0.5f;
			}

		}
		else if(isCrouching)
		{
			m_capsuleCollider2D.size = m_colliderSize;
			m_capsuleCollider2D.offset += new Vector2(0f, m_colliderSize.y /4f);
			isCrouching = false;
			moveSpeed = m_moveSpeed;
			CrouchNotify();
			//앉아 있었으면 일어서는 애니메이션 재생.
		}
	}
	*/

	void FixMove(KeyState _keyState)
	{
		fixMove = _keyState == KeyState.Held ? true : false;
	}

	void Dash()
	{
		if(death || stop)
		{
			return;
		}
		//can Stop during Dashing check DashStop.
		//if(!isCrouching)
		//	return;
		if(!onGround || isDashing || dialogueSystem.isShowing)
			return;

		isDashing = true;
		m_Animator.SetBool("Dash", true);
		SendMessage("Play", "Player_Dash");
		if(Mathf.Round(transform.rotation.eulerAngles.y) == 0f)
		{
			m_rigidbody2D.velocity = new Vector2(dashSpeed, m_rigidbody2D.velocity.y);
		}
		else
		{
			m_rigidbody2D.velocity = new Vector2(-dashSpeed, m_rigidbody2D.velocity.y);
		}
		if(m_capsuleCollider2D.size == m_colliderSize)
		{
			m_capsuleCollider2D.size = new Vector2(m_colliderSize.x, m_colliderSize.y / 2f);
			m_capsuleCollider2D.direction = CapsuleDirection2D.Horizontal;
			m_capsuleCollider2D.offset -= new Vector2(0f, m_colliderSize.y /4f);
		}
		StartCoroutine("DashDelay");
	}

	IEnumerator DashDelay()
	{
		//Debug.Log("DashStop Corutine in.");
		yield return new WaitForSeconds(dashTime);
		//Debug.Log("DashStop in Corutine");
		if(isDashing)
			DashStop();
	}

	void DashStop()
	{
		StopCoroutine("DashDelay");
		if(isDashing)
		{
			isDashing = false;
			StartCoroutine("AfterDashStandUp");
		}
	}
	IEnumerator AfterDashStandUp()
	{
		while(true)
		{
			yield return null;
			if(isDashing)
				break;
			if(m_rigidbody2D.velocity.x != 0f || m_rigidbody2D.velocity.y != 0f || PlayerController.currentInput.CheckMoveInput())
			{
				m_Animator.SetBool("Dash", false);
				m_capsuleCollider2D.offset += new Vector2(0f, m_colliderSize.y /4f);
				m_capsuleCollider2D.direction = CapsuleDirection2D.Vertical;
				m_capsuleCollider2D.size = m_colliderSize;
				break;
			}
		}
	}

	void Damaged(float _damage)
	{
		if(death || stop)
		{
			return;
		}
		DashStop();
		m_Animator.SetTrigger("GetDamage");
		SendMessage("Play", "Player_Hurt");
	}

	void Update()
	{
		if(death || stop)
		{
			m_rigidbody2D.velocity = Vector2.zero;
			return;
		}
		//onGround Checking
		Debug.DrawRay(transform.position, Vector2.down * laydistance);
		Debug.DrawRay(new Vector2(transform.position.x + m_colliderSize.x /2, transform.position.y), Vector2.down* laydistance);
		Debug.DrawRay(new Vector2(transform.position.x - m_colliderSize.x /2, transform.position.y), Vector2.down* laydistance);
		if(Physics2D.Raycast(transform.position, Vector2.down, laydistance, platformLayerMask) ||
			Physics2D.Raycast(new Vector2(transform.position.x + m_colliderSize.x /2, transform.position.y), Vector2.down, laydistance, platformLayerMask) ||
			Physics2D.Raycast(new Vector2(transform.position.x - m_colliderSize.x /2, transform.position.y), Vector2.down, laydistance, platformLayerMask))
		{
			m_Animator.SetBool("OnGround", true);
			onGround = true;
			gameObject.SendMessage("OnGround");
		}
		else
		{
			m_Animator.SetBool("OnGround", false);
			onGround = false;

		}
		
		


		if(Mathf.Abs(m_rigidbody2D.velocity.x) > moveSpeed && !isDashing && !onWall)
			m_rigidbody2D.velocity = new Vector2(m_rigidbody2D.velocity.x > 0 ? moveSpeed : -moveSpeed, m_rigidbody2D.velocity.y);
	}

	void OnGround()
	{
		/*
		if(fixMove)
			m_rigidbody2D.velocity = new Vector2(0f, m_rigidbody2D.velocity.y);
			*/
		currentJumpCount = m_jumpCount;
	}

	void OnWall(int inputHorizontal)
	{
		if(death || stop)
		{
			return;
		}
		//Debug.Log("Onwall fucntion in");
		if(!onWall)
		{
			transform.rotation = Quaternion.Euler(0f, inputHorizontal == 1f ? 180f : 0f, 0f);
		}
		m_rigidbody2D.velocity = new Vector2(0f, -onWallSlidingSpeed);
		m_Animator.SetBool("OnWall", true);
		if(isDashing)
			DashStop();
		onWall = true;
		m_rigidbody2D.gravityScale = 0f;
		currentJumpCount = m_jumpCount;
	}

	void KnockBack(int vectorX = 1)
	{
		float force = 300f;
		if(gameObject.transform.rotation.y == 0)
		{
			m_rigidbody2D.AddForce(new Vector2(-vectorX, 0.8f).normalized * force);
		}
		else
		{
			m_rigidbody2D.AddForce(new Vector2(vectorX, 0.8f).normalized * force);
		}
	}

	void Death()
	{
		m_Animator.SetBool("Death", true);
		death = true;
		m_rigidbody2D.simulated = false;
		DeathListener.Invoke();
	}

	
	void SetParameterFalse(string _parameter)
	{
		m_Animator.SetBool(_parameter, false);
	}
	void SetParameterTrue(string _parameter)
	{
		m_Animator.SetBool(_parameter, true);
	}
	

}
}