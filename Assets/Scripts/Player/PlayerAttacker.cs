using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YT
{
    public class PlayerAttacker : MonoBehaviour
    {
        private AnimatorHandler animatorHandler;
        private InputHandler inputHandler;
        public string lastAttack;

        private void Awake()
        {
            animatorHandler = GetComponentInChildren<AnimatorHandler>();
            inputHandler = GetComponent<InputHandler>();
        }

        public void HandleWeaponCombo(WeaponItemm weapon)
        {
            if (inputHandler.comboFlag)
            {
                animatorHandler.anim.SetBool("canDoCombo", false);
                if (lastAttack == weapon.Light_Attack_1)
                {
                    animatorHandler.PlayTargetAnimation(weapon.Light_Attack_2, true);
                }
            }
        }
        
        public void HandleLightAttack(WeaponItemm weapon)
        {
            animatorHandler.PlayTargetAnimation(weapon.Light_Attack_1, true);
            lastAttack = weapon.Light_Attack_1;
        }

        public void HandleHeavyAttack(WeaponItemm weapon)
        {
            animatorHandler.PlayTargetAnimation(weapon.Light_Attack_2, true);
            lastAttack = weapon.Light_Attack_2;
        }
    }
}
