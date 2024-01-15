using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YT
{
    public class PlayerAttacker : MonoBehaviour
    {
        private AnimatorHandler animatorHandler;

        private void Awake()
        {
            animatorHandler = GetComponentInChildren<AnimatorHandler>();
        }

        public void HandleLightAttack(WeaponItemm weapon)
        {
            animatorHandler.PlayTargetAnimation(weapon.Light_Attack_1, true);
        }

        public void HandleHeavyAttack(WeaponItemm weapon)
        {
            animatorHandler.PlayTargetAnimation(weapon.Heavy_Attack_1, true);
        }
    }
}
