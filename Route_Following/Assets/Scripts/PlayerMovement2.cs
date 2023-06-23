using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement2 : MonoBehaviour
{
    public string[] targetNames = { "DP1", "DP2", "DP3" }; // Array of target names
    public float moveSpeed = 5f; // Speed at which the player moves
    public float rotationSpeed = 10f; // Speed at which the player rotates towards the target

    private Transform[] targets; // Array of target transforms
    private int currentTargetIndex = 0; // Index of the current target
    private bool shouldMove = false; // Flag to control player movement

    private void Start()
    {
        // Initialize the targets array based on the targetNames
        targets = new Transform[targetNames.Length];

        // Find the target objects based on their names
        for (int i = 0; i < targetNames.Length; i++)
        {
            targets[i] = GameObject.Find(targetNames[i]).transform;

            // Verify if the target object was found
            if (targets[i] == null)
            {
                Debug.LogError("Target object '" + targetNames[i] + "' not found in the scene!");
            }
        }
    }

    private void Update()
    {
        if (shouldMove)
        {
            // Calculate the direction towards the current target
            Vector3 direction = (targets[currentTargetIndex].position - transform.position).normalized;

            // Rotate the player towards the current target
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            // Calculate the distance between the player and the current target
            float distance = Vector3.Distance(transform.position, targets[currentTargetIndex].position);

            if (distance > 0.1f) // Adjust the threshold distance as needed
            {
                // Move the player towards the current target
                transform.position += direction * moveSpeed * Time.deltaTime;
            }
            else
            {
                // Player has reached the current target, stop moving
                shouldMove = false;

                // Increment the current target index
                currentTargetIndex++;

                // Check if all targets have been reached
                if (currentTargetIndex >= targets.Length)
                {
                    Debug.Log("All targets reached!");
                }
            }
        }

        // Check if the space button is pressed to start moving towards the next target
        if (Input.GetKeyDown(KeyCode.Space) && currentTargetIndex < targets.Length)
        {
            shouldMove = true;
        }
    }
}

