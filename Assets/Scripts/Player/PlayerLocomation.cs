using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YT
{
    public class PlayerLocomation : MonoBehaviour
    {
        private Transform cameraObject;
        private InputHandler inputHandler;
        public Vector3 moveDirection;
        private PlayerManager playerManager;

        [HideInInspector] public Transform myTransform;
        [HideInInspector] public AnimatorHandler animatorHandler;

        public new Rigidbody rigidbody;
        public GameObject normalCamera;

        [Header("Ground & Air Detection Stats")] 
        [SerializeField]private float groundDetectionRayStartPoint = .5f;
        [SerializeField]private float minimumDistanceNeededToBeginFall = 1f;
        [SerializeField]private float groundDirectionRayDistance = .2f;
        private LayerMask ignoreForGroundCheck;
        public float inAirTimer;
        

        [Header("Movement Stats")] 
        [SerializeField] private float movementSpeed = 5;
        [SerializeField] private float walkingSpeed = 1;
        [SerializeField] private float rotationSpeed = 10;
        [SerializeField] private float sprintSpeed = 7;
        [SerializeField] private float fallingSpeed = 45;



        private void Start()
        {
            rigidbody = GetComponent<Rigidbody>();
            inputHandler = GetComponent<InputHandler>();
            playerManager = GetComponent<PlayerManager>();
            animatorHandler = GetComponentInChildren<AnimatorHandler>();
            cameraObject = Camera.main.transform;
            myTransform = transform;
            animatorHandler.Initialize();

            playerManager.isGrounded = true;
            ignoreForGroundCheck = ~(1 << 8 | 1 << 11);
        }
        
        #region Movement

        private Vector3 normalVector;
        private Vector3 targetPosition;

        private void HandleRotation(float delta)
        {
            Vector3 targetDir = Vector3.zero;
            float moveOverride = inputHandler.moveAmount;

            targetDir = cameraObject.forward * inputHandler.vertical;
            targetDir += cameraObject.right * inputHandler.horizontal;

            targetDir.Normalize();
            targetDir.y = 0;

            if (targetDir == Vector3.zero)
                targetDir = myTransform.forward;

            float rs = rotationSpeed;

            Quaternion tr = Quaternion.LookRotation(targetDir);
            Quaternion targetRotation = Quaternion.Slerp(myTransform.rotation, tr, rs * delta);

            myTransform.rotation = targetRotation;
        }

        public void HandleMovement(float delta)
        {
            if(inputHandler.rollFlag)
                return;
            
            if(playerManager.isInteracting)
                return;
            
            moveDirection = cameraObject.forward * inputHandler.vertical;
            moveDirection += cameraObject.right * inputHandler.horizontal;
            moveDirection.Normalize();
            moveDirection.y = 0;

            float speed = movementSpeed;

            if (inputHandler.sprintFlag && inputHandler.moveAmount > .5)
            {
                speed = sprintSpeed;
                playerManager.isSprinting = true;
                moveDirection *= speed;
            }
            else
            {
                if (inputHandler.moveAmount < .5)
                {
                    moveDirection *= walkingSpeed;
                    playerManager.isSprinting = false;
                }
                else
                {
                    moveDirection *= speed;
                    playerManager.isSprinting = false;
                }
                
            }
            
            Vector3 projectedVelocity = Vector3.ProjectOnPlane(moveDirection, normalVector);
            rigidbody.velocity = projectedVelocity;

            animatorHandler.UpdateAnimatorValue(inputHandler.moveAmount, 0,
                playerManager.isSprinting);

            if (animatorHandler.canRotate)
            {
                HandleRotation(delta);
            }
        }

        public void HandleRollingAndSprinting(float delta)
        {
            if (animatorHandler.anim.GetBool("isInteracting"))
                return;
            if (inputHandler.rollFlag)
            {
                moveDirection = cameraObject.forward * inputHandler.vertical;
                moveDirection += cameraObject.right * inputHandler.horizontal;

                if (inputHandler.moveAmount > 0)
                {
                    animatorHandler.PlayTargetAnimation("Rolling", true);
                    moveDirection.y = 0;
                    Quaternion rollRotation = Quaternion.LookRotation(moveDirection);
                    myTransform.rotation = rollRotation;
                }
                else
                {
                    animatorHandler.PlayTargetAnimation("Backstep",true);
                }
            }

            
        }

        public void HandleFalling(float delta, Vector3 moveDirection)
        {
            playerManager.isGrounded = false;
            RaycastHit hit;
            Vector3 origin = myTransform.position;
            origin.y += groundDetectionRayStartPoint;

            if (Physics.Raycast(origin, myTransform.forward, out hit, .4f))
            {
                moveDirection = Vector3.zero;
            }

            if (playerManager.isInAir)
            {
                rigidbody.AddForce(-Vector3.up * fallingSpeed);
                rigidbody.AddForce(moveDirection * fallingSpeed / 10f);
            }

            Vector3 dir = moveDirection;
            dir.Normalize();
            origin = origin + dir * groundDirectionRayDistance;
            targetPosition = myTransform.position;
            
            Debug.DrawRay(origin, -Vector3.up * minimumDistanceNeededToBeginFall,
                Color.red,.1f,false);
            if (Physics.Raycast(origin, -Vector3.up, out hit, minimumDistanceNeededToBeginFall,
                    ignoreForGroundCheck))
            {
                normalVector = hit.normal;
                Vector3 tp = hit.point;
                playerManager.isGrounded = true;
                targetPosition.y = tp.y;

                if (playerManager.isInAir)
                {
                    if (inAirTimer > .5f)
                    {
                        Debug.Log("You were in the air for " + inAirTimer);
                        animatorHandler.PlayTargetAnimation("Land",true);
                        inAirTimer = 0;
                    }
                    else
                    {
                        animatorHandler.PlayTargetAnimation("Empty", false);
                        inAirTimer = 0;
                    }

                    playerManager.isInAir = false;

                }
                else
                {
                    if (playerManager.isGrounded)
                    {
                        playerManager.isGrounded = false;
                    }
                    
                    // BAKILICAK
                  /*  if (playerManager.isGrounded)
                    {
                        if (playerManager.isInteracting || inputHandler.moveAmount > 0)
                        {
                            myTransform.position = Vector3.Lerp(myTransform.position, targetPosition, Time.deltaTime);
                        }
                        else
                        {
                            myTransform.position = targetPosition;
                        }
                    } */

                    if (playerManager.isInAir == false)
                    {
                        if (playerManager.isInteracting == false)
                        {
                            animatorHandler.PlayTargetAnimation("Falling",true);
                        }

                        Vector3 vl = rigidbody.velocity;
                        vl.Normalize();
                        rigidbody.velocity = vl * (movementSpeed / 2);
                        playerManager.isInAir = true;
                    }
                }

                if (playerManager.isInteracting || inputHandler.moveAmount > 0)
                {
                    myTransform.position = Vector3.Lerp(myTransform.position,
                        targetPosition, Time.deltaTime / 0.1f);
                }
                else
                {

                    myTransform.position = targetPosition;
                }
            }
        }
        
        
        #endregion
    }
}