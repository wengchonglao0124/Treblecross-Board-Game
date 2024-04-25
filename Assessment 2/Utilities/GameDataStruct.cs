using Assessment_2.Models;

namespace Assessment_2.Utilities;

public struct GameDataStruct
{
    public string GameModelName { get; set; }
    public int GameBoardRowSize { get; set; }
    public int GameBoardColSize { get; set; }
    public char[][] Map { get; set; }
    
    public char Player1Piece { get; set; }
    public PlayerType Player1Type { get; set; }
    public string Player1Name { get; set; }
    
    public char Player2Piece { get; set; }
    public PlayerType Player2Type { get; set; }
    public string Player2Name { get; set; }
    
    public int GameTurn { get; set; }
    
    public GameDataStruct(BoardGameModel gameModel, Player player1, Player player2, int gameTurn)
    {
        GameModelName = gameModel.GetType().Name;
        (int, int) gameBoardSize = gameModel.GetGameBoard().GetSize();
        GameBoardRowSize = gameBoardSize.Item1;
        GameBoardColSize = gameBoardSize.Item2;
        Map = gameModel.GetGameBoard().GetMap();
        
        Player1Piece = player1.GetPieceChar();
        Player1Type = player1.Type;
        if (player1.Type == PlayerType.Human)
        {
            HumanPlayer humanPlayer1 = (HumanPlayer) player1;
            Player1Name = humanPlayer1.Name;
        }
        
        Player2Piece = player2.GetPieceChar();
        Player2Type = player2.Type;
        if (player2.Type == PlayerType.Human)
        {
            HumanPlayer humanPlayer2 = (HumanPlayer) player2;
            Player2Name = humanPlayer2.Name;
        }
        
        GameTurn = gameTurn;
    }
}