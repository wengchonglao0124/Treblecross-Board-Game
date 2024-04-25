namespace Assessment_2.Models;

public class HelpSystem
{
    public readonly string GameRule;
    public readonly string[] AvailableCommands;
    public readonly string[] ExampleOfUsages;

    public HelpSystem(string gameRule, string[] availableCommands, string[] exampleOfUsages)
    {
        GameRule = gameRule;
        AvailableCommands = availableCommands;
        ExampleOfUsages = exampleOfUsages;
    }
}