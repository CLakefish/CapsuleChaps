using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementController : MonoBehaviour
{
    public enum MovementStates
    {
        Walking,
        Jumping,
        Diving,
        Falling,
    }

    [Header("References")]
    [SerializeField] internal Rigidbody rb;
    [SerializeField] private Transform viewPosition;
    [SerializeField] private CameraController c;

    [Header("Collision Parameters")]
    [SerializeField] private LayerMask hittableLayer;

    [Header("Movement Parameters")]
    [SerializeField] private float speed;
    [SerializeField] private Vector2 velocityChangeGround, velocityChangeAir;

    [Header("Jump Parameters")]
    [SerializeField] private float jumpForce;
    [SerializeField] private float jumpCancelSpeed;
    [SerializeField] private float jumpBufferTime, coyoteTime;

    [Header("Movement Variables")]
    [SerializeField] private float stateDuration;
    [SerializeField] private MovementStates currentState, previousState;
    private Vector3 groundedSquash;
    private Vector3 currentVelocity, currentRotation;
    private float jumpBuffer;

    private void Start()
    {
        groundedSquash = transform.localScale = new Vector3(1, 1, 1);
        c = FindObjectOfType<CameraController>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 inputs = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        Vector3 moveDir = inputs.x * inputs.x + inputs.y * inputs.y > 0 ? viewPosition.forward : new Vector3(0, 0, 0);

        bool inputting = inputs != new Vector2(0, 0);

        float velocitySpeed = speed;
        float velocityChange = inputting ? (Grounded ? velocityChangeGround.x : velocityChangeAir.x) : (Grounded ? velocityChangeGround.y : velocityChangeAir.y);

        if (currentState == MovementStates.Diving && Input.GetMouseButton(1)) velocityChange = 1f;

        UpdateMovementStates();

        jumpBuffer = Mathf.MoveTowards(jumpBuffer, 0, Time.deltaTime);

        #region Crouching

        if (Input.GetKey(KeyCode.C))
        {
            if (!Grounded) rb.AddForce(Vector3.down, ForceMode.Force);

            if (transform.localScale == Vector3.one) transform.localPosition -= new Vector3(0, 0.4f, 0);
            transform.localScale = new Vector3(1.2f, 0.6f, 1.2f);
            velocitySpeed = speed / 2;
        }
        else if (!Physics.Raycast(transform.position, Vector3.up, 1.5f, hittableLayer) && transform.localScale == new Vector3(1.2f, 0.6f, 1.2f))
        {
            transform.localPosition += new Vector3(0, 0.29f, 0);
            transform.localScale = groundedSquash = new Vector3(1f, 1f, 1f);
        }

        if (currentState != MovementStates.Falling) transform.localScale = Vector3.MoveTowards(transform.localScale, new Vector3(1, 1, 1), 1.5f * Time.deltaTime);

        #endregion

        rb.velocity = Vector3.SmoothDamp(rb.velocity, new Vector3(moveDir.x * velocitySpeed, rb.velocity.y, moveDir.z * velocitySpeed), ref currentVelocity, velocityChange);
    }

    private void FixedUpdate()
    {
        if (!Grounded) rb.AddForce(new Vector3(0, -0.36f, 0), ForceMode.Impulse);

        if (currentState == MovementStates.Falling) rb.AddForce(new Vector3(0, -0.12f, 0) * stateDuration, ForceMode.Impulse);

        if (currentState == MovementStates.Diving) rb.AddForce(new Vector3(0, -0.25f, 0) * stateDuration, ForceMode.Impulse);
    }

    private void UpdateMovementStates()
    {
        if (Input.GetMouseButtonDown(1) && currentState != MovementStates.Diving) ChangeState(MovementStates.Diving);

        if (stateDuration == 0)
        {
            switch (currentState)
            {
                case MovementStates.Walking:

                    transform.localScale = groundedSquash;
                    transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y, 0);

                    break;

                case MovementStates.Jumping:

                    transform.localScale = new Vector3(0.8f, 1.2f, 0.8f);
                    rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
                    rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

                    break;

                case MovementStates.Diving:

                    transform.localScale = new Vector3(1.2f, 0.8f, 1.2f);
                    rb.velocity = new Vector3(rb.velocity.x, Grounded ? 0 : 5, rb.velocity.z);
                    rb.AddForce(c.cameraPivot.forward * 26, ForceMode.Impulse);

                    break;

            }
        }

        stateDuration += Time.deltaTime;

        switch (currentState)
        {
            case MovementStates.Walking:

                if (Grounded && (Input.GetKeyDown(KeyCode.Space) || jumpBuffer > 0))
                {
                    jumpBuffer = 0;

                    ChangeState(MovementStates.Jumping);
                    break;
                }

                if (!Grounded && !Input.GetKeyDown(KeyCode.Space))
                {
                    ChangeState(MovementStates.Falling);
                    break;
                }

                break;

            case MovementStates.Jumping:

                if ((!Input.GetKey(KeyCode.Space) || Input.GetKeyUp(KeyCode.Space)) && stateDuration > 0.05f)
                {
                    StartCoroutine(CancelJump());
                    ChangeState(MovementStates.Falling);
                    break;
                }

                if (rb.velocity.y < 0 && stateDuration > 0.1f) ChangeState(MovementStates.Falling);

                break;

            case MovementStates.Diving:

                transform.localScale = Vector3.MoveTowards(transform.localScale, new Vector3(0.6f, 1.25f, 0.6f), 0.25f * Time.deltaTime);
                groundedSquash = new Vector3(1.08f + (stateDuration / 40), 0.9f - (stateDuration / 40), 1.08f + (stateDuration / 40));

                if (Input.GetKeyDown(KeyCode.Space))
                {
                    jumpBuffer = jumpBufferTime;
                }

                if (Grounded && stateDuration > .1f) ChangeState(MovementStates.Walking);

                break;

            case MovementStates.Falling:

                transform.localScale = Vector3.MoveTowards(transform.localScale, new Vector3(0.6f, 1.25f, 0.6f), 0.25f * Time.deltaTime);

                groundedSquash = new Vector3(1.08f + (stateDuration / 10), 0.95f - (stateDuration / 10), 1.08f + (stateDuration / 10));

                if (Input.GetKeyDown(KeyCode.Space))
                {
                    if (previousState == MovementStates.Walking && !Grounded && stateDuration <= coyoteTime)
                    {
                        ChangeState(MovementStates.Jumping);
                        break;
                    }

                    jumpBuffer = jumpBufferTime;
                }

                if (Grounded) ChangeState(MovementStates.Walking);

                break;
        }
    }

    private IEnumerator CancelJump()
    {
        while (rb.velocity.y > 0)
        {
            rb.velocity = new Vector3(rb.velocity.x, Mathf.MoveTowards(rb.velocity.y, 0, jumpCancelSpeed * Time.deltaTime), rb.velocity.z);

            yield return new WaitForEndOfFrame();
        }
    }

    private void ChangeState(MovementStates state)
    {
        previousState = currentState;
        currentState = state;
        stateDuration = 0;
    }


    public bool Grounded
    {
        get
        {
            Vector3 position = transform.position + (Vector3.down * 0.9f);

            Collider[] colliders = Physics.OverlapSphere(position, 0.35f, hittableLayer);

            if (colliders.Length > 0) return true;
            else return false;
        }
    }
}
