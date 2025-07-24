using System;
using System.Collections.Generic;

namespace ApogeeVGC_CS.sim
{
    // Enums for choice types
    public enum ChoiceType
    {
        Move, Switch, InstaSwitch, RevivalBlessing, Team, Shift, Pass
    }

    // ChosenAction class
    public class ChosenAction
    {
        public ChoiceType Choice { get; set; }
        public Pokemon? Pokemon { get; set; }
        public int? TargetLoc { get; set; }
        public string MoveId { get; set; } = string.Empty;
        public ActiveMove? Move { get; set; }
        public Pokemon? Target { get; set; }
        public int? Index { get; set; }
        public Side? Side { get; set; }
        public bool? Mega { get; set; }
        public bool? MegaX { get; set; }
        public bool? MegaY { get; set; }
        public string? ZMove { get; set; }
        public string? MaxMove { get; set; }
        public string? Terastallize { get; set; }
        public int? Priority { get; set; }
    }

    // Choice class
    public class Choice
    {
        public bool CantUndo { get; set; }
        public string Error { get; set; } = string.Empty;
        public List<ChosenAction> Actions { get; set; } = new();
        public int ForcedSwitchesLeft { get; set; }
        public int ForcedPassesLeft { get; set; }
        public HashSet<int> SwitchIns { get; set; } = new();
        public bool ZMove { get; set; }
        public bool Mega { get; set; }
        public bool Ultra { get; set; }
        public bool Dynamax { get; set; }
        public bool Terastallize { get; set; }
    }

    // Request data classes
    public class PokemonSwitchRequestData
    {
        public string Ident { get; set; } = string.Empty;
        public string Details { get; set; } = string.Empty;
        public string Condition { get; set; } = string.Empty;
        public bool Active { get; set; }
        public StatsTable Stats { get; set; } = new();
        public List<string> Moves { get; set; } = new();
        public string BaseAbility { get; set; } = string.Empty;
        public string Item { get; set; } = string.Empty;
        public string Pokeball { get; set; } = string.Empty;
        public string? Ability { get; set; }
        public bool? Commanding { get; set; }
        public bool? Reviving { get; set; }
        public string? TeraType { get; set; }
        public string? Terastallized { get; set; }
    }

    public class PokemonMoveData
    {
        public string Move { get; set; } = string.Empty;
        public string Id { get; set; } = string.Empty;
        public string? Target { get; set; }
        public object? Disabled { get; set; } // string or bool
        public string? DisabledSource { get; set; }
    }

    public class PokemonMoveRequestData
    {
        public List<PokemonMoveData> Moves { get; set; } = new();
        public bool? MaybeDisabled { get; set; }
        public bool? MaybeLocked { get; set; }
        public bool? Trapped { get; set; }
        public bool? MaybeTrapped { get; set; }
        public bool? CanMegaEvo { get; set; }
        public bool? CanMegaEvoX { get; set; }
        public bool? CanMegaEvoY { get; set; }
        public bool? CanUltraBurst { get; set; }
        public object? CanZMove { get; set; }
        public bool? CanDynamax { get; set; }
        public DynamaxOptions? MaxMoves { get; set; }
        public string? CanTerastallize { get; set; }
    }

    public class DynamaxMoveData
    {
        public string Move { get; set; } = string.Empty;
        public MoveTarget Target { get; set; }
        public bool? Disabled { get; set; }
    }

    public class DynamaxOptions
    {
        public List<DynamaxMoveData> MaxMoves { get; set; } = new();
        public string? Gigantamax { get; set; }
    }

    public class SideRequestData
    {
        public string Name { get; set; } = string.Empty;
        public SideID Id { get; set; }
        public List<PokemonSwitchRequestData> Pokemon { get; set; } = new();
        public bool? NoCancel { get; set; }
    }

    // Request types
    public abstract class ChoiceRequest
    {
        public bool? Wait { get; set; }
        public SideRequestData Side { get; set; } = new();
        public bool? NoCancel { get; set; }
    }

    public class SwitchRequest : ChoiceRequest
    {
        public List<bool> ForceSwitch { get; set; } = new();
    }

    public class TeamPreviewRequest : ChoiceRequest
    {
        public bool TeamPreview { get; set; } = true;
        public int? MaxChosenTeamSize { get; set; }
    }

    public class MoveRequest : ChoiceRequest
    {
        public List<PokemonMoveRequestData> Active { get; set; } = new();
        public SideRequestData? Ally { get; set; }
    }

    public class WaitRequest : ChoiceRequest
    {
        public new bool Wait { get; set; } = true;
    }

    // Side class
    public class Side
    {
        public Battle Battle { get; }
        public SideID Id { get; }
        public int N { get; }

        public string Name { get; set; } = string.Empty;
        public string Avatar { get; set; } = string.Empty;
        public Side Foe { get; set; } = null!; // set in battle.start()
        public Side? AllySide { get; set; } = null; // set in battle.start()
        public List<PokemonSet> Team { get; set; } = new();
        public List<Pokemon> Pokemon { get; set; } = new();
        public List<Pokemon?> Active { get; set; } = new();

        public int PokemonLeft { get; set; }
        public bool ZMoveUsed { get; set; }
        public bool DynamaxUsed { get; set; }

        public Pokemon? FaintedLastTurn { get; set; }
        public Pokemon? FaintedThisTurn { get; set; }
        public int TotalFainted { get; set; }
        public string LastSelectedMove { get; set; } = string.Empty; // Gen 1 only

        public Dictionary<string, EffectState> SideConditions { get; set; } = new();
        public List<Dictionary<string, EffectState>> SlotConditions { get; set; } = new();

        public ChoiceRequest? ActiveRequest { get; set; }
        public Choice Choice { get; set; } = new();

        public Move? LastMove { get; set; } // Gen 1 tracking

        public Side(string name, Battle battle, int sideNum, List<PokemonSet> team)
        {
            // Copy side scripts from battle if needed

            Battle = battle;
            Id = (SideID)sideNum;
            N = sideNum;

            Name = name;
            Avatar = string.Empty;

            Team = team;
            Pokemon = new List<Pokemon>();

            foreach (var set in Team)
            {
                AddPokemon(set);
            }

            // Initialize active slots based on game type
            switch (Battle.GameType)
            {
                case "doubles":
                    Active = new List<Pokemon?> { null, null };
                    break;
                case "triples":
                case "rotation":
                    Active = new List<Pokemon?> { null, null, null };
                    break;
                default:
                    Active = new List<Pokemon?> { null };
                    break;
            }

            PokemonLeft = Pokemon.Count;
            FaintedLastTurn = null;
            FaintedThisTurn = null;
            TotalFainted = 0;
            ZMoveUsed = false;
            DynamaxUsed = Battle.Gen != 8;

            SideConditions = new Dictionary<string, EffectState>();
            SlotConditions = new List<Dictionary<string, EffectState>>();

            // Initialize slot conditions for each active slot
            for (int i = 0; i < Active.Count; i++)
            {
                SlotConditions.Add(new Dictionary<string, EffectState>());
            }

            ActiveRequest = null;
            Choice = new Choice
            {
                CantUndo = false,
                Error = string.Empty,
                Actions = new List<ChosenAction>(),
                ForcedSwitchesLeft = 0,
                ForcedPassesLeft = 0,
                SwitchIns = new HashSet<int>(),
                ZMove = false,
                Mega = false,
                Ultra = false,
                Dynamax = false,
                Terastallize = false
            };

            LastMove = null;
        }

        private void AddPokemon(PokemonSet set)
        {
            // TODO: Implement AddPokemon logic
            var pokemon = new Pokemon(set, this);
            Pokemon.Add(pokemon);
        }
    }
}