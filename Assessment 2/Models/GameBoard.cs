using static System.Console;

namespace Assessment_2.Models;

public class GameBoard
{
    private char[][] map;
    private (int, int) size;

    private readonly char EMPTY;

    public GameBoard((int, int) size, char emptyChar)
    {
        this.size = size;
        this.EMPTY = emptyChar;
        
        int rowSize = size.Item1, colSize = size.Item2;
        
        char[] colElements = new char[colSize]; 
        for (int n = 0; n < colSize; n++) 
            colElements[n] = emptyChar;

        map = new char[rowSize][];
        for (int n = 0; n < rowSize; n++)
            this.map[n] = colElements;
    }


    public char[][] GetMap()
    {
        return this.map;
    }


    public void UpdateMap(char[][] newMap)
    {
        this.map = newMap;
    }


    public (int, int) GetSize()
    {
        return this.size;
    }


    public void PlacePiece(char piece, int row, int col)
    {
        this.map[row][col] = piece;
    }


    public char GetMapChar(int row, int col)
    {
        return GetMap()[row][col];
    }


    public char GetEmptyChar()
    {
        return EMPTY;
    }


    public bool IsOutOfRange(int row, int col)
    {
        int rowSize = size.Item1, colSize = size.Item2;

        if (row < 0 || row >= rowSize)
        {
            WriteLine("Invalid move! Row index out of range. Please enter a valid position.");
            return true;
        }
        if (col < 0 || col >= colSize)
        {
            WriteLine("Invalid move! Column index out of range. Please enter a valid position.");
            return true;
        }
        return false;
    }
}