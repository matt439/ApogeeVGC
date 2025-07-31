using System;
using System.Collections.Generic;

namespace ApogeeVGC_CS.sim
{
    public static class State
    {
        // Positions for up to 24 Pokémon
        public static readonly string Positions = "abcdefghijklmnopqrstuvwx";

        // Types that are serialized as references
        public static readonly HashSet<Type> ReferableTypes = new()
        {
        public static readonly HashSet<string> BATTLE = new()
        {
            "Dex", "Gen", "RuleTable", "Id", "Log", "inherit", "Format", "TeamGenerator",
            "HitSubstitute", "NotFail", "Fail", "SilentFail", "Field", "Sides", "Prng", "Hints",
            "Deserialized", "queue", "Actions"
        };

        public static readonly HashSet<string> FIELD = new() { "Id", "Battle" };

        public static readonly HashSet<string> SIDE = new() { "Battle", "team", "Pokemon", "Choice", "ActiveRequest" };

        public static readonly HashSet<string> POKEMON = new()
        {
            "Side", "Battle", "set", "name", "fullname", "id",
            "happiness", "level", "pokeball", "baseMoveSlots"
        };

        public static readonly HashSet<string> CHOICE = new() { "SwitchIns" };

        public static readonly HashSet<string> ACTIVE_MOVE = new() { "move" };
    }

    public class State
    {
        // REFERABLE is used to determine which objects are of the Referable type by
        // comparing their constructors. We set this dynamically due to circular module dependencies.
        private HashSet<Type>? _referable;

        private HashSet<Type> REFERABLE
        {
            get
            {
                if (_referable == null)
                {
                    _referable = new HashSet<Type>
                    {
            typeof(Battle), typeof(Field), typeof(Side), typeof(Pokemon),
            typeof(Condition), typeof(Ability), typeof(Item), typeof(Move), typeof(Species)
        };

        // Fields to skip or handle specially during serialization
        public static readonly HashSet<string> BattleSkipFields = new()
        {
            "dex", "gen", "ruleTable", "id", "log", "inherit", "format", "teamGenerator",
            "HIT_SUBSTITUTE", "NOT_FAIL", "FAIL", "SILENT_FAIL", "field", "sides", "prng", "hints",
            "deserialized", "queue", "actions"
        };

        public static readonly HashSet<string> FieldSkipFields = new() { "id", "battle" };
        public static readonly HashSet<string> SideSkipFields = new() { "battle", "team", "pokemon", "choice", "activeRequest" };
        public static readonly HashSet<string> PokemonSkipFields = new()
        {
            "side", "battle", "set", "name", "fullname", "id",
            "happiness", "level", "pokeball", "baseMoveSlots"
        };
        public static readonly HashSet<string> ChoiceSkipFields = new() { "switchIns" };
        public static readonly HashSet<string> ActiveMoveSkipFields = new() { "move" };

        // Dynamic referable set for runtime checks (if needed)
        public static HashSet<Type>? DynamicReferableTypes { get; set; }
    }
}