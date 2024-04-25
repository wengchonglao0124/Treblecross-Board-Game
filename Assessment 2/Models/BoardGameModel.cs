using Assessment_2.Utilities;
using static System.Console;

namespace Assessment_2.Models;

public abstract class BoardGameModel
{
    private GameBoard map;
    protected const char EMPTY_CHAR = '-';
    private HelpSystem helpSystem;
    public bool HasWon = false;
    public char WonChar;

    public static List<char> PieceCharLibrary = new List<char> { '!', '@', '#', '$', '%', '^', '&', '*', '+', '?' };
    
    public BoardGameModel() 
    {
        this.map = CreateGameBoard();
        this.helpSystem = CreateHelpSystem();
    }


    public BoardGameModel((int, int) gameBoardSize)
    {
        this.map = new GameBoard(gameBoardSize, EMPTY_CHAR);
        this.helpSystem = CreateHelpSystem();
    }


    protected virtual GameBoard CreateGameBoard()
    {
        return new GameBoard((1, 1), EMPTY_CHAR);
    }
    
    public GameBoard GetGameBoard() { return this.map; }
    public void UpdateGameBoard(char[][] newMap) { map.UpdateMap(newMap); }

    protected virtual HelpSystem CreateHelpSystem()
    {
        return new HelpSystem("", new string[0], new string[0]);
    }
    
    public HelpSystem GetHelpSystem() { return helpSystem; }

    public virtual bool MakeMove(char piece, int row, int col, PlayerType type)
    {
        if (CheckValidMove(piece, row, col, type))
        {
            map.PlacePiece(piece, row, col);
            CheckWin();
            return true;
        }
        else
            return false;
    }

    protected abstract bool CheckValidMove(char piece, int row, int col, PlayerType type);
    public abstract bool DecodeMoveCommand(string command, out (int, int) move);
    protected abstract void CheckWin();

    protected int EmptySpaceCount()
    {
        int count = 0;
        foreach (char[] row in map.GetMap())
        {
            foreach (char col in row)
            {
                if (col == map.GetEmptyChar())
                    count++;
            }
        }
        return count;
    }

    
    protected virtual bool CheckTie()
    {
        if (EmptySpaceCount() == 0)
            return true;
        else
            return false;
    }
}


public class TrebleCrossGameModel : BoardGameModel
{
    private const int MIN_SIZE = 5;
    private const int ROW_SIZE = 1;
    
    public TrebleCrossGameModel() : base() { }
    public TrebleCrossGameModel((int, int) gameBoardSize) : base(gameBoardSize) { }
    
    protected override GameBoard CreateGameBoard()
    {
        WriteLine("\n========== TrebleCross ==========\n");
        int colSize = AskForMapSize();
        return new GameBoard((ROW_SIZE, colSize), EMPTY_CHAR);
    }


    private int AskForMapSize()
    {
        int inputSize;
        do
        {
            inputSize = 0;
            Write($"Please enter a valid size for TrebleCross game (size >= {MIN_SIZE}) >> ");
        } while (!int.TryParse(ReadLine(), out inputSize) || inputSize < MIN_SIZE);

        return inputSize;
    }
    

    protected override HelpSystem CreateHelpSystem()
    {
        string gameRule = "The game is won if a player on their turn makes a line of three pieces in a row";
        string[] availableCommands = {"place [At position index]"};
        string[] exampleOfUsages = {"'place 1' => Place a piece character at position index of 1"};
        return new HelpSystem(gameRule, availableCommands, exampleOfUsages);
    }
    

    protected override bool CheckValidMove(char piece, int row, int col, PlayerType type)
    {
        GameBoard gb = GetGameBoard();
        if (gb.IsOutOfRange(row, col))
            return false;
        
        if (gb.GetMapChar(row, col) != EMPTY_CHAR)
        {
            if (type == PlayerType.Human)
                WriteLine("Invalid move! This position is already placed by other piece. Please enter a valid position.");
            return false;
        }
        else
            return true;
    }


    public override bool DecodeMoveCommand(string command, out (int, int) move)
    {
        move = (0, -1);
        
        string[] commandList = command.Split(" ", 2);
        if (commandList.Length != 2)
        {
            WriteLine("Invalid command!");
            return false;
        }
        
        int colNum;
        if (commandList[0] != "place" || !int.TryParse(commandList[1], out colNum))
        {
            WriteLine("Invalid command!");
            return false;
        }

        move.Item2 = colNum;
        return true;
    }


    protected override void CheckWin()
    {
        GameBoard currentGB = GetGameBoard();
        // Only having one row for TrebleCross Game Model
        int rowIndex = ROW_SIZE - 1;
        int colIndexSize = currentGB.GetSize().Item2;
        for (int colIndex = 0; colIndex < colIndexSize; colIndex++)
        {
            char mapChar = currentGB.GetMapChar(rowIndex, colIndex);
            if (mapChar != EMPTY_CHAR)
            {
                // Checking the next two map characters
                if (colIndex < colIndexSize - 2)
                {
                    char mapChar1 = currentGB.GetMapChar(rowIndex, colIndex + 1);
                    char mapChar2 = currentGB.GetMapChar(rowIndex, colIndex + 2);
                    if (mapChar == mapChar1 && mapChar == mapChar2)
                    {
                        HasWon = true;
                        WonChar = mapChar;
                        return;
                    }
                }
            }
        }
        // Tie
        if (CheckTie())
        {
            HasWon = true;
            WonChar = EMPTY_CHAR;
        }
    }
}