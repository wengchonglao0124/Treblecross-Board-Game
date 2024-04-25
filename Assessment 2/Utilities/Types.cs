namespace Assessment_2.Utilities;

public enum BasicCommandType
{
    NEW = 0, // New game
    END = 1, // End the current game
    help = 2, // Get help
    redo = 3, // Redo move
    undo = 4, // Undo move
    save = 5, // Save game to file
    load = 6, // Load game from file
    list = 7 // List all game files
}


public enum GameModelType
{
    TrebleCross = 1,
    Reversi = 2,
}


public enum PlayerType
{
    Computer = 1,
    Human = 2
}