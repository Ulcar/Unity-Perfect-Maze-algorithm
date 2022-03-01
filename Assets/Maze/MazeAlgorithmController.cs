using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;


/// <summary>
/// This class implements the 
/// </summary>
public class MazeAlgorithmController : MonoBehaviour
{

    private MazeNode[,] mazeNodes;

    [SerializeField]
    public int MazeXSize, MazeYSize;
    public int savedXSize, savedYSize;
    private List<MazeNode> VisitedNodes;

    [SerializeField]
    public int seed;

    [SerializeField]
    private int StepsPerFrame = 5;

    private int currentSteps = 0;

    [SerializeField]
    public bool generationFinished;

    private bool drawingStarted;

    public delegate MazeNode GetRandomCell(List<MazeNode> nodes);

    public GetRandomCell NodeChooser;

    private IMazeDrawer mazeDrawer;

    private System.Random randomInstance;

    private Thread thread;

    private GenerationMethod method;

    private GetRandomCell[] generationFunctions;

    void Start()
    {
        NodeChooser = ChooseLastCellFromList;
        generationFunctions = new GetRandomCell[] { ChooseLastCellFromList, ChooseFirstCellFromList, ChooseMiddleCellFromList, ChooseRandomCellFromList };
        // RunGeneration();
    }
    public void ChangeDrawer(IMazeDrawer drawer)
    {
        this.mazeDrawer = drawer;

    }
    public void OnGenerationMethodChanged(int method)
    {


        NodeChooser = generationFunctions[method];
    }

    public void OnThreadingModeChanged(GenerationMethod method)
    {
        this.method = method;
    }

    private MazeNode ChooseLastCellFromList(List<MazeNode> nodes)
    {
        // choose newest from list
        return nodes[nodes.Count - 1];
    }

    private MazeNode ChooseRandomCellFromList(List<MazeNode> nodes)
    {
        return nodes[randomInstance.Next(0, nodes.Count - 1)];
    }

    private MazeNode ChooseFirstCellFromList(List<MazeNode> nodes)
    {
        return nodes[0];
    }

    private MazeNode ChooseMiddleCellFromList(List<MazeNode> nodes)
    {
        return nodes[nodes.Count / 2];
    }

    private void GenerateNodeData()
    {
        mazeNodes = new MazeNode[savedXSize, savedYSize];
        for (int x = 0; x < savedXSize; x++)
        {
            for (int y = 0; y < savedYSize; y++)
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
        if (currentNode.savedY < savedYSize - 1)
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
                newNode.visited = true;
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
                newNode.visited = true;
                return true;
            }
        }

        return false;
    }

    private bool CheckEast(MazeNode currentNode)
    {
        if (currentNode.savedX < savedXSize - 1)
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
                newNode.visited = true;
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
                newNode.visited = true;
                return true;
            }
        }

        return false;
    }

    public void StopGeneration()
    {
        StopAllCoroutines();

        if (thread != null)
        {
            if (thread.IsAlive)
            {
                thread.Abort();
            }
        }
    }
    private void GrowingTreeThread(int seed)
    {
        VisitedNodes = new List<MazeNode>();
        randomInstance = new System.Random(seed);
        // add random starting cell
        int randomx = randomInstance.Next(0, savedXSize);
        int randomY = randomInstance.Next(0, savedYSize);
        VisitedNodes.Add(mazeNodes[randomx, randomY]);
        mazeNodes[randomx, randomY].savedX = randomx;
        mazeNodes[randomx, randomY].savedY = randomY;
        while (VisitedNodes.Count > 0)
        {
            currentSteps++;
            if (currentSteps >= StepsPerFrame)
            {
                currentSteps = 0;
            }
            // choose cell based on selection Function
            MazeNode currentNode = NodeChooser(VisitedNodes);
            List<Func<MazeNode, bool>> randomDirectionFunction = new List<Func<MazeNode, bool>>() { CheckNorth, CheckSouth, CheckEast, CheckWest };
            bool found = false;
            while (randomDirectionFunction.Count > 0)
            {
                int direction = randomInstance.Next(0, randomDirectionFunction.Count);

                if (randomDirectionFunction[direction](currentNode))
                {
                    found = true;
                    break;
                }
                //If not possible to go to direction, try again and remove direction from possible chosen directions
                randomDirectionFunction.RemoveAt(direction);
            }
            // no nodes found, removing current node and counting visited as true
            if (!found)
            {
                VisitedNodes.Remove(currentNode);
            }
        }
        generationFinished = true;

    }

    private IEnumerator GrowingTree(int seed)
    {
        VisitedNodes = new List<MazeNode>();
        randomInstance = new System.Random(seed);
        // add random starting cell
        int randomx = randomInstance.Next(0, savedXSize);
        int randomY = randomInstance.Next(0, savedYSize);
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
            MazeNode currentNode = NodeChooser(VisitedNodes);

            List<Func<MazeNode, bool>> randomDirectionFunction = new List<Func<MazeNode, bool>>() { CheckNorth, CheckSouth, CheckEast, CheckWest };
            bool found = false;
            while (randomDirectionFunction.Count > 0)
            {
                int direction = randomInstance.Next(0, randomDirectionFunction.Count);

                if (randomDirectionFunction[direction](currentNode))
                {
                    found = true;
                    break;
                }
                //If not possible to go to direction, try again and remove direction from possible chosen directions
                randomDirectionFunction.RemoveAt(direction);
            }
            // no nodes found, removing current node and counting visited as true
            if (!found)
            {
                VisitedNodes.Remove(currentNode);
            }

        }

        generationFinished = true;

    }



    public void RunGeneration()
    {
        // run generation based on options

        if (method == GenerationMethod.Couroutine)
        {
            RunGenerationCoroutine();
        }

        if (method == GenerationMethod.MultiThreaded)
        {
            StartThread();
        }
    }

    private void RunGenerationCoroutine()
    {

        GenerateNodeData();
        StartCoroutine(GrowingTree(seed));
    }

    private void RunGenerationThreaded()
    {
        GenerateNodeData();
        GrowingTreeThread(seed);
    }

    private void StartThread()
    {
        thread = new Thread(new ThreadStart(RunGenerationThreaded));
        thread.IsBackground = true;
        thread.Start();
    }


}
