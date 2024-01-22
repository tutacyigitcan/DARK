using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YT
{
    public class WeaponSlotManager : MonoBehaviour
    {
        private WeaponHolderSlot leftHandSlot;
        private WeaponHolderSlot rightHandSlot;

        private DamageCollider leftHandDamageCollider;
        private DamageCollider rightHandDamageCollider;
        
        private Animator animator;
        private PlayerStats playerStats;
        
        public WeaponItemm attackingWeapon;
        
        private void Awake()
        {
            animator = GetComponent<Animator>();
            playerStats = GetComponentInParent<PlayerStats>();
            
            WeaponHolderSlot[] weaponHolderSlots = GetComponentsInChildren<WeaponHolderSlot>();
            foreach (WeaponHolderSlot weaponSlot in weaponHolderSlots) 
            {
                if (weaponSlot.isLeftHandSlot)
                {
                    leftHandSlot = weaponSlot;
                }
                else if (weaponSlot.isRightHandSlot)
                {
                    rightHandSlot = weaponSlot;
                }
            }
        }
        
        public void LoadWeaponOnSlot(WeaponItemm weaponItemm, bool isLeft)
        {
            if (isLeft)
            {
                leftHandSlot.LoadWeaponModel(weaponItemm);
                LoadLeftWeaponDamageCollider();
                #region Handle Left Weapon Idle Animations

                if (weaponItemm != null)
                {
                    animator.CrossFade(weaponItemm.left_hand_idle, 0.2f);
                }
                else
                {
                    animator.CrossFade("Left Arm Empty" , 0.2f);
                }

                #endregion
               
            }
            else
            {
                rightHandSlot.LoadWeaponModel(weaponItemm);
                LoadRightWeaponDamageCollider();
                #region Handle Right Weapon Idle Animations

                if (weaponItemm != null)
                {
                    animator.CrossFade(weaponItemm.right_hand_idle,0.2f);
                }
                else
                {
                    animator.CrossFade("Right Arm Empty",0.2f);
                }

                #endregion
                
            }
        }

        #region Handle Weapon's Damage Collider
        private void LoadLeftWeaponDamageCollider()
        {
            leftHandDamageCollider = leftHandSlot.currentWeaponModel.GetComponentInChildren<DamageCollider>();
        }

        private void LoadRightWeaponDamageCollider()
        {
            rightHandDamageCollider = rightHandSlot.currentWeaponModel.GetComponentInChildren<DamageCollider>();
        }

        public void OpenRightDamageCollider()
        {
            rightHandDamageCollider.EnableDamageCollider();
        }

        public void OpenLeftDamageCollider()
        {
            leftHandDamageCollider.EnableDamageCollider();
        }

        public void CloseRightHandDamageCollider()
        {
            rightHandDamageCollider.DisableDamageCollider();
        }
        
        public void CloseLeftHandDamageCollider()
        {
            leftHandDamageCollider.DisableDamageCollider();
        }
        
        #endregion

        #region Handle Weapon's Stamina Drainage

        public void DrainStaminaLightAttack()
        {
            playerStats.TakeStaminaDamage(
                Mathf.RoundToInt(attackingWeapon.baseStamina * attackingWeapon.lightAttackMultiplier));
        }
        
        public void DrainStaminaHeavyAttack()
        {
            playerStats.TakeStaminaDamage(
                Mathf.RoundToInt(attackingWeapon.baseStamina * attackingWeapon.heavyAttackMultiplier));
        }
        
        #endregion
        
    }
}
