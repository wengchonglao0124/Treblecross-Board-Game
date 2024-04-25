using Assessment_2.Models;
using Assessment_2.Views;
using Assessment_2.Utilities;
using static System.Console;

namespace Assessment_2.Controllers;

public class GameProcessController
{
    private bool isPlaying = true;
    private BoardGameModel gameModel;

    private const int NUM_OF_PLAYER = 2;
    private Player player1;
    private Player player2;

    private int gameTurn;
    private MovementRecorder moveHistory;

    private char[][] currentMap
    {
        get
        {
            char[][] map = gameModel.GetGameBoard().GetMap();
            char[][] mapCopy = map.Clone() as char[][];
            for (int rowIndex = 0; rowIndex < map.Length; rowIndex++)
            {
                mapCopy[rowIndex] = map[rowIndex].Clone() as char[];
            }
            return mapCopy;
        }
    }

    private const string GAME_DATA_FOLDER = "GameData";

    private readonly Dictionary<BasicCommandType, (string, string)> BASIC_COMMANDS = new Dictionary<BasicCommandType, (string, string)>()
    {
        [BasicCommandType.NEW] = ("NEW", "'NEW' => Start a new game"),
        [BasicCommandType.END] = ("END", "'END' => End the program"),
        [BasicCommandType.help] = ("help", "'help' => Display help instruction of the game"),
        [BasicCommandType.redo] = ("redo", "'redo' => Redo a move"),
        [BasicCommandType.undo] = ("undo", "'undo' => Undo the last move"),
        [BasicCommandType.save] = ("save [At file name]", $"'save gameData1' => Save the current game info into 'gameData1.json' which is located in '{GAME_DATA_FOLDER}' folder"),
        [BasicCommandType.load] = ("load [From file name]", $"'load gameData1' => Load the game data from the 'gameData1.json' file which is located in '{GAME_DATA_FOLDER}' folder"),
        [BasicCommandType.list] = ("list", $"'list' => List all game data files which are located in '{GAME_DATA_FOLDER}' folder")
    };
    
    public GameProcessController() { }
    

    public void StartGame()
    {
        while (isPlaying)
        {
            NewGame();
            do
            {
                UpdateView();
                NextGameTurn();
            } while (!gameModel.HasWon && isPlaying);
        
            if (gameModel.HasWon)
                DisplayWinner();
        }
    }


    private void NewGame()
    {
        gameTurn = 0;
        moveHistory = new MovementRecorder();
        
        WriteLine("\n========== Welcome to Amazing Board Game World! ==========\n");
        SetupGameModel();
        SetupPlayers();
        WriteLine(">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>");
        WriteLine("Game Start!!!\n[Note: Make sure your window is long enough]");
    }


    private void SetupGameModel()
    {
        WriteLine("Please select one of the following Game Models to continue :");
        WriteLine("[Note : enter the ID number of the Game (e.g. 1)]\n");
        foreach (int value in Enum.GetValues(typeof(GameModelType)))
        {
            WriteLine($"    {value} : {Enum.GetName(typeof(GameModelType), value)}");
        }
        WriteLine("");
        
        int inputNum = -1;
        bool isSelectedGameModel = false;
        bool isValidNumber = false;
        do
        {
            Write("Please enter a valid number >> ");
            isSelectedGameModel = int.TryParse(ReadLine(), out inputNum);
            
            if (isSelectedGameModel)
                isValidNumber = Enum.IsDefined(typeof(GameModelType), inputNum);
                // Reversi is not implemented in this assessment
                if (inputNum == 2)
                {
                    WriteLine("    Reversi is not ready yet, it will be available in the future version.");
                    inputNum = -1;
                    isValidNumber = false;
                }
        } while (!isSelectedGameModel || !isValidNumber);

        switch ((GameModelType) inputNum)
        {
            case GameModelType.TrebleCross:
                gameModel = new TrebleCrossGameModel();
                break;
            
            case GameModelType.Reversi:
            // Reversi will be developed in the future version
            break;
        }
    }


    private void SetupPlayers()
    {
        WriteLine("\nPlayers Setup : ");
        
        List<char> pieceCharLibrary = new List<char>(BoardGameModel.PieceCharLibrary);
        for (int n = 0; n < NUM_OF_PLAYER; n++)
        {
            WriteLine("");
            WriteLine($"Please select one of the following Player Type for Player {n + 1} :");
            WriteLine("[Note : enter the ID number of the Player Type (e.g. 1)]\n");
            foreach (int value in Enum.GetValues(typeof(PlayerType)))
            {
                WriteLine($"    {value} : {Enum.GetName(typeof(PlayerType), value)}");
            }
            
            WriteLine("");
            WriteLine($"=== Player {n + 1} ===");
            
            int inputNum = -1;
            bool isSelectedPlayerType = false;
            bool isValidNumber = false;
            do
            {
                Write($"Please enter a valid number for Player {n + 1} >> ");
                isSelectedPlayerType = int.TryParse(ReadLine(), out inputNum);
            
                if (isSelectedPlayerType)
                    isValidNumber = Enum.IsDefined(typeof(PlayerType), inputNum);
            } while (!isSelectedPlayerType || !isValidNumber);
            WriteLine("");

            // Display Piece Char Library for users to select
            WriteLine("Available Piece Characters : ");
            for (int i = 0; i < pieceCharLibrary.Count; i++)
            {
                if (i % 3 == 0)
                    WriteLine("");
                Write($"    {i} : {pieceCharLibrary.ElementAt(i)}       ");
            }
            WriteLine("\n");
            
            // Ask for char input
            int inputCharNum;
            do
            {
                inputCharNum = -1;
                Write($"Please enter the valid ID of piece character for this {Enum.GetName(typeof(PlayerType), inputNum)} Player (e.g. 0) >> ");
            } while (!int.TryParse(ReadLine(), out inputCharNum) || inputCharNum < 0 || inputCharNum >= pieceCharLibrary.Count);
            
            char piece = pieceCharLibrary.ElementAt(inputCharNum);
            pieceCharLibrary.RemoveAt(inputCharNum);

            Player player;
            switch ((PlayerType) inputNum)
            {
                case PlayerType.Computer:
                    player = new ComputerPlayer(piece);
                    break;
            
                case PlayerType.Human:
                    Write("Please enter a name for this Human Player >> ");
                    player = new HumanPlayer(piece, ReadLine());
                    break;
                
                default:
                    player = new ComputerPlayer('.');
                    break;
            }
            if (n == 0)
                player1 = player;
            else
                player2 = player;
        }
    }


    private void NextGameTurn()
    {
        Player player;
        if (gameTurn % NUM_OF_PLAYER == 0)
            player = player1;
        else
            player = player2;

        int playerNum = gameTurn % NUM_OF_PLAYER == 0 ? 1 : 2;
        WriteLine($"Game Turn #{gameTurn} => Player {playerNum} move :");

        char[][] currentMapCopy = currentMap;
        
        bool isMoveSucceeded = false;
        switch (player.Type)
        {
            case PlayerType.Computer:
                ComputerPlayer computerPlayer = (ComputerPlayer) player;

                (int, int) mapSize = gameModel.GetGameBoard().GetSize();
                (int, int) randomMove;
                do
                {
                    randomMove = computerPlayer.PickRandomMove(mapSize);
                    int row = randomMove.Item1, col = randomMove.Item2;
                    computerPlayer.PlacePiece(gameModel, row, col, out isMoveSucceeded);
                } while (!isMoveSucceeded);
                WriteLine($"Computer Player Move : {randomMove}");
                break;
            
            case PlayerType.Human:
                HumanPlayer humanPlayer = (HumanPlayer) player;

                string inputCommand;
                (int, int) inputMove;
                do
                {
                    inputCommand = humanPlayer.InputCommand();
                    
                    // Check other basic commands
                    if (CheckInputCommand(inputCommand))
                        return;
                    
                    if (gameModel.DecodeMoveCommand(inputCommand, out inputMove))
                    {
                        int row = inputMove.Item1, col = inputMove.Item2;
                        humanPlayer.PlacePiece(gameModel, row, col, out isMoveSucceeded);
                    }
                } while (!isMoveSucceeded);
                WriteLine($"Human Player Move : {inputMove}");
                break;
        }
        WriteLine(">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>");
        moveHistory.StoreMove(currentMapCopy, gameTurn);
        gameTurn++;
    }


    private bool CheckInputCommand(string command)
    {
        // Basic commands check only
        string singleCommandString = String.Concat(command.Where(c => !Char.IsWhiteSpace(c)));
        // Help Command check
        if (singleCommandString == BasicCommandType.help.ToString())
        {
            GetHelp();
            return true;
        } 
        else if (singleCommandString == BasicCommandType.undo.ToString())
        {
            return UndoMove();
        }
        else if (singleCommandString == BasicCommandType.redo.ToString())
        {
            return RedoMove();
        }
        else if (singleCommandString == BasicCommandType.list.ToString())
        {
            ListAllGameFiles();
            return true;
        }
        else if (singleCommandString == BasicCommandType.NEW.ToString())
        {
            NewGame();
            return true;
        }
        else if (singleCommandString == BasicCommandType.END.ToString())
        {
            EndGame();
            return true;
        }

        string[] doubleCommandString = command.Split(" ", 2);
        if (doubleCommandString.Length == 2)
        {
            singleCommandString = doubleCommandString[0];
            if (singleCommandString == BasicCommandType.save.ToString())
            {
                string fileName = doubleCommandString[1];
                SaveFile(fileName);
                return true;
            }
            else if (singleCommandString == BasicCommandType.load.ToString())
            {
                string fileName = doubleCommandString[1];
                return LoadFile(fileName);
            }
        }
        return false;
    }


    private void UpdateView()
    {
        TextBasedView.DisplayGameInfo(gameModel.GetGameBoard(), player1, player2);
    }


    private void DisplayWinner()
    {
        if (gameModel.WonChar == gameModel.GetGameBoard().GetEmptyChar())
            TextBasedView.DisplayTieInfo(gameModel.GetGameBoard(), player1, player2);
        else
        {
            int winnerNum = gameModel.WonChar == player1.GetPieceChar() ? 1 : 2;
            TextBasedView.DisplayWinnerInfo(gameModel.GetGameBoard(), player1, player2, winnerNum);
        }
        
        string userInput;
        do
        {
            Write("Do you want to reset the game (y/n) >> ");
            userInput = ReadLine();
        } while (userInput != "y" && userInput != "n");

        if (userInput == "n")
            EndGame();
    }


    private void GetHelp()
    {
        HelpSystem helpSystem = gameModel.GetHelpSystem();
        string rule = helpSystem.GameRule;
        string[] availableCommands = helpSystem.AvailableCommands;
        string[] exampleOfUsages = helpSystem.ExampleOfUsages;
        int numOfCommands = helpSystem.AvailableCommands.Length;
        
        Dictionary<string, string> commandInfo = new Dictionary<string, string>();
        for (int index = 0; index < numOfCommands; index++)
        {
            // Game Model Commands
            commandInfo[availableCommands[index]] = exampleOfUsages[index];
        }
        foreach (var basicCommand in BASIC_COMMANDS)
        {
            // Basic Commands
            (string, string) basicCommandInfo = basicCommand.Value;
            commandInfo[basicCommandInfo.Item1] = basicCommandInfo.Item2;
        }
        TextBasedView.DisplayHelp(rule, commandInfo);
    }


    private bool UndoMove()
    {
        if (moveHistory.NumOfMove != 0 && gameTurn - 2 >= 0)
        {
            if (gameTurn == moveHistory.NumOfMove)
            {
                moveHistory.StoreMove(currentMap, gameTurn);
            }
            char[][] lastMap = moveHistory.GetMoveByIndex(gameTurn - 2);
            if (lastMap.Length == 0)
            {
                WriteLine($"\nError Message : You do not have any move to undo");
                return false;
            }
            gameModel.GetGameBoard().UpdateMap(lastMap);
            gameTurn -= 2;
            WriteLine($">> Successfully undo your last move");
            return true;
        }
        WriteLine($"\nError Message : You do not have any move to undo");
        return false;
    }


    private bool RedoMove()
    {
        if (moveHistory.NumOfMove > gameTurn + 2)
        {
            char[][] lastMap = moveHistory.GetMoveByIndex(gameTurn + 2);
            gameModel.GetGameBoard().UpdateMap(lastMap);
            gameTurn += 2;
            WriteLine($">> Successfully redo to your last move");
            return true;
        }
        WriteLine($"\nError Message : You do not have any move to redo");
        return false;
    }


    private void SaveFile(string fileName)
    {
        GameFileHelper.SaveFile(fileName, gameModel, player1, player2, gameTurn, GAME_DATA_FOLDER);
    }


    private bool LoadFile(string fileName)
    {
        GameDataStruct gameDataStruct;
        bool isLoadedFromFile = GameFileHelper.LoadFile(fileName, GAME_DATA_FOLDER, out gameDataStruct);
        if (isLoadedFromFile)
        {
            BoardGameModel newGameModel;
            Player newPlayer1;
            Player newPlayer2;
            int newGameTurn;
            if (GameFileHelper.ConvertDataToModels(gameDataStruct, out newGameModel, out newPlayer1, out newPlayer2, out newGameTurn))
            {
                gameModel = newGameModel;
                player1 = newPlayer1;
                player2 = newPlayer2;

                moveHistory = new MovementRecorder();
                if (newGameTurn % 2 == 0)
                    gameTurn = 0;
                else if (newGameTurn % 2 == 1)
                {
                    moveHistory.StoreMove(new char[0][], 0);
                    gameTurn = 1;
                }
                return true;
            }
        }
        return false;
    }


    private void ListAllGameFiles()
    {
        GameFileHelper.ListAllGameFiles(GAME_DATA_FOLDER);
    }
    
    
    private void EndGame()
    {
        isPlaying = false;
    }
}