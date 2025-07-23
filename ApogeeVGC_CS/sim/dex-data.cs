/**
 * Dex Data
 * Pokemon Showdown - http://pokemonshowdown.com/
 *
 * @license MIT
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace ApogeeVGC_CS.sim
{
    /// <summary>
    /// Converts anything to an ID. An ID must have only lowercase alphanumeric
    /// characters.
    /// 
    /// If a string is passed, it will be converted to lowercase and
    /// non-alphanumeric characters will be stripped.
    /// 
    /// If an object with an ID is passed, its ID will be returned.
    /// Otherwise, an empty string will be returned.
    /// 
    /// Generally assigned to the global toID, because of how
    /// commonly it's used.
    /// </summary>
    public static class DexUtilities
    {
        public static string ToID(object? text)
        {
            if (text == null) return string.Empty;
            
            string? textStr = null;
            
            if (text is string str)
            {
                textStr = str;
            }
            else if (text is int number)
            {
                textStr = number.ToString();
            }
            else
            {
                // Try to get ID from object properties
                var type = text.GetType();
                var idProp = type.GetProperty("Id") ?? type.GetProperty("ID") ?? 
                            type.GetProperty("UserId") ?? type.GetProperty("RoomId");
                
                if (idProp != null && idProp.GetValue(text) is string propValue)
                {
                    textStr = propValue;
                }
                else
                {
                    textStr = text.ToString();
                }
            }

            if (string.IsNullOrEmpty(textStr)) return string.Empty;

            return Regex.Replace(textStr.ToLowerInvariant(), @"[^a-z0-9]+", "");
        }
    }

    /// <summary>
    /// Like Object.assign but only assigns fields missing from self.
    /// Facilitates consistent field ordering in constructors.
    /// Modifies self in-place.
    /// </summary>
    public static class ObjectExtensions
    {
        public static void AssignMissingFields(IAnyObject self, IAnyObject data)
        {
            foreach (var kvp in data)
            {
                if (!self.ContainsKey(kvp.Key))
                {
                    self[kvp.Key] = kvp.Value;
                }
            }
        }
    }

    public abstract class BasicEffect : EffectData
    {
        /// <summary>
        /// ID. This will be a lowercase version of the name with all the
        /// non-alphanumeric characters removed. So, for instance, "Mr. Mime"
        /// becomes "mrmime", and "Basculin-Blue-Striped" becomes
        /// "basculinbluestriped".
        /// </summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// Name. Currently does not support Unicode letters, so "Flabébé"
        /// is "Flabebe" and "Nidoran♀" is "Nidoran-F".
        /// </summary>
        public new string Name { get; set; } = string.Empty;

        /// <summary>
        /// Full name. Prefixes the name with the effect type. For instance,
        /// Leftovers would be "item: Leftovers", confusion the status
        /// condition would be "confusion", etc.
        /// </summary>
        public string Fullname { get; set; } = string.Empty;

        /// <summary>Effect type.</summary>
        public new EffectType EffectType { get; set; }

        /// <summary>
        /// Does it exist? For historical reasons, when you use an accessor
        /// for an effect that doesn't exist, you get a dummy effect that
        /// doesn't do anything, and this field set to false.
        /// </summary>
        public bool Exists { get; set; }

        /// <summary>
        /// Dex number? For a Pokemon, this is the National Dex number. For
        /// other effects, this is often an internal ID (e.g. a move
        /// number). Not all effects have numbers, this will be 0 if it
        /// doesn't. Nonstandard effects (e.g. CAP effects) will have
        /// negative numbers.
        /// </summary>
        public int Num { get; set; }

        /// <summary>
        /// The generation of Pokemon game this was INTRODUCED (NOT
        /// necessarily the current gen being simulated.) Not all effects
        /// track generation; this will be 0 if not known.
        /// </summary>
        public int Gen { get; set; }

        /// <summary>
        /// A shortened form of the description of this effect.
        /// Not all effects have this.
        /// </summary>
        public new string? ShortDesc { get; set; }

        /// <summary>The full description for this effect.</summary>
        public new string? Desc { get; set; }

        /// <summary>
        /// Is this item/move/ability/pokemon nonstandard? Specified for effects
        /// that have no use in standard formats: made-up pokemon (CAP),
        /// glitches (MissingNo etc), Pokestar pokemon, etc.
        /// </summary>
        public new Nonstandard? IsNonstandard { get; set; }

        /// <summary>The duration of the condition - only for pure conditions.</summary>
        public new int? Duration { get; set; }

        /// <summary>Whether or not the condition is ignored by Baton Pass - only for pure conditions.</summary>
        public bool NoCopy { get; set; }

        /// <summary>Whether or not the condition affects fainted Pokemon.</summary>
        public bool AffectsFainted { get; set; }

        /// <summary>Moves only: what status does it set?</summary>
        public string? Status { get; set; }

        /// <summary>Moves only: what weather does it set?</summary>
        public string? Weather { get; set; }

        /// <summary>???</summary>
        public string SourceEffect { get; set; } = string.Empty;

        protected BasicEffect(IAnyObject data)
        {
            Name = GetString(data, "name").Trim();
            
            // Hidden Power hack
            if (data.ContainsKey("realMove") && data["realMove"] is string realMove)
            {
                Id = DexUtilities.ToID(realMove);
            }
            else
            {
                Id = DexUtilities.ToID(Name);
            }

            Fullname = GetString(data, "fullname");
            if (string.IsNullOrEmpty(Fullname))
                Fullname = Name;

            var effectTypeStr = GetString(data, "effectType");
            if (Enum.TryParse<EffectType>(effectTypeStr, true, out var effectType))
                EffectType = effectType;
            else
                EffectType = sim.EffectType.Condition;

            Exists = GetBool(data, "exists") ?? !string.IsNullOrEmpty(Id);
            Num = GetInt(data, "num") ?? 0;
            Gen = GetInt(data, "gen") ?? 0;
            ShortDesc = GetString(data, "shortDesc");
            Desc = GetString(data, "desc");
            
            var nonStandardStr = GetString(data, "isNonstandard");
            if (!string.IsNullOrEmpty(nonStandardStr) && Enum.TryParse<Nonstandard>(nonStandardStr, true, out var nonStandard))
                IsNonstandard = nonStandard;

            Duration = GetInt(data, "duration");
            NoCopy = GetBool(data, "noCopy") ?? false;
            AffectsFainted = GetBool(data, "affectsFainted") ?? false;
            Status = GetString(data, "status");
            Weather = GetString(data, "weather");
            SourceEffect = GetString(data, "sourceEffect") ?? string.Empty;
        }

        private static string GetString(IAnyObject data, string key)
        {
            return data.ContainsKey(key) && data[key] is string str ? str : string.Empty;
        }

        private static int? GetInt(IAnyObject data, string key)
        {
            if (!data.ContainsKey(key)) return null;
            var value = data[key];
            if (value is int i) return i;
            if (value is string s && int.TryParse(s, out var parsed)) return parsed;
            return null;
        }

        private static bool? GetBool(IAnyObject data, string key)
        {
            if (!data.ContainsKey(key)) return null;
            var value = data[key];
            if (value is bool b) return b;
            if (value is string s && bool.TryParse(s, out var parsed)) return parsed;
            return null;
        }

        public override string ToString()
        {
            return Name;
        }
    }

    public class Nature : BasicEffect
    {
        public override EffectType EffectType => sim.EffectType.Nature;
        public StatIDExceptHP? Plus { get; }
        public StatIDExceptHP? Minus { get; }

        public Nature(IAnyObject data) : base(data)
        {
            Fullname = $"nature: {Name}";
            EffectType = sim.EffectType.Nature;
            Gen = 3;

            if (data.ContainsKey("plus") && data["plus"] is string plusStr &&
                Enum.TryParse<StatIDExceptHP>(plusStr, true, out var plus))
            {
                Plus = plus;
            }

            if (data.ContainsKey("minus") && data["minus"] is string minusStr &&
                Enum.TryParse<StatIDExceptHP>(minusStr, true, out var minus))
            {
                Minus = minus;
            }

            ObjectExtensions.AssignMissingFields(new DefaultTextData(), data); // Placeholder for assignMissingFields
        }
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

    public class NatureDataTable : Dictionary<string, NatureData> { }

    public class DexNatures
    {
        private readonly ModdedDex _dex;
        private readonly Dictionary<string, Nature> _natureCache = new();
        private Nature[]? _allCache;

        private static readonly Nature EmptyNature = new(new DefaultTextData { ["name"] = "", ["exists"] = false });

        public DexNatures(ModdedDex dex)
        {
            _dex = dex;
        }

        public Nature Get(object? name)
        {
            if (name is Nature nature) return nature;
            return GetByID(DexUtilities.ToID(name));
        }

        public Nature GetByID(string id)
        {
            if (string.IsNullOrEmpty(id)) return EmptyNature;

            if (_natureCache.TryGetValue(id, out var nature))
                return nature;

            var alias = _dex.GetAlias(id);
            if (!string.IsNullOrEmpty(alias))
            {
                nature = Get(alias);
                if (nature.Exists)
                {
                    _natureCache[id] = nature;
                }
                return nature;
            }

            if (!string.IsNullOrEmpty(id) && _dex.Data.Natures?.ContainsKey(id) == true)
            {
                var natureData = _dex.Data.Natures[id];
                var data = new DefaultTextData
                {
                    ["name"] = natureData.Name,
                    ["plus"] = natureData.Plus?.ToString(),
                    ["minus"] = natureData.Minus?.ToString()
                };
                nature = new Nature(data);
                if (nature.Gen > _dex.Gen)
                    nature.IsNonstandard = Nonstandard.Future;
            }
            else
            {
                var data = new DefaultTextData { ["name"] = id, ["exists"] = false };
                nature = new Nature(data);
            }

            if (nature.Exists)
                _natureCache[id] = nature; // In real implementation, this would be deep frozen

            return nature;
        }

        public Nature[] All()
        {
            if (_allCache != null) return _allCache;

            var natures = new List<Nature>();
            if (_dex.Data.Natures != null)
            {
                foreach (var id in _dex.Data.Natures.Keys)
                {
                    natures.Add(GetByID(id));
                }
            }

            _allCache = natures.ToArray();
            return _allCache;
        }
    }

    public interface ITypeData
    {
        Dictionary<string, int> DamageTaken { get; set; }
        SparseStatsTable? HPdvs { get; set; }
        SparseStatsTable? HPivs { get; set; }
        Nonstandard? IsNonstandard { get; set; }
    }

    public class TypeData : ITypeData
    {
        public Dictionary<string, int> DamageTaken { get; set; } = new();
        public SparseStatsTable? HPdvs { get; set; }
        public SparseStatsTable? HPivs { get; set; }
        public Nonstandard? IsNonstandard { get; set; }
    }

    public class ModdedTypeData : TypeData
    {
        public bool Inherit { get; set; }
    }

    public class TypeDataTable : Dictionary<string, TypeData> { }
    public class ModdedTypeDataTable : Dictionary<string, ModdedTypeData> { }

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
        public string Id { get; }

        /// <summary>Name. e.g. 'Flying'</summary>
        public string Name { get; }

        /// <summary>Effect type.</summary>
        public TypeInfoEffectType EffectType { get; }

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
        public int Gen { get; }

        /// <summary>
        /// Set to 'Future' for types before they're released (like Fairy
        /// in Gen 5 or Dark in Gen 1).
        /// </summary>
        public Nonstandard? IsNonstandard { get; set; }

        /// <summary>
        /// Type chart, attackingTypeName:result, effectid:result
        /// result is: 0 = normal, 1 = weakness, 2 = resistance, 3 = immunity
        /// </summary>
        public Dictionary<string, int> DamageTaken { get; set; } = new();

        /// <summary>The IVs to get this Type Hidden Power (in gen 3 and later)</summary>
        public SparseStatsTable HPivs { get; set; } = new();

        /// <summary>The DVs to get this Type Hidden Power (in gen 2).</summary>
        public SparseStatsTable HPdvs { get; set; } = new();

        public TypeInfo(IAnyObject data)
        {
            Name = data.ContainsKey("name") && data["name"] is string name ? name : string.Empty;
            Id = data.ContainsKey("id") && data["id"] is string id ? id : string.Empty;

            var effectTypeStr = data.ContainsKey("effectType") && data["effectType"] is string effectType ? effectType : "Type";
            EffectType = Enum.TryParse<TypeInfoEffectType>(effectTypeStr, true, out var parsedEffectType) 
                ? parsedEffectType 
                : TypeInfoEffectType.Type;

            Exists = data.ContainsKey("exists") && data["exists"] is bool exists ? exists : !string.IsNullOrEmpty(Id);
            Gen = data.ContainsKey("gen") && data["gen"] is int gen ? gen : 0;

            var nonStandardStr = data.ContainsKey("isNonstandard") && data["isNonstandard"] is string nonStandard ? nonStandard : null;
            if (!string.IsNullOrEmpty(nonStandardStr) && Enum.TryParse<Nonstandard>(nonStandardStr, true, out var parsedNonStandard))
                IsNonstandard = parsedNonStandard;

            if (data.ContainsKey("damageTaken") && data["damageTaken"] is Dictionary<string, int> damageTaken)
                DamageTaken = damageTaken;

            if (data.ContainsKey("HPivs") && data["HPivs"] is SparseStatsTable hpIvs)
                HPivs = hpIvs;

            if (data.ContainsKey("HPdvs") && data["HPdvs"] is SparseStatsTable hpDvs)
                HPdvs = hpDvs;

            ObjectExtensions.AssignMissingFields(new DefaultTextData(), data); // Placeholder
        }

        public override string ToString()
        {
            return Name;
        }
    }

    public class DexTypes
    {
        private readonly ModdedDex _dex;
        private readonly Dictionary<string, TypeInfo> _typeCache = new();
        private TypeInfo[]? _allCache;
        private string[]? _namesCache;

        private static readonly TypeInfo EmptyTypeInfo = new(new DefaultTextData 
        { 
            ["name"] = "", 
            ["id"] = "", 
            ["exists"] = false, 
            ["effectType"] = "EffectType" 
        });

        public DexTypes(ModdedDex dex)
        {
            _dex = dex;
        }

        public TypeInfo Get(object? name)
        {
            if (name is TypeInfo typeInfo) return typeInfo;
            return GetByID(DexUtilities.ToID(name));
        }

        public TypeInfo GetByID(string id)
        {
            if (string.IsNullOrEmpty(id)) return EmptyTypeInfo;

            if (_typeCache.TryGetValue(id, out var type))
                return type;

            var typeName = char.ToUpperInvariant(id[0]) + id.Substring(1);
            
            if (!string.IsNullOrEmpty(typeName) && _dex.Data.TypeChart?.ContainsKey(id) == true)
            {
                var typeChartData = _dex.Data.TypeChart[id];
                var data = new DefaultTextData
                {
                    ["name"] = typeName,
                    ["id"] = id,
                    ["damageTaken"] = typeChartData.DamageTaken,
                    ["HPivs"] = typeChartData.HPivs,
                    ["HPdvs"] = typeChartData.HPdvs,
                    ["isNonstandard"] = typeChartData.IsNonstandard?.ToString()
                };
                type = new TypeInfo(data);
            }
            else
            {
                var data = new DefaultTextData
                {
                    ["name"] = typeName,
                    ["id"] = id,
                    ["exists"] = false,
                    ["effectType"] = "EffectType"
                };
                type = new TypeInfo(data);
            }

            if (type.Exists)
                _typeCache[id] = type; // In real implementation, would be deep frozen

            return type;
        }

        public string[] Names()
        {
            if (_namesCache != null) return _namesCache;

            _namesCache = All()
                .Where(type => type.IsNonstandard == null)
                .Select(type => type.Name)
                .ToArray();

            return _namesCache;
        }

        public bool IsName(string? name)
        {
            if (string.IsNullOrEmpty(name)) return false;
            var id = name.ToLowerInvariant();
            var typeName = char.ToUpperInvariant(id[0]) + id.Substring(1);
            return name == typeName && _dex.Data.TypeChart?.ContainsKey(id) == true;
        }

        public TypeInfo[] All()
        {
            if (_allCache != null) return _allCache;

            var types = new List<TypeInfo>();
            if (_dex.Data.TypeChart != null)
            {
                foreach (var id in _dex.Data.TypeChart.Keys)
                {
                    types.Add(GetByID(id));
                }
            }

            _allCache = types.ToArray();
            return _allCache;
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
            if (name == "Spd") return StatID.Spe;
            
            var id = DexUtilities.ToID(name);
            
            if (ReverseCache.TryGetValue(id, out var statId))
                return statId;
                
            if (Enum.TryParse<StatID>(id, true, out var parsedStatId) && IdsCache.Contains(parsedStatId))
                return parsedStatId;
                
            return null;
        }

        public StatID[] Ids()
        {
            return IdsCache;
        }
    }

    public class DexData
    {
        public NatureDataTable? Natures { get; set; }
        public TypeDataTable? TypeChart { get; set; }
    }
}
