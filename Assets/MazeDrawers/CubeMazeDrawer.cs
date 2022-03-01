using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


/// <summary>
/// Draws the maze in a 3D Representation
/// </summary>
public class CubeMazeDrawer : MonoBehaviour, IMazeDrawer
{



    public GameObject WallPrefab;

    public MazeAlgorithmController RecursiveBacktracker;

    private bool MazeNeedsUpdate = false;
    private int population = 0;

    private int sideWallCount = 0;
    // Material to use for drawing the meshes.
    [SerializeField]
    private Material material;

    private Matrix4x4[][] matrices;

    private Matrix4x4[] sideWallMatrices;
    private MaterialPropertyBlock block;

    public Mesh mesh;

   private Vector3 verticalScale = new Vector3(1.5f, 1, 0.5f);
   private Vector3 horizontalScale = new Vector3(0.5f, 1, 1.5f);

    private MazeUIController.DrawMazeCallback callback;

    private bool drawn = false;


    private void Start()
    {
        RecursiveBacktracker = FindObjectOfType<MazeAlgorithmController>();

    }
    private void Update()
    {
        if (drawn)
        {
            int currentRenderPos = 0;

            int currentRenderCount = population;
            // possible Performance improvement: Using custom shaders and DrawMeshInstancedIndirect for bigger batches at once (Current limit is 1023 meshes per batch)
            // Could also try creating walls as one mesh instead
            while (currentRenderCount > 1023)
            {
                Graphics.DrawMeshInstanced(mesh, 0, material, matrices[currentRenderPos], 1023, block);
                currentRenderCount -= 1023;
                currentRenderPos += 1;
            }
            Graphics.DrawMeshInstanced(mesh, 0, material, matrices[currentRenderPos], currentRenderCount, block);

            Graphics.DrawMeshInstanced(mesh, 0, material, sideWallMatrices, sideWallCount, block);

        }



    }
    public void RemoveWall(int x, int y, Directions direction)
    {
        var mat = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, Vector3.zero);


        // find wall in one dimensional array
        int index = y;

        index += x * RecursiveBacktracker.savedYSize;

        index *= 2;

        // each X adds +1, each Y adds +1


        if (direction == Directions.South)
        {
        }
        else if (direction == Directions.East)
        {
            index += 1;
        }

        else if (direction == Directions.West)
        {
        }
        int instanceIndex = index / 1023;

        matrices[instanceIndex][index % 1023] = mat;
    }


    private IEnumerator SpawnAllWalls() //Creates maze with borders as cubes
    {



        for (int x = 0; x < RecursiveBacktracker.savedXSize; x++)
        {
            for (int y = 0; y < RecursiveBacktracker.savedYSize; y++)
            {

                PlaceWall(x, y, Directions.North, population);
                population++;
                PlaceWall(x, y, Directions.East, population);
                population++;
                //side walls
                if (y == 0)
                {
                    PlaceBottomWall(x, y, Directions.South);
                }

                if (x == 0)
                {
                    PlaceBottomWall(x, y, Directions.West);
                }
            }
            yield return null;
        }
        callback();

        yield return null;
    }


    private void PlaceBottomWall(int x, int y, Directions d)
    {
        Vector3 direction = (d == Directions.West) ? Vector3.right : Vector3.forward;
        Vector3 position = new Vector3(x, 0, y) + direction * 0.5f * -1;


        Vector3 scale = (d == Directions.West) ? horizontalScale : verticalScale;

        var mat = Matrix4x4.TRS(position, Quaternion.identity, scale);


        int index = y;

        if (y == 0)
        {
            index = RecursiveBacktracker.savedYSize + x;
        }

        if (x == 0 && y == 0)
        {
            index = 0;
        }


        if (d == Directions.West)
        {
            index += 1;
        }

        sideWallMatrices[index] = mat;
        sideWallCount++;
    }


    private void PlaceWall(int x, int y, Directions d, int index)
    {

        Vector3 direction = (d == Directions.East) ? Vector3.right : Vector3.forward;
        Vector3 position = new Vector3(x, 0, y) + direction * 0.5f * 1;

        Vector3 scale = (d == Directions.East) ? horizontalScale : verticalScale;

        var mat = Matrix4x4.TRS(position, Quaternion.identity, scale);

        int instanceIndex = index / 1023;

        matrices[instanceIndex][index % 1023] = mat;
    }

    public void DrawMaze()
    {
        matrices = new Matrix4x4[((RecursiveBacktracker.savedXSize * RecursiveBacktracker.savedYSize * 4) / 1023) + 1][];
        sideWallMatrices = new Matrix4x4[RecursiveBacktracker.savedXSize + RecursiveBacktracker.savedYSize];
        for (int i = 0; i < matrices.Length; i++)
        {
            matrices[i] = new Matrix4x4[1023];
        }
        population = 0;
        sideWallCount = 0;
        drawn = true;
        StartCoroutine(SpawnAllWalls());
    }

    public void DrawMaze(MazeUIController.DrawMazeCallback drawMazeCallback)
    {
        StopAllCoroutines();
        callback = drawMazeCallback;
        DrawMaze();
    }

    public void SetActive(bool value)
    {
        gameObject.SetActive(value);
    }
}
