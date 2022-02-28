using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MazeDrawer))]
public class RecursiveBacktracker : MonoBehaviour
{

    private MazeNode[,] mazeNodes;


    [SerializeField]
    public int mazeXSize, mazeYSize;

    //TODO: Change to Array with seperate size parameter so the algorithm can be used in c# jobs
    private List<MazeNode> VisitedNodes;


    [SerializeField]
    private int seed;

    [SerializeField]
    private int StepsPerFrame = 5;

    private int currentSteps = 0;

    [SerializeField]
    private bool generationFinished;

    private bool drawingStarted;

    public delegate MazeNode GetRandomCell(List<MazeNode> nodes);

    private MazeDrawer mazeDrawer;

    void Start()
    {
        mazeNodes = new MazeNode[mazeXSize, mazeYSize];
        mazeDrawer = GetComponent<MazeDrawer>();
       // RunGeneration();
    }


    private MazeNode ChooseRandomCellFromList(List<MazeNode> nodes) 
    {
        // choose newest from list
        return nodes[nodes.Count - 1];
    }

    private void GenerateNodeData() 
    {
        for (int x = 0; x < mazeXSize; x++) 
        {
            for (int y = 0; y < mazeYSize; y++) 
            {
                mazeNodes[x, y] = new MazeNode();
                MazeNode currentNode = mazeNodes[x, y];
                currentNode.savedX = x;
                currentNode.savedY = y;
                currentNode.visited = false;
                currentNode.Walls = 0x0F;
            }
        }
    }


    private bool CheckNorth(MazeNode currentNode) 
    {
        if (currentNode.savedY < mazeYSize - 1)
        {
            MazeNode newNode = mazeNodes[currentNode.savedX, currentNode.savedY + 1];
            if (!newNode.visited)
            {
                // remove wall on both nodes
                currentNode.Walls &= (int)~Directions.North;
                mazeDrawer.RemoveWall(currentNode.savedX, currentNode.savedY, Directions.North);
                newNode.Walls &= (int)~Directions.South;
             //   mazeDrawer.RemoveWall(newNode.savedX, newNode.savedY, Directions.South);

                // add new node to growing tree
                VisitedNodes.Add(newNode);
                return true;
            }
        }

        return false;

    }


    private bool CheckSouth(MazeNode currentNode) 
    {
        if (currentNode.savedY > 0)
        {
            MazeNode newNode = mazeNodes[currentNode.savedX, currentNode.savedY - 1];
            if (!newNode.visited)
            {
                // remove wall on both nodes
                currentNode.Walls &= (int)~Directions.South;
             //   mazeDrawer.RemoveWall(currentNode.savedX, currentNode.savedY, Directions.South);
                newNode.Walls &= (int)~Directions.North;
                mazeDrawer.RemoveWall(newNode.savedX, newNode.savedY, Directions.North);

                // add new node to growing tree
                VisitedNodes.Add(newNode);
                return true;
            }
        }

        return false;
    }

    private bool CheckEast(MazeNode currentNode) 
    {
        if (currentNode.savedX < mazeXSize - 1)
        {
            MazeNode newNode = mazeNodes[currentNode.savedX + 1, currentNode.savedY];
            if (!newNode.visited)
            {
                // remove wall on both nodes
                currentNode.Walls &= (int)~Directions.East;
                mazeDrawer.RemoveWall(currentNode.savedX, currentNode.savedY, Directions.East);
                newNode.Walls &= (int)~Directions.West;
           //     mazeDrawer.RemoveWall(newNode.savedX, newNode.savedY, Directions.West);

                // add new node to growing tree
                VisitedNodes.Add(newNode);
                return true;
            }
        }

        return false;
    }

    private bool CheckWest(MazeNode currentNode) 
    {
        if (currentNode.savedX > 0)
        {
            MazeNode newNode = mazeNodes[currentNode.savedX - 1, currentNode.savedY];
            if (!newNode.visited)
            {
                // remove wall on both nodes
                currentNode.Walls &= (int)~Directions.West;
           //     mazeDrawer.RemoveWall(currentNode.savedX, currentNode.savedY, Directions.West);
                newNode.Walls &= (int)~Directions.East;
                mazeDrawer.RemoveWall(newNode.savedX, newNode.savedY, Directions.East);
                // add new node to growing tree
                VisitedNodes.Add(newNode);
                return true;
            }
        }

        return false;
    }

    private IEnumerator GrowingTree(int seed) 
    {

        VisitedNodes = new List<MazeNode>();

        System.Random randomInstance = new System.Random(seed);


        // add random starting cell
        int randomx = randomInstance.Next(0, mazeXSize);
        int randomY = randomInstance.Next(0, mazeYSize);
        VisitedNodes.Add(mazeNodes[randomx, randomY]);
        mazeNodes[randomx, randomY].savedX = randomx;
        mazeNodes[randomx, randomY].savedY = randomY;



        while (VisitedNodes.Count > 0) 
        {
            currentSteps++;
            if (currentSteps >= StepsPerFrame)
            {
                currentSteps = 0;
                yield return null;
            }

            // choose cell based on selection Function
            MazeNode currentNode = ChooseRandomCellFromList(VisitedNodes);
            currentNode.visited = true;

            Func<MazeNode, bool>[] randomDirectionFunction = { CheckNorth, CheckSouth, CheckEast, CheckWest };
            bool found = false;
            for (int i = 0; i < 4; i++) 
            {
               int direction =  randomInstance.Next(0, randomDirectionFunction.Length - 1);

                if (randomDirectionFunction[direction](currentNode)) 
                {
                    found = true;
                    break;
                }
                //If not possible to go to direction, try again and remove direction from possible chosen directions
                //FIXME: Possible small bias here (direction + 1 has a higher chance of appearing after this)
                randomDirectionFunction[direction] = randomDirectionFunction[(direction + 1) % 4];
            }




            // no nodes found, removing current node and counting visited as true
            if (!found) { 
            VisitedNodes.Remove(currentNode);
                }

        }

        generationFinished = true;

    
    }



    private void Update()
    {
        if (generationFinished && !drawingStarted) 
        {
            drawingStarted = true;
        }

        if (Input.GetKeyDown(KeyCode.Space)) 
        {
            RunGeneration();
        }
    }

    private void RunGeneration() 
    {

        GenerateNodeData();
        StartCoroutine(GrowingTree(seed));
    }



    IEnumerator ShowMaze()  //Creates maze with borders as cubes
    {
        GameObject VerticalBorder = GameObject.CreatePrimitive(PrimitiveType.Cube);  //vertically scaled cube for up and down borders
        VerticalBorder.transform.localScale = new Vector3(1.5f, 1, 0.5f);

        GameObject HorizontalBorder = GameObject.CreatePrimitive(PrimitiveType.Cube);//horizontally scaled cube for left and right borders
        HorizontalBorder.transform.localScale = new Vector3(0.5f, 1, 1.5f);

        for (int x = 0; x < mazeXSize; x++)
        {
            for (int y = 0; y < mazeYSize; y++)
            {
                if ((mazeNodes[x,y].Walls & (int)Directions.North) > 0)
                {
                    PlaceVerticalBorder(VerticalBorder, mazeNodes[x,y], Directions.North);
                }
                if ((mazeNodes[x, y].Walls & (int)Directions.South) > 0)
                {
                    PlaceVerticalBorder(VerticalBorder, mazeNodes[x, y], Directions.South);
                }

                if ((mazeNodes[x, y].Walls & (int)Directions.East) > 0)
                {
                    PlaceHorizontalBorder(HorizontalBorder, mazeNodes[x, y], Directions.East);
                }
                if ((mazeNodes[x, y].Walls & (int)Directions.West) > 0)
                {
                    PlaceHorizontalBorder(HorizontalBorder, mazeNodes[x, y], Directions.West);
                }
                yield return null;
            }
        }
        GameObject.Destroy(VerticalBorder); //destroy source objects
        GameObject.Destroy(HorizontalBorder);
        yield return null;
    }

    void PlaceHorizontalBorder(GameObject border, MazeNode c, Directions d) //borders are put moved away from the point to suround it
    {                                                   //upper border moved up, and lower border moved down along x axis
       GameObject tmp =  Instantiate(border, new Vector3(c.savedX, 0, c.savedY) + Vector3.right * 0.5f * ((d == Directions.East) ? 1 : -1), Quaternion.identity);
        tmp.name = "X = " + c.savedX + " Y: " + c.savedY + "Direction: " + d.ToString();
    }

    void PlaceVerticalBorder(GameObject border, MazeNode c, Directions d)
    {                                                   //left border moved left, and right border moved right along z axis
       GameObject tmp =  Instantiate(border, new Vector3(c.savedX, 0, c.savedY) + Vector3.forward * 0.5f * ((d == Directions.North) ? 1 : -1), Quaternion.identity);
        tmp.name = "X = " + c.savedX + " Y: " + c.savedY + "Direction: " + d.ToString();
    }
}
