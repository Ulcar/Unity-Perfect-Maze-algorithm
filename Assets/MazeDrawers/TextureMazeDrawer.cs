using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// Draws the maze in a 2D representation, rendering the maze to a 2D texture. 
/// This texture could in the future be used for a shader etc. 
/// </summary>
public class TextureMazeDrawer : MonoBehaviour, IMazeDrawer
{

    private MazeAlgorithmController mazeController;

    [SerializeField]
   private Texture2D texture;

    [SerializeField]
   private Color emptyColor = Color.black;

    [SerializeField]
   private Color wallColor = Color.red;


    [SerializeField]
   private RawImage image;

   private int width, height;
   private Color32[] colorBuffer;
   private int timeBetweenUpdates = 1;
   private float currentTime= 0;
    private bool updated;
    private void Awake()
    {
        mazeController = FindObjectOfType<MazeAlgorithmController>();
    }

    private void Update()
    {

        if (updated) 
        {
            currentTime += Time.deltaTime;
            if (currentTime > timeBetweenUpdates) 
            {
                currentTime = 0;
                updated = false;
                texture.SetPixels32(colorBuffer);

                texture.Apply();
                
            }
        }

    }

    private IEnumerator DrawMazeRoutine(MazeUIController.DrawMazeCallback drawMazeCallback) 
    {
        texture = new Texture2D((mazeController.savedXSize + 1) * 2, (mazeController.savedYSize + 1) * 2);
        width = texture.width;
        height = texture.height;
        colorBuffer = new Color32[texture.width * texture.height];
        // draw black pixels for centers, and white pixels for walls
        for (int x = 0; x < mazeController.savedXSize; x++)
        {
            for (int y = 0; y < mazeController.savedYSize; y++)
            {

                int xPixel = x * 2 + 1;

                int yPixel = y * 2 + 1;

                int i = xPixel + (width * yPixel);
                colorBuffer[i] = emptyColor;
                PlaceWall(x, y, Directions.North);
                PlaceWall(x, y, Directions.East);
                //side walls
                if (y == 0)
                {
                    PlaceWall(x, y, Directions.South);
                }

                if (x == 0)
                {
                    PlaceWall(x, y, Directions.West);
                } 
                updated = true;
            }

            yield return null;
        }
        drawMazeCallback();
        image.texture = texture;
        image.rectTransform.sizeDelta = new Vector2(800, 800);
        // Fixing aspect ratio of image
        if (texture.width > texture.height) 
        {
            float ratio = (float)texture.width / (float)texture.height;
            Debug.Log("Fixing aspect ratio " + ratio + " Solution: " + image.rectTransform.sizeDelta.y / ratio);

            image.rectTransform.sizeDelta = new Vector2(image.rectTransform.sizeDelta.x, image.rectTransform.sizeDelta.y / ratio);
        }

        if (texture.height > texture.width) 
        {
            float ratio = ((float)texture.height / (float)texture.width);
            Debug.Log("Fixing aspect ratio " + ratio + " Solution: " + image.rectTransform.sizeDelta.x / ratio);
            image.rectTransform.sizeDelta = new Vector2(image.rectTransform.sizeDelta.x / ratio, image.rectTransform.sizeDelta.y);
        }
        updated = true;


        yield return null;
    }
    public void DrawMaze(MazeUIController.DrawMazeCallback drawMazeCallback)
    {
        image.gameObject.SetActive(true);
        StartCoroutine(DrawMazeRoutine(drawMazeCallback));

    }



    private void OnDisable()
    {
        image.gameObject.SetActive(false);
    }


    public void RemoveWall(int x, int y, Directions d)
    {



        int xPixel, yPixel;

        if (d == Directions.North)
        {
            xPixel = x * 2 + 1;
            yPixel = y * 2 + 2;
        }

        else if (d == Directions.South)
        {
            xPixel = x * 2 + 1;
            yPixel = y * 2;
        }

        else if (d == Directions.East)
        {
            xPixel = x * 2 + 2;
            yPixel = y * 2 + 1;
        }

        else
        {
            xPixel = x * 2;
            yPixel = y * 2 + 1;
        }

        int i = xPixel + (width * yPixel);
        colorBuffer[i] = emptyColor;
        updated = true;

    }



    void PlaceWall(int x, int y, Directions d)
    {
        int xPixel, yPixel;

        if (d == Directions.North) 
        {
            xPixel = x * 2 + 1;
            yPixel = y * 2 + 2;
        }

      else  if (d == Directions.South) 
        {
            xPixel = x * 2 + 1;
            yPixel = y * 2;
        }

      else  if (d == Directions.East) 
        {
            xPixel = x * 2 + 2;
            yPixel = y * 2 + 1;
        }

        else 
        {
            xPixel = x * 2;
            yPixel = y * 2 + 1;
        }

       int i = xPixel + (width * yPixel);
        colorBuffer[i] = wallColor;


    }

    public void SetActive(bool value)
    {
        gameObject.SetActive(value);
    }
}
