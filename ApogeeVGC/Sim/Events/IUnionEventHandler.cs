namespace ApogeeVGC.Sim.Events;

public interface IUnionEventHandler
{
    Delegate? GetDelegate();
    bool IsConstant();
    object? GetConstantValue();
}