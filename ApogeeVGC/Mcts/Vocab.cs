using System.Text.Json;
using ApogeeVGC.Data;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.SpeciesClasses;

namespace ApogeeVGC.Mcts;

public sealed class Vocab
{
    public const int PadIndex = 0;
    public const int UnknownSpeciesIndex = 1;
    public const int NoneActionIndex = 1;  // "<none>"
    public const int CantActionIndex = 2;  // "<cant>"

    public int NumSpecies { get; }
    public int NumActions { get; }

    private readonly Dictionary<string, int> _speciesNameToIndex;
    private readonly Dictionary<string, int> _actionKeyToIndex;
    private readonly string[] _indexToActionKey;

    // C# enum -> vocab index caches
    private readonly int[] _specieIdToVocabIndex;
    private readonly int[] _moveIdToActionIndex;
    private readonly Dictionary<SpecieId, int> _specieIdToSwitchIndex;

    private Vocab(
        Dictionary<string, int> speciesNameToIndex,
        Dictionary<string, int> actionKeyToIndex,
        int numSpecies,
        int numActions,
        int[] specieIdToVocabIndex,
        int[] moveIdToActionIndex,
        Dictionary<SpecieId, int> specieIdToSwitchIndex)
    {
        _speciesNameToIndex = speciesNameToIndex;
        _actionKeyToIndex = actionKeyToIndex;
        NumSpecies = numSpecies;
        NumActions = numActions;
        _specieIdToVocabIndex = specieIdToVocabIndex;
        _moveIdToActionIndex = moveIdToActionIndex;
        _specieIdToSwitchIndex = specieIdToSwitchIndex;

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

        int numSpecies = root.GetProperty("num_species").GetInt32();
        int numActions = root.GetProperty("num_actions").GetInt32();

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

        // Build MoveId -> action index cache
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

        return new Vocab(
            speciesNameToIndex,
            actionKeyToIndex,
            numSpecies,
            numActions,
            specieIdToVocabIndex,
            moveIdToActionIndex,
            specieIdToSwitchIndex);
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
