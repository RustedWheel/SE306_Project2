using System;
using UnityEngine;


public class PlatformerCharacter2D : MonoBehaviour
{
    [SerializeField]
    private float m_MaxSpeed = 10f;                    // The fastest the player can travel in the x axis.
    [SerializeField]
    private float m_JumpForce = 400f;                  // Amount of force added when the player jumps.
    [Range(0, 1)]
    [SerializeField]
    private float m_CrouchSpeed = .36f;  // Amount of maxSpeed applied to crouching movement. 1 = 100%
    [SerializeField]
    private bool m_AirControl = false;                 // Whether or not a player can steer while jumping;
    public LayerMask m_WhatIsGround;                                    // A mask determining what is ground to the character
    [SerializeField]
    private Transform m_GroundCheck;
    public LayerMask m_LeverLayer;
	public AudioClip jumpAudio;			// Array of audio clips for when the player jumps
    const float k_GroundedRadius = .2f; // Radius of the overlap circle to determine if grounded
    private bool m_Grounded;            // Whether or not the player is grounded.
    private Transform m_CeilingCheck;   // A position marking where to check for ceilings
    const float k_CeilingRadius = .01f; // Radius of the overlap circle to determine if the player can stand up
    private Animator m_Anim;            // Reference to the player's animator component.
    private Rigidbody2D m_Rigidbody2D;
    private bool m_FacingRight = true;  // For determining which way the player is currently facing.
    public bool frozen = false;


    private void Awake()
    {
        // Setting up references.
        m_GroundCheck = transform.Find("GroundChecker");
        m_CeilingCheck = transform.Find("CeilingChecker");
        m_Anim = GetComponent<Animator>();
        m_Rigidbody2D = GetComponent<Rigidbody2D>();
    }



    private void FixedUpdate()
    {
        bool hit = Physics2D.OverlapCircle(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);
        updateGroundState(hit);

        // The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
        // This can be done using layers instead but Sample Assets will not overwrite your project settings.


        Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, 2f, m_LeverLayer);
        for (int i = 0; i < colliders.Length; i++)
        {

        }



        //CheckIfGrounded();

        // Set the vertical animation
        m_Anim.SetFloat("VelocityVertical", m_Rigidbody2D.velocity.y);
    }

    void OnCollisionEnter(Collision c)
    {
        if (c.gameObject.tag == "Player")
            Physics.IgnoreCollision(c.gameObject.GetComponent<Collider>(), c.collider);

    }


    void OnCollisionExit2D(Collision2D collider)
    {
        //CheckIfGrounded();
        //updateGroundState(false);
    }

    private void CheckIfGrounded()
    {

        RaycastHit2D hit;

        hit = Physics2D.Raycast(transform.position, Vector2.down, 1.8f, m_WhatIsGround);
        //Debug.Log("Hit " + hit);

        //Debug.DrawRay(transform.position, Vector2.down, Color.green);

        //Debug.DrawRay(transform.position, new Vector2(0, -1.8f), Color.green);
        Debug.DrawRay(transform.position, new Vector2(1, 0), Color.green);
        Debug.DrawRay(transform.position, new Vector2(-1, 0), Color.red);
        Debug.DrawRay(transform.position, new Vector2(0, 1), Color.green);
        Debug.DrawRay(transform.position, new Vector2(0, -1), Color.green);

        //if a collider was hit, we are grounded
        updateGroundState(hit != null);
    }

    private void setGrounded()
    {
        updateGroundState(true);
    }

    private void setUngrounded()
    {
        updateGroundState(false);
    }

    private void updateGroundState(bool b)
    {
        //Debug.Log(b);
        m_Grounded = b;
        m_Anim.SetBool("Ground", b);
    }


    public void Move(float move, bool crouch, bool jump)
    {
        if (!frozen)
        {
			m_Rigidbody2D.gravityScale = 1;
			// If crouching, check to see if the character can stand up
            if (!crouch && m_Anim.GetBool("Crouch"))
            {
                /*
                // If the character has a ceiling preventing them from standing up, keep them crouching
                if (Physics2D.OverlapCircle(m_CeilingCheck.position, k_CeilingRadius, m_WhatIsGround))
                {
                    crouch = true;
                }
                */
            }

            // Set whether or not the character is crouching in the animator
            m_Anim.SetBool("Crouch", crouch);

            //only control the player if grounded or airControl is turned on
            if (m_Grounded || m_AirControl)
            {
                // Reduce the speed if crouching by the crouchSpeed multiplier
                move = (crouch ? move * m_CrouchSpeed : move);

                // The Speed animator parameter is set to the absolute value of the horizontal input.
                m_Anim.SetFloat("Velocity", Mathf.Abs(move));

                // Move the character
                m_Rigidbody2D.velocity = new Vector2(move * m_MaxSpeed, m_Rigidbody2D.velocity.y);

                // If the input is moving the player right and the player is facing left...
                if (move > 0 && !m_FacingRight)
                    Flip();
                // Otherwise if the input is moving the player left and the player is facing right...
                else if (move < 0 && m_FacingRight)
                    // ... flip the player.
                    Flip();
            }
            // If the player should jump...
            if (m_Grounded && jump && m_Anim.GetBool("Ground"))
            {
                // Add a vertical force to the player.
                updateGroundState(false);

				if (jumpAudio != null){
					// Play the random jump audio clip.
					AudioSource.PlayClipAtPoint(jumpAudio, transform.position);
				}
					
                m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));
            }
        }
        else
        {
            m_Rigidbody2D.velocity = Vector3.zero;
			m_Rigidbody2D.gravityScale = 0;
        }
    }


    private void Flip()
    {
        // Switch the way the player is labelled as facing.
        m_FacingRight = !m_FacingRight;

        // Multiply the player's x local scale by -1.
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }
}
