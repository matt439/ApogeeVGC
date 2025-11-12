//using ApogeeVGC.Sim.BattleClasses;
//using ApogeeVGC.Sim.Effects;
//using ApogeeVGC.Sim.PokemonClasses;
//using ApogeeVGC.Sim.SideClasses;

//namespace ApogeeVGC.Sim.Events;

//public interface ISideEventMethods : IEventMethods
//{
//    /// <summary>
//    /// battle, target, source, sourceEffect
//    /// </summary>
//    Action<Battle, Side, Pokemon, IEffect>? OnSideStart { get; }

//    /// <summary>
//    /// battle, target, source, sourceEffect
//    /// </summary>
//    Action<Battle, Side, Pokemon, IEffect>? OnSideRestart { get; }

//    /// <summary>
//    /// battle, target, source, effect
//    /// </summary>
//    Action<Battle, Side, Pokemon, IEffect>? OnSideResidual { get; }

//    /// <summary>
//    /// battle, target
//    /// </summary>
//    Action<Battle, Side>? OnSideEnd { get; }
//    int? OnSideResidualOrder { get; }
//    int? OnSideResidualPriority { get; }
//    int? OnSideResidualSubOrder { get; }
//}