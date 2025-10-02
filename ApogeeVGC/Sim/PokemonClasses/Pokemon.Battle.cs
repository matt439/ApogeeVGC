using ApogeeVGC.Sim.Core;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.FieldClasses;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.Stats;
using ApogeeVGC.Sim.Ui;
using ApogeeVGC.Sim.Utils;


namespace ApogeeVGC.Sim.PokemonClasses;

public partial class Pokemon
{
    // Battle state properties
    public List<Condition> Conditions { get; init; } = [];
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
    public bool HasCondition(ConditionId conditionId)
    {
        return Conditions.Any(c => c.Id == conditionId);
    }

    public Condition? GetCondition(ConditionId conditionId)
    {
        return Conditions.FirstOrDefault(c => c.Id == conditionId);
    }

    public bool TrySetStatus(Condition status, Pokemon? source, IEffect? sourceEffect)
    {
        throw new NotImplementedException();
    }

    public RelayVar AddVolatile(ICondition status, Pokemon? source = null, IEffect? sourceEffect = null,
        ICondition? linkedStatus = null)
    {
        throw new NotImplementedException();
    }

    public void AddCondition(Condition condition, BattleContext context, Pokemon? source = null, IEffect? sourceEffect = null)
    {
        Condition? existingCondition = GetCondition(condition.Id);

        if (existingCondition is null)
        {
            Conditions.Add(condition);
            condition.OnStart?.Invoke(this, source, sourceEffect, context);
        }
        else
        {
            existingCondition.OnRestart?.Invoke(this, source, sourceEffect, context);
        }
    }

    public bool RemoveCondition(ConditionId conditionId)
    {
        Condition? condition = GetCondition(conditionId);
        return condition != null && Conditions.Remove(condition);
    }

    public Condition[] GetAllResidualConditions()
    {
        return Conditions.Where(c => c.OnResidual != null).ToArray();
    }

    // Battle event handlers
    public void OnSwitchOut()
    {
        Conditions.RemoveAll(c => c.ConditionVolatility == ConditionVolatility.Volatile);
        StatModifiers = new StatModifiers();
        ActiveMoveActions = 0;
        foreach (Move move in Moves)
        {
            move.Disabled = false;
        }
    }

    public void OnSwitchIn(Field field, Pokemon[] pokemons, BattleContext context)
    {
        Ability.OnStart?.Invoke(this, field, pokemons, Ability, context);
    }

    public void Terastillize(BattleContext context)
    {
        IsTeraUsed = true;
        if (context.PrintDebug)
        {
            UiGenerator.PrintTeraStart(this);
        }
    }

    public Pokemon Copy()
    {
        Pokemon copy = new(Specie, Evs, Ivs, Nature, Level, Trainer, SideId)
        {
            Moves = Moves.Select(m => m.Copy()).ToArray(),
            Conditions = Conditions.Select(c => c.Copy()).ToList(),
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