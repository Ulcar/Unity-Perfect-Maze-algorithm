using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeUIController : MonoBehaviour
{
    private RecursiveBacktracker MazeController;
    private IMazeDrawer drawer;
    private bool generating = false;


    private IMazeDrawer[] drawers;

    public delegate void DrawMazeCallback();
    void Start()
    {
        MazeController = FindObjectOfType<RecursiveBacktracker>();
        drawers = new IMazeDrawer[] {FindObjectOfType<TextureMazeDrawer>(), FindObjectOfType<CubeMazeDrawer>(), null};
        drawer = drawers[1];
        MazeController.ChangeDrawer(drawer);
    }



    public void OnThreadingMethodChanged(Int32 change) 
    {
        MazeController.OnThreadingModeChanged((GenerationMethod)change);
    }

    public void onDrawMethodChanged(Int32 change) 
    {

        drawer.SetActive(false);
        drawer = drawers[change];
        drawer.SetActive(true);
    }
    public void OnDrawMazeFinished() 
    {
        Debug.Log("Drawing finished");
        MazeController.ChangeDrawer(drawer);
        MazeController.RunGeneration();
    }

    public void OnGenerateButtonPressed() 
    {

       MazeController.savedXSize = MazeController.MazeXSize;
       MazeController.savedYSize = MazeController.MazeYSize;

        drawer.DrawMaze(OnDrawMazeFinished);
        MazeController.generationFinished = false;
    }
    public void OnGenerationDropDownChanged(Int32 change) 
    {
        MazeController.OnGenerationMethodChanged(change);
    }

    public void OnSeedChanged(string value) 
    {
        if (MazeController.generationFinished)
        {
            int convertedValue = int.Parse(value);
            MazeController.seed = convertedValue;
        }
    }
    public void OnXValueChanged(string value) 
    {

            int convertedValue = int.Parse(value);
            MazeController.MazeXSize = convertedValue;


    }

    public void OnYValueChanged(string value) 
    {

            int convertedValue = int.Parse(value);
            MazeController.MazeYSize = convertedValue;


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
