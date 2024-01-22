using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using JetBrains.Annotations;
using UnityEngine;

namespace YT
{
    [CreateAssetMenu(menuName = "Items/Weapon Item")]
    public class WeaponItemm : Item
    {
        public GameObject modelPrefab;
        public bool isUnarmed;

        [Header("Idle Animations")] 
        public string right_hand_idle;
        public string left_hand_idle;
        public string th_idle;
        
        [Header("Attack Animations")]
        public string Light_Attack_1;
        public string Light_Attack_2;
        public string Heavy_Attack_1;

        [Header("Stamina Costs")] 
        public int baseStamina;
        public float lightAttackMultiplier;
        public float heavyAttackMultiplier;

    }
}

