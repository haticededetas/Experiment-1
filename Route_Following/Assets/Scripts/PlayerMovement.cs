using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerMovement : MonoBehaviour
{
    public Transform[] targets; // Array of target objects
    public float moveSpeed = 15f; // Speed at which the player moves
    public float rotationSpeed = 10f; // Speed at which the player rotates towards the target

    public NavMeshAgent navMeshAgent; // Reference to the NavMeshAgent component
    private int currentTargetIndex = 0; // Index of the current target
    private bool shouldMove = false; // Flag to control player movement



    private void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.autoBraking = false; // Disable auto-braking to allow for smooth rotation
    }

    private void Update()
    {
        if (shouldMove)
        {
            // Check if the player has reached the current target
            if (navMeshAgent.enabled && !navMeshAgent.pathPending && navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
            {
                // Stop the player at the target destination
                navMeshAgent.isStopped = true;
                shouldMove = false;

                // Increment the current target index
                currentTargetIndex++;


                // Check if all targets have been reached
                if (currentTargetIndex >= targets.Length)
                {
                    Debug.Log("All targets reached!");
                    shouldMove = false;
                    return;
                }
            }

            // If the player is not at the target destination, continue moving towards it
            if (!navMeshAgent.isStopped)
            {
                navMeshAgent.SetDestination(targets[currentTargetIndex].position);
            }
        }

        // Check if the space button is pressed to start moving towards the next target
        if (!shouldMove && Input.GetKeyDown(KeyCode.Space) && currentTargetIndex < targets.Length)
        {
            // Enable the NavMeshAgent and set the initial destination
            navMeshAgent.enabled = true;
            navMeshAgent.isStopped = false;
            navMeshAgent.SetDestination(targets[currentTargetIndex].position);
            shouldMove = true;
        }

        // Rotate the player towards the current target direction
        if (shouldMove)
        {
            Quaternion targetRotation = Quaternion.LookRotation(navMeshAgent.velocity.normalized);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }
}
