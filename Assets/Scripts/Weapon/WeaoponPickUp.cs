using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace YT
{
    public class WeaoponPickUp : Interactable
    {
        public WeaponItemm weapon;

        public override void Interact(PlayerManager playerManager)
        {
            base.Interact(playerManager);
            PickUpItem(playerManager);
        }

        private void PickUpItem(PlayerManager playerManager)
        {
            PlayerInventory playerInventory;
            PlayerLocomation playerLocomation;
            AnimatorHandler animatorHandler;
            playerInventory = playerManager.GetComponent<PlayerInventory>();
            playerLocomation = playerManager.GetComponent<PlayerLocomation>();
            animatorHandler = playerManager.GetComponentInChildren<AnimatorHandler>();
            
            playerLocomation.rigidbody.velocity = Vector3.zero;; // Stops the player pickup ıtem
            animatorHandler.PlayTargetAnimation("Pick Up Item",true); // Plays animation lootting
            playerInventory.weaponsInventory.Add(weapon);
            playerManager.itemIntectableGameObject.GetComponentInChildren<Text>().text = weapon.itemName;
            playerManager.itemIntectableGameObject.SetActive(true);
            Destroy(gameObject);
        }
    }
}
