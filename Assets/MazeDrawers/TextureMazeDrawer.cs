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

    private MazeAlgorithmController MazeController;

    [SerializeField]
    Texture2D texture;

    [SerializeField]
    Color EmptyColor = Color.black;

    [SerializeField]
    Color WallColor = Color.red;


    [SerializeField]
    RawImage image;



    int width, height;

    Color32[] ColorBuffer;


    int timeBetweenUpdates = 1;
    float currentTime= 0;


    private bool updated;
    private void Awake()
    {
        MazeController = FindObjectOfType<MazeAlgorithmController>();
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
                texture.SetPixels32(ColorBuffer);

                texture.Apply();
                
            }
        }

    }

    private IEnumerator DrawMazeRoutine(MazeUIController.DrawMazeCallback drawMazeCallback) 
    {
        texture = new Texture2D((MazeController.savedXSize + 1) * 2, (MazeController.savedYSize + 1) * 2);
        width = texture.width;
        height = texture.height;
        ColorBuffer = new Color32[texture.width * texture.height];
        // draw black pixels for centers, and white pixels for walls
        for (int x = 0; x < MazeController.savedXSize; x++)
        {
            for (int y = 0; y < MazeController.savedYSize; y++)
            {

                int xPixel = x * 2 + 1;

                int yPixel = y * 2 + 1;

                int i = xPixel + (width * yPixel);
                ColorBuffer[i] = EmptyColor;
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
        ColorBuffer[i] = EmptyColor;
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
        ColorBuffer[i] = WallColor;


    }

    public void SetActive(bool value)
    {
        gameObject.SetActive(value);
    }
}
