using ApogeeVGC.Sim.Core;
using ApogeeVGC.Sim.GameObjects;
using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.Choices;

public abstract record TeamPreviewChoice : BattleChoice
{
    private TeamPreviewChoice()
    {
    }

    /// <summary>
    /// Gets the trainer associated with this choice.
    /// </summary>
    public abstract override Trainer Trainer { get; }
    public abstract override SideId SideId { get; }
    public abstract SlotId SlotId { get; }
    public abstract IReadOnlyList<Pokemon> Pokemon { get; }


    public sealed record SinglesTeamPreviewChoice : TeamPreviewChoice
    {
        public override IReadOnlyList<Pokemon> Pokemon { get; }
        public override Trainer Trainer => Pokemon[0].Trainer;
        public override SideId SideId => Pokemon[0].SideId;
        public override SlotId SlotId => SlotId.Slot1;
        internal SinglesTeamPreviewChoice(IReadOnlyList<Pokemon> pokemon)
        {
            // length must be 6
            if (pokemon.Count != 6)
            {
                throw new ArgumentException("Team preview must consist of exactly 6 Pokémon.", nameof(pokemon));
            }
            // all pokemon must belong to the same trainer and side
            Trainer trainer = pokemon[0].Trainer;
            SideId sideId = pokemon[0].SideId;
            if (pokemon.Any(p => p.Trainer != trainer))
            {
                throw new ArgumentException("All Pokémon in team preview must belong to the same trainer.",
                    nameof(pokemon));
            }
            if (pokemon.Any(p => p.SideId != sideId))
            {
                throw new ArgumentException("All Pokémon in team preview must belong to the same side.",
                    nameof(pokemon));
            }
            // no pokemon can be fainted
            if (pokemon.Any(p => p.IsFainted))
            {
                throw new ArgumentException("No Pokémon in team preview can be fainted.", nameof(pokemon));
            }
            // there must be exactly one pokemon in each of the 6 slots
            if (pokemon.Select(p => p.SlotId).Distinct().Count() != 6)
            {
                throw new ArgumentException("There must be exactly one Pokémon in each of the 6 slots.",
                    nameof(pokemon));
            }
            Pokemon = pokemon;
        }
    }

    public sealed record DoublesTeamPreviewChoice : TeamPreviewChoice
    {
        public override IReadOnlyList<Pokemon> Pokemon { get; }
        public override Trainer Trainer => Pokemon[0].Trainer;
        public override SideId SideId => Pokemon[0].SideId;
        public override SlotId SlotId => SlotId.Slot1;
        internal DoublesTeamPreviewChoice(IReadOnlyList<Pokemon> pokemon)
        {
            // length must be 6
            if (pokemon.Count != 4)
            {
                throw new ArgumentException("Team preview must consist of exactly 4 Pokémon.", nameof(pokemon));
            }
            // all pokemon must belong to the same trainer and side
            Trainer trainer = pokemon[0].Trainer;
            SideId sideId = pokemon[0].SideId;
            if (pokemon.Any(p => p.Trainer != trainer))
            {
                throw new ArgumentException("All Pokémon in team preview must belong to the same trainer.",
                    nameof(pokemon));
            }
            if (pokemon.Any(p => p.SideId != sideId))
            {
                throw new ArgumentException("All Pokémon in team preview must belong to the same side.",
                    nameof(pokemon));
            }
            // no pokemon can be fainted
            if (pokemon.Any(p => p.IsFainted))
            {
                throw new ArgumentException("No Pokémon in team preview can be fainted.", nameof(pokemon));
            }
            // there must be exactly one pokemon in each of the 4 slots
            if (pokemon.Select(p => p.SlotId).Distinct().Count() != 4)
            {
                throw new ArgumentException("There must be exactly one Pokémon in each of the 4 slots.",
                    nameof(pokemon));
            }
            // slot 5 and 6 cannot be used
            if (pokemon.Any(p => p.SlotId is SlotId.Slot5 or SlotId.Slot6))
            {
                throw new ArgumentException("Slot 5 and 6 cannot be used in doubles team preview.",
                    nameof(pokemon));
            }
            Pokemon = pokemon;
        }
    }

    public static SinglesTeamPreviewChoice CreateSinglesTeamPreview(IReadOnlyList<Pokemon> pokemon)
    {
        // length must be 6
        if (pokemon.Count != 6)
        {
            throw new ArgumentException("Team preview must consist of exactly 6 Pokémon.", nameof(pokemon));
        }
        // all pokemon must belong to the same trainer and side
        Trainer trainer = pokemon[0].Trainer;
        SideId sideId = pokemon[0].SideId;
        if (pokemon.Any(p => p.Trainer != trainer))
        {
            throw new ArgumentException("All Pokémon in team preview must belong to the same trainer.",
                nameof(pokemon));
        }
        if (pokemon.Any(p => p.SideId != sideId))
        {
            throw new ArgumentException("All Pokémon in team preview must belong to the same side.",
                nameof(pokemon));
        }
        // no pokemon can be fainted
        if (pokemon.Any(p => p.IsFainted))
        {
            throw new ArgumentException("No Pokémon in team preview can be fainted.", nameof(pokemon));
        }
        // there must be exactly one pokemon in each of the 6 slots
        if (pokemon.Select(p => p.SlotId).Distinct().Count() != 6)
        {
            throw new ArgumentException("There must be exactly one Pokémon in each of the 6 slots.",
                nameof(pokemon));
        }
        return new SinglesTeamPreviewChoice(pokemon);
    }

    public static DoublesTeamPreviewChoice CreateDoublesTeamPreview(IReadOnlyList<Pokemon> pokemon)
    {
        // length must be 4
        if (pokemon.Count != 4)
        {
            throw new ArgumentException("Team preview must consist of exactly 4 Pokémon.", nameof(pokemon));
        }

        // all pokemon must belong to the same trainer and side
        Trainer trainer = pokemon[0].Trainer;
        SideId sideId = pokemon[0].SideId;
        if (pokemon.Any(p => p.Trainer != trainer))
        {
            throw new ArgumentException("All Pokémon in team preview must belong to the same trainer.",
                nameof(pokemon));
        }

        if (pokemon.Any(p => p.SideId != sideId))
        {
            throw new ArgumentException("All Pokémon in team preview must belong to the same side.",
                nameof(pokemon));
        }

        // no pokemon can be fainted
        if (pokemon.Any(p => p.IsFainted))
        {
            throw new ArgumentException("No Pokémon in team preview can be fainted.", nameof(pokemon));
        }

        // there must be exactly one pokemon in each of the 4 slots
        if (pokemon.Select(p => p.SlotId).Distinct().Count() != 4)
        {
            throw new ArgumentException("There must be exactly one Pokémon in each of the 4 slots.",
                nameof(pokemon));
        }

        // slot 5 and 6 cannot be used
        if (pokemon.Any(p => p.SlotId is SlotId.Slot5 or SlotId.Slot6))
        {
            throw new ArgumentException("Slot 5 and 6 cannot be used in doubles team preview.",
                nameof(pokemon));
        }

        return new DoublesTeamPreviewChoice(pokemon);
    }

    public bool IsSinglesTeamPreviewChoice => this is SinglesTeamPreviewChoice;
    public bool IsDoublesTeamPreviewChoice => this is DoublesTeamPreviewChoice;
}