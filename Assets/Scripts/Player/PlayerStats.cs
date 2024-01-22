using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YT
{
    public class PlayerStats : MonoBehaviour
    {
        public int healthLevel = 100;
        public int maxHealth;
        public int currentHealth;

        public int staminaLevel = 10;
        public int maxStamina;
        public int currentStamina;

        private HealtBar healtBar;
        private StaminaBar staminaBar;

        private AnimatorHandler animatorHandler;

        private void Awake()
        {
            healtBar = FindObjectOfType<HealtBar>();
            staminaBar = FindObjectOfType<StaminaBar>();
            animatorHandler = GetComponentInChildren<AnimatorHandler>();
        }

        private void Start()
        {
            maxHealth = SetMaxHealthFromHealthLevel();
            currentHealth = maxHealth;
            healtBar.SetMaxHealth(maxHealth);
            healtBar.SetCurrentHealth(currentHealth);

            maxStamina = SetMaxStaminaFromHStaminaLevel();
            currentStamina = maxStamina;
        }

        private int SetMaxHealthFromHealthLevel()
        {
            maxHealth = healthLevel * 10;
            return maxHealth;
        }
        
        private int SetMaxStaminaFromHStaminaLevel()
        {
            maxStamina = staminaLevel * 10;
            return maxStamina;
        }

        public void TakeDamage(int damage)
        {
            currentHealth = -damage;
            
            healtBar.SetCurrentHealth(currentHealth);

            if (currentHealth <= 0)
            {
                currentHealth = 0;
                animatorHandler.PlayTargetAnimation("Death",true);
                // HANDLE PLAYER DEATH
            }
            
        }

        public void TakeStaminaDamage(int damage)
        {
            currentStamina = currentStamina - damage;
            staminaBar.SetCurrentStamina(currentStamina);
        }
        
    }
}
