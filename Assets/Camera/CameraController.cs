using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    // Start is called before the first frame update

    RecursiveBacktracker mazeController;

    Camera camera;
    void Start()
    {
        mazeController = FindObjectOfType<RecursiveBacktracker>();
        camera = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void OnGenerate() 
    {
        CenterCameraOnMaze(camera, false);
    }
    /// <summary>
    /// Adjust the camera to zoom fit the game object
    /// There are multiple directions to get zoom-fit view of the game object,
    /// if ViewFromRandomDirecion is true, then random viewing direction is chosen
    /// else, the camera's forward direction will be used.
    /// </summary>
    /// <param name="c"> The camera, whose position and view direction will be 
    ///                   adjusted to implement zoom-fit effect </param>
    /// <param name="ViewFromRandomDirecion"> if random viewing direction is chozen. </param>
    /// Source: https://gist.github.com/hkusoft/83ad16d71a7d7bcf12216ddf03804f25
    private void CenterCameraOnMaze(Camera c, bool ViewFromRandomDirection = false) 
    {
        Bounds b = new Bounds(new Vector3(mazeController.savedXSize / 2, 0, mazeController.savedYSize / 2), new Vector3(mazeController.savedXSize, 0, mazeController.savedYSize));
        Debug.Log(b);
        Vector3 max = b.size;
        float radius = Mathf.Max(max.x, Mathf.Max(max.y, max.z));
        float dist = radius / (Mathf.Sin(c.fieldOfView * Mathf.Deg2Rad));
        Debug.Log("Radius = " + radius + " dist = " + dist);

        Vector3 view_direction = ViewFromRandomDirection ? UnityEngine.Random.onUnitSphere : Vector3.up;


        Vector3 pos = view_direction * dist + b.center;
        c.transform.position = pos;
        c.transform.LookAt(b.center);
    }
}
