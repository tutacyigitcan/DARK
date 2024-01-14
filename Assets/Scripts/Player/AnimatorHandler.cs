using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YT
{
    public class AnimatorHandler : MonoBehaviour
    {
        private PlayerManager playerManager;
        public Animator anim;
        private InputHandler inputHandler;
        private PlayerLocomation playerLocomation;
        
        private int vertical;
        private int horizontal;
        public bool canRotate;

        public void Initialize()
        {
            playerManager = GetComponentInParent<PlayerManager>();
            anim = GetComponent<Animator>();
            inputHandler = GetComponentInParent<InputHandler>();
            playerLocomation = GetComponentInParent<PlayerLocomation>();
            vertical = Animator.StringToHash("Vertical");
            horizontal = Animator.StringToHash("Horizontal");
        }

        public void UpdateAnimatorValue(float verticalMovement, float horizontalMovement, bool isSprinting)
        {
            #region Vertical

            float v = 0;

            if (verticalMovement > 0 && verticalMovement < .55f)
            {
                v = .5f;
            }
            else if (verticalMovement > .5f)
            {
                v = 1;
            }
            else if (verticalMovement < 0 && verticalMovement > -.55f)
            {
                v = -.5f;
            }
            else if (verticalMovement < -.55f)
            {
                v = -1;
            }
            else
            {
                v = 0;
            }

            #endregion

            #region Horizontal

            float h = 0;

            if (horizontalMovement > 0 && horizontalMovement < .55f)
            {
                h = .5f;
            }
            else if (horizontalMovement > .5f)
            {
                h = 1;
            }
            else if (horizontalMovement < 0 && horizontalMovement > -.55f)
            {
                h = -.5f;
            }
            else if (horizontalMovement < -.55f)
            {
                h = -1;
            }
            else
            {
                h = 0;
            }
            

            #endregion

            if (isSprinting)
            {
                v = 2;
                h = horizontalMovement;
            }

            anim.SetFloat(vertical, v, .1f, Time.deltaTime);
            anim.SetFloat(horizontal,h,.1f,Time.deltaTime);
        }

        public void PlayTargetAnimation(string targetAnim, bool isInteracting)
        {
            anim.applyRootMotion = isInteracting;
            anim.SetBool("isInteracting",isInteracting);
            anim.CrossFade(targetAnim,0.2f);
        }

        public void CanRotate()
        {
            canRotate = true;
        }

        public void StopRotation()
        {
            canRotate = false;
        }

        private void OnAnimatorMove()
        {
            if(playerManager.isInteracting == false)
                return;

            float delta = Time.deltaTime;
            playerLocomation.rigidbody.drag = 0;
            Vector3 deltaPosition = anim.deltaPosition;
            deltaPosition.y = 0;
            Vector3 velocity = deltaPosition / delta;
            playerLocomation.rigidbody.velocity = velocity;
        }
    }
}

