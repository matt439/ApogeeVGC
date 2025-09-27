using ApogeeVGC.Data;
using ApogeeVGC.Sim.GameObjects;
using ApogeeVGC.Sim.Moves;

namespace ApogeeVGC.Sim.Events;

/// <summary>
/// Extension methods for effects (moves, abilities, items) to easily access delegates
/// </summary>
public static class EffectDelegateExtensions
{
    #region Move Extensions
    
    /// <summary>
    /// Get a delegate for this move
    /// </summary>
    /// <typeparam name="T">The delegate type</typeparam>
    /// <param name="move">The move</param>
    /// <param name="library">The library containing delegates</param>
    /// <param name="eventId">The event type</param>
    /// <returns>The delegate if found, null otherwise</returns>
    public static T? GetDelegate<T>(this Move move, Library library, EventId eventId) where T : Delegate
    {
        return library.GetMoveDelegate<T>(eventId, move.Id);
    }
    
    /// <summary>
    /// Check if this move has a delegate registered
    /// </summary>
    /// <param name="move">The move</param>
    /// <param name="library">The library containing delegates</param>
    /// <param name="eventId">The event type</param>
    /// <returns>True if delegate exists</returns>
    public static bool HasDelegate(this Move move, Library library, EventId eventId)
    {
        return library.HasDelegate(eventId, move.Id.ToString());
    }
    
    #region CommonHandlers Delegate Aliases (Intuitive Names)
    
    /// <summary>
    /// Get the OnAfterHit delegate for this move (uses VoidSourceMove)
    /// </summary>
    /// <param name="move">The move</param>
    /// <param name="library">The library containing delegates</param>
    /// <returns>The OnAfterHit delegate if found</returns>
    public static VoidSourceMoveHandler? GetOnAfterHitDelegate(this Move move, Library library)
    {
        return move.GetDelegate<VoidSourceMoveHandler>(library, EventId.VoidSourceMoveHandler);
    }
    
    /// <summary>
    /// Get the OnAfterMove delegate for this move (uses VoidSourceMove)
    /// </summary>
    /// <param name="move">The move</param>
    /// <param name="library">The library containing delegates</param>
    /// <returns>The OnAfterMove delegate if found</returns>
    public static VoidSourceMoveHandler? GetOnAfterMoveDelegate(this Move move, Library library)
    {
        return move.GetDelegate<VoidSourceMoveHandler>(library, EventId.VoidSourceMoveHandler);
    }
    
    /// <summary>
    /// Get the OnAfterMoveSecondarySelf delegate for this move (uses VoidSourceMove)
    /// </summary>
    /// <param name="move">The move</param>
    /// <param name="library">The library containing delegates</param>
    /// <returns>The OnAfterMoveSecondarySelf delegate if found</returns>
    public static VoidSourceMoveHandler? GetOnAfterMoveSecondarySelfDelegate(this Move move, Library library)
    {
        return move.GetDelegate<VoidSourceMoveHandler>(library, EventId.VoidSourceMoveHandler);
    }
    
    /// <summary>
    /// Get the OnUseMoveMessage delegate for this move (uses VoidSourceMove)
    /// </summary>
    /// <param name="move">The move</param>
    /// <param name="library">The library containing delegates</param>
    /// <returns>The OnUseMoveMessage delegate if found</returns>
    public static VoidSourceMoveHandler? GetOnUseMoveMessageDelegate(this Move move, Library library)
    {
        return move.GetDelegate<VoidSourceMoveHandler>(library, EventId.VoidSourceMoveHandler);
    }
    
    /// <summary>
    /// Get the OnAfterMoveSecondary delegate for this move (uses VoidMove)
    /// </summary>
    /// <param name="move">The move</param>
    /// <param name="library">The library containing delegates</param>
    /// <returns>The OnAfterMoveSecondary delegate if found</returns>
    public static VoidMoveHandler? GetOnAfterMoveSecondaryDelegate(this Move move, Library library)
    {
        return move.GetDelegate<VoidMoveHandler>(library, EventId.VoidMoveHandler);
    }
    
    /// <summary>
    /// Get the OnMoveFail delegate for this move (uses VoidMove)
    /// </summary>
    /// <param name="move">The move</param>
    /// <param name="library">The library containing delegates</param>
    /// <returns>The OnMoveFail delegate if found</returns>
    public static VoidMoveHandler? GetOnMoveFailDelegate(this Move move, Library library)
    {
        return move.GetDelegate<VoidMoveHandler>(library, EventId.VoidMoveHandler);
    }
    
    /// <summary>
    /// Get the OnBasePower delegate for this move (uses ModifierSourceMove)
    /// </summary>
    /// <param name="move">The move</param>
    /// <param name="library">The library containing delegates</param>
    /// <returns>The OnBasePower delegate if found</returns>
    public static ModifierSourceMoveHandler? GetOnBasePowerDelegate(this Move move, Library library)
    {
        return move.GetDelegate<ModifierSourceMoveHandler>(library, EventId.ModifierSourceMoveHandler);
    }
    
    /// <summary>
    /// Get the OnModifyPriority delegate for this move (uses ModifierSourceMove)
    /// </summary>
    /// <param name="move">The move</param>
    /// <param name="library">The library containing delegates</param>
    /// <returns>The OnModifyPriority delegate if found</returns>
    public static ModifierSourceMoveHandler? GetOnModifyPriorityDelegate(this Move move, Library library)
    {
        return move.GetDelegate<ModifierSourceMoveHandler>(library, EventId.ModifierSourceMoveHandler);
    }
    
    /// <summary>
    /// Get the OnHit delegate for this move (uses ResultMove)
    /// </summary>
    /// <param name="move">The move</param>
    /// <param name="library">The library containing delegates</param>
    /// <returns>The OnHit delegate if found</returns>
    public static ResultMoveHandler? GetOnHitDelegate(this Move move, Library library)
    {
        return move.GetDelegate<ResultMoveHandler>(library, EventId.ResultMoveHandler);
    }
    
    /// <summary>
    /// Get the OnHitField delegate for this move (uses ResultMove)
    /// </summary>
    /// <param name="move">The move</param>
    /// <param name="library">The library containing delegates</param>
    /// <returns>The OnHitField delegate if found</returns>
    public static ResultMoveHandler? GetOnHitFieldDelegate(this Move move, Library library)
    {
        return move.GetDelegate<ResultMoveHandler>(library, EventId.ResultMoveHandler);
    }
    
    /// <summary>
    /// Get the OnPrepareHit delegate for this move (uses ResultMove)
    /// </summary>
    /// <param name="move">The move</param>
    /// <param name="library">The library containing delegates</param>
    /// <returns>The OnPrepareHit delegate if found</returns>
    public static ResultMoveHandler? GetOnPrepareHitDelegate(this Move move, Library library)
    {
        return move.GetDelegate<ResultMoveHandler>(library, EventId.ResultMoveHandler);
    }
    
    /// <summary>
    /// Get the OnTryImmunity delegate for this move (uses ResultMove)
    /// </summary>
    /// <param name="move">The move</param>
    /// <param name="library">The library containing delegates</param>
    /// <returns>The OnTryImmunity delegate if found</returns>
    public static ResultMoveHandler? GetOnTryImmunityDelegate(this Move move, Library library)
    {
        return move.GetDelegate<ResultMoveHandler>(library, EventId.ResultMoveHandler);
    }
    
    /// <summary>
    /// Get the OnTry delegate for this move (uses ResultSourceMove)
    /// </summary>
    /// <param name="move">The move</param>
    /// <param name="library">The library containing delegates</param>
    /// <returns>The OnTry delegate if found</returns>
    public static ResultSourceMoveHandler? GetOnTryDelegate(this Move move, Library library)
    {
        return move.GetDelegate<ResultSourceMoveHandler>(library, EventId.ResultSourceMoveHandler);
    }
    
    /// <summary>
    /// Get the OnTryMove delegate for this move (uses ResultSourceMove)
    /// </summary>
    /// <param name="move">The move</param>
    /// <param name="library">The library containing delegates</param>
    /// <returns>The OnTryMove delegate if found</returns>
    public static ResultSourceMoveHandler? GetOnTryMoveDelegate(this Move move, Library library)
    {
        return move.GetDelegate<ResultSourceMoveHandler>(library, EventId.ResultSourceMoveHandler);
    }
    
    /// <summary>
    /// Get the OnTryHit delegate for this move (uses ExtResultSourceMove)
    /// </summary>
    /// <param name="move">The move</param>
    /// <param name="library">The library containing delegates</param>
    /// <returns>The OnTryHit delegate if found</returns>
    public static ExtResultSourceMoveHandler? GetOnTryHitDelegate(this Move move, Library library)
    {
        return move.GetDelegate<ExtResultSourceMoveHandler>(library, EventId.ExtResultSourceMoveHandler);
    }
    
    #endregion
    
    #region Move-Specific Event Methods (Unique Delegates)
    
    /// <summary>
    /// Get the BasePowerCallback for this move
    /// </summary>
    /// <param name="move">The move</param>
    /// <param name="library">The library containing delegates</param>
    /// <returns>The BasePowerCallback delegate if found</returns>
    public static BasePowerCallbackHandler? GetBasePowerCallbackDelegate(this Move move, Library library)
    {
        return move.GetDelegate<BasePowerCallbackHandler>(library, EventId.BasePowerCallback);
    }
    
    /// <summary>
    /// Get the BeforeMoveCallback for this move
    /// </summary>
    /// <param name="move">The move</param>
    /// <param name="library">The library containing delegates</param>
    /// <returns>The BeforeMoveCallback delegate if found</returns>
    public static BeforeMoveCallbackHandler? GetBeforeMoveCallbackDelegate(this Move move, Library library)
    {
        return move.GetDelegate<BeforeMoveCallbackHandler>(library, EventId.BeforeMoveCallback);
    }
    
    /// <summary>
    /// Get the DamageCallback for this move
    /// </summary>
    /// <param name="move">The move</param>
    /// <param name="library">The library containing delegates</param>
    /// <returns>The DamageCallback delegate if found</returns>
    public static DamageCallbackHandler? GetDamageCallbackDelegate(this Move move, Library library)
    {
        return move.GetDelegate<DamageCallbackHandler>(library, EventId.DamageCallback);
    }
    
    /// <summary>
    /// Get the OnEffectiveness handler for this move
    /// </summary>
    /// <param name="move">The move</param>
    /// <param name="library">The library containing delegates</param>
    /// <returns>The OnEffectiveness delegate if found</returns>
    public static OnEffectivenessHandler? GetOnEffectivenessDelegate(this Move move, Library library)
    {
        return move.GetDelegate<OnEffectivenessHandler>(library, EventId.OnEffectiveness);
    }
    
    /// <summary>
    /// Get the OnModifyMove handler for this move
    /// </summary>
    /// <param name="move">The move</param>
    /// <param name="library">The library containing delegates</param>
    /// <returns>The OnModifyMove delegate if found</returns>
    public static OnModifyMoveHandler? GetOnModifyMoveDelegate(this Move move, Library library)
    {
        return move.GetDelegate<OnModifyMoveHandler>(library, EventId.OnModifyMove);
    }
    
    /// <summary>
    /// Get the OnAfterSubDamage handler for this move
    /// </summary>
    /// <param name="move">The move</param>
    /// <param name="library">The library containing delegates</param>
    /// <returns>The OnAfterSubDamage delegate if found</returns>
    public static OnAfterSubDamageHandler? GetOnAfterSubDamageDelegate(this Move move, Library library)
    {
        return move.GetDelegate<OnAfterSubDamageHandler>(library, EventId.OnAfterSubDamage);
    }
    
    /// <summary>
    /// Get the OnDamage handler for this move
    /// </summary>
    /// <param name="move">The move</param>
    /// <param name="library">The library containing delegates</param>
    /// <returns>The OnDamage delegate if found</returns>
    public static OnDamageHandler? GetOnDamageDelegate(this Move move, Library library)
    {
        return move.GetDelegate<OnDamageHandler>(library, EventId.OnDamage);
    }
    
    #endregion
    
    #region Legacy Methods (Kept for Compatibility)
    
    /// <summary>
    /// Get a VoidSourceMove delegate for this move (generic access)
    /// </summary>
    /// <param name="move">The move</param>
    /// <param name="library">The library containing delegates</param>
    /// <returns>The VoidSourceMove delegate if found</returns>
    public static VoidSourceMoveHandler? GetVoidSourceMoveDelegate(this Move move, Library library)
    {
        return move.GetDelegate<VoidSourceMoveHandler>(library, EventId.VoidSourceMoveHandler);
    }
    
    /// <summary>
    /// Get a ModifierSourceMove delegate for this move (generic access)
    /// </summary>
    /// <param name="move">The move</param>
    /// <param name="library">The library containing delegates</param>
    /// <returns>The ModifierSourceMove delegate if found</returns>
    public static ModifierSourceMoveHandler? GetModifierSourceMoveDelegate(this Move move, Library library)
    {
        return move.GetDelegate<ModifierSourceMoveHandler>(library, EventId.ModifierSourceMoveHandler);
    }
    
    /// <summary>
    /// Get a ResultMove delegate for this move (generic access)
    /// </summary>
    /// <param name="move">The move</param>
    /// <param name="library">The library containing delegates</param>
    /// <returns>The ResultMove delegate if found</returns>
    public static ResultMoveHandler? GetResultMoveDelegate(this Move move, Library library)
    {
        return move.GetDelegate<ResultMoveHandler>(library, EventId.ResultMoveHandler);
    }
    
    /// <summary>
    /// Get a ExtResultSourceMove delegate for this move (generic access)
    /// </summary>
    /// <param name="move">The move</param>
    /// <param name="library">The library containing delegates</param>
    /// <returns>The ExtResultSourceMove delegate if found</returns>
    public static ExtResultSourceMoveHandler? GetExtResultSourceMoveDelegate(this Move move, Library library)
    {
        return move.GetDelegate<ExtResultSourceMoveHandler>(library, EventId.ExtResultSourceMoveHandler);
    }
    
    #endregion
    
    #endregion
    
    #region Ability Extensions
    
    /// <summary>
    /// Get a delegate for this ability
    /// </summary>
    /// <typeparam name="T">The delegate type</typeparam>
    /// <param name="ability">The ability</param>
    /// <param name="library">The library containing delegates</param>
    /// <param name="eventId">The event type</param>
    /// <returns>The delegate if found, null otherwise</returns>
    public static T? GetDelegate<T>(this Ability ability, Library library, EventId eventId) where T : Delegate
    {
        return library.GetAbilityDelegate<T>(eventId, ability.Id);
    }
    
    /// <summary>
    /// Check if this ability has a delegate registered
    /// </summary>
    /// <param name="ability">The ability</param>
    /// <param name="library">The library containing delegates</param>
    /// <param name="eventId">The event type</param>
    /// <returns>True if delegate exists</returns>
    public static bool HasDelegate(this Ability ability, Library library, EventId eventId)
    {
        return library.HasDelegate(eventId, ability.Id.ToString());
    }
    
    #endregion
    
    #region Item Extensions
    
    /// <summary>
    /// Get a delegate for this item
    /// </summary>
    /// <typeparam name="T">The delegate type</typeparam>
    /// <param name="item">The item</param>
    /// <param name="library">The library containing delegates</param>
    /// <param name="eventId">The event type</param>
    /// <returns>The delegate if found, null otherwise</returns>
    public static T? GetDelegate<T>(this Item item, Library library, EventId eventId) where T : Delegate
    {
        return library.GetItemDelegate<T>(eventId, item.Id);
    }
    
    /// <summary>
    /// Check if this item has a delegate registered
    /// </summary>
    /// <param name="item">The item</param>
    /// <param name="library">The library containing delegates</param>
    /// <param name="eventId">The event type</param>
    /// <returns>True if delegate exists</returns>
    public static bool HasDelegate(this Item item, Library library, EventId eventId)
    {
        return library.HasDelegate(eventId, item.Id.ToString());
    }
    
    #endregion
}