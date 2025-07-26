using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Runtime.CompilerServices;

namespace ApogeeVGC_CS.sim
{
    public abstract class BasicEffect
    {
        /// <summary>
        /// ID. This will be a lowercase version of the name with all the
        /// non-alphanumeric characters removed. So, for instance, "Mr. Mime"
        /// becomes "mrmime", and "Basculin-Blue-Striped" becomes
        /// "basculinbluestriped".
        /// </summary>
        public Id Id { get; set; } = new Id();

        /// <summary>
        /// Name. Currently does not support Unicode letters, so "Flabébé"
        /// is "Flabebe" and "Nidoran♀" is "Nidoran-F".
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Full name. Prefixes the name with the effect type. For instance,
        /// Leftovers would be "item: Leftovers", confusion the status
        /// condition would be "confusion", etc.
        /// </summary>
        public string Fullname { get; set; } = string.Empty;

        /// <summary>Effect type.</summary>
        public EffectType EffectType { get; set; } = EffectType.Condition;

        /// <summary>
        /// Does it exist? For historical reasons, when you use an accessor
        /// for an effect that doesn't exist, you get a dummy effect that
        /// doesn't do anything, and this field set to false.
        /// </summary>
        public bool Exists { get; set; } = false;

        /// <summary>
        /// Dex number? For a Pokemon, this is the National Dex number. For
        /// other effects, this is often an internal ID (e.g. a move
        /// number). Not all effects have numbers, this will be 0 if it
        /// doesn't. Nonstandard effects (e.g. CAP effects) will have
        /// negative numbers.
        /// </summary>
        public int Num { get; set; } = 0;

        /// <summary>
        /// The generation of Pokemon game this was INTRODUCED (NOT
        /// necessarily the current gen being simulated.) Not all effects
        /// track generation; this will be 0 if not known.
        /// </summary>
        public int Gen { get; set; } = 0;

        /// <summary>
        /// A shortened form of the description of this effect.
        /// Not all effects have this.
        /// </summary>
        public string? ShortDesc { get; set; } = string.Empty;

        /// <summary>The full description for this effect.</summary>
        public string? Desc { get; set; } = string.Empty;

        /// <summary>
        /// Is this item/move/ability/pokemon nonstandard? Specified for effects
        /// that have no use in standard formats: made-up pokemon (CAP),
        /// glitches (MissingNo etc), Pokestar pokemon, etc.
        /// </summary>
        public Nonstandard? IsNonstandard { get; set; } = null;

        /// <summary>The duration of the condition - only for pure conditions.</summary>
        public int? Duration { get; set; } = null;

        /// <summary>Whether or not the condition is ignored by Baton Pass - only for pure conditions.</summary>
        public bool NoCopy { get; set; } = false;

        /// <summary>Whether or not the condition affects fainted Pokemon.</summary>
        public bool AffectsFainted { get; set; } = false;

        /// <summary>Moves only: what status does it set?</summary>
        public Id? Status { get; set; } = null;

        /// <summary>Moves only: what weather does it set?</summary>
        public Id? Weather { get; set; } = null;

        /// <summary>???</summary>
        public string SourceEffect { get; set; } = string.Empty;

        public Func<Battle, Pokemon, Pokemon, IEffect?, int>? DurationCallback { get; set; } = null;
        public string? EffectTypeString { get; set; } = null;
        public bool? Infiltrates { get; set; } = null;
        public string? RealMove { get; set; } = null; // Added this for the Init method

        public BasicEffect() { }
        
        public BasicEffect(IBasicEffect other)
        {
            // Copy all fields without any logic
            Id = other.Id;
            Name = other.Name;
            Fullname = other.Fullname;
            EffectType = other.EffectType;
            Exists = other.Exists;
            Num = other.Num;
            Gen = other.Gen;
            ShortDesc = other.ShortDesc;
            Desc = other.Desc;
            IsNonstandard = other.IsNonstandard;
            Duration = other.Duration;
            NoCopy = other.NoCopy;
            AffectsFainted = other.AffectsFainted;
            Status = other.Status;
            Weather = other.Weather;
            SourceEffect = other.SourceEffect;
            DurationCallback = other.DurationCallback;
            EffectTypeString = other.EffectTypeString;
            Infiltrates = other.Infiltrates;
            RealMove = other.RealMove;

            Init();
        }

        private void Init()
        {
            if (Name != string.Empty)
            {
                Name = Name.Trim();
            }
            else
            {
                throw new ArgumentException("Name is required for BasicEffect.");
            }

            if (RealMove != null)
            {
                Id = new Id(RealMove);
            }
            else
            {
                Id = new Id(Name);
            }

            if (Fullname == string.Empty)
            {
                Fullname = Name;
            }

            if (!Exists)
            {                 
                Exists = !Id.IsEmpty;
            }
        }

        //protected BasicEffect(IAnyObject data)
        //{
        //    if (data.TryGetString("name", out string? name))
        //    {
        //        Name = name.Trim();
        //    }
        //    else
        //    {
        //        throw new ArgumentException("Name is required for BasicEffect.");
        //    }

        //    if (data.TryGetString("realMove", out string? realMove))
        //    {
        //        Id = new Id(realMove);
        //    }
        //    else
        //    {
        //        Id = new Id(Name);
        //    }

        //    if (data.TryGetString("fullname", out string? fullname))
        //    {
        //        Fullname = fullname; // No trim
        //    }
        //    else
        //    {
        //        Fullname = Name;
        //    }

        //    if (data.TryGetEnum<EffectType>("effectType", out var effectType))
        //    {
        //        EffectType = effectType;
        //    }


        //    if (data.TryGetBool("exists", out var exists))
        //    {
        //        Exists = exists;
        //    }
        //    else
        //    {
        //        Exists = !Id.IsEmpty;
        //    }

        //    if (data.TryGetInt("num", out var num))
        //    {
        //        Num = num;
        //    }

        //    if (data.TryGetInt("gen", out var gen))
        //    {
        //        Gen = gen;
        //    }

        //    if (data.TryGetString("shortDesc", out string? shortDesc))
        //    {
        //        ShortDesc = shortDesc;
        //    }

        //    if (data.TryGetString("desc", out string? desc))
        //    {
        //        Desc = desc;
        //    }

        //    if (data.TryGetEnum<Nonstandard>("isNonstandard", out var nonstandard))
        //    {
        //        IsNonstandard = nonstandard;
        //    }

        //    if (data.TryGetInt("duration", out var duration))
        //    {
        //        Duration = duration;
        //    }

        //    if (data.TryGetBool("noCopy", out var noCopy))
        //    {
        //        NoCopy = noCopy;
        //    }

        //    if (data.TryGetBool("affectsFainted", out var affectsFainted))
        //    {
        //        AffectsFainted = affectsFainted;
        //    }

        //    if (data.TryGetId("status", out Id? status))
        //    {
        //        Status = status;
        //    }

        //    if (data.TryGetId("weather", out Id? weather))
        //    {
        //        Weather = weather;
        //    }

        //    if (data.TryGetString("sourceEffect", out string? sourceEffect))
        //    {
        //        SourceEffect = sourceEffect;
        //    }
        //}

        public override string ToString()
        {
            return Name;
        }
    }

    public class Nature : BasicEffect, INatureData
    {
        // public EffectType EffectType => EffectType.Nature;
        public StatIDExceptHP? Plus { get; set; }
        public StatIDExceptHP? Minus { get; set; }

        public Nature(IAnyObject data) : base(data)
        {
            Fullname = $"nature: {Name}";
            EffectType = EffectType.Nature;
            Gen = 3;

            if (data.TryGetEnum<StatIDExceptHP>("plus", out var plus))
            {
                Plus = plus;
            }

            if (data.TryGetEnum<StatIDExceptHP>("minus", out var minus))
            {
                Minus = minus;
            }

            // AssignMissingFields(data);
        }

        //private void AssignMissingFields(IAnyObject data)
        //{

        //}
    }

    public static class NatureConstants
    {
        public static readonly Nature EmptyNature = new(new DefaultTextData
        {
            ["name"] = "",
            ["exists"] = false
        });
    }

    public interface INatureData
    {
        string Name { get; set; }
        StatIDExceptHP? Plus { get; set; }
        StatIDExceptHP? Minus { get; set; }
    }

    public class NatureData : INatureData
    {
        public string Name { get; set; } = string.Empty;
        public StatIDExceptHP? Plus { get; set; }
        public StatIDExceptHP? Minus { get; set; }
    }

    public class ModdedNatureData : NatureData
    {
        public bool Inherit { get; set; }
    }

    public interface INatureDataTable : IDictionary<IdEntry, NatureData> { }

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

        public Nature GetByID(Id id)
        {
            throw new Exception();
        }

        public Nature[] All()
        {
            throw new Exception();
        }
    }

    public interface ITypeData
    {
        Dictionary<string, int> DamageTaken { get; set; }
        SparseStatsTable? HPdvs { get; set; }
        SparseStatsTable? HPivs { get; set; }
        Nonstandard? IsNonstandard { get; set; }
    }

    //public class TypeData : ITypeData
    //{
    //    public Dictionary<string, int> DamageTaken { get; set; } = new();
    //    public SparseStatsTable? HPdvs { get; set; }
    //    public SparseStatsTable? HPivs { get; set; }
    //    public Nonstandard? IsNonstandard { get; set; }
    //}

    public interface IModdedTypeData : ITypeData
    {
        public bool Inherit { get; set; }
    }

    public interface ITypeDataTable : IDictionary<string, ITypeData> { }
    public interface IModdedTypeDataTable : IDictionary<string, IModdedTypeData> { }

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
        public Id Id { get; }

        /// <summary>Name. e.g. 'Flying'</summary>
        public string Name { get; }

        /// <summary>Effect type.</summary>
        public TypeInfoEffectType EffectType { get; } = TypeInfoEffectType.Type;

        /// <summary>
        /// Does it exist? For historical reasons, when you use an accessor
        /// for an effect that doesn't exist, you get a dummy effect that
        /// doesn't do anything, and this field set to false.
        /// </summary>
        public bool Exists { get; }

        /// <summary>
        /// The generation of Pokemon game this was INTRODUCED (NOT
        /// necessarily the current gen being simulated.) Not all effects
        /// track generation; this will be 0 if not known.
        /// </summary>
        public int Gen { get; } = 0;

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

        public TypeInfo(IAnyObject data)
        {
            if (data.TryGetString("name", out string? name))
            {
                Name = name; // no trim
            }
            else
            {
                throw new ArgumentException("Name is required for BasicEffect.");
            }

            if (data.TryGetId("id", out Id? id))
            {
                Id = id;
            }
            else
            {
                throw new ArgumentException("ID is required for TypeInfo.");
            }

            if (data.TryGetEnum<TypeInfoEffectType>("effectType", out TypeInfoEffectType effectType))
            {
                EffectType = effectType;
            }

            if (data.TryGetBool("exists", out bool exists))
            {
                Exists = exists;
            }
            else
            {
                Exists = !Id.IsEmpty;
            }

            if (data.TryGetInt("gen", out int gen))
            {
                Gen = gen;
            }

            if (data.TryGetEnum<Nonstandard>("isNonstandard", out Nonstandard nonstandard))
            {
                IsNonstandard = nonstandard;
            }

            if (data.TryGetDictionary<string, int>("damageTaken", out Dictionary<string, int>? damageTaken))
            {
                DamageTaken = damageTaken;
            }

            if (data.TryGetClass<SparseStatsTable>("HPivs", out SparseStatsTable? hpivs))
            {
                HPivs = hpivs;
            }

            if (data.TryGetClass<SparseStatsTable>("HPdvs", out SparseStatsTable? hpdvs))
            {
                HPdvs = hpdvs;
            }

            //AssignMissingFields(data);
        }
        //private void AssignMissingFields(IAnyObject data)
        //{

        //}

        public override string ToString()
        {
            return Name;
        }
    }

    public static class TypeInfoConstants
    {
        public static readonly TypeInfo EmptyTypeInfo = new(new DefaultTextData
        {
            ["name"] = "",
            ["id"] = "",
            ["exists"] = false,
            ["effectType"] = "EffectType"
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

        public TypeInfo GetByID(Id id)
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
        public Dictionary<StatID, string> ShortNames { get; }
        public Dictionary<StatID, string> MediumNames { get; }
        public Dictionary<StatID, string> Names { get; }

        private static readonly StatID[] IdsCache = { StatID.Hp, StatID.Atk, StatID.Def, StatID.Spa, StatID.Spd, StatID.Spe };
        
        private static readonly Dictionary<string, StatID> ReverseCache = new()
        {
            ["hitpoints"] = StatID.Hp,
            ["attack"] = StatID.Atk,
            ["defense"] = StatID.Def,
            ["specialattack"] = StatID.Spa,
            ["spatk"] = StatID.Spa,
            ["spattack"] = StatID.Spa,
            ["specialatk"] = StatID.Spa,
            ["special"] = StatID.Spa,
            ["spc"] = StatID.Spa,
            ["specialdefense"] = StatID.Spd,
            ["spdef"] = StatID.Spd,
            ["spdefense"] = StatID.Spd,
            ["specialdef"] = StatID.Spd,
            ["speed"] = StatID.Spe,
        };

        public DexStats(ModdedDex dex)
        {
            if (dex.Gen != 1)
            {
                ShortNames = new Dictionary<StatID, string>
                {
                    [StatID.Hp] = "HP",
                    [StatID.Atk] = "Atk", 
                    [StatID.Def] = "Def",
                    [StatID.Spa] = "SpA",
                    [StatID.Spd] = "SpD",
                    [StatID.Spe] = "Spe"
                };

                MediumNames = new Dictionary<StatID, string>
                {
                    [StatID.Hp] = "HP",
                    [StatID.Atk] = "Attack",
                    [StatID.Def] = "Defense",
                    [StatID.Spa] = "Sp. Atk",
                    [StatID.Spd] = "Sp. Def",
                    [StatID.Spe] = "Speed"
                };

                Names = new Dictionary<StatID, string>
                {
                    [StatID.Hp] = "HP",
                    [StatID.Atk] = "Attack",
                    [StatID.Def] = "Defense",
                    [StatID.Spa] = "Special Attack",
                    [StatID.Spd] = "Special Defense",
                    [StatID.Spe] = "Speed"
                };
            }
            else
            {
                ShortNames = new Dictionary<StatID, string>
                {
                    [StatID.Hp] = "HP",
                    [StatID.Atk] = "Atk",
                    [StatID.Def] = "Def",
                    [StatID.Spa] = "Spc",
                    [StatID.Spd] = "[SpD]",
                    [StatID.Spe] = "Spe"
                };

                MediumNames = new Dictionary<StatID, string>
                {
                    [StatID.Hp] = "HP",
                    [StatID.Atk] = "Attack",
                    [StatID.Def] = "Defense",
                    [StatID.Spa] = "Special",
                    [StatID.Spd] = "[Sp. Def]",
                    [StatID.Spe] = "Speed"
                };

                Names = new Dictionary<StatID, string>
                {
                    [StatID.Hp] = "HP",
                    [StatID.Atk] = "Attack",
                    [StatID.Def] = "Defense",
                    [StatID.Spa] = "Special",
                    [StatID.Spd] = "[Special Defense]",
                    [StatID.Spe] = "Speed"
                };
            }
        }

        public StatID? GetID(string name)
        {
            throw new Exception();
        }

        public StatID[] Ids()
        {
            return IdsCache;
        }
    }
}
