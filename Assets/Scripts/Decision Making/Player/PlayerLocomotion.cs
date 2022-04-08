using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace encapsulate
{
    public class PlayerLocomotion : MonoBehaviour
    {
        Transform cameraObject;
        InputHandler inputHandler;
        Vector3 moveDirection;

        [HideInInspector]
        public Transform myTransform;

       // public new RigidBody rigidBody;
        public GameObject normalCamera;

        [Header("Stats")]
        [SerializeField]
        float movementSpeed = 5;
        [SerializeField]
        float rotation = 10;

        // Start is called before the first frame update
        void Start()
        {
          //  rigidBody = GetComponent<Rigidbody>();
            inputHandler = GetComponent<InputHandler>();
            cameraObject = Camera.main.transform;
            myTransform = transform;
        }

        #region Movement
        

        #endregion
    }
}