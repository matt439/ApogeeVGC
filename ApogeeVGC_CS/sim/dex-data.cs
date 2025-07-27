using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Runtime.CompilerServices;

namespace ApogeeVGC_CS.sim
{
    public abstract class BasicEffect : IBasicEffect
    {
        /// <summary>
        /// ID. This will be a lowercase version of the name with all the
        /// non-alphanumeric characters removed. So, for instance, "Mr. Mime"
        /// becomes "mrmime", and "Basculin-Blue-Striped" becomes
        /// "basculinbluestriped".
        /// </summary>
        public ID Id { get; set; } = new ID();

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
        public ID? Status { get; set; } = null;

        /// <summary>Moves only: what weather does it set?</summary>
        public ID? Weather { get; set; } = null;

        /// <summary>???</summary>
        public string SourceEffect { get; set; } = string.Empty;

        public Func<Battle, Pokemon, Pokemon, IEffect?, int>? DurationCallback { get; set; } = null;
        //public string? EffectTypeString { get; set; } = null;
        public bool? Infiltrates { get; set; } = null;
        public string? RealMove { get; set; } = null; // Added this for the Init method
        
        protected BasicEffect(IBasicEffect other)
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
            //EffectTypeString = other.EffectTypeString;
            Infiltrates = other.Infiltrates;
            RealMove = other.RealMove;
        }

        public void InitBasicEffect()
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
                Id = new ID(RealMove);
            }
            else
            {
                Id = new ID(Name);
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

        public override string ToString()
        {
            return Name;
        }
    }

    public interface INature : IBasicEffect
    {
        public StatIDExceptHP? Plus { get; set; }
        public StatIDExceptHP? Minus { get; set; }
    }

    public class Nature : BasicEffect, INature
    {
        public StatIDExceptHP? Plus { get; set; } = null;
        public StatIDExceptHP? Minus { get; set; } = null;

        public Nature(INatureData other) : base(other)
        {
            Plus = other.Plus;
            Minus = other.Minus;
        }

        public void Init()
        {
            InitBasicEffect();
            
            Fullname = $"nature: {Name}";
            EffectType = EffectType.Nature;
            Gen = 3;
        }
    }

    public static class NatureConstants
    {
        public static readonly Nature EmptyNature = new(new NatureData
        {
            Name = string.Empty,
            Exists = false
        });
    }

    public interface INatureData : INature { }

    public class NatureData : INatureData
    {
        public StatIDExceptHP? Plus { get; set; }
        public StatIDExceptHP? Minus { get; set; }
        public ID Id { get; set; }
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
        public ID? Status { get; set; }
        public ID? Weather { get; set; }
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

    public class NatureDataTable : Dictionary<IDEntry, INatureData> { }

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

        public Nature GetByID(ID id)
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
        public ID Id { get; set; }
        public string Name { get; set; }
        public TypeInfoEffectType EffectType { get; set; }
        public bool Exists { get; set; }
        public int Gen { get; set; }
    }

    public interface IModdedTypeData : ITypeData
    {
        public bool Inherit { get; set; }
    }

    public class TypeDataTable : Dictionary<IDEntry, ITypeData> { }
    public class IModdedTypeDataTable : Dictionary<IDEntry, IModdedTypeData> { }

    public enum TypeInfoEffectType
    {
        Type,
        EffectType
    }

    public interface ITypeInfo
    {
        public ID Id { get; set; }
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
        public ID Id { get; set; } = new ID();

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
            Id = new ID(),
            Exists = false,
            EffectType = TypeInfoEffectType.EffectType
        });
    }

    public class DexTypes(ModdedDex dex)
    {
        private readonly ModdedDex _dex = dex;
        private readonly Dictionary<ID, TypeInfo> _typeCache = [];
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

        public TypeInfo GetByID(ID id)
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
