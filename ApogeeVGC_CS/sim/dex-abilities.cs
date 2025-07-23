using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApogeeVGC_CS.sim;

namespace ApogeeVGC_CS.sim
{
    // Interface for ability event methods
    public interface IAbilityEventMethods
    {
        Action<Battle, Pokemon>? OnCheckShow { get; set; }
        Action<Battle, Pokemon>? OnEnd { get; set; }
        Action<Battle, Pokemon>? OnStart { get; set; }
    }

    // Possible Ability flags
    public class AbilityFlags
    {
        public bool Breakable { get; set; } // Can be suppressed by Mold Breaker and related effects
        public bool CantSuppress { get; set; } // Ability can't be suppressed by e.g. Gastro Acid or Neutralizing Gas
        public bool FailRolePlay { get; set; } // Role Play fails if target has this Ability
        public bool FailSkillSwap { get; set; } // Skill Swap fails if either the user or target has this Ability
        public bool NoEntrain { get; set; } // Entrainment fails if user has this Ability
        public bool NoReceiver { get; set; } // Receiver and Power of Alchemy will not activate if an ally faints with this Ability
        public bool NoTrace { get; set; } // Trace cannot copy this Ability
        public bool NoTransform { get; set; } // Disables the Ability if the user is Transformed
    }

    // Interface for ability data extending multiple interfaces
    public interface IAbilityData : IAbilityEventMethods, IPokemonEventMethods
    {
        string Name { get; set; }
        // Additional properties from Ability can be added here
    }

    // Class implementing ability data
    public class AbilityData : IAbilityData
    {
        public string Name { get; set; } = string.Empty;

        // IAbilityEventMethods implementation
        public Action<Battle, Pokemon>? OnCheckShow { get; set; }
        public Action<Battle, Pokemon>? OnEnd { get; set; }
        public Action<Battle, Pokemon>? OnStart { get; set; }

        // IPokemonEventMethods implementation (placeholder - define based on actual interface)
        // Add Pokemon event method properties here as needed
    }

    // Modded ability data with inheritance support
    public class ModdedAbilityData : AbilityData
    {
        public bool Inherit { get; set; }
    }

    // Type aliases for data tables
    public class AbilityDataTable : Dictionary<IdEntry, AbilityData> { }
    public class ModdedAbilityDataTable : Dictionary<IdEntry, ModdedAbilityData> { }

    // Main Ability class extending BasicEffect
    public class Ability : IBasicEffect, IAbility
    {
        public string EffectType { get; set; } = "Ability";

        // Rating from -1 Detrimental to +5 Essential
        public int Rating { get; set; }
        public bool SuppressWeather { get; set; }
        public AbilityFlags Flags { get; set; } = new();
        public IConditionData? Condition { get; set; }

        // IBasicEffect implementation
        public Id Id { get; set; } = new();
        EffectType IBasicEffect.EffectType { get; set; } = sim.EffectType.Ability;
        public bool Exists { get; set; } = true;
        public string Fullname { get; set; } = string.Empty;
        public int Gen { get; set; }
        public string SourceEffect { get; set; } = string.Empty;

        // Additional properties that might be inherited from BasicEffect
        public string Name { get; set; } = string.Empty;
        public string? Desc { get; set; }
        public string? ShortDesc { get; set; }
        public int Num { get; set; }

        public Ability(IAnyObject data)
        {
            // Initialize from data object
            if (data.ContainsKey("name"))
                Name = data["name"].ToString() ?? string.Empty;

            if (data.ContainsKey("rating") && int.TryParse(data["rating"].ToString(), out int rating))
                Rating = rating;

            if (data.ContainsKey("suppressWeather"))
                SuppressWeather = Convert.ToBoolean(data["suppressWeather"]);

            if (data.ContainsKey("flags") && data["flags"] is AbilityFlags flags)
                Flags = flags;
            else
                Flags = new AbilityFlags();

            if (data.ContainsKey("num") && int.TryParse(data["num"].ToString(), out int num))
                Num = num;

            Fullname = $"ability: {Name}";
            EffectType = "Ability";

            // Auto-assign generation based on ability number
            if (Gen == 0)
            {
                if (Num >= 268)
                    Gen = 9;
                else if (Num >= 234)
                    Gen = 8;
                else if (Num >= 192)
                    Gen = 7;
                else if (Num >= 165)
                    Gen = 6;
                else if (Num >= 124)
                    Gen = 5;
                else if (Num >= 77)
                    Gen = 4;
                else if (Num >= 1)
                    Gen = 3;
            }

            // AssignMissingFields equivalent would go here
            AssignMissingFields(data);
        }

        private void AssignMissingFields(IAnyObject data)
        {
            // Implementation for assigning missing fields from data
            // This would be similar to the TypeScript assignMissingFields function
            if (data.ContainsKey("desc"))
                Desc = data["desc"].ToString();

            if (data.ContainsKey("shortDesc"))
                ShortDesc = data["shortDesc"].ToString();

            if (data.ContainsKey("exists"))
                Exists = Convert.ToBoolean(data["exists"]);
        }
    }

    // Static empty ability instance
    public static class AbilityConstants
    {
        public static readonly Ability EmptyAbility = new(new DefaultTextData
        {
            ["id"] = "",
            ["name"] = "",
            ["exists"] = false
        });
    }

    // Main DexAbilities class
    public class DexAbilities
    {
        public IModdedDex Dex { get; }
        private readonly Dictionary<Id, Ability> _abilityCache = new();
        private List<Ability>? _allCache = null;

        public DexAbilities(IModdedDex dex)
        {
            Dex = dex;
        }

        public Ability Get(string name = "")
        {
            return Get((object)name);
        }

        public Ability Get(Ability ability)
        {
            return ability;
        }

        public Ability Get(object nameOrAbility)
        {
            if (nameOrAbility is Ability ability)
                return ability;

            string name = nameOrAbility?.ToString()?.Trim() ?? string.Empty;
            Id id = ToId(name);
            return GetById(id);
        }

        public Ability GetById(Id id)
        {
            if (string.IsNullOrEmpty(id.Value))
                return AbilityConstants.EmptyAbility;

            if (_abilityCache.TryGetValue(id, out Ability? cachedAbility))
                return cachedAbility;

            Ability resultAbility;

            // Check for alias
            var alias = Dex.GetAlias(id);
            if (alias != null && !string.IsNullOrEmpty(alias.Value))
            {
                resultAbility = Get(alias.Value);
            }
            else if (!string.IsNullOrEmpty(id.Value) && Dex.Data.Abilities.ContainsKey(id.Value))
            {
                // Get ability data from dex
                var abilityData = Dex.Data.Abilities[id.Value];
                var abilityTextData = Dex.GetDescriptions("Abilities", id.Value);

                // Create combined data object
                var combinedData = new DefaultTextData();
                combinedData["name"] = id.Value;

                // Copy ability data properties
                // This would need to be implemented based on actual data structure

                // Copy text data
                combinedData["desc"] = abilityTextData.Desc;
                combinedData["shortDesc"] = abilityTextData.ShortDesc;

                resultAbility = new Ability(combinedData);

                // Handle generation-specific logic
                if (resultAbility.Gen > Dex.Gen)
                {
                    // Mark as Future nonstandard
                }

                if (Dex.CurrentMod == "gen7letsgo" && resultAbility.Id.Value != "noability")
                {
                    // Mark as Past nonstandard
                }

                if ((Dex.CurrentMod == "gen7letsgo" || Dex.Gen <= 2) && resultAbility.Id.Value == "noability")
                {
                    // Mark as standard
                }
            }
            else
            {
                // Create non-existent ability
                resultAbility = new Ability(new DefaultTextData
                {
                    ["id"] = id.Value,
                    ["name"] = id.Value,
                    ["exists"] = false
                });
            }

            if (resultAbility.Exists)
            {
                _abilityCache[id] = resultAbility; // Dex.DeepFreeze equivalent
            }

            return resultAbility;
        }

        public List<Ability> All()
        {
            if (_allCache != null)
                return _allCache;

            var abilities = new List<Ability>();

            foreach (var id in Dex.Data.Abilities.Keys)
            {
                abilities.Add(GetById(new Id { Value = id });
            }

            _allCache = abilities;
            return _allCache;
        }

        // Helper method to convert string to Id (placeholder - implement based on actual Id structure)
        private static Id ToId(string name)
        {
            return new Id { Value = name.ToLowerInvariant() };
        }
    }

    // Placeholder interfaces that need to be defined based on actual requirements
    public interface IPokemonEventMethods
    {
        // Define Pokemon event methods here
    }

    public interface IConditionData
    {
        // Define condition data interface
    }

    public interface IModdedDex
    {
        int Gen { get; }
        string CurrentMod { get; }
        AbilityDataTable Data { get; }
        Id? GetAlias(Id id);
        Descriptions GetDescriptions(string table, string id);
    }

    // Extension of AbilityDataTable to include Abilities property
    public class ModdedDexData
    {
        public AbilityDataTable Abilities { get; set; } = new();
    }
}