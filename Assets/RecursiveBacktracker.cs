using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecursiveBacktracker : MonoBehaviour
{

    private MazeNode[,] mazeNodes;


    [SerializeField]
    private int mazeXSize, mazeYSize;


    private List<MazeNode> VisitedNodes;


    [SerializeField]
    private int seed;


    [SerializeField]
    private bool generationFinished;

    private bool drawingStarted;

    void Start()
    {
        mazeNodes = new MazeNode[mazeXSize, mazeYSize];

        RunGeneration();
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
            yield return null;
            // choose cell based on selection Function
            MazeNode currentNode = ChooseRandomCellFromList(VisitedNodes);
            currentNode.visited = true;

          
            //TODO: Fix direction bias of generation
            if (currentNode.savedY < mazeYSize - 1) 
            {
                MazeNode newNode = mazeNodes[currentNode.savedX, currentNode.savedY + 1];
                if (!newNode.visited)
                {
                    // remove wall on both nodes
                    currentNode.Walls &= (int)~Directions.North;
                    newNode.Walls &= (int)~Directions.South;

                    // add new node to growing tree
                    VisitedNodes.Add(newNode);
                    continue;
                }
            }

            if (currentNode.savedY > 0) 
            {
                MazeNode newNode = mazeNodes[currentNode.savedX, currentNode.savedY - 1];
                if (!newNode.visited)
                {
                    // remove wall on both nodes
                    currentNode.Walls &= (int)~Directions.South;
                    newNode.Walls &= (int)~Directions.North;

                    // add new node to growing tree
                    VisitedNodes.Add(newNode);
                    continue;
                }
            }

            if (currentNode.savedX < mazeXSize - 1)
            {
                MazeNode newNode = mazeNodes[currentNode.savedX + 1, currentNode.savedY];
                if (!newNode.visited)
                {
                    // remove wall on both nodes
                    currentNode.Walls &= (int)~Directions.East;
                    newNode.Walls &= (int)~Directions.West;

                    // add new node to growing tree
                    VisitedNodes.Add(newNode);
                    continue;
                }
            }
            if (currentNode.savedX > 0)
            {
                MazeNode newNode = mazeNodes[currentNode.savedX - 1, currentNode.savedY];
                if (!newNode.visited)
                {
                    // remove wall on both nodes
                    currentNode.Walls &= (int)~Directions.West;
                    newNode.Walls &= (int)~Directions.East;

                    // add new node to growing tree
                    VisitedNodes.Add(newNode);
                    continue;
                }
            }

            // no nodes found, removing current node and counting visited as true
            VisitedNodes.Remove(currentNode);


        }

        generationFinished = true;

        
        yield return null;
    }



    private void Update()
    {
        if (generationFinished && !drawingStarted) 
        {
            drawingStarted = true;
            StartCoroutine(ShowMaze());
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
