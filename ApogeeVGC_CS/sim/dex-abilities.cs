using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApogeeVGC_CS.sim;

namespace ApogeeVGC_CS.sim
{
    public interface IAbilityEventMethods
    {
        Action<Battle, Pokemon>? OnCheckShow { get; set; }
        Action<Battle, Pokemon>? OnEnd { get; set; }
        Action<Battle, Pokemon>? OnStart { get; set; }
    }

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

    public interface IAbilityData : IAbilityEventMethods, IPokemonEventMethods
    {
        string Name { get; set; }
        // Additional properties from Ability can be added here
    }

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
        public bool Inherit { get; set; } = true;
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
        public int? Duration {  get; set; }
        public Func<Battle, Pokemon, Pokemon, IEffect?, int>? DurationCallback { get; set; }
        public bool? Infiltrates { get; set; }
        public Nonstandard? IsNonstandard { get; set; }

        public Ability(IAnyObject data)
        {
            // Initialize from data object
            if (data.TryGetValue("name", out object? nameValue))
            {
                Name = nameValue?.ToString() ?? string.Empty;
            }

            if (data.TryGetValue("rating", out object? ratingValue) &&
                ratingValue != null && int.TryParse(ratingValue.ToString(), out int rating))
            {
                Rating = rating;
            }

            if (data.TryGetValue("suppressWeather", out object? weatherValue))
            {
                SuppressWeather = Convert.ToBoolean(weatherValue);
            }

            if (data.TryGetValue("flags", out object? flagsValue) && flagsValue is AbilityFlags flags)
            {
                Flags = flags;
            }
            else
            {
                Flags = new AbilityFlags();
            }

            if (data.TryGetValue("num", out object? numValue) &&
                int.TryParse(numValue?.ToString(), out int num))
            {
                Num = num;
            }

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

            AssignMissingFields(data);
        }

        private void AssignMissingFields(IAnyObject data)
        {
            if (data.TryGetValue("desc", out object? descValue))
            {
                Desc = descValue.ToString();
            }

            if (data.TryGetValue("shortDesc", out object? shortDescValue))
            { 
                ShortDesc = shortDescValue.ToString();
            }

            if (data.TryGetValue("exists", out object? existsValue))
            {
                Exists = Convert.ToBoolean(existsValue);
            }
        }
    }

    public static class AbilityConstants
    {
        public static readonly Ability EmptyAbility = new(new DefaultTextData
        {
            ["id"] = "",
            ["name"] = "",
            ["exists"] = false
        });
    }

    public class DexAbilities(ModdedDex dex)
    {
        public ModdedDex Dex { get; } = dex;
        private readonly Dictionary<Id, Ability> _abilityCache = [];
        private List<Ability>? _allCache = null;

        public Ability Get(string name = "")
        {
            throw new NotImplementedException("Get method is not implemented yet.");
        }

        public Ability Get(Ability ability)
        {
            throw new NotImplementedException();
        }

        public Ability GetById(Id id)
        {
            throw new NotImplementedException();
        }

        public List<Ability> All()
        {
            throw new NotImplementedException("All method is not implemented yet.");
        }
    }
}