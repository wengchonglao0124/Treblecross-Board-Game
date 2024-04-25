namespace Assessment_2.Models;

public class MovementRecorder
{
    private List<char[][]> history = new List<char[][]>();

    public int NumOfMove
    {
        get { return history.Count; }
    }

    public char[][] GetMoveByIndex(int index)
    {
        return history.ElementAt(index);
    }


    public void StoreMove(char[][] move, int index)
    {
        history.Insert(index, move);
        history = history.GetRange(0, index + 1);
    }


    public void RemoveMoveRecord(int index)
    {
        history.RemoveAt(index);
    }
}