using ApogeeVGC.Sim;

namespace ApogeeVGC.Player;

public enum PlayerId
{
    Player1,
    Player2,
    None,
}

public enum Choice
{
    Move1,
    Move2,
    Move3,
    Move4,

    Switch1,
    Switch2,
    Switch3,
    Switch4,
    Switch5,
    Switch6,

    Select1,
    Select2,
    Select3,
    Select4,
    Select5,
    Select6,

    TargetSlot1,
    TargetSlot2,
    TargetSlot3,
    TargetSlot4,

    TargetSide1,
    TargetSide2,

    Quit,
    None,
    Invalid,
}

public static class ChoiceTools
{
    public static string GetChoiceName(this Choice choice)
    {
        return choice switch
        {
            Choice.Move1 => "Move 1",
            Choice.Move2 => "Move 2",
            Choice.Move3 => "Move 3",
            Choice.Move4 => "Move 4",
            Choice.Switch1 => "Switch 1",
            Choice.Switch2 => "Switch 2",
            Choice.Switch3 => "Switch 3",
            Choice.Switch4 => "Switch 4",
            Choice.Switch5 => "Switch 5",
            Choice.Switch6 => "Switch 6",
            Choice.Select1 => "Select 1",
            Choice.Select2 => "Select 2",
            Choice.Select3 => "Select 3",
            Choice.Select4 => "Select 4",
            Choice.Select5 => "Select 5",
            Choice.Select6 => "Select 6",
            Choice.Quit => "Quit",
            Choice.None => "None",
            Choice.Invalid => "Invalid",
            _ => throw new ArgumentOutOfRangeException(nameof(choice), choice, "Invalid choice value.")
        };
    }

    public static Choice GetChoiceFromMoveIndex(this int index)
    {
        if (index is < 0 or > 3)
        {
            throw new ArgumentOutOfRangeException(nameof(index), "Move index must be between 0 and 3.");
        }
        return (Choice)(index + (int)Choice.Move1);
    }

    public static int GetMoveIndexFromChoice(this Choice choice)
    {
        if (choice is < Choice.Move1 or > Choice.Move4)
        {
            throw new ArgumentOutOfRangeException(nameof(choice), "Choice must be a valid move choice.");
        }
        return (int)choice - (int)Choice.Move1;
    }

    public static bool IsMoveChoice(this Choice choice)
    {
        return choice is >= Choice.Move1 and <= Choice.Move4;
    }

    public static Choice GetChoiceFromSwitchIndex(this int index)
    {
        if (index is < 0 or > 5)
        {
            throw new ArgumentOutOfRangeException(nameof(index), "Switch index must be between 0 and 5.");
        }
        return (Choice)(index + (int)Choice.Switch1);
    }
    public static int GetSwitchIndexFromChoice(this Choice choice)
    {
        if (choice is < Choice.Switch1 or > Choice.Switch6)
        {
            throw new ArgumentOutOfRangeException(nameof(choice), "Choice must be a valid switch choice.");
        }
        return (int)choice - (int)Choice.Switch1;
    }

    public static bool IsSwitchChoice(this Choice choice)
    {
        return choice is >= Choice.Switch1 and <= Choice.Switch6;
    }

    public static Choice GetChoiceFromSelectIndex(this int index)
    {
        if (index is < 0 or > 5)
        {
            throw new ArgumentOutOfRangeException(nameof(index), "Select index must be between 0 and 5.");
        }
        return (Choice)(index + (int)Choice.Select1);
    }

    public static int GetSelectIndexFromChoice(this Choice choice)
    {
        if (choice is < Choice.Select1 or > Choice.Select6)
        {
            throw new ArgumentOutOfRangeException(nameof(choice), "Choice must be a valid select choice.");
        }
        return (int)choice - (int)Choice.Select1;
    }

    public static bool IsSelectChoice(this Choice choice)
    {
        return choice is >= Choice.Select1 and <= Choice.Select6;
    }
}

public class ChoiceDetails
{

}

public class PlayerChoices
{
    public PlayerId PlayerId { get; init; }
    public Dictionary<Choice, ChoiceDetails> Choices { get; init; } = [];
}


public interface IPlayer
{
    //int GetInputCommand();  
    //void InputCurrentPlayerChoices(PlayerChoices choices);

    PlayerId PlayerId { get; }

    Choice GetNextChoice(Choice[] availableChoices);

    //void InputBattle(Battle battle);
}