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

        [Header("One Handed Attack Animations")] [CanBeNull]
        public string Light_Attack_1;
        public string Light_Attack_2;
        public string Heavy_Attack_1;
    }
}

