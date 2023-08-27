using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class LoadLevel : MonoBehaviour
{
    public static int i = 0;


    private EditorBuildSettingsScene[] gamescenes = GetSubID.scenes;
    private string[] sceneSequence = GetSubID.sceneorder;

    public void LoadNextMap ()
    {
        if (i == 3) // Check if i is 3
        {
            Debug.Log("Game ended. i is 3.");
            // You can implement your end game logic here
            return; // Exit the method
        }

        SceneManager.LoadScene(sceneSequence[i]); // open the corresponding scene in the scene order
        Debug.Log(sceneSequence[i]);
        i++;
        
    }
 


    
}
