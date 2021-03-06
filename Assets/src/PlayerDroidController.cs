using UnityEngine;
using System.Collections;
using System.Linq;

[RequireComponent(typeof(CharacterController))]
public class PlayerDroidController : MonoBehaviour {
	public float gravity = 10.0f;
	public float movementSpeed = 5.0f;
	public float jumpTime = 0.75f;
	public float jumpSpeed = 15.0f;
	public float minimumJumpTime = 0.25f;
	public float fallOffTime = 0.25f;
	public float modelTurnRate = 200f;
	public GameObject model;
	public GameObject jumpThrusterEmitterContainer;
	public GameObject jumpThrusterSound;
//	public float shootingRate = 1.0f;
//	public float shootThresholdAngle = 0.1f;
//	public GameObject shootPosition;
//	public GameObject shotPrefab;
//	public AudioClip shotSound;
	public float syncDistanceThreshold = 1.0f;
	
	bool useNetworkInput;
	
	// movement related
	CharacterController controller;
	float currentHorizontalMovement;
	float currentVerticalMovement;
	
	// facing related
	bool facingRight = true;
	Quaternion faceLeft;
	Quaternion faceRight;
	
	// jumping related
	float jumpTimeEnd;
	float minimumJumpTimeEnd;
	bool jumpReleased;
	bool isNetworkJumping;
	ParticleEmitter[] jumpThrusterEmitters;
	AudioSource jumpThrusterSoundSource;
	
	// shooting related
//	float timeSinceLastShot;
	
	bool IsJumpReady { get { return controller.isGrounded; } }
	
	
	/// <summary>
	/// Is the Droid jumping.
	/// When true, thrusters should fire and sound effects play
	/// </summary>
	bool IsDroidJumping {
		get {
			//return useNetworkInput ? currentVerticalMovement > 0.01f : (IsPlayerJumping && jumpTimeEnd > Time.time);
			return currentVerticalMovement > 0.01f;
		}
	}
	
	/// <summary>
	/// Is the player trying to jump, and is he validly jumping?
	/// When true, the Droid moves up (even when reaching peak of jump).
	/// </summary>
	bool IsPlayerJumping {
		get { return (Input.GetButton("Jump") && !jumpReleased) || minimumJumpTimeEnd > Time.time; }	
	}
	
	void Start() {
		faceRight = Quaternion.Euler(0.0f,   0.0f, 0.0f);
		faceLeft  = Quaternion.Euler(0.0f, 180.0f, 0.0f);
		controller = GetComponent<CharacterController>();
		jumpThrusterEmitters = jumpThrusterEmitterContainer.GetComponentsInChildren<ParticleEmitter>().Where(p => p.gameObject != jumpThrusterEmitterContainer).ToArray();
		jumpThrusterSoundSource = jumpThrusterSound.GetComponent<AudioSource>();
	}
	
	float GetVerticalMovement() {
		var y = -gravity;
		if (Input.GetButtonDown("Jump") && IsJumpReady) {
			jumpTimeEnd = jumpTime + Time.time;
			minimumJumpTimeEnd = minimumJumpTime + Time.time;
			jumpReleased = false;
		}
		
		if (Input.GetButtonUp("Jump")) jumpReleased = true;
		
		if (IsPlayerJumping) {
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
					}
					y += fallOffSpeed;
				}
			}
		}
		return y;
	}
	
	void Update() {
		if (IsConsoleClosedLocally()) {
			var y = useNetworkInput ? currentVerticalMovement : GetVerticalMovement();
			
			var desiredMoveDirection = Input.GetAxis("Horizontal");
			var x = useNetworkInput ? currentHorizontalMovement : (desiredMoveDirection * movementSpeed);
			
			controller.Move(new Vector3(x, y, 0.0f) * Time.deltaTime);
			
			FaceFromMovement(x);
			ShowJumpThrusters();
			PlayJumpThrusterSound();
			
			currentHorizontalMovement = x;
			currentVerticalMovement = y;
		}
	}
	
	void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info) {
		if (stream.isWriting) {
			var position = transform.position;
			var rotation = transform.rotation;
//			var isDroidJumping = IsDroidJumping;
			
			stream.Serialize(ref movementSpeed);
			stream.Serialize(ref currentHorizontalMovement);
			stream.Serialize(ref currentVerticalMovement);
			stream.Serialize(ref position);
			stream.Serialize(ref rotation);
			stream.Serialize(ref jumpTimeEnd);
		}
		else {
			Vector3 position = Vector3.zero;
			Quaternion rotation = Quaternion.identity;
			
			stream.Serialize(ref movementSpeed);
			stream.Serialize(ref currentHorizontalMovement);
			stream.Serialize(ref currentVerticalMovement);
			stream.Serialize(ref position);
			stream.Serialize(ref rotation);
			
			if (Vector3.Distance(position, transform.position) > syncDistanceThreshold) {
				transform.position = position;
			}
			transform.rotation = rotation;
			
			useNetworkInput = true;
		}
	}
	
	void FaceFromMovement(float x) {
		var originallyFacingRight = facingRight;
		
		if (x > 0.01f) facingRight = true;
		else if (x < -0.01f) facingRight = false;
		
		var facingChanged = originallyFacingRight != facingRight;
		
		var euler = model.transform.rotation.eulerAngles;
		if (facingRight) {
			euler.y = faceRight.eulerAngles.y;
		}
		else {
			euler.y = faceLeft.eulerAngles.y;
		}
		
		var y = model.transform.rotation.eulerAngles.y;
		
		if (facingRight && (y < -0.1f || y > 0.1f)) {
			model.transform.Rotate(new Vector3(0f, -modelTurnRate * Time.deltaTime, 0f));
			var newY = model.transform.rotation.eulerAngles.y;
			if (newY > 180f) model.transform.rotation = Quaternion.Euler(0f,0f,0f);
		} else if (!facingRight && (y < 179.9f || y > 180.1f)) {
			model.transform.Rotate(new Vector3(0f, modelTurnRate * Time.deltaTime, 0f));
			var newY = model.transform.rotation.eulerAngles.y;
			if (newY > 180f) model.transform.rotation = Quaternion.Euler(0f,180f,0f);
		}
	}
	
	void ShowJumpThrusters() {
		foreach(var emitter in jumpThrusterEmitters) emitter.emit = IsDroidJumping;
	}
	
	void PlayJumpThrusterSound() {
		if (IsDroidJumping) {
			if (!jumpThrusterSoundSource.isPlaying) jumpThrusterSoundSource.Play();
		} else {
			jumpThrusterSoundSource.Stop();
		}
	}
	
	bool IsConsoleClosedLocally()
	{
		return !(Console.IsOpen && !useNetworkInput);
	}
	
	
//	
//	bool IsValidShootingTime() {
//		return timeSinceLastShot + (1.0f / shootingRate) < Time.time; 
//	}
//	
//	bool IsValidShootingRotation() {
//		var yRotation = model.transform.rotation.eulerAngles.y;
//
//		var withinLeft =  yRotation > (180.0f - shootThresholdAngle) &&
//				          yRotation < (180.0f + shootThresholdAngle);
//		var withinRight = yRotation < (  0.0f + shootThresholdAngle) &&
//		        		  yRotation > (  0.0f - shootThresholdAngle);
//		
//		return withinLeft || withinRight;
//	}
	
}