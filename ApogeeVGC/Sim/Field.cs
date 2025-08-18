namespace ApogeeVGC.Sim;

public class Field
{
    /// <summary>
    /// Creates a copy of this Field for MCTS simulation purposes.
    /// Currently Field is empty, so just create a new instance.
    /// </summary>
    /// <returns>A new Field instance</returns>
    public Field Copy()
    {
        // TODO: When Field gets properties, implement proper copying here
        return new Field();
    }
}