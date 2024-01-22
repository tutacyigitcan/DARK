using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YT
{
    public class PlayerManager : MonoBehaviour
    {
        private InputHandler inputHandler;
        private Animator anim;
        private CameraHandler cameraHandler;
        private PlayerLocomation playerLocomation;
        
        private InteractableUI interactableUI;
        public GameObject interactableUIGameObject;
        public GameObject itemIntectableGameObject;
        
        public bool isInteracting;
        
        [Header("Players Flags")]
        public  bool isSprinting;
        public bool isInAir;
        public bool isGrounded ;
        public bool canDoCombo;

        private void Awake()
        {
            cameraHandler = FindObjectOfType<CameraHandler>();
        }

        
        private void Start()
        {
            inputHandler = GetComponent<InputHandler>();
            anim = GetComponentInChildren<Animator>();
            playerLocomation = GetComponent<PlayerLocomation>();
            interactableUI = FindObjectOfType<InteractableUI>();
        }

        private void Update()
        {
            float delta = Time.deltaTime;
            isInteracting = anim.GetBool("isInteracting");
            canDoCombo = anim.GetBool("canDoCombo");
            anim.SetBool("isInAir",isInAir);
            
            inputHandler.TickInput(delta);
            playerLocomation.HandleMovement(delta);
            playerLocomation.HandleRollingAndSprinting(delta);
            playerLocomation.HandleFalling(delta,playerLocomation.moveDirection);
            playerLocomation.HandleJumping();
            
            CheckForInteractableObject();
        }
        
        private void FixedUpdate()
        {
            float delta = Time.fixedDeltaTime;

            if (cameraHandler != null)
            {
                cameraHandler.FollowTarget(delta);
                cameraHandler.HandleCameraRotation(delta,inputHandler.mouseX,inputHandler.mouseY);
            }
        }

        private void LateUpdate()
        {
            inputHandler.rollFlag = false;
            inputHandler.sprintFlag = false;
            inputHandler.rb_Input = false;
            inputHandler.rt_Input = false;
            inputHandler.d_Pad_Up = false;
            inputHandler.d_Pad_Down = false;
            inputHandler.d_Pad_Left = false;
            inputHandler.d_Pad_Right = false;
            inputHandler.a_Input = false;
            inputHandler.jump_Input = false;

            if (isInAir)
            {
                playerLocomation.inAirTimer = playerLocomation.inAirTimer + Time.deltaTime;
            }
        }

        public void CheckForInteractableObject()
        {
            RaycastHit hit;
            
            if (Physics.SphereCast(transform.position, .3f, transform.forward, out hit, 1f,
                    cameraHandler.ignoreLayers))
            {
                if (hit.collider.tag == "Interactable")
                {
                    Interactable interactableObject = hit.collider.GetComponent<Interactable>();

                    if (interactableObject != null)
                    {
                        string interactablaText = interactableObject.interactableText;
                        interactableUI.interactableText.text = interactablaText;
                        interactableUIGameObject.SetActive(true);
                        
                        if (inputHandler.a_Input)
                        {
                            hit.collider.GetComponent<Interactable>().Interact(this);
                        }
                    }
                }
            }
            else
            {
                if (interactableUIGameObject != null)
                {
                    interactableUIGameObject.SetActive(false);
                }

                if (itemIntectableGameObject != null && inputHandler.a_Input)
                {
                    itemIntectableGameObject.SetActive(false);
                }
            }
        }
    }
}