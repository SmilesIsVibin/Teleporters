using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerController : MonoBehaviour
{
    private float horizontal;
    public PhotonView pv;
    [SerializeField] private float speed;
    [SerializeField] private float jumpingPower;
    private bool isFacingRight = true;

    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;

    public bool isActive;
    Animator animator;
    public string playerName;

    private void Start()
    {
        pv = GetComponent<PhotonView>();
        animator = GetComponent<Animator>();
        isActive = true;
    }

    void Update()
    {
        if (pv.IsMine)
        {
            if (isActive)
            {
                horizontal = Input.GetAxisRaw("Horizontal");
                if (Input.GetButtonDown("Jump") && IsGrounded())
                {
                    rb.velocity = new Vector2(rb.velocity.x, jumpingPower);
                }
                if (Input.GetButtonDown("Jump") && rb.velocity.y > 0f)
                {
                    rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
                }
                Flip();

                if (Input.GetKeyDown(KeyCode.R))
                {
                    GameManager.Instance.CharacterSwapPostion();
                }

                if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D) || Input.GetButton("Jump"))
                {
                    animator.SetBool("isWalking", true);
                }
                else if (Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.D) || Input.GetButtonUp("Jump"))
                {
                    animator.SetBool("isWalking", false);
                }
            }
        }
    }

    void FixedUpdate()
    {
        rb.velocity = new Vector2(horizontal * speed, rb.velocity.y);
    }

    private bool IsGrounded()
    {
        return (Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer));
    }

    private void Flip()
    {
        if(isFacingRight && horizontal < 0f || !isFacingRight && horizontal > 0f)
        {
            isFacingRight = !isFacingRight;
            Vector3 localRotation = transform.localEulerAngles;
            localRotation.y += -180f;
            transform.localEulerAngles = localRotation;
        }
    }

}
