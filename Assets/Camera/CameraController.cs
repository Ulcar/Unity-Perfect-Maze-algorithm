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



    private Vector3 Origin, Difference;


    public float mainSpeed = 10.0f;   // Regular speed
    public float shiftAdd = 25.0f;   // Amount to accelerate when shift is pressed
    public float maxShift = 100.0f;  // Maximum speed when holding shift
    public float camSens = 0.15f;   // Mouse sensitivity

    private Vector3 lastMouse = new Vector3(255, 255, 255);  // kind of in the middle of the screen, rather than at the top (play)
    private float totalRun = 1.0f;

    private bool mouseWasDown = false;

    private bool mouseDrag = false;

    void Update()
    {
        // Only handle camera angle when right clicking

        if (Input.GetMouseButton(0))
        {
            Difference = camera.ScreenToWorldPoint(Input.mousePosition) - camera.transform.position;
            if (mouseDrag == false)
            {
                mouseDrag = true;
                Origin = camera.ScreenToWorldPoint(Input.mousePosition);
            }
        }

        else 
        {
            mouseDrag = false;
        }

        if (mouseDrag) 
        {
            camera.transform.position = Origin - Difference;
        }

        if (Input.GetMouseButton(1))
        {
            lastMouse = Input.mousePosition - lastMouse;
            lastMouse = new Vector3(-lastMouse.y * camSens, lastMouse.x * camSens, 0);
            lastMouse = new Vector3(transform.eulerAngles.x + lastMouse.x, transform.eulerAngles.y + lastMouse.y, 0);
            if (mouseWasDown)
            {
                transform.eulerAngles = lastMouse;
            }
            lastMouse = Input.mousePosition;
            mouseWasDown = true;
        }
        else
        {
            mouseWasDown = false;
        }
        // Mouse camera angle done.  

        // Keyboard commands
        Vector3 p = GetBaseInput();
        if (Input.GetKey(KeyCode.LeftShift))
        {
            totalRun += Time.deltaTime;
            p *= totalRun * shiftAdd;
            p.x = Mathf.Clamp(p.x, -maxShift, maxShift);
            p.y = Mathf.Clamp(p.y, -maxShift, maxShift);
            p.z = Mathf.Clamp(p.z, -maxShift, maxShift);
        }
        else
        {
            totalRun = Mathf.Clamp(totalRun * 0.5f, 1f, 1000f);
            p *= mainSpeed;
        }

        p *= Time.deltaTime;
        transform.Translate(p);
    }

    // Returns the basic values, if it's 0 than it's not active.
    private Vector3 GetBaseInput()
    {
        Vector3 p_Velocity = new Vector3();

        // Forwards
        if (Input.GetKey(KeyCode.W))
            p_Velocity += new Vector3(0, 0, 1);

        // Backwards
        if (Input.GetKey(KeyCode.S))
            p_Velocity += new Vector3(0, 0, -1);

        // Left
        if (Input.GetKey(KeyCode.A))
            p_Velocity += new Vector3(-1, 0, 0);

        // Right
        if (Input.GetKey(KeyCode.D))
            p_Velocity += new Vector3(1, 0, 0);

        // Up
        if (Input.GetKey(KeyCode.Space))
            p_Velocity += new Vector3(0, 1, 0);

        // Down
        if (Input.GetKey(KeyCode.LeftControl))
            p_Velocity += new Vector3(0, -1, 0);

        return p_Velocity;
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
