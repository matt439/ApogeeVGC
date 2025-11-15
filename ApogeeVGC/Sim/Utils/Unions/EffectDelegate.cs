namespace ApogeeVGC.Sim.Utils.Unions;

/// <summary>
/// Delegate? | OnFlinch | OnCriticalHit | OnFractionalPriority | OnTakeItem | OnTryHeal | OnTryEatItem |
/// OnNegateImmunity | OnLockMove
/// </summary>
public abstract record EffectDelegate
{
    public abstract Delegate? GetDelegate(int index = 0);

    public static implicit operator EffectDelegate(Delegate del) => new DelegateEffectDelegate(del);
  public static EffectDelegate? FromNullableDelegate(Delegate? del)
    {
        return del is null ? null : new DelegateEffectDelegate(del);
    }

    public static implicit operator EffectDelegate(OnFlinch onFlinch) => new OnFlinchEffectDelegate(onFlinch);
    public static EffectDelegate? FromNullableOnFlinch(OnFlinch? del)
    {
        return del is null ? null : new OnFlinchEffectDelegate(del);
    }

    public static implicit operator EffectDelegate(OnCriticalHit onCriticalHit) =>
 new OnCriticalHitEffectDelegate(onCriticalHit);

 public static EffectDelegate? FromNullableOnCriticalHit(OnCriticalHit? del)
    {
        return del is null ? null : new OnCriticalHitEffectDelegate(del);
    }

    public static implicit operator EffectDelegate(OnFractionalPriority onFractionalPriority) =>
     new OnFractionalPriorityEffectDelegate(onFractionalPriority);
  public static EffectDelegate? FromNullableOnFractionalPriority(OnFractionalPriority? del)
    {
        return del is null ? null : new OnFractionalPriorityEffectDelegate(del);
    }

    public static implicit operator EffectDelegate(OnTakeItem onTakeItem) =>
     new OnTakeItemEffectDelegate(onTakeItem);
    public static EffectDelegate? FromNullableOnTakeItem(OnTakeItem? del)
    {
    return del is null ? null : new OnTakeItemEffectDelegate(del);
    }

    public static implicit operator EffectDelegate(OnTryHeal onTryHeal) => new OnTryHealEffectDelegate(onTryHeal);
    public static EffectDelegate? FromNullableOnTryHeal(OnTryHeal? del)
    {
  return del is null ? null : new OnTryHealEffectDelegate(del);
    }

    public static implicit operator EffectDelegate(OnTryEatItem onTryEatItem) =>
     new OnTryEatItemEffectDelegate(onTryEatItem);
    public static EffectDelegate? FromNullableOnTryEatItem(OnTryEatItem? del)
    {
    return del is null ? null : new OnTryEatItemEffectDelegate(del);
    }

    public static implicit operator EffectDelegate(OnNegateImmunity onNegateImmunity) =>
 new OnNegateImmunityEffectDelegate(onNegateImmunity);
    public static EffectDelegate? FromNullableOnNegateImmunity(OnNegateImmunity? del)
    {
    return del is null ? null : new OnNegateImmunityEffectDelegate(del);
    }

    public static implicit operator EffectDelegate(OnLockMove onLockMove) =>
        new OnLockMoveEffectDelegate(onLockMove);
    public static EffectDelegate? FromNullableOnLockMove(OnLockMove? del)
    {
        return del is null ? null : new OnLockMoveEffectDelegate(del);
 }
}

public record DelegateEffectDelegate(Delegate Del) : EffectDelegate
{
    public override Delegate GetDelegate(int index = 0) => Del;
}

public record OnFlinchEffectDelegate(OnFlinch OnFlinch) : EffectDelegate
{
    public override Delegate? GetDelegate(int index = 0) => OnFlinch switch
    {
        OnFlinchFunc f => f.Func,
        _ => null,
    };
}

public record OnCriticalHitEffectDelegate(OnCriticalHit OnCriticalHit) : EffectDelegate
{
    public override Delegate? GetDelegate(int index = 0) => OnCriticalHit switch
 {
        OnCriticalHitFunc f => f.Function,
    _ => null,
    };
}

public record OnFractionalPriorityEffectDelegate(OnFractionalPriority OnFractionalPriority) : EffectDelegate
{
    public override Delegate? GetDelegate(int index = 0) => OnFractionalPriority switch
    {
        OnFractionalPriorityFunc f => f.Function,
        _ => null,
    };
}

public record OnTakeItemEffectDelegate(OnTakeItem OnTakeItem) : EffectDelegate
{
    public override Delegate? GetDelegate(int index = 0) => OnTakeItem switch
    {
        OnTakeItemFunc f => f.Func,
        _ => null,
    };
}

public record OnTryHealEffectDelegate(OnTryHeal OnTryHeal) : EffectDelegate
{
    public override Delegate? GetDelegate(int index = 0)
    {
        if (OnTryHeal is OnTryHealBool) return null;
        return index switch
        {
  0 when OnTryHeal is OnTryHealFunc1 f1 => f1.Func,
            1 when OnTryHeal is OnTryHealFunc2 f2 => f2.Func,
            _ => null,
        };
    }
}

public record OnTryEatItemEffectDelegate(OnTryEatItem OnTryEatItem) : EffectDelegate
{
    public override Delegate? GetDelegate(int index = 0) => OnTryEatItem switch
    {
 FuncOnTryEatItem f => f.Func,
        _ => null,
    };
}

public record OnNegateImmunityEffectDelegate(OnNegateImmunity OnNegateImmunity) : EffectDelegate
{
    public override Delegate? GetDelegate(int index = 0) => OnNegateImmunity switch
    {
        OnNegateImmunityFunc f => f.Func,
        _ => null,
    };
}

public record OnLockMoveEffectDelegate(OnLockMove OnLockMove) : EffectDelegate
{
    public override Delegate? GetDelegate(int index = 0) => OnLockMove switch
    {
        OnLockMoveFunc f => f.Func,
      _ => null,
    };
}
