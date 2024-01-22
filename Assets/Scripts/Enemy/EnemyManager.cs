using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YT
{
    public class EnemyManager : CharacterManager
    {

        private EnemyLocomationManager enemyLocomationManager;
        
        public bool isPreformingAction;

        [Header("A.I Settings")] 
        public float detectionRadius = 20;
        public float maximumDetectionangle = 50;
        public float minimumDetectionangle = -50;
        
        
        private void Awake()
        {
            enemyLocomationManager = GetComponent<EnemyLocomationManager>();
        }
        

        private void FixedUpdate()
        {
            HandleCurrentAction();
        }

        private void HandleCurrentAction()
        {
            if (enemyLocomationManager.currentTarget == null)
            {
                enemyLocomationManager.HandleDetection();
            }
            else
            {
                enemyLocomationManager.HandleMoveToTarget();
            }
        }
    }
}
