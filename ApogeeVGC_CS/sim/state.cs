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