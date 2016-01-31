using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
	const float UNCONSCIENT_TIME = 1f;

    public float speed = 6.0F;
    public float jumpSpeed = 8.0F;
    public float gravity = 20.0F;
    private Vector3 moveDirection = Vector3.zero;

	public Animator anim;
	public LuckHolder luck;

	public Transform ragdollparent;
	public Rigidbody ragdollChest;
	Rigidbody[] ragdollBodies;
	Collider[] ragdollColliders;
	float ragdollTime = 0f;

	Collider playerCollider;
	public CharacterController playerController;
	public ObjectInteract objectGrabber;

	public bool isRagdoll = false;
	AnimatorOverrideController overrideController;

	bool needAnimReinitialize = false;

	public bool isOnFire = false;

	float fireCounter = 3f;

    int playerId;

    public int getPlayerId()
    {
        return playerId;
    }

	public void setPlayerId(int value)
	{
		playerId = value;
		SkinnedMeshRenderer renderer = GetComponentInChildren<SkinnedMeshRenderer>();

		Color[] colors = new[]
		{
			Color.red,
			Color.yellow,
			Color.green,
			Color.red
		};

		renderer.material.color = colors[playerId-1];
	}

    void Start ()
	{
        playerCollider = GetComponent<Collider> ();

		ragdollBodies = ragdollparent.GetComponentsInChildren<Rigidbody> ();
		ragdollColliders = ragdollparent.GetComponentsInChildren<Collider> ();
		overrideController = new AnimatorOverrideController ();
		overrideController.runtimeAnimatorController = anim.runtimeAnimatorController;

		DisableRagdoll ();

		setPlayerId (1);
	}

    void Update()
    {
		if (isOnFire) {
			fireCounter -= Time.deltaTime;
			if (fireCounter <= 0) {
				fireCounter += 2f + Random.Range (0f, 2.5f*(1f - luck.GetUnluckyFactor()));
				GetComponent<UnluckyDeath> ().deathChance (.2f);
			}
		}
		if (needAnimReinitialize) {
			anim.Play ("IdleFromRagdoll");
			needAnimReinitialize = false;
			anim.SetBool ("dead", false);
		}

		if (Input.GetButtonDown ("A" + playerId.ToString())) {
			if (!isRagdoll) {
				EnableRagdoll ();
			}
		}

		if (!isRagdoll) {
			if (anim.isInitialized) {
				if (playerController.isGrounded) {
					moveDirection = new Vector3 (Input.GetAxis ("Horizontal"+playerId.ToString()), 0, Input.GetAxis ("Vertical" + playerId.ToString()));
					if (moveDirection.sqrMagnitude > float.Epsilon) {
						transform.forward = moveDirection.normalized;
						anim.SetBool ("run", true);
					} else {
						anim.SetBool ("run", false);
					}
					moveDirection *= speed;

				}
				moveDirection.y -= gravity * Time.deltaTime;
				playerController.Move (moveDirection * Time.deltaTime);
			}
		} else {
			if (ragdollTime > 0f) {
				ragdollTime -= Time.deltaTime;

				if (ragdollTime <= 0f) {
					DisableRagdoll ();
				}
			}
		}
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Rigidbody body = hit.collider.attachedRigidbody;
        if (body == null || body.isKinematic)
            return;

        if (hit.moveDirection.y < -0.3F)
            return;

		float power = Mathf.Min (3f, 10f / body.mass);

        Vector3 pushDir = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z);
		body.velocity = pushDir * power;
    }

	void DisableRagdoll() {
		foreach (Rigidbody r in ragdollBodies) {
			r.detectCollisions = false;
			r.isKinematic = true;
		}

		foreach (Collider c in ragdollColliders) {
			if (!c.isTrigger) c.enabled = false;
		}

		playerCollider.enabled = true;
		isRagdoll = false;

		AnimationClip clip = new AnimationClip();

		Transform aux = ragdollChest.transform.parent;
		ragdollChest.transform.parent = null;
		transform.position = new Vector3 (ragdollChest.transform.position.x, transform.position.y, ragdollChest.transform.position.z);
		ragdollChest.transform.parent = aux;

		BuildClip (ref clip, ragdollparent, ragdollparent.name);
		overrideController ["IdleFromRagdoll"] = clip;

		anim.runtimeAnimatorController = overrideController;
		anim.Play ("IdleFromRagdoll");
		anim.enabled = !isRagdoll;
		playerController.enabled = !isRagdoll;
		needAnimReinitialize = true;
	}

	void BuildClip(ref AnimationClip clip, Transform t, string path) {
		string newPath = path;
		for (int i = 0; i < t.childCount; ++i) {
			Transform child = t.GetChild (i);
			newPath = path + child.name;
			clip.SetCurve (newPath, typeof(Transform), "localPosition.x", BuildCurve(child.transform.localPosition.x));
			clip.SetCurve (newPath, typeof(Transform), "localPosition.y", BuildCurve(child.transform.localPosition.y));
			clip.SetCurve (newPath, typeof(Transform), "localPosition.z", BuildCurve(child.transform.localPosition.z));
			clip.SetCurve (newPath, typeof(Transform), "localRotation.x", BuildCurve(child.transform.localRotation.x));
			clip.SetCurve (newPath, typeof(Transform), "localRotation.y", BuildCurve(child.transform.localRotation.y));
			clip.SetCurve (newPath, typeof(Transform), "localRotation.z", BuildCurve(child.transform.localRotation.z));
			clip.SetCurve (newPath, typeof(Transform), "localRotation.w", BuildCurve(child.transform.localRotation.w));

			BuildClip (ref clip, child, newPath);
		}
	}

	AnimationCurve BuildCurve(float v) {
		AnimationCurve c = new AnimationCurve();
		c.AddKey(0, v);
		return c;
	}

	void EnableRagdoll() {
		foreach (Rigidbody r in ragdollBodies) {
			r.detectCollisions = true;
			r.isKinematic = false;
			r.velocity = playerController.velocity;// * Mathf.Max(0f, Mathf.Min(1f, (r.transform.position.y / 2f)));
		}

		foreach (Collider c in ragdollColliders) {
			c.enabled = true;
		}

		playerCollider.enabled = false;
		isRagdoll = true;
		anim.SetBool ("dead", true);
		anim.Play ("IdleFromRagdoll");
		anim.enabled = !isRagdoll;
		playerController.enabled = !isRagdoll;

		ragdollTime = UNCONSCIENT_TIME + Random.Range (0f, UNCONSCIENT_TIME * 3f * luck.GetUnluckyFactor ());
		objectGrabber.Throw ();
	}

	public void Fall() {
		if (!isRagdoll) luck.ShitHappened ();
		EnableRagdoll ();
		GetComponent<UnluckyDeath> ().deathChance (.01f + Random.Range(0f, 0.1f*luck.GetUnluckyFactor()));
	}

	public void SetOnFire(bool v) {
		isOnFire = v;
	}
}