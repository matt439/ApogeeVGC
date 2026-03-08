using System.Text.Json;
using ApogeeVGC.Data;
using ApogeeVGC.Sim.Abilities;
using ApogeeVGC.Sim.Items;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.SpeciesClasses;

namespace ApogeeVGC.Mcts;

public sealed class Vocab
{
    public const int PadIndex = 0;
    public const int UnknownIndex = 1;
    public const int UnknownSpeciesIndex = 1;
    public const int NoneActionIndex = 1; // "<none>"
    public const int CantActionIndex = 2; // "<cant>"

    public int NumSpecies { get; }
    public int NumActions { get; }
    public int NumMoves { get; }
    public int NumAbilities { get; }
    public int NumItems { get; }
    public int NumTeraTypes { get; }

    private readonly Dictionary<string, int> _speciesNameToIndex;
    private readonly Dictionary<string, int> _actionKeyToIndex;
    private readonly string[] _indexToActionKey;

    // C# enum -> vocab index caches
    private readonly int[] _specieIdToVocabIndex;
    private readonly int[] _moveIdToActionIndex;
    private readonly Dictionary<SpecieId, int> _specieIdToSwitchIndex;

    // Embedding vocab caches (for model input features)
    private readonly int[] _moveIdToMoveVocabIndex;
    private readonly int[] _abilityIdToVocabIndex;
    private readonly int[] _itemIdToVocabIndex;
    private readonly int[] _moveTypeToTeraVocabIndex;

    private Vocab(
        Dictionary<string, int> speciesNameToIndex,
        Dictionary<string, int> actionKeyToIndex,
        int numSpecies,
        int numActions,
        int numMoves,
        int numAbilities,
        int numItems,
        int numTeraTypes,
        int[] specieIdToVocabIndex,
        int[] moveIdToActionIndex,
        Dictionary<SpecieId, int> specieIdToSwitchIndex,
        int[] moveIdToMoveVocabIndex,
        int[] abilityIdToVocabIndex,
        int[] itemIdToVocabIndex,
        int[] moveTypeToTeraVocabIndex)
    {
        _speciesNameToIndex = speciesNameToIndex;
        _actionKeyToIndex = actionKeyToIndex;
        NumSpecies = numSpecies;
        NumActions = numActions;
        NumMoves = numMoves;
        NumAbilities = numAbilities;
        NumItems = numItems;
        NumTeraTypes = numTeraTypes;
        _specieIdToVocabIndex = specieIdToVocabIndex;
        _moveIdToActionIndex = moveIdToActionIndex;
        _specieIdToSwitchIndex = specieIdToSwitchIndex;
        _moveIdToMoveVocabIndex = moveIdToMoveVocabIndex;
        _abilityIdToVocabIndex = abilityIdToVocabIndex;
        _itemIdToVocabIndex = itemIdToVocabIndex;
        _moveTypeToTeraVocabIndex = moveTypeToTeraVocabIndex;

        _indexToActionKey = new string[numActions];
        foreach ((string key, int idx) in actionKeyToIndex)
        {
            _indexToActionKey[idx] = key;
        }
    }

    public static Vocab Load(string vocabJsonPath, Library library)
    {
        string json = File.ReadAllText(vocabJsonPath);
        using JsonDocument doc = JsonDocument.Parse(json);
        JsonElement root = doc.RootElement;

        // Parse species map
        var speciesNameToIndex = new Dictionary<string, int>();
        foreach (JsonProperty prop in root.GetProperty("species").EnumerateObject())
        {
            speciesNameToIndex[prop.Name] = prop.Value.GetInt32();
        }

        // Parse action map
        var actionKeyToIndex = new Dictionary<string, int>();
        foreach (JsonProperty prop in root.GetProperty("actions").EnumerateObject())
        {
            actionKeyToIndex[prop.Name] = prop.Value.GetInt32();
        }

        // Parse move embedding map
        var moveNameToIndex = new Dictionary<string, int>();
        foreach (JsonProperty prop in root.GetProperty("moves").EnumerateObject())
        {
            moveNameToIndex[prop.Name] = prop.Value.GetInt32();
        }

        // Parse ability embedding map
        var abilityNameToIndex = new Dictionary<string, int>();
        foreach (JsonProperty prop in root.GetProperty("abilities").EnumerateObject())
        {
            abilityNameToIndex[prop.Name] = prop.Value.GetInt32();
        }

        // Parse item embedding map
        var itemNameToIndex = new Dictionary<string, int>();
        foreach (JsonProperty prop in root.GetProperty("items").EnumerateObject())
        {
            itemNameToIndex[prop.Name] = prop.Value.GetInt32();
        }

        // Parse tera type embedding map
        var teraNameToIndex = new Dictionary<string, int>();
        foreach (JsonProperty prop in root.GetProperty("tera_types").EnumerateObject())
        {
            teraNameToIndex[prop.Name] = prop.Value.GetInt32();
        }

        int numSpecies = root.GetProperty("num_species").GetInt32();
        int numActions = root.GetProperty("num_actions").GetInt32();
        int numMoves = root.GetProperty("num_moves").GetInt32();
        int numAbilities = root.GetProperty("num_abilities").GetInt32();
        int numItems = root.GetProperty("num_items").GetInt32();
        int numTeraTypes = root.GetProperty("num_tera_types").GetInt32();

        // Build SpecieId -> vocab index cache
        int specieIdCount = Enum.GetValues<SpecieId>().Length;
        var specieIdToVocabIndex = new int[specieIdCount];
        Array.Fill(specieIdToVocabIndex, UnknownSpeciesIndex);

        foreach ((SpecieId specieId, Species species) in library.Species)
        {
            var enumIndex = (int)specieId;
            if (enumIndex >= 0 && enumIndex < specieIdCount)
            {
                if (speciesNameToIndex.TryGetValue(species.Name, out int vocabIdx))
                {
                    specieIdToVocabIndex[enumIndex] = vocabIdx;
                }
            }
        }

        // Build MoveId -> action index cache (for policy output)
        int moveIdCount = Enum.GetValues<MoveId>().Length;
        var moveIdToActionIndex = new int[moveIdCount];
        Array.Fill(moveIdToActionIndex, NoneActionIndex);

        foreach ((MoveId moveId, Move move) in library.Moves)
        {
            var enumIndex = (int)moveId;
            if (enumIndex >= 0 && enumIndex < moveIdCount)
            {
                var actionKey = $"move:{move.Name}";
                if (actionKeyToIndex.TryGetValue(actionKey, out int actionIdx))
                {
                    moveIdToActionIndex[enumIndex] = actionIdx;
                }
            }
        }

        // Build SpecieId -> switch action index cache
        var specieIdToSwitchIndex = new Dictionary<SpecieId, int>();
        foreach ((SpecieId specieId, Species species) in library.Species)
        {
            var switchKey = $"switch:{species.Name}";
            if (actionKeyToIndex.TryGetValue(switchKey, out int switchIdx))
            {
                specieIdToSwitchIndex[specieId] = switchIdx;
            }
        }

        // Build MoveId -> move embedding index cache (for model input)
        var moveIdToMoveVocabIndex = new int[moveIdCount];
        // None -> pad (0), all others default to unknown (1)
        Array.Fill(moveIdToMoveVocabIndex, UnknownIndex);
        moveIdToMoveVocabIndex[(int)MoveId.None] = PadIndex;

        foreach ((MoveId moveId, Move move) in library.Moves)
        {
            var enumIndex = (int)moveId;
            if (enumIndex >= 0 && enumIndex < moveIdCount)
            {
                if (moveNameToIndex.TryGetValue(move.Name, out int vocabIdx))
                {
                    moveIdToMoveVocabIndex[enumIndex] = vocabIdx;
                }
            }
        }

        // Build AbilityId -> ability embedding index cache
        int abilityIdCount = Enum.GetValues<AbilityId>().Length;
        var abilityIdToVocabIndex = new int[abilityIdCount];
        Array.Fill(abilityIdToVocabIndex, UnknownIndex);
        abilityIdToVocabIndex[(int)AbilityId.None] = PadIndex;

        foreach ((AbilityId abilityId, Ability ability) in library.Abilities)
        {
            var enumIndex = (int)abilityId;
            if (enumIndex >= 0 && enumIndex < abilityIdCount)
            {
                if (abilityNameToIndex.TryGetValue(ability.Name, out int vocabIdx))
                {
                    abilityIdToVocabIndex[enumIndex] = vocabIdx;
                }
            }
        }

        // Build ItemId -> item embedding index cache
        int itemIdCount = Enum.GetValues<ItemId>().Length;
        var itemIdToVocabIndex = new int[itemIdCount];
        Array.Fill(itemIdToVocabIndex, UnknownIndex);
        itemIdToVocabIndex[(int)ItemId.None] = PadIndex;

        foreach ((ItemId itemId, Item item) in library.Items)
        {
            var enumIndex = (int)itemId;
            if (enumIndex >= 0 && enumIndex < itemIdCount)
            {
                if (itemNameToIndex.TryGetValue(item.Name, out int vocabIdx))
                {
                    itemIdToVocabIndex[enumIndex] = vocabIdx;
                }
            }
        }

        // Build MoveType -> tera type embedding index cache
        int moveTypeCount = Enum.GetValues<MoveType>().Length;
        var moveTypeToTeraVocabIndex = new int[moveTypeCount];
        // Unknown MoveType -> pad (0)
        foreach (MoveType mt in Enum.GetValues<MoveType>())
        {
            var enumIndex = (int)mt;
            if (enumIndex >= 0 && enumIndex < moveTypeCount)
            {
                var name = mt.ToString();
                if (teraNameToIndex.TryGetValue(name, out int vocabIdx))
                    moveTypeToTeraVocabIndex[enumIndex] = vocabIdx;
            }
        }

        return new Vocab(
            speciesNameToIndex,
            actionKeyToIndex,
            numSpecies,
            numActions,
            numMoves,
            numAbilities,
            numItems,
            numTeraTypes,
            specieIdToVocabIndex,
            moveIdToActionIndex,
            specieIdToSwitchIndex,
            moveIdToMoveVocabIndex,
            abilityIdToVocabIndex,
            itemIdToVocabIndex,
            moveTypeToTeraVocabIndex);
    }

    public int GetSpeciesIndex(SpecieId id)
    {
        var enumIndex = (int)id;
        if (enumIndex >= 0 && enumIndex < _specieIdToVocabIndex.Length)
            return _specieIdToVocabIndex[enumIndex];
        return UnknownSpeciesIndex;
    }

    public int GetMoveActionIndex(MoveId moveId)
    {
        var enumIndex = (int)moveId;
        if (enumIndex >= 0 && enumIndex < _moveIdToActionIndex.Length)
            return _moveIdToActionIndex[enumIndex];
        return NoneActionIndex;
    }

    public int GetSwitchActionIndex(SpecieId specieId)
    {
        return _specieIdToSwitchIndex.GetValueOrDefault(specieId, NoneActionIndex);
    }

    public int GetMoveEmbedIndex(MoveId moveId)
    {
        var enumIndex = (int)moveId;
        if (enumIndex >= 0 && enumIndex < _moveIdToMoveVocabIndex.Length)
            return _moveIdToMoveVocabIndex[enumIndex];
        return UnknownIndex;
    }

    public int GetAbilityIndex(AbilityId abilityId)
    {
        var enumIndex = (int)abilityId;
        if (enumIndex >= 0 && enumIndex < _abilityIdToVocabIndex.Length)
            return _abilityIdToVocabIndex[enumIndex];
        return UnknownIndex;
    }

    public int GetItemIndex(ItemId itemId)
    {
        var enumIndex = (int)itemId;
        if (enumIndex >= 0 && enumIndex < _itemIdToVocabIndex.Length)
            return _itemIdToVocabIndex[enumIndex];
        return UnknownIndex;
    }

    public int GetTeraTypeIndex(MoveType teraType)
    {
        var enumIndex = (int)teraType;
        if (enumIndex >= 0 && enumIndex < _moveTypeToTeraVocabIndex.Length)
            return _moveTypeToTeraVocabIndex[enumIndex];
        return PadIndex;
    }

    public string GetActionKey(int index)
    {
        if (index >= 0 && index < _indexToActionKey.Length)
            return _indexToActionKey[index];
        return $"<out-of-range:{index}>";
    }

    public bool TryGetActionIndex(string key, out int index)
    {
        return _actionKeyToIndex.TryGetValue(key, out index);
    }
}