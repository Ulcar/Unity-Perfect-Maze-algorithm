using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


   public interface IMazeDrawer
    {

    void RemoveWall(int x, int y, Directions direction);

    void DrawMaze(MazeUIController.DrawMazeCallback drawMazeCallback);

    }
