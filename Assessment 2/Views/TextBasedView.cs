using Assessment_2.Models;
using Assessment_2.Utilities;
using static System.Console;

namespace Assessment_2.Views;

public static class TextBasedView
{
    public static void DisplayGameInfo(GameBoard gameBoard, Player player1, Player player2)
    {
        DisplayGameBoard(gameBoard);
        DisplayPlayerInfo(player1, player2);
        WriteLine("        Enter 'help' to display game rule and command instructions\n");
    }


    public static void DisplayWinnerInfo(GameBoard gameBoard, Player player1, Player player2, int winnerNum)
    {
        DisplayGameBoard(gameBoard);
        DisplayPlayerInfo(player1, player2);
        WriteLine($"\n        >> Congratulation! Player {winnerNum} has won this game!\n");
    }
    
    
    public static void DisplayTieInfo(GameBoard gameBoard, Player player1, Player player2)
    {
        DisplayGameBoard(gameBoard);
        DisplayPlayerInfo(player1, player2);
        WriteLine("\n        >> This game is a tie! There is no empty space left\n");
    }


    private static void DisplayGameBoard(GameBoard gameBoard)
    {
        (int, int) size = gameBoard.GetSize();
        int rowSize = size.Item1, colSize = size.Item2;
        char emptyChar = gameBoard.GetEmptyChar();
        
        for (int rowIndex = 0; rowIndex < rowSize; rowIndex++)
        {
            string indexInfo = "        ";
            string headerCover = "        ";
            
            string topCover = "        |";
            string colString = $"    {rowIndex}   |";
            string bottomCover = "        |";
            
            
            for (int colIndex = 0; colIndex < colSize; colIndex++)
            {
                if (rowIndex == 0)
                {
                    headerCover += " _______";
                    indexInfo += $"    {colIndex}   ";
                }
                char mapChar = gameBoard.GetMapChar(rowIndex, colIndex);
                string mainChar = mapChar != emptyChar ? Char.ToString(mapChar) : " ";
                colString += $"   {mainChar}   |";

                topCover += "       |";
                bottomCover += "_______|";
            }
            if (rowIndex == 0)
            {
                WriteLine("");
                WriteLine(indexInfo);
                WriteLine(headerCover);
            }
            WriteLine(topCover);
            WriteLine(colString);
            WriteLine(bottomCover);
        }
    }


    private static void DisplayPlayerInfo(Player player1, Player player2)
    {
        string player1Name = "";
        if (player1.Type == PlayerType.Human)
        {
            HumanPlayer humanPlayer1 = (HumanPlayer) player1;
            player1Name += " - " + humanPlayer1.Name;
        }
        string player2Name = "";
        if (player2.Type == PlayerType.Human)
        {
            HumanPlayer humanPlayer2 = (HumanPlayer) player2;
            player2Name += " - " + humanPlayer2.Name;
        }
        WriteLine($"        Player 1 ({player1.Type.ToString()}){player1Name} : '{player1.GetPieceChar()}'");
        WriteLine($"        Player 2 ({player2.Type.ToString()}){player2Name} : '{player2.GetPieceChar()}'");
    }


    public static void DisplayHelp(string rule, Dictionary<string, string> commandInfo)
    {
        WriteLine("\nRule : " + rule);
        foreach (var command in commandInfo)
        {
            WriteLine(String.Format("     >> {0, -30}{1, -10}{2, -5}", command.Key, "|Example : ", command.Value));
        }
        WriteLine("");
    }
}