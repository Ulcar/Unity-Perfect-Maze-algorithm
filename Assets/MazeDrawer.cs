using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

   public class MazeDrawer:MonoBehaviour
    {


    public GameObject[,,] wallArray;

    public RecursiveBacktracker recursiveBacktracker;

    private void Start()
    {
        recursiveBacktracker = GetComponent<RecursiveBacktracker>();
        wallArray = new GameObject[recursiveBacktracker.mazeXSize, recursiveBacktracker.mazeYSize, 4];

        StartCoroutine(SpawnAllWalls());
    }

    public void RemoveWall(int x, int y, Directions direction) 
    {
        if (direction == Directions.North)
        {
            wallArray[x, y, 0].SetActive(false);
        }
        else if (direction == Directions.South)
        {
            wallArray[x, y, 1].SetActive(false);
        }
        else if (direction == Directions.East)
        {
            wallArray[x, y, 2].SetActive(false);
        }

        else
        {
            wallArray[x, y, 3].SetActive(false);
        }
    }


    IEnumerator SpawnAllWalls() //Creates maze with borders as cubes
        {
            GameObject VerticalBorder = GameObject.CreatePrimitive(PrimitiveType.Cube);  //vertically scaled cube for up and down borders
            VerticalBorder.transform.localScale = new Vector3(1.5f, 1, 0.5f);

            GameObject HorizontalBorder = GameObject.CreatePrimitive(PrimitiveType.Cube);//horizontally scaled cube for left and right borders
            HorizontalBorder.transform.localScale = new Vector3(0.5f, 1, 1.5f);

            for (int x = 0; x < recursiveBacktracker.mazeXSize; x++)
            {
                for (int y = 0; y < recursiveBacktracker.mazeYSize; y++)
                {

                        PlaceVerticalBorder(VerticalBorder, x, y, Directions.North);
                //side walls
                if (y == 0)
                {
                    PlaceVerticalBorder(VerticalBorder, x, y, Directions.South);
                }
                        PlaceHorizontalBorder(HorizontalBorder, x, y, Directions.East);
                if (x == 0)
                {
                    PlaceHorizontalBorder(HorizontalBorder, x, y, Directions.West);
                }
                }
                // return each row
            yield return null;
        }
            GameObject.Destroy(VerticalBorder); //destroy source objects
            GameObject.Destroy(HorizontalBorder);
            yield return null;
        }

        void PlaceHorizontalBorder(GameObject border, int x, int y, Directions d) //borders are put moved away from the point to suround it
        {                                                   //upper border moved up, and lower border moved down along x axis
            GameObject tmp = Instantiate(border, new Vector3(x, 0, y) + Vector3.right * 0.5f * ((d == Directions.East) ? 1 : -1), Quaternion.identity);
            tmp.name = "X = " + x + " Y: " + y + "Direction: " + d.ToString();
        int pos = ((d == Directions.East) ? 2 : 3);
        wallArray[x, y, pos] = tmp;
    }

        void PlaceVerticalBorder(GameObject border, int x, int y,  Directions d)
        {                                                   //left border moved left, and right border moved right along z axis
            GameObject tmp = Instantiate(border, new Vector3(x, 0, y) + Vector3.forward * 0.5f * ((d == Directions.North) ? 1 : -1), Quaternion.identity);
            tmp.name = "X = " + x + " Y: " + y + "Direction: " + d.ToString();
        int pos = ((d == Directions.North) ? 0 : 1);
        wallArray[x, y, pos] = tmp;
    }
    }
