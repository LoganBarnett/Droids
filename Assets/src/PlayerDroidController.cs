using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
public class PlayerDroidController : MonoBehaviour {
	public float gravity = 10.0f;
	public float movementSpeed = 5.0f;
	public float jumpTime = 0.75f;
	public float jumpSpeed = 15.0f;
	public float minimumJumpTime = 0.25f;
	public float fallOffTime = 0.25f;
	
	CharacterController controller;
	float currentHorizontalMovement;
	bool useNetworkInput;
	
	float jumpTimeEnd;
	float minimumJumpTimeEnd;
	
	bool isJumpReady;
	bool IsJumpReady {
		get {
			isJumpReady = isJumpReady || controller.isGrounded;
			return isJumpReady;
		}
		set { isJumpReady = value; }
	}
	
	bool IsPlayerJumping {
		get { return Input.GetButton("Jump") || minimumJumpTimeEnd > Time.time; }	
	}
	
	void Start() {
		controller = GetComponent<CharacterController>();
	}
	
	void Update() {
		var y = -gravity;
		if (Input.GetButtonDown("Jump") && IsJumpReady) {
			jumpTimeEnd = jumpTime + Time.time;
			minimumJumpTimeEnd = minimumJumpTime + Time.time;
		}
		
		if (IsPlayerJumping && IsJumpReady) {
			if (jumpTimeEnd > Time.time)
			{
				y += jumpSpeed;
			}
			else
			{
				if (fallOffTime + jumpTimeEnd > Time.time) {
					var fallOffPercent = 1.0f - (Time.time - jumpTimeEnd) / (fallOffTime);
					var fallOffSpeed = jumpSpeed * fallOffPercent;
					
					if (fallOffSpeed < 0.0f) {
						fallOffSpeed = 0.0f;
						IsJumpReady = false;
					}
					y += fallOffSpeed;
				}
				else {
					IsJumpReady = false;
					Debug.Log("Turning jump ready off");
				}
			}
		}
		else {
			IsJumpReady = false;
		}
		
		var desiredMoveDirection = Input.GetAxis("Horizontal"); // : currentHorizontalMovement;
		var x = useNetworkInput ? currentHorizontalMovement : (desiredMoveDirection * movementSpeed);
		
		controller.Move(new Vector3(x, y, 0.0f) * Time.deltaTime);
		
		currentHorizontalMovement = x;
	}
	
	
	void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info) {
		if (stream.isWriting) {
			stream.Serialize(ref movementSpeed);
			stream.Serialize(ref currentHorizontalMovement);
		}
		else {
			stream.Serialize(ref movementSpeed);
			stream.Serialize(ref currentHorizontalMovement);
			useNetworkInput = true;
		}
	}
}