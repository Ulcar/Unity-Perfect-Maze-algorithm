using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class MazeDrawer : MonoBehaviour
{



    public GameObject wallPrefab;

    public RecursiveBacktracker recursiveBacktracker;

    private bool MazeNeedsUpdate = false;
    private int population = 0;

    private int sideWallCount = 0;
    // Material to use for drawing the meshes.
    public Material material;

    private Matrix4x4[][] matrices;

    private Matrix4x4[] sideWallMatrices;
    private MaterialPropertyBlock block;

    public Mesh mesh;

    Vector3 VerticalScale = new Vector3(1.5f, 1, 0.5f);
    Vector3 HorizontalScale = new Vector3(0.5f, 1, 1.5f);

    private ComputeBuffer meshPropertiesBuffer;
    private ComputeBuffer argsBuffer;


    private struct MeshProperties
    {
        public Matrix4x4 mat;
        public Vector4 color;

        public static int Size()
        {
            return
                sizeof(float) * 4 * 4 + // matrix;
                sizeof(float) * 4;      // color;
        }
    }


    private void Start()
    {
        recursiveBacktracker = GetComponent<RecursiveBacktracker>();
        matrices = new Matrix4x4[((recursiveBacktracker.mazeXSize * recursiveBacktracker.mazeYSize * 4) / 1023) + 1][];
        sideWallMatrices = new Matrix4x4[recursiveBacktracker.mazeXSize + recursiveBacktracker.mazeYSize];
        for (int i = 0; i < matrices.Length; i++)
        {
            matrices[i] = new Matrix4x4[1023];
        }



        StartCoroutine(SpawnAllWalls());
    }

    private void Update()
    {



        int currentRenderPos = 0;

        int currentRenderCount = population;
        // possible Performance improvement: Using custom shaders and DrawMeshInstancedIndirect for bigger batches at once (Current limit is 1023 meshes)
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
    public void RemoveWall(int x, int y, Directions direction)
    {
        var mat = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, Vector3.zero);


        // find wall in one dimensional array
        int index = y;

        index += x * recursiveBacktracker.mazeYSize;

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


    IEnumerator SpawnAllWalls() //Creates maze with borders as cubes
    {



        for (int x = 0; x < recursiveBacktracker.mazeXSize; x++)
        {
            for (int y = 0; y < recursiveBacktracker.mazeYSize; y++)
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

        yield return null;
    }


    void PlaceBottomWall(int x, int y, Directions d)
    {
        Vector3 direction = (d == Directions.West) ? Vector3.right : Vector3.forward;
        Vector3 position = new Vector3(x, 0, y) + direction * 0.5f * -1;


        Vector3 scale = (d == Directions.West) ? HorizontalScale : VerticalScale;

        var mat = Matrix4x4.TRS(position, Quaternion.identity, scale);


        int index = y;

        if (y == 0) 
        {
            index = recursiveBacktracker.mazeYSize + x;
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


    void PlaceWall(int x, int y, Directions d, int index) 
    {                                                   

        Vector3 direction = (d == Directions.East) ? Vector3.right : Vector3.forward;
        Vector3 position = new Vector3(x, 0, y) + direction * 0.5f * 1;

        Vector3 scale = (d == Directions.East) ? HorizontalScale: VerticalScale;

        var mat = Matrix4x4.TRS(position, Quaternion.identity, scale);

        int instanceIndex = index / 1023;

        matrices[instanceIndex][index % 1023] = mat;
    }


}
