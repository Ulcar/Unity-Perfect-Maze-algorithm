using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// class containing data for a Node in the maze.
/// </summary>
public class MazeNode
{
    /// <summary>
    /// bitmask of walls
    /// </summary>
    public int Walls;
    /// <summary>
    /// Is the node visited during the algorithm
    /// </summary>
    public bool visited;
    /// <summary>
    /// Saving X and Y positions in struct for easier access in algorithm
    /// </summary>
    public int savedX, savedY;
}


public enum Directions
{
    North = 0x01, South = 0x02, West = 0x04, East = 0x08
}
