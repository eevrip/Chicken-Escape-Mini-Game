﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using Cinemachine;
 

public class ChickenMovement : MonoBehaviour
{
    
    public float moveSpeed = 25.0f;
    public float startSpeed = 25.0f;
    public float maxSpeed = 35.0f;
    public float jumpForce = 25.0f;
    public float superJumpForce = 40.0f;
    protected float originalSpeed;
    protected Rigidbody rb;
    protected SphereCollider col;
    public LayerMask groundLayers;
    public bool isgrounded;
    protected Animator animator;
    protected ChickenStatus chSt;
    protected Vector3 vel = Vector3.zero;
    public GameObject terrain;
    public GameObject chicken;
    public GameObject uiFinish;
    private bool canMove = true;
    public GameObject initialfog;
    float startTime;
    public bool increaseSpeed = true;
    int eggCount = 0;
    int eggMax = 6;
    private ChickenStatus playerStatus;
    //public CinemachineVirtualCamera vcam;
   // CinemachineComposer com;
    // Start is called before the first frame update
    void Start()
    {
        startTime = Time.time;
        originalSpeed = moveSpeed;
        rb = GetComponent<Rigidbody>();
        col = GetComponent<SphereCollider>();
        animator = transform.GetChild(0).gameObject.GetComponent<Animator>(); //GetComponent<Animator>();
        chSt = GetComponent<ChickenStatus>();
        
        playerStatus = GameObject.FindGameObjectWithTag("Player").GetComponent<ChickenStatus>();
        StartCoroutine(ExecuteAfterTime(4f));
        //com = vcam.GetCinemachineComponent<CinemachineComposer>();
       // com.m_HorizontalDamping = 7f;
    }
    IEnumerator ExecuteAfterTime(float time)
    {
        yield return new WaitForSeconds(time); 
        // Code to execute after the delay
        initialfog.GetComponent<ParticleSystem>().Stop();
      
    }
   
    //Check if the player is grounded.
    private bool IsGrounded()
    {
        return Physics.CheckSphere(transform.position, 0.2f, groundLayers, QueryTriggerInteraction.Ignore);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Barn"))
        {

            uiFinish.SetActive(true);
            canMove = false;

        }
        if (other.gameObject.CompareTag("GoldenEgg"))
        {
            eggCount++;
        }
        //if (other.gameObject.tag == "TurnCollider") {
         //   com.m_HorizontalDamping = 0.5f;
        //}
           

    }
    
    void OnTriggerStay(Collider other)
    {

        if (other.gameObject.tag == "Left")
        {
           // com.m_HorizontalDamping = 0.5f;
            rb.rotation = Quaternion.Slerp(rb.rotation, Quaternion.LookRotation(-other.gameObject.transform.right), 0.2f);
        }
        if (other.gameObject.tag == "Right")
        {
           // com.m_HorizontalDamping = 0.5f;
            rb.rotation = Quaternion.Slerp(rb.rotation, Quaternion.LookRotation(other.gameObject.transform.right), 0.2f);
        }

    }


   
    void FixedUpdate()
    {
        if (canMove)
        {
            float hmove = Input.GetAxis("Horizontal");
            //float vmove = Input.GetAxis("Vertical");
            //vmove = Mathf.Clamp01(vmove);
            float vmove = 1.0f;
            Camera cam = Camera.main;
            Vector3 forward = cam.transform.forward;
            forward.y = 0;
            forward.Normalize();

            Vector3 right = cam.transform.right;
            right.y = 0;
            right.Normalize();

            if (vmove != 0)
            {
                if (IsGrounded() && !chSt.isHit)
                {
                    // Vector3 move = hmove * transfom.right + vmove * forward;//One of the two, don't know which one is better
                    Vector3 move = hmove * right + vmove * forward;
                    rb.MovePosition(rb.position + move * Time.fixedDeltaTime * moveSpeed);
                    transform.GetChild(0).gameObject.transform.rotation = Quaternion.LookRotation(move);//change direction of chicken mesh according with the movement(go right-left)
                }
                else if (!IsGrounded())
                {
                    Vector3 move = hmove * right + vmove * forward;
                    rb.MovePosition(rb.position + move * Time.fixedDeltaTime * moveSpeed);
                    transform.GetChild(0).gameObject.transform.rotation = Quaternion.LookRotation(move);//change direction of chicken mesh according with the movement(go right-left)
                }
            }
            //Set Run Animation
            animator.SetFloat("ChickenSpeed", vmove);
            //Set part of Jump animation
            animator.SetBool("IsGrounded", IsGrounded());
        }
        else {
            animator.SetFloat("ChickenSpeed", 0f);
            animator.SetBool("IsGrounded", IsGrounded());
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (canMove)
        {
            if (IsGrounded() && Input.GetKeyDown(KeyCode.Space) && !chSt.isHit)
            {   //Jump Force
                rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
                //Set part of Jump animation
                animator.SetTrigger("Jump");

            }

            Debug.Log(playerStatus.strengthMode);
            if (IsGrounded() && playerStatus.strengthMode && Input.GetKeyDown(KeyCode.UpArrow))
            {
                
                //Jump Force
                rb.AddForce(Vector3.up * superJumpForce, ForceMode.Impulse);
                //Set part of Jump animation
                animator.SetTrigger("Jump");
            }

            isgrounded = IsGrounded();
            // Move terrain based on player movement
            Vector3 terrain_pos = chicken.transform.position;
            terrain_pos[1] = -2;
            terrain_pos[0] = terrain_pos[0] - 500;
            terrain_pos[2] = terrain_pos[2] - 500;
            terrain.transform.position = terrain_pos;
        }


        // Increase Speed
        if (increaseSpeed)
        {
            // Increase spped based on collected egg number 
            float diff = maxSpeed - startSpeed;
            Debug.Log("In increased speed");
            Debug.Log(eggCount);
            Debug.Log(eggMax);
            moveSpeed = startSpeed + ((diff/eggMax) * eggCount);
        }
        
       

    }
}
