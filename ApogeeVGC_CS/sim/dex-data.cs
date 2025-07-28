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
        public string? ShortDesc { get; protected set; }

        /// <summary>The full description for this effect.</summary>
        public string? Desc { get; protected set; }

        /// <summary>
        /// Is this item/move/ability/pokemon nonstandard? Specified for effects
        /// that have no use in standard formats: made-up pokemon (CAP),
        /// glitches (MissingNo etc), Pokestar pokemon, etc.
        /// </summary>
        public Nonstandard? IsNonstandard { get; protected set; }

        /// <summary>The duration of the condition - only for pure conditions.</summary>
        public int? Duration { get; protected set; }

        /// <summary>Whether or not the condition is ignored by Baton Pass - only for pure conditions.</summary>
        public required bool NoCopy { get; init; }

        /// <summary>Whether or not the condition affects fainted Pokemon.</summary>
        public required bool AffectsFainted { get; init; }

        /// <summary>Moves only: what status does it set?</summary>
        public Id? Status { get; protected set; }

        /// <summary>Moves only: what weather does it set?</summary>
        public Id? Weather { get; protected set; }

        /// <summary>???</summary>
        public required string SourceEffect { get; init; }
        public Func<Battle, Pokemon, Pokemon, IEffect?, int>? DurationCallback { get; protected set; }
        public bool? Infiltrates { get; protected set; }
        public string? RealMove { get; protected set; }

        //public void InitBasicEffect()
        //{
        //    if (Name != string.Empty)
        //    {
        //        Name = Name.Trim();
        //    }
        //    else
        //    {
        //        throw new ArgumentException("Name is required for BasicEffect.");
        //    }

        //    if (RealMove != null)
        //    {
        //        Id = new Id(RealMove);
        //    }
        //    else
        //    {
        //        Id = new Id(Name);
        //    }

        //    if (Fullname == string.Empty)
        //    {
        //        Fullname = Name;
        //    }

        //    if (!Exists)
        //    {                 
        //        Exists = !Id.IsEmpty;
        //    }
        //}

        public override string ToString()
        {
            return Name;
        }
    }

    //public interface INature : IBasicEffect
    //{
    //    public StatIdExceptHp? Plus { get; set; }
    //    public StatIdExceptHp? Minus { get; set; }
    //}

    //public class Nature : BasicEffect, INature
    //{
    //    public StatIdExceptHp? Plus { get; set; } = null;
    //    public StatIdExceptHp? Minus { get; set; } = null;

    //    public Nature(INatureData other) : base(other)
    //    {
    //        Plus = other.Plus;
    //        Minus = other.Minus;
    //    }

    //    public void Init()
    //    {
    //        InitBasicEffect();

    //        Fullname = $"nature: {Name}";
    //        EffectType = EffectType.Nature;
    //        Gen = 3;
    //    }
    //}

    //public class Nature(Nature other) : BasicEffect(other), IBasicEffect, INatureData
    //{
    //    public StatIdExceptHp? Plus { get; } = other.Plus;
    //    public StatIdExceptHp? Minus { get; } = other.Minus;

    //    public void Init()
    //    {
    //        InitBasicEffect();

    //        Fullname = $"nature: {Name}";
    //        EffectType = EffectType.Nature;
    //        Gen = 3;
    //    }
    //}

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

    // public interface INatureData : INature { }


    public interface INatureData
    {
        public string Name { get; }
        public StatIdExceptHp? Plus { get; }
        public StatIdExceptHp? Minus { get; }
    }

    public class NatureData : INatureData
    {
        public StatIdExceptHp? Plus { get; set; }
        public StatIdExceptHp? Minus { get; set; }
        public Id Id { get; set; }
        public string Name { get; set; }
        public string Fullname { get; set; }
        public EffectType EffectType { get; set; }
        public bool Exists { get; set; }
        public int Num { get; set; }
        public int Gen { get; set; }
        public string? ShortDesc { get; set; }
        public string? Desc { get; set; }
        public Nonstandard? IsNonstandard { get; set; }
        public int? Duration { get; set; }
        public bool NoCopy { get; set; }
        public bool AffectsFainted { get; set; }
        public Id? Status { get; set; }
        public Id? Weather { get; set; }
        public string SourceEffect { get; set; }
        public Func<Battle, Pokemon, Pokemon, IEffect?, int>? DurationCallback { get; set; }
        //public string? EffectTypeString { get; set; }
        public bool? Infiltrates { get; set; }
        public string? RealMove { get; set; }
    }

    public class ModdedNatureData : NatureData
    {
        public bool Inherit { get; set; }
    }

    public class NatureDataTable : Dictionary<IdEntry, INatureData> { }

    public class DexNatures(ModdedDex dex)
    {
        private readonly ModdedDex _dex = dex;
        private readonly Dictionary<string, Nature> _natureCache = [];
        private Nature[]? _allCache = null;

        public Nature Get(string name)
        {
            throw new Exception("Get method is not implemented yet.");
        }

        public Nature Get(Nature nature)
        {
            throw new Exception("Get method is not implemented yet.");
        }

        public Nature GetById(Id id)
        {
            throw new Exception();
        }

        public Nature[] All()
        {
            throw new Exception();
        }
    }

    public interface ITypeData : ITypeInfo
    {
        Dictionary<string, int> DamageTaken { get; set; }
        SparseStatsTable? HPdvs { get; set; }
        SparseStatsTable? HPivs { get; set; }
        Nonstandard? IsNonstandard { get; set; }
    }

    public class TypeData : ITypeData
    {
        public Dictionary<string, int> DamageTaken { get; set; }
        public SparseStatsTable? HPdvs { get; set; }
        public SparseStatsTable? HPivs { get; set; }
        public Nonstandard? IsNonstandard { get; set; }
        public Id Id { get; set; }
        public string Name { get; set; }
        public TypeInfoEffectType EffectType { get; set; }
        public bool Exists { get; set; }
        public int Gen { get; set; }
    }

    public interface IModdedTypeData : ITypeData
    {
        public bool Inherit { get; set; }
    }

    public class TypeDataTable : Dictionary<IdEntry, ITypeData> { }
    public class IModdedTypeDataTable : Dictionary<IdEntry, IModdedTypeData> { }

    public enum TypeInfoEffectType
    {
        Type,
        EffectType
    }

    public interface ITypeInfo
    {
        public Id Id { get; set; }
        public string Name { get; set; }
        public TypeInfoEffectType EffectType { get; set; }
        public bool Exists { get; set; }
        public int Gen { get; set; }
    }

    public class TypeInfo : ITypeData
    {
        /// <summary>
        /// ID. This will be a lowercase version of the name with all the
        /// non-alphanumeric characters removed. e.g. 'flying'
        /// </summary>
        public Id Id { get; set; } = new Id();

        /// <summary>Name. e.g. 'Flying'</summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>Effect type.</summary>
        public TypeInfoEffectType EffectType { get; set; } = TypeInfoEffectType.Type;

        /// <summary>
        /// Does it exist? For historical reasons, when you use an accessor
        /// for an effect that doesn't exist, you get a dummy effect that
        /// doesn't do anything, and this field set to false.
        /// </summary>
        public bool Exists { get; set; } = true;

        /// <summary>
        /// The generation of Pokemon game this was INTRODUCED (NOT
        /// necessarily the current gen being simulated.) Not all effects
        /// track generation; this will be 0 if not known.
        /// </summary>
        public int Gen { get; set; } = 0;

        /// <summary>
        /// Set to 'Future' for types before they're released (like Fairy
        /// in Gen 5 or Dark in Gen 1).
        /// </summary>
        public Nonstandard? IsNonstandard { get; set; } = null;

        /// <summary>
        /// Type chart, attackingTypeName:result, effectid:result
        /// result is: 0 = normal, 1 = weakness, 2 = resistance, 3 = immunity
        /// </summary>
        public Dictionary<string, int> DamageTaken { get; set; } = [];

        /// <summary>The IVs to get this Type Hidden Power (in gen 3 and later)</summary>
        public SparseStatsTable HPivs { get; set; } = [];

        /// <summary>The DVs to get this Type Hidden Power (in gen 2).</summary>
        public SparseStatsTable HPdvs { get; set; } = [];

        public TypeInfo(ITypeData data)
        {
            
            Id = data.Id;
            Name = data.Name;
            EffectType = data.EffectType;
            Exists = data.Exists;
            Gen = data.Gen;
            IsNonstandard = data.IsNonstandard;
            DamageTaken = data.DamageTaken;
            HPivs = data.HPivs;
            HPdvs = data.HPdvs;
        }

        public void Init()
        {
            if (!Exists)
            {
                Exists = !Id.IsEmpty;
            }
        }

        public override string ToString()
        {
            return Name;
        }
    }

    public static class TypeInfoConstants
    {
        public static readonly TypeInfo EmptyTypeInfo = new(new TypeData
        {
            Name = "",
            Id = new Id(),
            Exists = false,
            EffectType = TypeInfoEffectType.EffectType
        });
    }

    public class DexTypes(ModdedDex dex)
    {
        private readonly ModdedDex _dex = dex;
        private readonly Dictionary<Id, TypeInfo> _typeCache = [];
        private TypeInfo[]? _allCache = null;
        private string[]? _namesCache = null;

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

        private static readonly StatId[] IdsCache = { StatId.Hp, StatId.Atk, StatId.Def, StatId.Spa, StatId.Spd, StatId.Spe };
        
        private static readonly Dictionary<string, StatId> ReverseCache = new()
        {
            ["hitpoints"] = StatId.Hp,
            ["attack"] = StatId.Atk,
            ["defense"] = StatId.Def,
            ["specialattack"] = StatId.Spa,
            ["spatk"] = StatId.Spa,
            ["spattack"] = StatId.Spa,
            ["specialatk"] = StatId.Spa,
            ["special"] = StatId.Spa,
            ["spc"] = StatId.Spa,
            ["specialdefense"] = StatId.Spd,
            ["spdef"] = StatId.Spd,
            ["spdefense"] = StatId.Spd,
            ["specialdef"] = StatId.Spd,
            ["speed"] = StatId.Spe,
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
