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

        //string sceneName = System.IO.Path.GetFileNameWithoutExtension(gamescenes[sceneSequence[i]].path);
        SceneManager.LoadScene(sceneSequence[i]);
        Debug.Log(sceneSequence[i]);
        i++;
        
    }
 


    
}
