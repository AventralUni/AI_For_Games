using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class DecisionMakingGame : Characters
{
    private Vector3 playerMovement;
    private Vector2 playerMouse;
    private float xRotation;
    private float yRotation;

    [SerializeField]
    private Rigidbody playerBody;

    [SerializeField]
    private Transform playerCamera;

    [SerializeField]
    private float speed;

    [SerializeField]
    private float sensitivity;

    // Update is called once per frame
    void Update()
    {
        playerMovement = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
        playerMouse = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

        MovePlayer();
        MovePlayerCamera();
    }

    private void MovePlayer()
    {
        Vector3 moveVector = transform.TransformDirection(playerMovement) * speed;
        playerBody.velocity = new Vector3(moveVector.x, playerBody.velocity.y, moveVector.z);
    }

    private void MovePlayerCamera()
    {
        xRotation -= playerMouse.y * sensitivity;
        yRotation += playerMouse.x * sensitivity;

        playerCamera.transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0f);
    }
}
