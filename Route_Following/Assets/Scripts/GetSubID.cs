using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GetSubID : MonoBehaviour
{

    public static string exptime; // to get the time of the experiment 

    public static int[] sceneorder; // to define the order of the maps,scenes

    public static int subIDint; 

    public static EditorBuildSettingsScene[] scenes; // I made it static to call from other scripts

    public void ReadSubID(string s)
    {
        PlayerMovement.SubID = s; // the SubID string in the Player Movement script is the input that this function gets
        Debug.Log(PlayerMovement.SubID);
        subIDint = int.Parse(s); // turn string SubID to integer
        Debug.Log(subIDint);

        if (subIDint % 2 == 0) // Check if SubID is even
        {
            sceneorder = new int[] { 2, 3 };
        }
        else // SubID is odd
        {
            sceneorder = new int[] { 3, 2 };
        }
        Debug.Log(sceneorder[0]);
    }





    public void Start()
    {
        // Get the current date and time to use in data folder
        DateTime currentDateTime = DateTime.Now;

        // Format the current date and time as a string
        exptime = currentDateTime.ToString("yyyy-MM-dd HH-mm-ss");

        scenes = EditorBuildSettings.scenes;

        

    }
}
