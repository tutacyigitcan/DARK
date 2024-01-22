using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YT
{
    public class CameraHandler : MonoBehaviour
    {
        private InputHandler inputHandler;
        private PlayerManager playerManager;
        
        public Transform targetTransform;
        public Transform cameraTransform;
        public Transform cameraPivotTransform;
        private Transform myTransform;
        private Vector3 cameraTransformPosition;
        public LayerMask ignoreLayers;
        public LayerMask enviromentLayer;
        private Vector3 cameraFollowVelocity = Vector3.zero;

        public static CameraHandler singleton;

        public float lookSpeed = .1f;
        public float followSpeed = .1f;
        public float pivotSpeed = .03f;

        private float targetPosition;
        private float defaultPosition;
        private float lookAngle;
        private float pivotAngle;
        public float minimumPivot = -35f;
        public float maximumPivot = 35f;

        public float cameraSphereRadius = .2f;
        public float camaraCollisionOffSet = .2f;
        public float minimumCollisionOffset = .2f;
        public float lockedPivotPosition = 2.25f;
        public float unlockedPivotPosition = 1.65f;

        public Transform currentLockOnTarget;
        
        private List<CharacterManager> availableTargets = new List<CharacterManager>();
        public Transform nearestLockOnTarget;
        public Transform leftLockTarget;
        public Transform rightLockTarget;
        public float maximumLockOnDistance = 30;

        private void Awake()
        {
            myTransform = transform;
            defaultPosition = cameraTransform.localPosition.z;
            ignoreLayers = ~(1 << 8 | 1 << 9 | 1 << 10);
            targetTransform = FindObjectOfType<PlayerManager>().transform;
            inputHandler = FindObjectOfType<InputHandler>();
            playerManager = FindObjectOfType<PlayerManager>();
        }

        private void Start()
        {
            enviromentLayer = LayerMask.NameToLayer("Environment");
        }

        public void FollowTarget(float delta)
        {
           // Vector3 targetPosition = Vector3.Lerp(myTransform.position, targetTransform.position, delta / followSpeed);
           Vector3 targetPosition = Vector3.SmoothDamp(myTransform.position, targetTransform.position,
               ref cameraFollowVelocity, delta / followSpeed);
            myTransform.position = targetPosition;
            
            HandleCameraCollisions(delta);
        }

        public void HandleCameraRotation(float delta, float mouseXInput, float mouseYInput)
        {
            if (inputHandler.lockOnFlag == false)
            {
                lookAngle += (mouseXInput * lookSpeed) / delta;
                pivotAngle -= (mouseYInput * pivotSpeed) / delta;
                pivotAngle = Mathf.Clamp(pivotAngle, minimumPivot, maximumPivot);

                Vector3 rotaiton = Vector3.zero;
                rotaiton.y = lookAngle;
                Quaternion targetRotation = Quaternion.Euler(rotaiton);
                myTransform.rotation = targetRotation;

                rotaiton = Vector3.zero;
                rotaiton.x = pivotAngle;

                targetRotation = Quaternion.Euler(rotaiton);
                cameraPivotTransform.localRotation = targetRotation;
            }
            else
            {
                float velocity = 0;
                Vector3 dir = currentLockOnTarget.position - transform.position;
                dir.Normalize();
                dir.y = 0;

                Quaternion targetRotation = Quaternion.LookRotation(dir);
                transform.rotation = targetRotation;

                dir = currentLockOnTarget.position - cameraPivotTransform.position;
                dir.Normalize();

                targetRotation = Quaternion.LookRotation(dir);
                Vector3 eularAnlge = targetRotation.eulerAngles;
                eularAnlge.y = 0;
                cameraPivotTransform.localEulerAngles = eularAnlge;
            }
          
        }

        private void HandleCameraCollisions(float delta)
        {
            targetPosition = defaultPosition;
            RaycastHit hit;
            Vector3 direction = cameraTransform.position - cameraPivotTransform.position;
            direction.Normalize();

            if (Physics.SphereCast(cameraPivotTransform.position, cameraSphereRadius, direction, out hit,
                    Mathf.Abs(targetPosition), ignoreLayers))
            {
                float dis = Vector3.Distance(cameraPivotTransform.position, hit.point);
                targetPosition = -(dis - camaraCollisionOffSet);
            }

            if (Mathf.Abs(targetPosition) < minimumCollisionOffset)
            {
                targetPosition = -minimumCollisionOffset;
            }

            cameraTransformPosition.z = Mathf.Lerp(cameraTransform.localPosition.z, targetPosition, delta / .2f);
            cameraTransform.localPosition = cameraTransformPosition;
        }

        public void HandleLockOn()
        {
            float shortesDistance = Mathf.Infinity;
            float shortesDistanceOfLeftTarget = Mathf.Infinity;
            float shortesDistanceOfRightTarget = Mathf.Infinity;
            
            Collider[] colliders = Physics.OverlapSphere(targetTransform.position, 26);
            for (int i = 0; i < colliders.Length; i++)
            {
                CharacterManager character = colliders[i].GetComponent<CharacterManager>();

                if (character != null)
                {
                    Vector3 lockTargetDirection = character.transform.position - targetTransform.position;
                    float distanceFromTarget =
                        Vector3.Distance(targetTransform.position , character.transform.position);
                    float viewableAngle = Vector3.Angle(lockTargetDirection, cameraTransform.forward);
                    
                    RaycastHit hit;
                    
                    if(character.transform.root != targetTransform.transform.root && viewableAngle > -50 && 
                       viewableAngle < 50  && distanceFromTarget <= maximumLockOnDistance)
                    {
                        if (Physics.Linecast(playerManager.lookOnTransform.position, character.lookOnTransform.position,
                                out hit))
                        {
                            Debug.DrawLine(playerManager.lookOnTransform.position, character.lookOnTransform.position);

                            if (hit.transform.gameObject.layer == enviromentLayer)
                            {
                                // Cannot look onta target, object in the way
                            }
                            else
                            {
                                availableTargets.Add(character);
                            }
                        }
                    }
                }
            }

            for (int k = 0; k < availableTargets.Count; k++)
            {
                float distanceFromTarget =
                    Vector3.Distance(targetTransform.position, availableTargets[k].transform.position);

                if (distanceFromTarget < shortesDistance)
                {
                    shortesDistance = distanceFromTarget;
                    nearestLockOnTarget = availableTargets[k].lookOnTransform;
                }

                if (inputHandler.lockOnFlag)
                {
                    Vector3 relativeEnemyPosition =
                        currentLockOnTarget.InverseTransformPoint(availableTargets[k].transform.position);
                    var distanceFromLeftTarget = currentLockOnTarget.transform.position.x -
                                                 availableTargets[k].transform.position.x;
                    var distanceFromRightTarget = currentLockOnTarget.transform.position.x +
                                                  availableTargets[k].transform.position.x;

                    if (relativeEnemyPosition.x > 0.00 && distanceFromTarget < shortesDistanceOfLeftTarget)
                    {
                        shortesDistanceOfLeftTarget = distanceFromTarget;
                        leftLockTarget = availableTargets[k].lookOnTransform;
                    }

                    if (relativeEnemyPosition.x < 0.00 && distanceFromTarget < shortesDistanceOfRightTarget)
                    {
                        shortesDistanceOfRightTarget = distanceFromTarget;
                        rightLockTarget = availableTargets[k].lookOnTransform;
                    }
                }
            }
            
        }

        public void ClearLockOnTargets()
        {
            availableTargets.Clear();
            nearestLockOnTarget = null;
            currentLockOnTarget = null;
        }

        public void SetCameraHeight()
        {
            Vector3 velocity = Vector3.zero;
            Vector3 newLockedPosition = new Vector3(0, lockedPivotPosition);
            Vector3 newUnlockedPosition = new Vector3(0, unlockedPivotPosition);

            if (currentLockOnTarget != null)
            {
                cameraPivotTransform.transform.localPosition = Vector3.SmoothDamp(
                    cameraPivotTransform.transform.localPosition, newLockedPosition, ref velocity, Time.deltaTime);
            }
            else
            {
                cameraPivotTransform.transform.localPosition = Vector3.SmoothDamp(
                    cameraPivotTransform.transform.localPosition, newUnlockedPosition, ref velocity, Time.deltaTime);
            }
        }
     }
}

