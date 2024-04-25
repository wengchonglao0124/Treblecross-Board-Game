using Assessment_2.Controllers;

namespace Assessment_2;

/*
 * IFN563 Assessment 2 - Game Implementation (TrebleCross Game only)
 * Team 10
 * Developers: Weng Chong LAO (n11679719) & Zhiyun Pan (n9319638)
 */
class Game
{
    static void Main(string[] args)
    {
        GameProcessController gameController = new GameProcessController();
        gameController.StartGame();
    }
}