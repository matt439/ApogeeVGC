namespace ApogeeVGC_CS.sim
{
    public interface IAbilityEventMethods
    {
        Action<Battle, Pokemon>? OnCheckShow { get; }
        Action<Battle, Pokemon>? OnEnd { get; }
        Action<Battle, Pokemon>? OnStart { get; }
    }

    public interface IAbilityFlags
    {
        public bool? Breakable { get; } // Can be suppressed by Mold Breaker and related effects
        public bool? CantSuppress { get; } // Ability can't be suppressed by e.g. Gastro Acid or Neutralizing Gas
        public bool? FailRolePlay { get; } // Role Play fails if target has this Ability
        public bool? FailSkillSwap { get; } // Skill Swap fails if either the user or target has this Ability
        public bool? NoEntrain { get; } // Entrainment fails if user has this Ability
        public bool? NoReceiver { get; } // Receiver and Power of Alchemy will not activate if an ally faints with this Ability
        public bool? NoTrace { get; } // Trace cannot copy this Ability
        public bool? NoTransform { get; } // Disables the Ability if the user is Transformed
    }

    //public interface IAbilityData : IAbilityEventMethods, IPokemonEventMethods
    //{
    //    string Name { get; set; }
    //    // Additional properties from Ability can be added here
    //}

    

    // Modded ability data with inheritance support
    public class ModdedAbilityData : AbilityData
    {
        public bool Inherit { get; set; } = true;
    }

    // Type aliases for data tables
    public class AbilityDataTable : Dictionary<IdEntry, AbilityData> { }
    public class ModdedAbilityDataTable : Dictionary<IdEntry, ModdedAbilityData> { }

    /// <summary>
    /// Represents an ability effect.
    /// </summary>
    public interface IAbility : IBasicEffect, IEffect 
    {
        int Rating { get; set; }
        bool SuppressWeather { get; set; }
        AbilityFlags Flags { get; set; }
        IConditionData? Condition { get; set; }
    }

    // Main Ability class extending BasicEffect
    public class Ability : BasicEffect, IAbility
    {
        // Rating from -1 Detrimental to +5 Essential
        public int Rating { get; set; } = 0;
        public bool SuppressWeather { get; set; } = false;
        public AbilityFlags Flags { get; set; } = new();
        public IConditionData? Condition { get; set; } = null;

        public Ability(IAbility other) : base(other)
        {
            Rating = other.Rating;
            SuppressWeather = other.SuppressWeather;
            Flags = other.Flags;
            Condition = other.Condition;
        }

        public void Init()
        {
            InitBasicEffect();
            
            Fullname = $"ability: {Name}";
            EffectType = EffectType.Ability;

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
        }
    }

    public static class AbilityConstants
    {
        public static readonly Ability EmptyAbility = new(new AbilityData
        {
            Id = new Id(string.Empty),
            Name = "",
            Exists = false
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