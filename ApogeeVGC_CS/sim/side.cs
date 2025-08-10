namespace ApogeeVGC_CS.sim
{
    public enum ChoiceType
    {
        Move, Switch, InstaSwitch, RevivalBlessing, Team, Shift, Pass
    }

    public class ChosenAction
    {
        public required ChoiceType Choice { get; init; }
        public Pokemon? Pokemon { get; init; }
        public int? TargetLoc { get; init; }
        public required string MoveId { get; init; }
        public ActiveMove? Move { get; init; }
        public Pokemon? Target { get; init; }
        public int? Index { get; init; }
        public Side? Side { get; init; }
        public bool? Mega { get; init; }
        public bool? MegaX { get; init; }
        public bool? MegaY { get; init; }
        public string? ZMove { get; init; }
        public string? MaxMove { get; init; }
        public string? Terastallize { get; init; }
        public int? Priority { get; init; }
    }

    public class Choice
    {
        public required bool CantUndo { get; init; }
        public required string Error { get; init; }
        public required List<ChosenAction> Actions { get; init; }
        public required int ForcedSwitchesLeft { get; init; }
        public required int ForcedPassesLeft { get; init; }
        public required HashSet<int> SwitchIns { get; init; }
        public required bool ZMove { get; init; }
        public required bool Mega { get; init; }
        public required bool Ultra { get; init; }
        public required bool Dynamax { get; init; }
        public required bool Terastallize { get; init; }
    }

    public class PokemonSwitchRequestData
    {
        public required string Ident { get; init; }
        public required string Details { get; init; }
        public required string Condition { get; init; }
        public required bool Active { get; init; }
        public required StatsTable Stats { get; init; }
        public required List<Id> Moves { get; init; }
        public required Id BaseAbility { get; init; }
        public required Id Item { get; init; }
        public required Id Pokeball { get; init; }
        public Id? Ability { get; init; }
        public bool? Commanding { get; init; }
        public bool? Reviving { get; init; }
        public string? TeraType { get; init; }
        public string? Terastallized { get; init; }
    }

    public class PokemonMoveData
    {
        public required string Move { get; init; }
        public required Id Id { get; init; }
        public string? Target { get; init; }

        public BoolStringUnion? Disabled { get; init; }
        public string? DisabledSource { get; init; }
    }

    public class PokemonMoveRequestData
    {
        public required List<PokemonMoveData> Moves { get; init; }
        public bool? MaybeDisabled { get; init; }
        public bool? MaybeLocked { get; init; }
        public bool? Trapped { get; init; }
        public bool? MaybeTrapped { get; init; }
        public bool? CanMegaEvo { get; init; }
        public bool? CanMegaEvoX { get; init; }
        public bool? CanMegaEvoY { get; init; }
        public bool? CanUltraBurst { get; init; }
        public IAnyObject? CanZMove { get; init; }
        public bool? CanDynamax { get; init; }
        public DynamaxOptions? MaxMoves { get; init; }
        public string? CanTerastallize { get; init; }
    }

    public class DynamaxMoveData
    {
        public required string Move { get; init; }
        public required MoveTarget Target { get; init; }
        public bool? Disabled { get; init; }
    }

    public class DynamaxOptions
    {
        public required List<DynamaxMoveData> MaxMoves { get; init; }
        public string? Gigantamax { get; init; }
    }

    public class SideRequestData
    {
        public required string Name { get; init; }
        public required SideId Id { get; init; }
        public required List<PokemonSwitchRequestData> Pokemon { get; init; }
        public bool? NoCancel { get; init; }
    }

    public abstract class ChoiceRequest
    {
        public virtual bool? Wait { get; init; }
        public required SideRequestData Side { get; init; }
        public bool? NoCancel { get; init; }
        public virtual bool? TeamPreview { get; init; }
    }

    public class SwitchRequest : ChoiceRequest
    {
        public required List<bool> ForceSwitch { get; init; }
    }

    public class TeamPreviewRequest : ChoiceRequest
    {
        public override bool? TeamPreview
        {
            get => true;
            init { }
        }
        public int? MaxChosenTeamSize { get; init; }
        public List<bool>? ForceSwitch { get; init; }
    }

    public class MoveRequest : ChoiceRequest
    {
        public required List<PokemonMoveRequestData> Active { get; init; }
        public SideRequestData? Ally { get; init; }
        public List<bool>? ForceSwitch { get; init; }
    }

    public class WaitRequest : ChoiceRequest
    {
        public override bool? Wait
        {
            get => true;
            init { }
        }
        public List<bool>? ForceSwitch { get; init; }
    }

    // event: 'mega' | 'megax' | 'megay' | 'zmove' | 'ultra' | 'dynamax' | 'terastallize' | ''
    public enum EventType
    {
        Mega,
        MegaX,
        MegaY,
        ZMove,
        Ultra,
        Dynamax,
        Terastallize,
        None,
    }

    public class Side
    {
        public Battle Battle { get; }
        public SideId Id { get; }
        public int N { get; }

        public required string Name { get; init; }
        public required string Avatar { get; init; }
        public Side Foe { get; init; } = null!; // set in battle.start()
        public Side? AllySide { get; init; } = null; // set in battle.start()
        public required List<PokemonSet> Team { get; init; }
        public required List<Pokemon> Pokemon { get; init; }
        public required List<Pokemon> Active { get; init; }

        public int PokemonLeft => Pokemon.Count;
        public required bool ZMoveUsed { get; init; }
        public required bool DynamaxUsed { get; init; }

        public Pokemon? FaintedLastTurn { get; init; }
        public Pokemon? FaintedThisTurn { get; init; }
        public required int TotalFainted { get; init; }
        public Id LastSelectedMove { get; init; } = new(); // Gen 1 tracking

        public required Dictionary<string, EffectState> SideConditions { get; init; }
        public required List<Dictionary<string, EffectState>> SlotConditions { get; init; }

        public ChoiceRequest? ActiveRequest { get; init; }
        public required Choice Choice { get; init; }

        public Move? LastMove { get; init; } // Gen 1 tracking

        public Side(string name, Battle battle, int sideNum, List<PokemonSet> team)
        {
            // Copy side scripts from battle if needed

            Battle = battle;
            Id = (SideId)sideNum;
            N = sideNum;

            Name = name;
            Avatar = string.Empty;

            Team = team;
            Pokemon = [];

            foreach (var set in Team)
            {
                AddPokemon(set);
            }

            // Initialize active slots based on game type
            Active = Battle.GameType switch
            {
                GameType.Doubles => [null!, null!],
                GameType.Triples or GameType.Rotation => [null!, null!, null!],
                _ => [null!]
            };

            //PokemonLeft = Pokemon.Count;
            FaintedLastTurn = null;
            FaintedThisTurn = null;
            TotalFainted = 0;
            ZMoveUsed = false;
            DynamaxUsed = Battle.Gen != 8;

            SideConditions = [];
            SlotConditions = [];

            // Initialize slot conditions for each active slot
            for (var i = 0; i < Active.Count; i++)
            {
                SlotConditions.Add(new Dictionary<string, EffectState>());
            }

            ActiveRequest = null;
            Choice = new Choice
            {
                CantUndo = false,
                Error = string.Empty,
                Actions = [],
                ForcedSwitchesLeft = 0,
                ForcedPassesLeft = 0,
                SwitchIns = [],
                ZMove = false,
                Mega = false,
                Ultra = false,
                Dynamax = false,
                Terastallize = false
            };

            LastMove = null;
        }

        public object ToJson()
        {
            throw new NotImplementedException("ToJson method is not implemented yet.");
        }

        public RequestState GetRequestState()
        {
            throw new NotImplementedException();
        }

        public Pokemon? AddPokemon(PokemonSet set)
        {
            throw new NotImplementedException();
        }

        public bool CanDynamaxNow()
        {
            throw new NotImplementedException("CanDynamaxNow method is not implemented yet.");
        }

        public string GetChoice()
        {
            throw new NotImplementedException("GetChoice method is not implemented yet.");
        }

        public override string ToString()
        {
            return $"{Id.ToString()}: {Name}";
        }

        public SideRequestData GetRequestData(bool? forAlly = null)
        {
            throw new NotImplementedException("GetRequestData method is not implemented yet.");
        }

        public Pokemon? RandomFoe()
        {
            throw new NotImplementedException("RandomFoe method is not implemented yet.");
        }

        public List<Side> FoeSideWithConditions()
        {
            throw new NotImplementedException("FoeSideWithConditions method is not implemented yet.");
        }

        public int FoePokemonLeft()
        {
            throw new NotImplementedException("FoePokemonLeft method is not implemented yet.");
        }

        public bool Allies(bool? all = null)
        {
            throw new NotImplementedException("Allies method is not implemented yet.");
        }

        public bool Foes(bool? all = null)
        {
            throw new NotImplementedException("Allies method is not implemented yet.");
        }

        public List<Pokemon> ActiveTeam()
        {
            throw new NotImplementedException("ActiveTeam method is not implemented yet.");
        }

        public bool HasAlly(Pokemon pokemon)
        {
            throw new NotImplementedException("HasAlly method is not implemented yet.");
        }

        public bool AddSideCondition(string status, Pokemon? source = null, IEffect? sourceEffect = null)
        {
            throw new NotImplementedException("AddSideCondition method is not implemented yet.");
        }

        public bool AddSideCondition(Condition status, Pokemon? source = null, IEffect? sourceEffect = null)
        {
            throw new NotImplementedException("AddSideCondition method is not implemented yet.");
        }

        public bool AddSideCondition(string status, bool debug, IEffect? sourceEffect = null)
        {
            throw new NotImplementedException("AddSideCondition method is not implemented yet.");
        }

        public bool AddSideCondition(Condition status, bool debug, IEffect? sourceEffect = null)
        {
            throw new NotImplementedException("AddSideCondition method is not implemented yet.");
        }

        public IEffect? GetSideCondition(string status)
        {
            throw new NotImplementedException("GetSideCondition method is not implemented yet.");
        }

        public IEffect? GetSideCondition(IEffect status)
        {
            throw new NotImplementedException("GetSideCondition method is not implemented yet.");
        }

        public bool RemoveSideCondition(string status)
        {
            throw new NotImplementedException("RemoveSideCondition method is not implemented yet.");
        }

        public bool RemoveSideCondition(IEffect status)
        {
            throw new NotImplementedException("RemoveSideCondition method is not implemented yet.");
        }

        public bool AddSlotCondition(Pokemon target, string status,
            Pokemon? source = null, IEffect? sourceEffect = null)
        {
            throw new NotImplementedException("AddSlotCondition method is not implemented yet.");
        }

        public bool AddSlotCondition(Pokemon target, Condition status,
            Pokemon? source = null, IEffect? sourceEffect = null)
        {
            throw new NotImplementedException("AddSlotCondition method is not implemented yet.");
        }

        public bool AddSlotCondition(int target, string status,
            Pokemon? source = null, IEffect? sourceEffect = null)
        {
            throw new NotImplementedException("AddSlotCondition method is not implemented yet.");
        }

        public bool AddSlotCondition(int target, Condition status,
            Pokemon? source = null, IEffect? sourceEffect = null)
        {
            throw new NotImplementedException("AddSlotCondition method is not implemented yet.");
        }

        public IEffect? GetSlotCondition(Pokemon target, string status)
        {
            throw new NotImplementedException("GetSlotCondition method is not implemented yet.");
        }

        public IEffect? GetSlotCondition(Pokemon target, IEffect status)
        {
            throw new NotImplementedException("GetSlotCondition method is not implemented yet.");
        }

        public IEffect? GetSlotCondition(int target, string status)
        {
            throw new NotImplementedException("GetSlotCondition method is not implemented yet.");
        }

        public IEffect? GetSlotCondition(int target, IEffect status)
        {
            throw new NotImplementedException("GetSlotCondition method is not implemented yet.");
        }

        public bool RemoveSlotCondition(Pokemon target, string status)
        {
            throw new NotImplementedException("RemoveSlotCondition method is not implemented yet.");
        }

        public bool RemoveSlotCondition(Pokemon target, IEffect status)
        {
            throw new NotImplementedException("RemoveSlotCondition method is not implemented yet.");
        }

        public bool RemoveSlotCondition(int target, string status)
        {
            throw new NotImplementedException("RemoveSlotCondition method is not implemented yet.");
        }

        public bool RemoveSlotCondition(int target, IEffect status)
        {
            throw new NotImplementedException("RemoveSlotCondition method is not implemented yet.");
        }

        public void Send(List<object> parts)
        {
            throw new NotImplementedException("Send method is not implemented yet.");
        }

        public void EmitRequest(ChoiceRequest update)
        {
            throw new NotImplementedException("EmitRequest method is not implemented yet.");
        }

        public void EmitRequest()
        {
            EmitRequest(ActiveRequest!);
        }

        public void EmitChoiceError(string message, Pokemon? pokemon = null,
            Func<PokemonMoveRequestData, bool?>? update = null)
        {
            throw new NotImplementedException();
        }

        public bool IsChoiceDone()
        {
            throw new NotImplementedException("IsChoiceDone method is not implemented yet.");
        }

        public bool ChooseMove(string moveText, int targetLoc = 0, EventType eventType = EventType.None)
        {
            throw new NotImplementedException("ChooseMove method is not implemented yet.");
        }

        public bool ChooseSwitch(int moveText, int targetLoc = 0, EventType eventType = EventType.None)
        {
            throw new NotImplementedException("ChooseSwitch method is not implemented yet.");
        }

        public bool ChooseInstaSwitch(int targetLoc = 0, EventType eventType = EventType.None)
        {
            throw new NotImplementedException("ChooseInstaSwitch method is not implemented yet.");
        }

        public bool UpdateRequestForPokemon(Pokemon pokemon, Func<PokemonMoveRequestData, bool?> update)
        {
            throw new NotImplementedException();
        }

        public Side? ChooseSwith(string? slotText = null)
        {
            throw new NotImplementedException("ChooseSwith method is not implemented yet.");
        }

        /**
         * The number of pokemon you must choose in Team Preview.
         *
         * Note that PS doesn't support choosing fewer than this number of pokemon.
         * In the games, it is sometimes possible to bring fewer than this, but
         * since that's nearly always a mistake, we haven't gotten around to
         * supporting it.
         */
        public int PickedTeamSize()
        {
            throw new NotImplementedException("PickedTeamSize method is not implemented yet.");
        }

        public bool ChooseTeam(string data = "")
        {
            throw new NotImplementedException("ChooseTeam method is not implemented yet.");
        }
        public bool ChooseShift()
        {
            throw new NotImplementedException("ChooseShift method is not implemented yet.");
        }

        public void ClearChoice()
        {
            throw new NotImplementedException("ClearChoice method is not implemented yet.");
        }

        public bool Choose(string input)
        {
            throw new NotImplementedException("Choose method is not implemented yet.");
        }

        public int GetChoiceIndex(bool? isPass = null)
        {
            throw new NotImplementedException("GetChoiceIndex method is not implemented yet.");
        }

        public Side ChoosePass()
        {
            throw new NotImplementedException("ChoosePass method is not implemented yet.");
        }

        public bool AutoChoose()
        {
            throw new NotImplementedException();
        }

        public void Destroy()
        {
            throw new NotImplementedException("Destroy method is not implemented yet.");
        }
    }
}