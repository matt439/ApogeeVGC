using ApogeeVGC.Sim.Abilities;
using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Conditions;
using ApogeeVGC.Sim.FieldClasses;
using ApogeeVGC.Sim.Items;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.SideClasses;
using ApogeeVGC.Sim.SpeciesClasses;
using ApogeeVGC.Sim.Utils.Extensions;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Utils;

public static partial class State
{
    public static bool IsReferable(object obj)
    {
        return obj switch
        {
            IBattle => true,
            Field => true,
            Side => true,
            Pokemon => true,
            Condition => true,
            Ability => true,
            Item => true,
            Moves.ActiveMove => true,  // Check ActiveMove before Move since it inherits from Move
            Move => true,
            Species => true,
            _ => false,
        };
    }

    public static string ToRef(Referable obj)
    {
        return obj switch
        {
            // Pokemon's 'id' is not only more verbose than a position, it also isn't guaranteed
            // to be uniquely identifying in custom games without Nickname/Species Clause.
            PokemonReferable p =>
                $"[Pokemon:{p.Pokemon.Side.Id.GetSideIdName()}{Positions[p.Pokemon.Position]}]",

            SideReferable s => $"[Side:{s.Side.Id.GetSideIdName()}]",

            // Battle and Field don't need IDs as there's only one instance of each
            BattleReferable => "[Battle]",
            FieldReferable => "[Field]",

            // For immutable data types (Dex types), use their ID
            ConditionReferable c => $"[Condition:{c.Condition.Id}]",
            AbilityReferable a => $"[Ability:{a.Ability.Id}]",
            ItemReferable i => $"[Item:{i.Item.Id}]",
            MoveReferable m => $"[Move:{m.Move.Id}]",
            SpeciesReferable s => $"[Species:{s.Species.Id}]",

            _ => throw new ArgumentException($"Unknown referable type: {obj.GetType()}", nameof(obj))
        };
    }

    public static ReferableUndefinedUnion FromRef(string reference, IBattle battle)
    {
        // References are sort of fragile - we're mostly just counting on there
        // being a low chance that some string field in a simulator object will not
        // 'look' like one. However, it also needs to match one of the Referable
        // class types to be decoded, so we're probably OK. We could make the reference
        // markers more esoteric with additional sigils etc to avoid collisions, but
        // we're making a conscious decision to favor readability over robustness.
        if (!reference.StartsWith('[') || !reference.EndsWith(']'))
        {
            return ReferableUndefinedUnion.FromUndefined();
        }

        // Remove the brackets
        string content = reference.Substring(1, reference.Length - 2);

        // There's only one instance of these thus they don't need an id to differentiate.
        if (content == "Battle")
        {
            return Referable.FromIBattle(battle);
        }
        if (content == "Field")
        {
            return (Referable)battle.Field;
        }

        // Split on the first colon to get type and id
        int colonIndex = content.IndexOf(':');
        if (colonIndex == -1)
        {
            // No colon means it's invalid (except for Battle/Field which we handled above)
            return ReferableUndefinedUnion.FromUndefined();
        }

        string type = content[..colonIndex];
        string id = content[(colonIndex + 1)..];

        return type switch
        {
            "Side" => ParseSideRef(id, battle),
            "Pokemon" => ParsePokemonRef(id, battle),
            "Ability" => (Referable)battle.Library.Abilities[Enum.Parse<AbilityId>(id)],
            "Item" => (Referable)battle.Library.Items[Enum.Parse<ItemId>(id)],
            "Move" => ParseMoveRef(id, battle),
            "Condition" => (Referable)battle.Library.Conditions[Enum.Parse<ConditionId>(id)],
            "Species" => (Referable)battle.Library.Species[Enum.Parse<SpecieId>(id)],
            _ => ReferableUndefinedUnion.FromUndefined() // Unknown type, might just be a regular string
        };
    }

    private static Referable ParseSideRef(string id, IBattle battle)
    {
        // Side reference format: "p1" or "p2"
        // The side ID has a format like "p1" where the number indicates the side index

        // Parse "p1" -> index 0, "p2" -> index 1
        if (id.Length < 2 || id[0] != 'p')
        {
            throw new ArgumentException($"Invalid side reference format: {id}", nameof(id));
        }

        int sideIndex = int.Parse(id[1..]) - 1;

        if (sideIndex < 0 || sideIndex >= battle.Sides.Count)
        {
            throw new ArgumentOutOfRangeException(nameof(id),
                $"Side index {sideIndex} is out of range (0-{battle.Sides.Count - 1})");
        }

        return battle.Sides[sideIndex];
    }

    private static Referable ParsePokemonRef(string id, IBattle battle)
    {
        // Pokemon reference format: "p1a" where "p1" is side and "a" is position
        // Format: {sideId}{positionLetter}

        if (id.Length < 3)
        {
            throw new ArgumentException($"Invalid pokemon reference format: {id}", nameof(id));
        }

        // Extract side ID (e.g., "p1")
        string sideId = id[..2];

        // Extract position letter (e.g., "a")
        char positionLetter = id[2];

        // Parse the side
        int sideIndex = int.Parse(sideId[1..]) - 1;

        if (sideIndex < 0 || sideIndex >= battle.Sides.Count)
        {
            throw new ArgumentOutOfRangeException(nameof(id),
                $"Side index {sideIndex} is out of range (0-{battle.Sides.Count - 1})");
        }

        Side side = battle.Sides[sideIndex];

        // Find position index from letter
        int position = Positions.IndexOf(positionLetter);

        if (position == -1)
        {
            throw new ArgumentException($"Invalid position letter: {positionLetter}", nameof(id));
        }

        if (position < 0 || position >= side.Pokemon.Count)
        {
            throw new ArgumentOutOfRangeException(nameof(id),
                $"Position {position} is out of range (0-{side.Pokemon.Count - 1})");
        }

        return side.Pokemon[position];
    }

    private static Referable ParseMoveRef(string id, IBattle battle)
    {
        // Move references store only the base Move, not ActiveMove
        // ActiveMove is handled specially in IsActiveMove/SerializeActiveMove/DeserializeActiveMove
        Move move = battle.Library.Moves[Enum.Parse<MoveId>(id)];
        return (Referable)move;
    }
}