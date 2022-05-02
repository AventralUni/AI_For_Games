using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//public class EnemyLocomotionManager : MonoBehaviour
//{
//    DM_EnemyManager enemyManager;
//    public LayerMask detectionLayer;
//    Characters currentTarget;

//    public void Awake()
//    {
//        enemyManager = GetComponent<enemyManager>();
//    }
//    public void HandleDetection()
//    {
//        Collider[] colliders = Physics.OverlapSphere(transform.position, enemyManager.detectionRadius, detectionLayer);

//        for (int i = 0; i < colliders.Length; i++)
//        {
//            Characters character = colliders[i].transform.GetComponent<Character>();

//            if (character != null)
//            {
//                //check for team ID

//                Vector3 targetDirection = CharacterStats.transform.position - transform.position;
//                float viewableAngle = Vector3.Angle(targetDirection, transform.forward);

//                if (viewableAngle > enemyManager.minumDetectionAngle && viewableAngle < enemyManager.maximumDetectionAngle)
//                {
//                    currentTarget = Character;
//                }

//            }
//        }
//    }
//}
