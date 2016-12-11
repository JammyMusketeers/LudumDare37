using UnityEngine;
using System;
using System.Collections;

public class PlayerControl : MonoBehaviour
{

	public float walkSpeed = 0.15f;
	public float runSpeed = 1.0f;
	public float sprintSpeed = 2.0f;
	public float stormSpeedDecay;

	public float turnSmoothing = 3.0f;
	public float aimTurnSmoothing = 15.0f;
	public float speedDampTime = 0.1f;

	public float jumpHeight = 5.0f;
	public float jumpCooldown = 1.0f;

	private float timeToNextJump = 0;
	
	private float speed;

	private Action _tweenToCallback;
	private Vector3 lastDirection;

	private Animator anim;
	private int speedFloat;
	private int jumpBool;
	private int hFloat;
	private int vFloat;
	private int aimBool;
	private int groundedBool;
	private Transform cameraTransform;

	private bool _isJumpingOnToTram;
	private bool _hasStartedJumpAnim;
	private Vector3 _startJumpTramPos;
	private Transform _tweenTo;

	private Player _player;

	private float h;
	private float v;

	private bool run;
	private bool sprint;

	private bool isMoving;

	private float distToGround;
	private float sprintFactor;

	void Awake()
	{
		anim = GetComponent<Animator> ();
		cameraTransform = Camera.main.transform;

		speedFloat = Animator.StringToHash("Speed");
		jumpBool = Animator.StringToHash("Jump");
		hFloat = Animator.StringToHash("H");
		vFloat = Animator.StringToHash("V");
		aimBool = Animator.StringToHash("Aim");
		groundedBool = Animator.StringToHash("Grounded");
		distToGround = GetComponent<Collider>().bounds.extents.y;
		sprintFactor = sprintSpeed / runSpeed;

		_player = GetComponent<Player>();
	}

	bool IsGrounded()
	{
		return Physics.Raycast(transform.position, -Vector3.up, distToGround + 0.1f);
	}

	void Update()
	{
		h = Input.GetAxis("Horizontal");
		v = Input.GetAxis("Vertical");
		isMoving = Mathf.Abs(h) > 0.1 || Mathf.Abs(v) > 0.1;

		if (_isJumpingOnToTram)
		{	
			if (!_hasStartedJumpAnim)
			{
				if (IsAnimationName("IdleJump") || IsAnimationName("LocomotionJump"))
				{
					_hasStartedJumpAnim = true;
				}
				else
				{
					return;
				}
			}

			var animationProgress = GetCurrentAnimationProgress();

			if (animationProgress >= 0.9f)
			{
				GetComponent<Rigidbody>().isKinematic = false;

				transform.position = _tweenTo.position;

				if (_tweenToCallback != null)
				{
					_tweenToCallback();
				}

				SetJumpAnim(false);

				_isJumpingOnToTram = false;
				_tweenTo = null;

				return;
			}

			var worldPosTarget = GameManager.Instance.CurrentTram.transform.TransformPoint(_startJumpTramPos);

			transform.position = Vector3.Slerp(worldPosTarget, _tweenTo.position, animationProgress);

			var currentRotation = transform.eulerAngles;

			transform.LookAt(_tweenTo);

			var newRotation = transform.eulerAngles;

			newRotation.x = 0f;
			newRotation.z = 0f;
			
			transform.eulerAngles = Vector3.Lerp(currentRotation, newRotation, animationProgress);

			SetPosition(transform.position);
			SetRotation(transform.rotation);
		}
	}

	void FixedUpdate()
	{
		if (!_isJumpingOnToTram)
		{
			anim.SetFloat(hFloat, h);
			anim.SetFloat(vFloat, v);
		
			GetComponent<Rigidbody>().useGravity = true;
			anim.SetBool(groundedBool, IsGrounded ());

			MovementManagement (h, v, run, sprint);

			if (!_player.IsInsideTram() && !_player.CanEnterTram())
			{
				JumpManagement();
			}
		}
	}

	public void JumpOnToTram(Tram tram, Action callback)
	{
		_tweenToCallback = callback;
		_hasStartedJumpAnim = false;
		_startJumpTramPos = tram.transform.InverseTransformPoint(transform.position);
		_isJumpingOnToTram = true;
		_tweenTo = tram.insideSpawn;

		transform.parent = tram.transform;

		SetJumpAnim(true);
	}

	public void SetJumpAnim(bool isActive)
	{
		anim.SetBool(jumpBool, isActive);
	}

	void JumpManagement()
	{
		if (GetComponent<Rigidbody>().velocity.y < 10) // already jumped
		{
			anim.SetBool (jumpBool, false);
			if(timeToNextJump > 0)
				timeToNextJump -= Time.deltaTime;
		}

		if (Input.GetButtonDown ("Jump"))
		{
			anim.SetBool(jumpBool, true);
			if(speed > 0 && timeToNextJump <= 0 )
			{
				GetComponent<Rigidbody>().velocity = new Vector3(0, jumpHeight, 0);
				timeToNextJump = jumpCooldown;
			}
		}
	}

	void MovementManagement(float horizontal, float vertical, bool running, bool sprinting)
	{
		Rotating(horizontal, vertical);

		if(isMoving)
		{
			if(sprinting)
			{
				speed = sprintSpeed - stormSpeedDecay;
			}
			else if (running)
			{
				speed = runSpeed - stormSpeedDecay;
			}
			else
			{
				speed = walkSpeed - stormSpeedDecay;
			}

			anim.SetFloat(speedFloat, speed, speedDampTime, Time.deltaTime);
		}
		else
		{
			speed = 0f;
			anim.SetFloat(speedFloat, 0f);
		}

		GetComponent<Rigidbody>().AddForce(Vector3.forward*speed);
	}

	Vector3 Rotating(float horizontal, float vertical)
	{
		Vector3 forward = cameraTransform.TransformDirection(Vector3.forward);

		forward.y = 0.0f;
		forward = forward.normalized;

		Vector3 right = new Vector3(forward.z, 0, -forward.x);

		Vector3 targetDirection;

		float finalTurnSmoothing;

		targetDirection = forward * vertical + right * horizontal;
		finalTurnSmoothing = turnSmoothing;


		if((isMoving && targetDirection != Vector3.zero))
		{
			Quaternion targetRotation = Quaternion.LookRotation (targetDirection, Vector3.up);
			Quaternion newRotation = Quaternion.Slerp(GetComponent<Rigidbody>().rotation, targetRotation, finalTurnSmoothing * Time.deltaTime);
			
			GetComponent<Rigidbody>().MoveRotation (newRotation);
			lastDirection = targetDirection;
		}

		if(!(Mathf.Abs(h) > 0.9 || Mathf.Abs(v) > 0.9))
		{
			Repositioning();
		}

		return targetDirection;
	}

	public void SetPosition(Vector3 position)
	{
		GetComponent<Rigidbody>().MovePosition(position);
	}

	public void SetRotation(Quaternion rotation)
	{
		GetComponent<Rigidbody>().MoveRotation(rotation);
	}

	private void Repositioning()
	{
		Vector3 repositioning = lastDirection;
		if(repositioning != Vector3.zero)
		{
			repositioning.y = 0;
			Quaternion targetRotation = Quaternion.LookRotation (repositioning, Vector3.up);
			Quaternion newRotation = Quaternion.Slerp(GetComponent<Rigidbody>().rotation, targetRotation, turnSmoothing * Time.deltaTime);
			GetComponent<Rigidbody>().MoveRotation (newRotation);
		}
	}

	public float GetCurrentAnimationProgress()
	{
		 var currentState = anim.GetCurrentAnimatorStateInfo(0);
     
		 return currentState.normalizedTime % 1;
	}

	public bool IsAnimationName(string name)
	{
		 var currentState = anim.GetCurrentAnimatorStateInfo(0);
     
		 return currentState.IsName(name);
	}

	public bool isSprinting()
	{
		return sprint && (isMoving);
	}
}
