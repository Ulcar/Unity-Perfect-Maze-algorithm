using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// Interface to draw the Maze with.
/// Implement this interface on any script you want to render mazes with.
/// </summary>
public interface IMazeDrawer
{
    /// <summary>
    /// Removes wall at specified x and y position.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="direction"></param>
    void RemoveWall(int x, int y, Directions direction);

    /// <summary>
    /// Draws the base of the maze, drawing all walls as closed.
    /// </summary>
    /// <param name="drawMazeCallback">Function to be called when drawing maze is finished. 
    /// Used to call Generation after drawing is finished</param>
    void DrawMaze(MazeUIController.DrawMazeCallback drawMazeCallback);

    /// <summary>
    /// Disables this drawer. Used when switching drawing mode. 
    /// </summary>
    /// <param name="value"></param>
    void SetActive(bool value);

}
