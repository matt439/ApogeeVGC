using ApogeeVGC.Data;
using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Core;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.FieldClasses;
using ApogeeVGC.Sim.GameObjects;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Stats;
using ApogeeVGC.Sim.Ui;
using ApogeeVGC.Sim.Utils;


namespace ApogeeVGC.Sim.PokemonClasses;

public partial class Pokemon
{
    // Battle state properties
    public Dictionary<ConditionId, EffectState> Volatiles { get; } = [];
    public bool IsTeraUsed { get; private set; }
    public bool IgnoringItem => false;
    public Move? LastMoveUsed { get; set; }
    public StatIdExceptHp? BestStat { get; set; }
    public int ActiveMoveActions { get; set; }

    // HP and health management
    public int CurrentHp { get; private set; }
    public double CurrentHpRatio => (double)CurrentHp / UnmodifiedHp;
    public int CurrentHpPercentage => (int)Math.Ceiling(CurrentHpRatio * 100);
    public bool IsFainted => CurrentHp <= 0;

    public bool HasStatus => throw new NotImplementedException();

    

    // HP modification methods
    public int Heal(int amount)
    {
        int previousHp = CurrentHp;
        CurrentHp = Math.Min(CurrentHp + amount, UnmodifiedStats.Hp);
        return CurrentHp - previousHp;
    }

    public int Damage(int amount)
    {
        int previousHp = CurrentHp;
        CurrentHp = Math.Max(CurrentHp - amount, 0);
        return previousHp - CurrentHp;
    }

    // Condition management
    public bool TrySetStatus(Condition status, Pokemon? source, IEffect? sourceEffect)
    {
        throw new NotImplementedException();
    }

    public RelayVar AddVolatile(IBattle battle, Condition status, Pokemon? source = null,
        IEffect? sourceEffect = null, Condition? linkedStatus = null)
    {
        throw new NotImplementedException();
    }

    public EffectState? GetVolatile(ConditionId volatileId)
    {
        return Volatiles.GetValueOrDefault(volatileId);
    }

    public bool RemoveVolatile(IBattle battle, IEffect status)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Deletes a volatile condition without running the extra logic from RemoveVolatile
    /// </summary>
    public bool DeleteVolatile(ConditionId volatileId)
    {
        return Volatiles.Remove(volatileId);
    }

    public bool IgnoringAbility(IBattle battle)
    {
        throw new NotImplementedException();
    }

    

    public Pokemon Copy()
    {
        Pokemon copy = new(Specie, Evs, Ivs, Nature, Level, Trainer, SideId)
        {
            Moves = Moves.Select(m => m.Copy()).ToArray(),
            Item = Item,
            Ability = Ability,
            Name = Name,
            Shiny = Shiny,
            TeraType = TeraType,
            Gender = Gender,
            PrintDebug = PrintDebug,
            SlotId = SlotId,
        };

        int hpDifference = UnmodifiedHp - CurrentHp;
        if (hpDifference > 0)
        {
            copy.Damage(hpDifference);
        }

        copy.StatModifiers = StatModifiers.Copy();
        copy.LastMoveUsed = LastMoveUsed;
        copy.ActiveMoveActions = ActiveMoveActions;
        copy.IsTeraUsed = IsTeraUsed;
        copy.BestStat = BestStat;

        return copy;
    }
}