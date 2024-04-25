using System.Runtime.CompilerServices;
using Assessment_2.Models;
using static System.Console;
using System.Text.Json;

namespace Assessment_2.Utilities;

public static class GameFileHelper
{
    private const string DATA_FILE_FORMAT = ".json";
    
    public static void SaveFile(string fileName, BoardGameModel gameModel, Player player1, Player player2, int gameTurn, string gameDataFolder)
    {
        // Get the current working directory
        string currentDirectory = Directory.GetCurrentDirectory();

        // Moving into GameData folder
        string gameDataFolderPath = Path.Combine(currentDirectory, gameDataFolder);
        string gameDataPath = Path.Combine(gameDataFolderPath, $"{fileName}.json");
        
        if (!Directory.Exists(gameDataFolderPath))
            Directory.CreateDirectory(gameDataFolderPath);

        GameDataStruct dataStruct = new GameDataStruct(gameModel, player1, player2, gameTurn);
        File.WriteAllText(gameDataPath, JsonSerializer.Serialize(dataStruct));
        WriteLine($"\n>> Successfully saved the current game data into '{fileName}.json'");
        WriteLine($"Note : Game data files are located in '{gameDataFolder}' folder");
    }


    public static bool LoadFile(string fileName, string gameDataFolder, out GameDataStruct gameDataStruct)
    {
        gameDataStruct = new GameDataStruct();
        // Get the current working directory
        string currentDirectory = Directory.GetCurrentDirectory();

        // Moving into GameData folder
        string gameDataFolderPath = Path.Combine(currentDirectory, gameDataFolder);
        string gameDataPath = Path.Combine(gameDataFolderPath, $"{fileName}.json");

        if (!Directory.Exists(gameDataFolderPath))
        {
            WriteLine($"\nError Message : '{gameDataFolder}' folder does not exist");
            return false;
        }
        if (!File.Exists(gameDataPath))
        {
            WriteLine($"\nError Message : '{fileName}.json' does not exist in '{gameDataFolder}' folder");
            return false;
        }
        
        gameDataStruct = JsonSerializer.Deserialize<GameDataStruct>(File.ReadAllText(gameDataPath));
        WriteLine($"\n>> Successfully loaded '{fileName}.json' data into the current game");
        return true;
    }


    public static bool ConvertDataToModels(GameDataStruct gameDataStruct, out BoardGameModel gameModel, out Player player1, out Player player2, out int gameTurn)
    {
        gameModel = new TrebleCrossGameModel((1, 1));
        player1 = new Player('-', PlayerType.Computer);
        player2 = new Player('-', PlayerType.Computer);
        gameTurn = -1;
        
        string gameModelName = gameDataStruct.GameModelName;

        int rowSize = gameDataStruct.GameBoardRowSize, colSize = gameDataStruct.GameBoardColSize;
        switch (gameModelName)
        {
            case nameof(TrebleCrossGameModel):
                gameModel = new TrebleCrossGameModel((rowSize, colSize));
                char[][] gameMap = gameDataStruct.Map;
                gameModel.UpdateGameBoard(gameMap);
                break;
            
            // Reversi Game Model will be developed in the future version
            // case nameof(ReversiGameModel):
            //     break;
            
            default:
                WriteLine($"\nError Message : Cannot identify the Game Model Type in the file");
                return false;
        }

        PlayerType player1Type = gameDataStruct.Player1Type;
        switch (player1Type)
        {
            case PlayerType.Computer:
                player1 = new ComputerPlayer(gameDataStruct.Player1Piece);
                break;
            case PlayerType.Human:
                player1 = new HumanPlayer(gameDataStruct.Player1Piece, gameDataStruct.Player1Name);
                break;
        }
        
        PlayerType player2Type = gameDataStruct.Player2Type;
        switch (player2Type)
        {
            case PlayerType.Computer:
                player2 = new ComputerPlayer(gameDataStruct.Player2Piece);
                break;
            case PlayerType.Human:
                player2 = new HumanPlayer(gameDataStruct.Player2Piece, gameDataStruct.Player2Name);
                break;
        }

        gameTurn = gameDataStruct.GameTurn;
        WriteLine($">> Successfully convert game data to game models");
        return true;
    }


    public static void ListAllGameFiles(string gameDataFolder)
    {
        // Get the current working directory
        string currentDirectory = Directory.GetCurrentDirectory();

        // Moving into GameData folder
        string gameDataFolderPath = Path.Combine(currentDirectory, gameDataFolder);

        if (!Directory.Exists(gameDataFolderPath))
        {
            WriteLine($"\nError Message : '{gameDataFolder}' folder does not exist");
            return;
        }
        // Get all files in the GameData folder
        string[] files = Directory.GetFiles(gameDataFolderPath);
        files = files.Where(f => f.Contains(DATA_FILE_FORMAT)).ToArray();
        WriteLine($"\n>> There are {files.Length} game data files in '{gameDataFolder}' folder : ");
        foreach (string file in files)
        {
            string fileName = Path.GetFileName(file);
            WriteLine($"   {fileName}");
        }
        WriteLine("");
    }
}