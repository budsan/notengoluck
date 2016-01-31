using UnityEngine;
using System.Collections;

public class CatMovement : MonoBehaviour
{
    public float speed = 2.0F;
    public float gravity = 20.0F;
    public float movingProbability = .75f;
    private Vector3 moveDirection = Vector3.zero;

	Animator anim;	
	CharacterController catController;

    float counter = 0;

	void Start ()
	{
        anim = GetComponent<Animator>();
        catController = GetComponent<CharacterController>();

        counter = Random.Range(1f, 4f);
        bool isMoving =  (Random.Range(0f, 1f) < movingProbability);

        if (isMoving)
        {
            moveDirection = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));
            transform.forward = moveDirection.normalized;
        }
        else moveDirection = Vector3.zero;
        moveDirection *= speed;

        anim.SetBool("isMoving", isMoving);


    }

    void Update()
    {
		if (catController.isGrounded) {
            counter -= Time.deltaTime;
            if(counter <= 0)
            {
                counter = Random.Range(1f, 5f);
                bool isMoving = (Random.Range(0f, 1f) < movingProbability);

                if (isMoving)
                {
                    moveDirection = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));
                    transform.forward = moveDirection.normalized;
                }
                else moveDirection = Vector3.zero;
                moveDirection *= speed;

                anim.SetBool("isMoving", isMoving);
            }            
		}
		moveDirection.y -= gravity * Time.deltaTime;
        catController.Move (moveDirection * Time.deltaTime);			
    }

    public float pushPower = 2.0F;
    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Rigidbody body = hit.collider.attachedRigidbody;
        if (body == null || body.isKinematic)
            return;

        if (hit.moveDirection.y < -0.3F)
            return;

        Vector3 pushDir = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z);
        body.velocity = pushDir * pushPower;
    }
}