using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.IO;

public class PlayerMovement : MonoBehaviour
{
    public enum CorrectAnswer
    {
        A,
        W,
        D,
        X
    }

    [System.Serializable]
    public struct DecisionPoint
    {
        public Transform target; // Target object
        public CorrectAnswer correctAnswer; // Correct answer for the decision point
    }

    // Data save
    
    public CsvExporter csvExporter; // Reference to the CsvExporter script


    // Prepare the data list.

    class Trial_Data
    {
        public string DP_Name { get; set; } // decision point name
        public int Index_Number { get; set; } // the index number        
        public float ErrorNumber { get; set; } // number of errors

        public float RT_rotation_correct { get; set; } // reaction time during rotation
        public float RT_decision_correct { get; set; } // reaction time during decision
        public float RT_correct { get; set; } // total reaction time during decision

        public float RT_rotation_mistake1 { get; set; } // reaction time during first rotation mistake
        public float RT_decision_mistake1 { get; set; } // reaction time during first decision mistake
        public float RT_mistake1 { get; set; } // total reaction time during first decision mistake

        public float RT_rotation_mistake2 { get; set; } // reaction time during second rotation mistake
        public float RT_decision_mistake2 { get; set; } // reaction time during second decision mistake
        public float RT_mistake2 { get; set; } // total reaction time during first decision mistake

        public float RT_rotation_mistake3 { get; set; } // reaction time during third rotation mistake
        public float RT_decision_mistake3 { get; set; } // reaction time during third decision mistake
        public float RT_mistake3 { get; set; } // total reaction time during first decision mistake

    }

    List<Trial_Data> data_list = new List<Trial_Data>(); // the list which consists of trial data
    private string fileName = "data.csv";


    public string filePath; // File path for the CSV file
    public DecisionPoint[] decisionPoints; // Array of decision points
    private string decision_point_name; // It will be used to save data correctly
    
    
    private float rotationSpeed = 30f; // Speed at which the player rotates towards the target
    private float rotationTimeLimit = 30f; // Time limit for rotation (in seconds)

    public NavMeshAgent navMeshAgent; // Reference to the NavMeshAgent component
    public AlertMessage alertMessageComponent; // Reference to the AlertMessage component


    // Create parameters for the start of the task
    private bool shouldMove = false; // Flag to control player movement
    private bool isRotating = false; // Flag to control player rotation
    private float rotationTimer = 0f; // Timer for rotation
    private float decisionTimer = 0f; // Timer for rotation
    private bool canMakeDecision = false; // Flag to allow decision-making
    private bool decCorrect = true; // If the decision is correct or not. Created to define the messages presented.

    private Quaternion initialRotation; // Initial rotation when arriving at target

    public Transform initialOrientation; // Object to determine the initial rotation


    //Create Indicies 
    private int currentDecisionIndex = 0; // Index of the current decision point
    private int numberofError = 0;  // Index for the number of errors

    // Create possible reaction times for each rotation movement
    private float RT_rotation_correct = 0; // RT for the first try
    private float RT_rotation_mistake1 = 0; // RT for the second try
    private float RT_rotation_mistake2 = 0; // RT for the third try
    private float RT_rotation_mistake3 = 0; // RT for the third try

    // Create possible reaction times for each decision process
    private float RT_decision_correct = 0;
    private float RT_decision_mistake1 = 0;
    private float RT_decision_mistake2 = 0;
    private float RT_decision_mistake3 = 0;

    // Create variables for total RTs.
    private float RT_correct = 0;
    private float RT_mistake1 = 0;
    private float RT_mistake2 = 0;
    private float RT_mistake3 = 0;

    private void Start()
    {
        

        // create 10 rows for the list
        

        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.autoBraking = false; // Disable auto-braking to allow for smooth rotation

        // Allow rotation from the start
        isRotating = true;

        // Set the initial rotation based on the initialOrientation object
        if (initialOrientation != null)
        {
            Vector3 direction = initialOrientation.position - transform.position;
            initialRotation = Quaternion.LookRotation(direction);

            // Apply the initial rotation immediately
            transform.rotation = initialRotation;
            isRotating = true;
        }


        // Check if the AlertMessage component is found
        if (alertMessageComponent == null)
        {
            Debug.LogError("AlertMessage component reference not set!");
        }

        //message
        string rotationMessage = "Please rotate around to see your options and press space button to proceed to the decision phase.";
        alertMessageComponent.ShowAlert(rotationMessage);


    }

    private void Update()
    {
        if (shouldMove)
        {
            alertMessageComponent.HideAlert();
            // Check if the player has reached the current decision point
            if (navMeshAgent.enabled && !navMeshAgent.pathPending && navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
            {
                // Stop the player at the target destination
                navMeshAgent.isStopped = true;
                shouldMove = false;

                // Increment the current decision index
                currentDecisionIndex++;

                // Check if all decision points have been reached
                if (currentDecisionIndex >= decisionPoints.Length)
                {
                    Debug.Log("All decision points reached!");
                    shouldMove = false;


                    // SAVE DATA
                    // name data file
                    string filePath = Path.Combine(Application.dataPath, fileName);


                    //SaveDecisionPointsToCSV();
                    csvExporter.ExportListToCsv(data_list, filePath);
                   
                    
                    return;

                    
                    
                }

                // Set the initial rotation when arriving at the decision point
                initialRotation = transform.rotation;

                // Start the rotation timer
                rotationTimer = 0f;

                // Enable rotation for a limited time
                isRotating = true;

                // Disable decision-making until space button is pressed
                canMakeDecision = false;
            }
        }

        // Rotate the player using keyboard arrows for a limited time
        if (isRotating)
        {
            if (decCorrect)
            {
                string rotationMessage = "Please rotate around to see your options and press button to proceed to the decision phase.";
                alertMessageComponent.ShowAlert(rotationMessage);

            }
            else if (!decCorrect)
            {
                string errorMessage = "Your decision is wrong. You can rotate again or make a new response after pressing the space button.";
                alertMessageComponent.ShowAlert(errorMessage);
            }


            rotationTimer += Time.deltaTime;

            if (Input.GetKey(KeyCode.RightArrow))
            {
                transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
            }
            else if (Input.GetKey(KeyCode.LeftArrow))
            {
                transform.Rotate(Vector3.up, -rotationSpeed * Time.deltaTime);
            }


            // Check if the rotation time limit has been reached or space button is pressed
            if (rotationTimer >= rotationTimeLimit || Input.GetKeyDown(KeyCode.Space))
            {
                
                transform.rotation = initialRotation;
                isRotating = false;

                // Enable decision-making
                canMakeDecision = true;

                // Display appropriate message based on condition
                string message = rotationTimer >= rotationTimeLimit ? "Your time is up, please make your decision." : "Please make your decision.";
                alertMessageComponent.ShowAlert(message);

                decisionTimer += Time.deltaTime;
            }

            

            


        }

        // Check if the 'a', 'w', 'd', or 'x' key is pressed to start moving towards the next decision point
        if (canMakeDecision && currentDecisionIndex < decisionPoints.Length)
        {
            DecisionPoint currentDecision = decisionPoints[currentDecisionIndex];

            // Check if the input is one of the allowed response options
            if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.X))
            {
                // Check if the response is correct
                if ((Input.GetKeyDown(KeyCode.A) && currentDecision.correctAnswer == CorrectAnswer.A) ||
                    (Input.GetKeyDown(KeyCode.W) && currentDecision.correctAnswer == CorrectAnswer.W) ||
                    (Input.GetKeyDown(KeyCode.D) && currentDecision.correctAnswer == CorrectAnswer.D) ||
                    (Input.GetKeyDown(KeyCode.X) && currentDecision.correctAnswer == CorrectAnswer.X))
                {
                    RotateTowardsNextDecisionPoint(currentDecision.target);
                    decCorrect = true; //decision is correct
                    
                    
                    if (currentDecisionIndex == 0)
                    {
                        decision_point_name = "DP1";
                    }
                    else 
                    {
                        decision_point_name = decisionPoints[currentDecisionIndex-1].target.name;
                    }

                    RT_rotation_correct = rotationTimer;
                    RT_decision_correct = decisionTimer;
                    RT_correct = rotationTimer + decisionTimer;


                    data_list.Insert(currentDecisionIndex, new Trial_Data { DP_Name = decision_point_name, 
                        Index_Number = currentDecisionIndex,                         
                        ErrorNumber = numberofError,
                        RT_rotation_correct = RT_rotation_correct,
                        RT_decision_correct = RT_decision_correct,
                        RT_correct = RT_correct,

                        RT_rotation_mistake1 = RT_rotation_mistake1,
                        RT_decision_mistake1 = RT_decision_mistake1,
                        RT_mistake1 = RT_mistake1,

                        RT_rotation_mistake2 = RT_rotation_mistake2,
                        RT_decision_mistake2 = RT_decision_mistake2,
                        RT_mistake2 = RT_mistake2,


                        RT_rotation_mistake3 = RT_rotation_mistake3,
                        RT_decision_mistake3 = RT_decision_mistake3,
                        RT_mistake3 = RT_mistake3,

                    });


                    // name data file
                    
                    string filePath = Path.Combine(Application.dataPath, fileName);
                    //SaveDecisionPointsToCSV();
                    csvExporter.ExportListToCsv(data_list, filePath);


                    // reset the RTs and the error number
                    RT_rotation_correct = 0;
                    RT_decision_correct = 0;

                    RT_rotation_mistake1 = 0;
                    RT_rotation_mistake2 = 0;
                    RT_rotation_mistake3 = 0;

                    RT_decision_mistake1 = 0;
                    RT_decision_mistake2 = 0;
                    RT_decision_mistake3 = 0;

                    RT_correct = 0;
                    RT_mistake1 = 0;
                    RT_mistake2 = 0;
                    RT_mistake3 = 0;

                    numberofError = 0;
                }
                else
                {
                    numberofError++;

                    if (numberofError == 1) 
                    {
                        RT_rotation_mistake1 = rotationTimer;
                        RT_decision_mistake1 = decisionTimer;
                        RT_mistake1 = RT_rotation_mistake1 + RT_decision_mistake1;
                    }
                    else if (numberofError == 2)
                    {
                        RT_rotation_mistake2 = rotationTimer;
                        RT_decision_mistake2 = decisionTimer;
                        RT_mistake2 = RT_rotation_mistake2 + RT_decision_mistake2;
                    }
                    else if (numberofError == 3)
                    {
                        RT_rotation_mistake3 = rotationTimer;
                        RT_decision_mistake3 = decisionTimer;
                        RT_mistake3 = RT_rotation_mistake3 + RT_decision_mistake3;
                    }
                    
                    decCorrect = false;
                    string errorMessage = "Your decision is wrong. You can rotate again or make a new response after pressing the space button.";
                    alertMessageComponent.ShowAlert(errorMessage);

                    rotationTimer = 0f;
                    isRotating = true;
                    canMakeDecision = false; //decision is incorrect


                }
            }

            


        }

        
    }

    private void RotateTowardsNextDecisionPoint(Transform target)
    {
        // Calculate the direction to the target
        Vector3 direction = target.position - transform.position;
        Quaternion targetRotation = Quaternion.LookRotation(direction);

        // Start rotating towards the target destination
        StartCoroutine(RotateTowardsTarget(targetRotation, () =>
        {
            // After the rotation is complete, start moving towards the target
            navMeshAgent.enabled = true;
            navMeshAgent.isStopped = false;
            navMeshAgent.SetDestination(target.position);
            shouldMove = true;
            canMakeDecision = false; // Disable decision-making until the player reaches the target
        }));
    }

    

    private IEnumerator RotateTowardsTarget(Quaternion targetRotation, System.Action onRotationComplete)
    {
        float elapsedTime = 0f;
        float rotationDuration = 1f; // Duration of rotation (adjust as needed)

        Quaternion startRotation = transform.rotation;

        while (elapsedTime < rotationDuration)
        {
            elapsedTime += Time.deltaTime;

            // Calculate the interpolation factor based on the elapsed time
            float t = Mathf.Clamp01(elapsedTime / rotationDuration);

            // Slerp between the start rotation and the target rotation
            transform.rotation = Quaternion.Slerp(startRotation, targetRotation, t);

            yield return null;
        }

        // Set the final rotation to ensure accuracy
        transform.rotation = targetRotation;

        // Invoke the callback function when the rotation is complete
        onRotationComplete?.Invoke();
    }
}
