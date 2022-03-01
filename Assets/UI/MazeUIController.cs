using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeUIController : MonoBehaviour
{
    private RecursiveBacktracker MazeController;
    private IMazeDrawer drawer;

    public delegate void DrawMazeCallback();
    void Start()
    {
        MazeController = FindObjectOfType<RecursiveBacktracker>();
        drawer = FindObjectOfType<CubeMazeDrawer>();
    }

    public void OnThreadingMethodChanged(Int32 change) 
    {
        MazeController.OnThreadingModeChanged((GenerationMethod)change);
    }
    public void OnDrawMazeFinished() 
    {
        Debug.Log("Drawing finished");
        MazeController.RunGeneration();
    }

    public void OnGenerateButtonPressed() 
    {
        drawer.DrawMaze(OnDrawMazeFinished);
    }
    public void OnGenerationDropDownChanged(Int32 change) 
    {
        MazeController.OnGenerationMethodChanged(change);
    }

    public void OnSeedChanged(string value) 
    {
        int convertedValue = int.Parse(value);
        MazeController.seed = convertedValue;
    }
    public void OnXValueChanged(string value) 
    {
        int convertedValue = int.Parse(value);
        MazeController.mazeXSize = convertedValue;
    }

    public void OnYValueChanged(string value) 
    {
        int convertedValue = int.Parse(value);
        MazeController.mazeYSize = convertedValue;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

public enum GenerationMethod 
{
    Couroutine,
    MultiThreaded
}
