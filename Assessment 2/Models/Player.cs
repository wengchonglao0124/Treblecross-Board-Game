using Assessment_2.Utilities;
using static System.Console;
    
namespace Assessment_2.Models;

public class Player
{
    private char pieceChar;
    public readonly PlayerType Type;
    
    public Player(char pieceChar, PlayerType type)
    {
        this.pieceChar = pieceChar;
        this.Type = type;
    }
    
    
    public char GetPieceChar()
    {
        return pieceChar;
    }


    public virtual void PlacePiece(BoardGameModel gameModel, int row, int col, out bool isSucceeded)
    {
        isSucceeded = gameModel.MakeMove(pieceChar, row, col, Type);
    }
}


public class ComputerPlayer : Player
{
    public ComputerPlayer(char pieceChar) : base(pieceChar, PlayerType.Computer) { }


    public (int, int) PickRandomMove((int, int) size)
    {
        int row, col;
        int rowLimit = size.Item1, colLimit = size.Item2;

        Random r = new Random();
        row = r.Next(0, rowLimit);
        col = r.Next(0, colLimit);

        return (row, col);
    }
}


public class HumanPlayer : Player
{
    public string Name { get; set; }
    
    public HumanPlayer(char pieceChar, string name) : base(pieceChar, PlayerType.Human)
    {
        this.Name = name;
    }


    public string InputCommand()
    {
        Write("Please enter a valid command >> ");
        return ReadLine();
    }
}