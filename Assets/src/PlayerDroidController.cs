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
	public float modelTurnRate = 4.0f;
	public GameObject model;
	public GameObject jumpThrusterEmitterContainer;
	public GameObject jumpThrusterSound;
	
	CharacterController controller;
	float currentHorizontalMovement;
	float currentVerticalMovement;
	bool useNetworkInput;
	bool facingRight = true;
	Quaternion faceLeft;
	Quaternion faceRight;
	
	float jumpTimeEnd;
	float minimumJumpTimeEnd;
	bool jumpReleased;
	ParticleEmitter[] jumpThrusterEmitters;
	AudioSource jumpThrusterSoundSource;
	
	bool IsJumpReady { get { return controller.isGrounded; } }
	
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
	
	void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info) {
		if (stream.isWriting) {
			var position = transform.position;
			var rotation = transform.rotation;
			
			stream.Serialize(ref movementSpeed);
			stream.Serialize(ref currentHorizontalMovement);
			stream.Serialize(ref currentVerticalMovement);
			stream.Serialize(ref position);
			stream.Serialize(ref rotation);
		}
		else {
			Vector3 position = Vector3.zero;
			Quaternion rotation = Quaternion.identity;
			
			stream.Serialize(ref movementSpeed);
			stream.Serialize(ref currentHorizontalMovement);
			stream.Serialize(ref currentVerticalMovement);
			stream.Serialize(ref position);
			stream.Serialize(ref rotation);
			
			transform.position = position;
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
		
		if (facingChanged) {
			StopCoroutine("FaceDirection");
			StartCoroutine(FaceDirection(facingRight, Time.time));
		}
	}
	
	IEnumerator FaceDirection(bool desireFaceRight, float time) {
		var desiredFacing = desireFaceRight ? faceRight : faceLeft;
		var oppositeFacing = desireFaceRight ? faceLeft : faceRight;
		
		var i = model.transform.rotation.eulerAngles.y / 180.0f;
		if (desireFaceRight) i = 1.0f - i;
		
	    var rate = modelTurnRate;
	    while (i < 1.0f) {
			if (facingRight != desireFaceRight) yield break;
	        i += Time.deltaTime * rate;
	        model.transform.rotation = Quaternion.Lerp(oppositeFacing, desiredFacing, i);
	        if (i < 1.0f) yield return i;
			else yield break;
	    }
	}
	
	void ShowJumpThrusters() {
		foreach(var emitter in jumpThrusterEmitters) emitter.emit = IsPlayerJumping && jumpTimeEnd > Time.time;
	}
	
	void PlayJumpThrusterSound() {
		if (IsPlayerJumping && jumpTimeEnd > Time.time) {
			if (!jumpThrusterSoundSource.isPlaying) jumpThrusterSoundSource.Play();
		} else {
			jumpThrusterSoundSource.Stop();
		}
	}
}