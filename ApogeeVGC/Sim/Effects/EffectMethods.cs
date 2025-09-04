using ApogeeVGC.Sim.Core;
using ApogeeVGC.Sim.Methods;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Unions;

namespace ApogeeVGC.Sim.Effects;

public record EffectMethods : ITestMethods
{
    public Action<Battle, int, Pokemon, Pokemon, IEffect>? OnSourceAfterFaint { get; init; }
    public Func<Battle, Pokemon, Pokemon, IEffect, bool>? OnStart { get; init; }
    public Func<Battle, int, Pokemon, Pokemon, Move, int>? OnModifyPriority { get; init; }
    public Action<Battle, Pokemon, Pokemon, IEffect>? OnTerrainChange { get; init; }
    public Action<Battle, Pokemon, Pokemon, IEffect>? OnResidual { get; init; }
    public int? OnResidualOrder { get; init; }
    public int? OnResidualSubOrder { get; init; }
    public Action<Battle, Pokemon, Pokemon, Move>? OnBeforeMove { get; init; }
    public int? OnBeforeMovePriority { get; init; }
    public Action<Battle, Pokemon>? OnDisableMove { get; init; }
    public Func<Battle, Pokemon, bool>? OnStallMove { get; init; }
    public Func<Battle, Pokemon, Pokemon, IEffect, bool>? OnRestart { get; init; }
    public Func<Battle, Pokemon, Pokemon, Move, BoolIntBoolUnion>? OnTryHit { get; init; }
    public int? OnTryHitPriority { get; init; }
    public Func<Battle, int, Pokemon, Pokemon, Move, double>? OnAnyModifyDamage { get; init; }
    public Action<Battle, int, Pokemon, Pokemon, Move>? OnDamagingHit { get; init; }
    public int? OnDamagingHitOrder { get; init; }
    public Action<Battle, Move, Pokemon, Pokemon?>? OnModifyMove { get; init; }
    public int? OnModifyMovePriority { get; init; }
    public Func<Battle, Pokemon, Pokemon, Move, bool>? OnTryImmunity { get; init; }
    public Func<Battle, Pokemon, Pokemon, Move, bool>? OnHit { get; init; }
    public Func<Battle, int, Pokemon, Pokemon, Move, double>? OnBasePower { get; init; }
    public int? OnBasePowerPriority { get; init; }
    public Func<Battle, Pokemon, Pokemon, Move, bool>? OnTry { get; init; }
    public Func<Battle, Pokemon, Pokemon, Move, IntFalseUnion>? BasePowerCallback { get; init; }
    public int? BasePowerCallbackPriority { get; init; }
    public Func<Battle, Pokemon, Pokemon, Move, bool>? OnPrepareHit { get; init; }
    public Func<Battle, int, Pokemon, Pokemon, Move, double>? OnModifyAtk { get; init; }
    public int? OnModifyAtkPriority { get; init; }
    public Func<Battle, int, Pokemon, Pokemon, Move, double>? OnModifyDef { get; init; }
    public int? OnModifyDefPriority { get; init; }
    public Func<Battle, int, Pokemon, Pokemon, Move, double>? OnModifySpA { get; init; }
    public int? OnModifySpAPriority { get; init; }
    public Func<Battle, int, Pokemon, Pokemon, Move, double>? OnModifySpD { get; init; }
    public int? OnModifySpDPriority { get; init; }
    public Func<Battle, int, Pokemon, double>? OnModifySpe { get; init; }
    public int? OnModifySpePriority { get; init; }

    //public EffectMethods CopyMethods()
    //{
    //    return this with { };
    //}
}