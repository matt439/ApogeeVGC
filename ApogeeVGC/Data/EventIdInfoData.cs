using System.Collections.ObjectModel;
using ApogeeVGC.Sim.Events;

namespace ApogeeVGC.Data;

public record EventIdInfoData
{
    public IReadOnlyDictionary<EventId, EventIdInfo> EventData { get; }

    public EventIdInfoData()
    {
        EventData = new ReadOnlyDictionary<EventId, EventIdInfo>(_eventInfos);
    }

    private readonly Dictionary<EventId, EventIdInfo> _eventInfos = new()
    {
        // Basic events
        [EventId.Accuracy] = new EventIdInfo { Id = EventId.Accuracy },

        // Switch-in related events
        [EventId.SwitchIn] = new EventIdInfo
        {
            Id = EventId.SwitchIn,
            Suffix = EventSuffix.SwitchIn,
            UsesEffectOrder = true,
            UsesSpeed = true,
            UsesFractionalSpeed = true,
        },
        [EventId.AnySwitchIn] = new EventIdInfo
        {
            Id = EventId.AnySwitchIn,
            Prefix = EventPrefix.Any,
            Suffix = EventSuffix.SwitchIn,
            BaseEventId = EventId.SwitchIn,
            UsesEffectOrder = true,
            UsesSpeed = true,
            UsesFractionalSpeed = true,
        },
        [EventId.BeforeSwitchIn] =
            new EventIdInfo { Id = EventId.BeforeSwitchIn, UsesSpeed = true },
        [EventId.BeforeSwitchOut] = new EventIdInfo
            { Id = EventId.BeforeSwitchOut, UsesSpeed = true },
        [EventId.SwitchOut] = new EventIdInfo { Id = EventId.SwitchOut, UsesSpeed = true },
        [EventId.AfterSwitchInSelf] = new EventIdInfo
            { Id = EventId.AfterSwitchInSelf, UsesSpeed = true },

        // Redirect events
        [EventId.RedirectTarget] = new EventIdInfo
        {
            Id = EventId.RedirectTarget,
            Suffix = EventSuffix.RedirectTarget,
            UsesEffectOrder = true,
            UsesSpeed = true,
        },
        [EventId.FoeRedirectTarget] = new EventIdInfo
        {
            Id = EventId.FoeRedirectTarget,
            Prefix = EventPrefix.Foe,
            BaseEventId = EventId.RedirectTarget,
            UsesSpeed = true,
        },

        // Residual events
        [EventId.Residual] = new EventIdInfo { Id = EventId.Residual, UsesSpeed = true },
        [EventId.FieldResidual] = new EventIdInfo
        {
            Id = EventId.FieldResidual,
            FieldEventId = EventId.FieldResidual,
            BaseEventId = EventId.Residual,
            UsesSpeed = true,
        },
        [EventId.SideResidual] = new EventIdInfo
        {
            Id = EventId.SideResidual,
            SideEventId = EventId.SideResidual,
            BaseEventId = EventId.Residual,
            UsesSpeed = true,
        },

        // Turn-related events
        [EventId.BeforeTurn] = new EventIdInfo { Id = EventId.BeforeTurn, UsesSpeed = true },
        [EventId.Update] = new EventIdInfo { Id = EventId.Update, UsesSpeed = true },

        // Trap events
        [EventId.TrapPokemon] = new EventIdInfo { Id = EventId.TrapPokemon, UsesSpeed = true },
        [EventId.MaybeTrapPokemon] = new EventIdInfo
            { Id = EventId.MaybeTrapPokemon, UsesSpeed = true },
        [EventId.FoeMaybeTrapPokemon] = new EventIdInfo
        {
            Id = EventId.FoeMaybeTrapPokemon,
            Prefix = EventPrefix.Foe,
            BaseEventId = EventId.MaybeTrapPokemon,
            UsesSpeed = true,
        },

        // Move-related events
        [EventId.DisableMove] = new EventIdInfo { Id = EventId.DisableMove, UsesSpeed = true },
        [EventId.ModifyPriority] =
            new EventIdInfo { Id = EventId.ModifyPriority, UsesSpeed = true },
        [EventId.BeforeMove] = new EventIdInfo { Id = EventId.BeforeMove, UsesSpeed = true },
        [EventId.FoeBeforeMove] = new EventIdInfo
        {
            Id = EventId.FoeBeforeMove,
            Prefix = EventPrefix.Foe,
            BaseEventId = EventId.BeforeMove,
            UsesSpeed = true,
        },
        [EventId.TryMove] = new EventIdInfo { Id = EventId.TryMove, UsesSpeed = true },
        [EventId.UseItem] = new EventIdInfo { Id = EventId.UseItem, UsesSpeed = true },
        [EventId.AfterUseItem] = new EventIdInfo { Id = EventId.AfterUseItem, UsesSpeed = true },
        [EventId.TakeItem] = new EventIdInfo { Id = EventId.TakeItem, UsesSpeed = true },
        [EventId.AfterTakeItem] = new EventIdInfo { Id = EventId.AfterTakeItem, UsesSpeed = true },
        [EventId.EatItem] = new EventIdInfo { Id = EventId.EatItem, UsesSpeed = true },
        [EventId.TryEatItem] = new EventIdInfo { Id = EventId.TryEatItem, UsesSpeed = true },

        // Hit-related events
        [EventId.TryHit] = new EventIdInfo
        {
            Id = EventId.TryHit,
            UsesSpeed = true,
            UsesLeftToRightOrder = true,
        },
        [EventId.TryHitSide] = new EventIdInfo { Id = EventId.TryHitSide, UsesSpeed = true },
        [EventId.AllyTryHitSide] = new EventIdInfo
        {
            Id = EventId.AllyTryHitSide,
            Prefix = EventPrefix.Ally,
            BaseEventId = EventId.TryHitSide,
            UsesSpeed = true,
        },
        [EventId.TryHitField] = new EventIdInfo { Id = EventId.TryHitField, UsesSpeed = true },
        [EventId.TryPrimaryHit] = new EventIdInfo { Id = EventId.TryPrimaryHit, UsesSpeed = true },
        [EventId.Hit] = new EventIdInfo { Id = EventId.Hit, UsesSpeed = true },
        [EventId.PrepareHit] = new EventIdInfo { Id = EventId.PrepareHit, UsesSpeed = true },
        [EventId.AnyPrepareHit] = new EventIdInfo
        {
            Id = EventId.AnyPrepareHit,
            Prefix = EventPrefix.Any,
            BaseEventId = EventId.PrepareHit,
            UsesSpeed = true,
        },
        [EventId.Invulnerability] = new EventIdInfo
        {
            Id = EventId.Invulnerability,
            UsesSpeed = true,
            UsesLeftToRightOrder = true,
        },
        [EventId.AnyInvulnerability] = new EventIdInfo
        {
            Id = EventId.AnyInvulnerability,
            Prefix = EventPrefix.Any,
            BaseEventId = EventId.Invulnerability,
            UsesSpeed = true,
        },
        [EventId.SourceInvulnerability] = new EventIdInfo
        {
            Id = EventId.SourceInvulnerability,
            Prefix = EventPrefix.Source,
            BaseEventId = EventId.Invulnerability,
            UsesSpeed = true,
        },
        [EventId.DamagingHit] = new EventIdInfo
        {
            Id = EventId.DamagingHit,
            UsesSpeed = true,
            UsesLeftToRightOrder = true,
        },
        [EventId.AfterHit] = new EventIdInfo { Id = EventId.AfterHit, UsesSpeed = true },
        [EventId.AfterMove] = new EventIdInfo { Id = EventId.AfterMove, UsesSpeed = true },
        [EventId.AfterMoveSelf] = new EventIdInfo { Id = EventId.AfterMoveSelf, UsesSpeed = true },
        [EventId.AfterMoveSecondary] = new EventIdInfo
            { Id = EventId.AfterMoveSecondary, UsesSpeed = true },
        [EventId.AfterMoveSecondarySelf] = new EventIdInfo
            { Id = EventId.AfterMoveSecondarySelf, UsesSpeed = true },
        [EventId.MoveAborted] = new EventIdInfo { Id = EventId.MoveAborted, UsesSpeed = true },
        [EventId.MoveFail] = new EventIdInfo { Id = EventId.MoveFail, UsesSpeed = true },

        // Damage and healing events
        [EventId.Damage] = new EventIdInfo { Id = EventId.Damage, UsesSpeed = true },
        [EventId.Heal] = new EventIdInfo { Id = EventId.Heal, UsesSpeed = true },
        [EventId.TryHeal] = new EventIdInfo { Id = EventId.TryHeal, UsesSpeed = true },
        [EventId.AfterSubDamage] =
            new EventIdInfo { Id = EventId.AfterSubDamage, UsesSpeed = true },

        // Status and volatile events
        [EventId.SetStatus] = new EventIdInfo { Id = EventId.SetStatus, UsesSpeed = true },
        [EventId.AfterSetStatus] =
            new EventIdInfo { Id = EventId.AfterSetStatus, UsesSpeed = true },
        [EventId.TryAddVolatile] =
            new EventIdInfo { Id = EventId.TryAddVolatile, UsesSpeed = true },
        [EventId.Start] = new EventIdInfo { Id = EventId.Start, UsesSpeed = true },
        [EventId.End] = new EventIdInfo { Id = EventId.End, UsesSpeed = true },
        [EventId.Restart] = new EventIdInfo { Id = EventId.Restart, UsesSpeed = true },

        // Boost events
        [EventId.Boost] = new EventIdInfo { Id = EventId.Boost, UsesSpeed = true },
        [EventId.TryBoost] = new EventIdInfo { Id = EventId.TryBoost, UsesSpeed = true },
        [EventId.AfterBoost] = new EventIdInfo { Id = EventId.AfterBoost, UsesSpeed = true },
        [EventId.AfterEachBoost] =
            new EventIdInfo { Id = EventId.AfterEachBoost, UsesSpeed = true },
        [EventId.ChangeBoost] = new EventIdInfo { Id = EventId.ChangeBoost, UsesSpeed = true },
        [EventId.ModifyBoost] = new EventIdInfo { Id = EventId.ModifyBoost, UsesSpeed = true },

        // Stat modification events  
        [EventId.ModifyAtk] = new EventIdInfo { Id = EventId.ModifyAtk, UsesSpeed = true },
        [EventId.ModifyDef] = new EventIdInfo { Id = EventId.ModifyDef, UsesSpeed = true },
        [EventId.ModifySpA] = new EventIdInfo { Id = EventId.ModifySpA, UsesSpeed = true },
        [EventId.ModifySpD] = new EventIdInfo { Id = EventId.ModifySpD, UsesSpeed = true },
        [EventId.ModifySpe] = new EventIdInfo { Id = EventId.ModifySpe, UsesSpeed = true },
        [EventId.AllyModifyAtk] = new EventIdInfo
        {
            Id = EventId.AllyModifyAtk,
            Prefix = EventPrefix.Ally,
            BaseEventId = EventId.ModifyAtk,
            UsesSpeed = true,
        },
        [EventId.AllyModifySpA] = new EventIdInfo
        {
            Id = EventId.AllyModifySpA,
            Prefix = EventPrefix.Ally,
            BaseEventId = EventId.ModifySpA,
            UsesSpeed = true,
        },
        [EventId.AllyModifySpD] = new EventIdInfo
        {
            Id = EventId.AllyModifySpD,
            Prefix = EventPrefix.Ally,
            BaseEventId = EventId.ModifySpD,
            UsesSpeed = true,
        },
        [EventId.FoeModifyDef] = new EventIdInfo
        {
            Id = EventId.FoeModifyDef,
            Prefix = EventPrefix.Foe,
            BaseEventId = EventId.ModifyDef,
            UsesSpeed = true,
        },
        [EventId.FoeModifySpD] = new EventIdInfo
        {
            Id = EventId.FoeModifySpD,
            Prefix = EventPrefix.Foe,
            BaseEventId = EventId.ModifySpD,
            UsesSpeed = true,
        },
        [EventId.SourceModifyAtk] = new EventIdInfo
        {
            Id = EventId.SourceModifyAtk,
            Prefix = EventPrefix.Source,
            BaseEventId = EventId.ModifyAtk,
            UsesSpeed = true,
        },
        [EventId.SourceModifySpA] = new EventIdInfo
        {
            Id = EventId.SourceModifySpA,
            Prefix = EventPrefix.Source,
            BaseEventId = EventId.ModifySpA,
            UsesSpeed = true,
        },
        [EventId.SourceModifyDamage] = new EventIdInfo
        {
            Id = EventId.SourceModifyDamage,
            Prefix = EventPrefix.Source,
            BaseEventId = EventId.ModifyDamage,
            UsesSpeed = true,
        },

        // Move property modification events
        [EventId.BasePower] = new EventIdInfo { Id = EventId.BasePower, UsesSpeed = true },
        [EventId.AllyBasePower] = new EventIdInfo
        {
            Id = EventId.AllyBasePower,
            Prefix = EventPrefix.Ally,
            BaseEventId = EventId.BasePower,
            UsesSpeed = true,
        },
        [EventId.FoeBasePower] = new EventIdInfo
        {
            Id = EventId.FoeBasePower,
            Prefix = EventPrefix.Foe,
            BaseEventId = EventId.BasePower,
            UsesSpeed = true,
        },
        [EventId.SourceBasePower] = new EventIdInfo
        {
            Id = EventId.SourceBasePower,
            Prefix = EventPrefix.Source,
            BaseEventId = EventId.BasePower,
            UsesSpeed = true,
        },
        [EventId.AnyBasePower] = new EventIdInfo
        {
            Id = EventId.AnyBasePower,
            Prefix = EventPrefix.Any,
            BaseEventId = EventId.BasePower,
            UsesSpeed = true,
        },
        [EventId.ModifyDamage] = new EventIdInfo { Id = EventId.ModifyDamage, UsesSpeed = true },
        [EventId.ModifyDamagePhase1] = new EventIdInfo
            { Id = EventId.ModifyDamagePhase1, UsesSpeed = true },
        [EventId.ModifyDamagePhase2] = new EventIdInfo
            { Id = EventId.ModifyDamagePhase2, UsesSpeed = true },
        [EventId.ModifyAccuracy] =
            new EventIdInfo { Id = EventId.ModifyAccuracy, UsesSpeed = true },
        [EventId.AnyModifyAccuracy] = new EventIdInfo
        {
            Id = EventId.AnyModifyAccuracy,
            Prefix = EventPrefix.Any,
            BaseEventId = EventId.ModifyAccuracy,
            UsesSpeed = true,
        },
        [EventId.SourceModifyAccuracy] = new EventIdInfo
        {
            Id = EventId.SourceModifyAccuracy,
            Prefix = EventPrefix.Source,
            BaseEventId = EventId.ModifyAccuracy,
            UsesSpeed = true,
        },
        [EventId.ModifyCritRatio] = new EventIdInfo
            { Id = EventId.ModifyCritRatio, UsesSpeed = true },
        [EventId.CriticalHit] = new EventIdInfo { Id = EventId.CriticalHit, UsesSpeed = true },
        [EventId.ModifyMove] = new EventIdInfo { Id = EventId.ModifyMove, UsesSpeed = true },
        [EventId.ModifyType] = new EventIdInfo { Id = EventId.ModifyType, UsesSpeed = true },
        [EventId.ModifyTarget] = new EventIdInfo { Id = EventId.ModifyTarget, UsesSpeed = true },
        [EventId.ModifyStab] = new EventIdInfo { Id = EventId.ModifyStab, UsesSpeed = true },
        [EventId.ModifySecondaries] = new EventIdInfo
            { Id = EventId.ModifySecondaries, UsesSpeed = true },
        [EventId.ModifyWeight] = new EventIdInfo { Id = EventId.ModifyWeight, UsesSpeed = true },
        [EventId.ModifySpecie] = new EventIdInfo { Id = EventId.ModifySpecie, UsesSpeed = true },

        // Type and effectiveness events
        [EventId.Type] = new EventIdInfo { Id = EventId.Type, UsesSpeed = true },
        [EventId.Effectiveness] = new EventIdInfo { Id = EventId.Effectiveness, UsesSpeed = true },
        [EventId.Immunity] = new EventIdInfo { Id = EventId.Immunity, UsesSpeed = true },
        [EventId.NegateImmunity] =
            new EventIdInfo { Id = EventId.NegateImmunity, UsesSpeed = true },
        [EventId.TryImmunity] = new EventIdInfo { Id = EventId.TryImmunity, UsesSpeed = true },

        // Faint events
        [EventId.Faint] = new EventIdInfo { Id = EventId.Faint, UsesSpeed = true },
        [EventId.BeforeFaint] = new EventIdInfo { Id = EventId.BeforeFaint, UsesSpeed = true },
        [EventId.AfterFaint] = new EventIdInfo { Id = EventId.AfterFaint, UsesSpeed = true },
        [EventId.AnyFaint] = new EventIdInfo
        {
            Id = EventId.AnyFaint,
            Prefix = EventPrefix.Any,
            BaseEventId = EventId.Faint,
            UsesSpeed = true,
        },

        // Weather and terrain events
        [EventId.SetWeather] = new EventIdInfo { Id = EventId.SetWeather, UsesSpeed = true },
        [EventId.Weather] = new EventIdInfo { Id = EventId.Weather, UsesSpeed = true },
        [EventId.WeatherChange] = new EventIdInfo { Id = EventId.WeatherChange, UsesSpeed = true },
        [EventId.WeatherModifyDamage] = new EventIdInfo
            { Id = EventId.WeatherModifyDamage, UsesSpeed = true },
        [EventId.Terrain] = new EventIdInfo { Id = EventId.Terrain, UsesSpeed = true },
        [EventId.TerrainChange] = new EventIdInfo { Id = EventId.TerrainChange, UsesSpeed = true },
        [EventId.TryTerrain] = new EventIdInfo { Id = EventId.TryTerrain, UsesSpeed = true },
        [EventId.FieldStart] = new EventIdInfo { Id = EventId.FieldStart, UsesSpeed = true },
        [EventId.FieldEnd] = new EventIdInfo { Id = EventId.FieldEnd, UsesSpeed = true },
        [EventId.FieldRestart] = new EventIdInfo { Id = EventId.FieldRestart, UsesSpeed = true },

        // Side condition events
        [EventId.SideStart] = new EventIdInfo { Id = EventId.SideStart, UsesSpeed = true },
        [EventId.SideEnd] = new EventIdInfo { Id = EventId.SideEnd, UsesSpeed = true },
        [EventId.SideRestart] = new EventIdInfo { Id = EventId.SideRestart, UsesSpeed = true },
        [EventId.SideConditionStart] = new EventIdInfo
            { Id = EventId.SideConditionStart, UsesSpeed = true },
        [EventId.PseudoWeatherChange] = new EventIdInfo
            { Id = EventId.PseudoWeatherChange, UsesSpeed = true },
        [EventId.HitSide] = new EventIdInfo { Id = EventId.HitSide, UsesSpeed = true },
        [EventId.HitField] = new EventIdInfo { Id = EventId.HitField, UsesSpeed = true },

        // Entry hazard events
        [EventId.EntryHazard] = new EventIdInfo
        {
            Id = EventId.EntryHazard,
            UsesSpeed = true,
            UsesLeftToRightOrder = true,
        },

        // Special move events
        [EventId.ChargeMove] = new EventIdInfo { Id = EventId.ChargeMove, UsesSpeed = true },
        [EventId.PriorityChargeCallback] = new EventIdInfo
            { Id = EventId.PriorityChargeCallback, UsesSpeed = true },
        [EventId.LockMove] = new EventIdInfo { Id = EventId.LockMove, UsesSpeed = true },
        [EventId.StallMove] = new EventIdInfo { Id = EventId.StallMove, UsesSpeed = true },
        [EventId.Flinch] = new EventIdInfo { Id = EventId.Flinch, UsesSpeed = true },
        [EventId.FractionalPriority] = new EventIdInfo
            { Id = EventId.FractionalPriority, UsesSpeed = true },
        [EventId.DragOut] = new EventIdInfo { Id = EventId.DragOut, UsesSpeed = true },
        [EventId.Attract] = new EventIdInfo { Id = EventId.Attract, UsesSpeed = true },
        [EventId.Swap] = new EventIdInfo { Id = EventId.Swap, UsesSpeed = true },
        [EventId.DeductPp] = new EventIdInfo { Id = EventId.DeductPp, UsesSpeed = true },

        // Ability events
        [EventId.SetAbility] = new EventIdInfo { Id = EventId.SetAbility, UsesSpeed = true },
        [EventId.CheckShow] = new EventIdInfo { Id = EventId.CheckShow, UsesSpeed = true },
        [EventId.EmergencyExit] = new EventIdInfo { Id = EventId.EmergencyExit, UsesSpeed = true },

        // Mega/Tera events
        [EventId.AfterMega] = new EventIdInfo { Id = EventId.AfterMega, UsesSpeed = true },
        [EventId.AfterTerastallization] = new EventIdInfo
            { Id = EventId.AfterTerastallization, UsesSpeed = true },

        // Misc events
        [EventId.OverrideAction] =
            new EventIdInfo { Id = EventId.OverrideAction, UsesSpeed = true },
        [EventId.BasePowerCallback] = new EventIdInfo
            { Id = EventId.BasePowerCallback, UsesSpeed = true },
        [EventId.BeforeTurnCallback] = new EventIdInfo
            { Id = EventId.BeforeTurnCallback, UsesSpeed = true },
        [EventId.BeforeMoveCallback] = new EventIdInfo
            { Id = EventId.BeforeMoveCallback, UsesSpeed = true },
        [EventId.DamageCallback] =
            new EventIdInfo { Id = EventId.DamageCallback, UsesSpeed = true },
        [EventId.Copy] = new EventIdInfo { Id = EventId.Copy, UsesSpeed = true },
        [EventId.Eat] = new EventIdInfo { Id = EventId.Eat, UsesSpeed = true },
        [EventId.Try] = new EventIdInfo { Id = EventId.Try, UsesSpeed = true },
        [EventId.Use] = new EventIdInfo { Id = EventId.Use, UsesSpeed = true },
        [EventId.UseMoveMessage] =
            new EventIdInfo { Id = EventId.UseMoveMessage, UsesSpeed = true },
    };
}