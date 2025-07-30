namespace ApogeeVGC_CS.sim
{
    public abstract class BasicEffect : IEffectData
    {
        /// <summary>
        /// ID. This will be a lowercase version of the name with all the
        /// non-alphanumeric characters removed. So, for instance, "Mr. Mime"
        /// becomes "mrmime", and "Basculin-Blue-Striped" becomes
        /// "basculinbluestriped".
        /// </summary>
        public Id Id => RealMove != null ? new Id(RealMove) : new Id(Name);

        /// <summary>
        /// Name. Currently does not support Unicode letters, so "Flabébé"
        /// is "Flabebe" and "Nidoran♀" is "Nidoran-F".
        /// </summary>
        public required string Name
        {
            get;
            init
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value), "Name cannot be null.");
                }
                if (value.Length == 0)
                {
                    throw new ArgumentException("Name cannot be empty.", nameof(value));
                }

                field = value.Trim();
            }
        }

        /// <summary>
        /// Full name. Prefixes the name with the effect type. For instance,
        /// Leftovers would be "item: Leftovers", confusion the status
        /// condition would be "confusion", etc.
        /// </summary>
        public virtual required string Fullname
        {
            get => string.IsNullOrEmpty(field) ? Name : field;
            init;
        }

        /// <summary>Effect type.</summary>
        public virtual required EffectType EffectType { get; init; }

        /// <summary>
        /// Does it exist? For historical reasons, when you use an accessor
        /// for an effect that doesn't exist, you get a dummy effect that
        /// doesn't do anything, and this field set to false.
        /// </summary>
        public required bool Exists { get; init; }

        /// <summary>
        /// Dex number? For a Pokemon, this is the National Dex number. For
        /// other effects, this is often an internal ID (e.g. a move
        /// number). Not all effects have numbers, this will be 0 if it
        /// doesn't. Nonstandard effects (e.g. CAP effects) will have
        /// negative numbers.
        /// </summary>
        public required int Num { get; init; }

        /// <summary>
        /// The generation of Pokemon game this was INTRODUCED (NOT
        /// necessarily the current gen being simulated.) Not all effects
        /// track generation; this will be 0 if not known.
        /// </summary>
        public virtual required int Gen { get; init; }

        /// <summary>
        /// A shortened form of the description of this effect.
        /// Not all effects have this.
        /// </summary>
        public string? ShortDesc { get; init; }

        /// <summary>The full description for this effect.</summary>
        public string? Desc { get; init; }

        /// <summary>
        /// Is this item/move/ability/pokemon nonstandard? Specified for effects
        /// that have no use in standard formats: made-up pokemon (CAP),
        /// glitches (MissingNo etc), Pokestar pokemon, etc.
        /// </summary>
        public Nonstandard? IsNonstandard { get; init; }

        /// <summary>The duration of the condition - only for pure conditions.</summary>
        public int? Duration { get; init; }

        /// <summary>Whether or not the condition is ignored by Baton Pass - only for pure conditions.</summary>
        public required bool NoCopy { get; init; }

        /// <summary>Whether or not the condition affects fainted Pokemon.</summary>
        public required bool AffectsFainted { get; init; }

        /// <summary>Moves only: what status does it set?</summary>
        public Id? Status { get; init; }

        /// <summary>Moves only: what weather does it set?</summary>
        public Id? Weather { get; init; }

        /// <summary>???</summary>
        public required string SourceEffect { get; init; }
        public Func<Battle, Pokemon, Pokemon, IEffect?, int>? DurationCallback { get; init; }
        public bool? Infiltrates { get; init; }
        public string? RealMove { get; init; }

        public override string ToString()
        {
            return Name;
        }
    }

    public class Nature : BasicEffect, IBasicEffect, INatureData
    {
        public StatIdExceptHp? Plus { get; init; }
        public StatIdExceptHp? Minus { get; init; }

        public override required string Fullname
        {
            get => $"nature: {Name}";
            init { } // Placeholder: does nothing, but required by the base class
        }
        public override required EffectType EffectType
        {
            get => EffectType.Nature;
            init { } // Placeholder: does nothing, but required by the base class
        }
        public override required int Gen
        {
            get => 3;
            init { } // Placeholder: does nothing, but required by the base class
        }
    }

    public static class NatureConstants
    {
        public static readonly Nature EmptyNature = new()
        {
            Name = "",
            Exists = false,
            EffectType = EffectType.Nature,
            Plus = null,
            Minus = null,
            Fullname = string.Empty,
            Gen = 0,
            Num = 0,
            NoCopy = false,
            AffectsFainted = false,
            SourceEffect = string.Empty
        };
    }

    public interface INatureData
    {
        public string Name { get; }
        public StatIdExceptHp? Plus { get; }
        public StatIdExceptHp? Minus { get; }
    }

    public class NatureData : INatureData
    {
        public required string Name { get; init; }
        public StatIdExceptHp? Plus { get; init; }
        public StatIdExceptHp? Minus { get; init; }
    }

    public class ModdedNatureData : NatureData
    {
        public static bool Inherit => true;
    }

    public class NatureDataTable : Dictionary<IdEntry, INatureData>;

    public class DexNatures(ModdedDex dex)
    {
        private ModdedDex Dex { get; }= dex;
        private Dictionary<Id, Nature> NatureCache { get;} = new();
        private Nature[]? AllCache { get; } = null;

        public Nature Get(string name)
        {
            return GetById(new Id(name));
        }

        public static Nature Get(Nature nature)
        {
            return nature;
        }

        public Nature GetById(Id id)
        {
            throw new Exception("GetById method is not implemented yet.");
        }

        public Nature[] All()
        {
            throw new Exception();
        }
    }

    public interface ITypeData
    {
        public Dictionary<string, int> DamageTaken { get; }
        public SparseStatsTable? HpDvs { get; }
        public SparseStatsTable? HpIvs { get; }
        public Nonstandard? IsNonstandard { get; }
    }

    public class TypeData : ITypeData
    {
        public required Dictionary<string, int> DamageTaken { get; init; }
        public SparseStatsTable? HpDvs { get; init; }
        public SparseStatsTable? HpIvs { get; init; }
        public Nonstandard? IsNonstandard { get; init; }
    }

    public interface IModdedTypeData : ITypeData
    {
        public static bool Inherit => true;
    }

    public class TypeDataTable : Dictionary<IdEntry, ITypeData>;

    public class ModdedTypeDataTable : Dictionary<IdEntry, IModdedTypeData>;

    public enum TypeInfoEffectType
    {
        Type,
        EffectType
    }

    public class TypeInfo : ITypeData
    {
        /// <summary>
        /// ID. This will be a lowercase version of the name with all the
        /// non-alphanumeric characters removed. e.g. 'flying'
        /// </summary>
        public required Id Id { get; init; }

        /// <summary>Name. e.g. 'Flying'</summary>
        public required string Name { get; init; }

        /// <summary>Effect type.</summary>
        public required TypeInfoEffectType EffectType { get; init; }

        /// <summary>
        /// Does it exist? For historical reasons, when you use an accessor
        /// for an effect that doesn't exist, you get a dummy effect that
        /// doesn't do anything, and this field set to false.
        /// </summary>
        public required bool Exists { get; init; }

        /// <summary>
        /// The generation of Pokemon game this was INTRODUCED (NOT
        /// necessarily the current gen being simulated.) Not all effects
        /// track generation; this will be 0 if not known.
        /// </summary>
        public required int Gen { get; init; }

        /// <summary>
        /// Set to 'Future' for types before they're released (like Fairy
        /// in Gen 5 or Dark in Gen 1).
        /// </summary>
        public Nonstandard? IsNonstandard { get; init; }

        /// <summary>
        /// Type chart, attackingTypeName:result, effectid:result
        /// result is: 0 = normal, 1 = weakness, 2 = resistance, 3 = immunity
        /// </summary>
        public required Dictionary<string, int> DamageTaken { get; init; }

        /// <summary>The IVs to get this Type Hidden Power (in gen 3 and later)</summary>
        public required SparseStatsTable HpIvs { get; init; }

        /// <summary>The DVs to get this Type Hidden Power (in gen 2).</summary>
        public required SparseStatsTable HpDvs { get; init; }

        public override string ToString()
        {
            return Name;
        }
    }

    public static class TypeInfoConstants
    {
        public static readonly TypeInfo EmptyTypeInfo = new()
        {
            Name = "",
            Id = new Id(),
            Exists = false,
            EffectType = TypeInfoEffectType.EffectType,
            Gen = 0,
            DamageTaken = [],
            HpIvs = [],
            HpDvs = []
        };
    }

    public class DexTypes(ModdedDex dex)
    {
        private ModdedDex Dex { get; } = dex;
        private Dictionary<Id, TypeInfo> TypeCache { get; } = [];
        private TypeInfo[]? AllCache{ get; } = null;
        private string[]? NamesCache { get; } = null;

        public TypeInfo Get(string name)
        {
            throw new Exception("Get method is not implemented yet.");
        }

        public TypeInfo Get(TypeInfo type)
        {
            throw new Exception("Get method is not implemented yet.");
        }

        public TypeInfo GetById(Id id)
        {
            throw new Exception();
        }

        public string[] Names()
        {
            throw new Exception("Names method is not implemented yet.");
        }

        public bool IsName(string? name)
        {
            throw new Exception("IsName method is not implemented yet.");
        }

        public TypeInfo[] All()
        {
            throw new Exception("All method is not implemented yet.");
        }
    }

    public class DexStats
    {
        public Dictionary<StatId, string> ShortNames { get; }
        public Dictionary<StatId, string> MediumNames { get; }
        public Dictionary<StatId, string> Names { get; }

        private static readonly StatId[] IdsCache =
        [
            StatId.Hp,
            StatId.Atk,
            StatId.Def,
            StatId.Spa,
            StatId.Spd,
            StatId.Spe
        ];
        
        public static readonly Dictionary<IdEntry, StatId> ReverseCache = new()
        {
            [new IdEntry("hitpoints")] = StatId.Hp,
            [new IdEntry("attack")] = StatId.Atk,
            [new IdEntry("defense")] = StatId.Def,
            [new IdEntry("specialattack")] = StatId.Spa,
            [new IdEntry("spatk")] = StatId.Spa,
            [new IdEntry("spattack")] = StatId.Spa,
            [new IdEntry("specialatk")] = StatId.Spa,
            [new IdEntry("special")] = StatId.Spa,
            [new IdEntry("spc")] = StatId.Spa,
            [new IdEntry("specialdefense")] = StatId.Spd,
            [new IdEntry("spdef")] = StatId.Spd,
            [new IdEntry("spdefense")] = StatId.Spd,
            [new IdEntry("specialdef")] = StatId.Spd,
            [new IdEntry("speed")] = StatId.Spe,
        };

        public DexStats(ModdedDex dex)
        {
            if (dex.Gen != 1)
            {
                ShortNames = new Dictionary<StatId, string>
                {
                    [StatId.Hp] = "HP",
                    [StatId.Atk] = "Atk", 
                    [StatId.Def] = "Def",
                    [StatId.Spa] = "SpA",
                    [StatId.Spd] = "SpD",
                    [StatId.Spe] = "Spe"
                };

                MediumNames = new Dictionary<StatId, string>
                {
                    [StatId.Hp] = "HP",
                    [StatId.Atk] = "Attack",
                    [StatId.Def] = "Defense",
                    [StatId.Spa] = "Sp. Atk",
                    [StatId.Spd] = "Sp. Def",
                    [StatId.Spe] = "Speed"
                };

                Names = new Dictionary<StatId, string>
                {
                    [StatId.Hp] = "HP",
                    [StatId.Atk] = "Attack",
                    [StatId.Def] = "Defense",
                    [StatId.Spa] = "Special Attack",
                    [StatId.Spd] = "Special Defense",
                    [StatId.Spe] = "Speed"
                };
            }
            else
            {
                ShortNames = new Dictionary<StatId, string>
                {
                    [StatId.Hp] = "HP",
                    [StatId.Atk] = "Atk",
                    [StatId.Def] = "Def",
                    [StatId.Spa] = "Spc",
                    [StatId.Spd] = "[SpD]",
                    [StatId.Spe] = "Spe"
                };

                MediumNames = new Dictionary<StatId, string>
                {
                    [StatId.Hp] = "HP",
                    [StatId.Atk] = "Attack",
                    [StatId.Def] = "Defense",
                    [StatId.Spa] = "Special",
                    [StatId.Spd] = "[Sp. Def]",
                    [StatId.Spe] = "Speed"
                };

                Names = new Dictionary<StatId, string>
                {
                    [StatId.Hp] = "HP",
                    [StatId.Atk] = "Attack",
                    [StatId.Def] = "Defense",
                    [StatId.Spa] = "Special",
                    [StatId.Spd] = "[Special Defense]",
                    [StatId.Spe] = "Speed"
                };
            }
        }

        public StatId? GetId(string name)
        {
            throw new Exception();
        }

        public StatId[] Ids()
        {
            return IdsCache;
        }
    }
}
