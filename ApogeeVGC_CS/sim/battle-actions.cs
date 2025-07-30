using System;
using System.Collections.Generic;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ApogeeVGC_CS.sim
{
    public static class ChoosableTargets
    {
        public static readonly HashSet<string> Targets = new()
        {
            "normal", "any", "adjacentAlly", "adjacentAllyOrSelf", "adjacentFoe"
        };
    }

    public class BattleActions
    {
        public Battle Battle { get; }
        public ModdedDex Dex { get; }

        public static readonly Dictionary<string, string> MaxMoves = new()
        {
            ["Flying"] = "Max Airstream",
            ["Dark"] = "Max Darkness",
            ["Fire"] = "Max Flare",
            ["Bug"] = "Max Flutterby",
            ["Water"] = "Max Geyser",
            ["Status"] = "Max Guard",
            ["Ice"] = "Max Hailstorm",
            ["Fighting"] = "Max Knuckle",
            ["Electric"] = "Max Lightning",
            ["Psychic"] = "Max Mindstorm",
            ["Poison"] = "Max Ooze",
            ["Grass"] = "Max Overgrowth",
            ["Ghost"] = "Max Phantasm",
            ["Ground"] = "Max Quake",
            ["Rock"] = "Max Rockfall",
            ["Fairy"] = "Max Starfall",
            ["Steel"] = "Max Steelspike",
            ["Normal"] = "Max Strike",
            ["Dragon"] = "Max Wyrmwind"
        };

        public static readonly Dictionary<string, string> ZMoves = new()
        {
            ["Poison"] = "Acid Downpour",
            ["Fighting"] = "All-Out Pummeling",
            ["Dark"] = "Black Hole Eclipse",
            ["Grass"] = "Bloom Doom",
            ["Normal"] = "Breakneck Blitz",
            ["Rock"] = "Continental Crush",
            ["Steel"] = "Corkscrew Crash",
            ["Dragon"] = "Devastating Drake",
            ["Electric"] = "Gigavolt Havoc",
            ["Water"] = "Hydro Vortex",
            ["Fire"] = "Inferno Overdrive",
            ["Ghost"] = "Never-Ending Nightmare",
            ["Bug"] = "Savage Spin-Out",
            ["Psychic"] = "Shattered Psyche",
            ["Ice"] = "Subzero Slammer",
            ["Flying"] = "Supersonic Skystrike",
            ["Ground"] = "Tectonic Rage",
            ["Fairy"] = "Twinkle Tackle"
        };

        public BattleActions(Battle battle)
        {
            // TODO
        }

        public enum SwitchInResult
        {
            Success,
            Fail,
            PursuitFaint,
        }

        public SwitchInResult SwitchIn(Pokemon pokemon, int pos, IEffect? sourceEffect = null, bool isDrag = false)
        {
            throw new NotImplementedException("SwitchIn method is not implemented yet.");
        }

        public bool DragIn(Side side, int pos)
        {
            throw new NotImplementedException("DragIn method is not implemented yet.");
        }

        public bool RunSwitch(Pokemon pokemon)
        {
            throw new NotImplementedException("RunSwitch method is not implemented yet.");
        }

        // Helper struct for RunMove method
        public struct RunMoveResult
        {
            public EffectState? SourceEffect { get; set; }
            public string? ZMove { get; set; }
            public bool? ExternalMove { get; set; }
            public string? MaxMove { get; set; }
            public Pokemon? OriginalTarget { get; set; }
        }

        public RunMoveResult RunMove(Move move, Pokemon pokemon, int targetLoc,
            IEffect? sourceEffect = null, string? zMove = null, bool externalMove = false,
            string? maxMove = null, Pokemon? originalTarget = null)
        {
            throw new NotImplementedException("RunMove method is not implemented yet.");
        }

        public RunMoveResult RunMove(string moveName, Pokemon pokemon, int targetLoc,
            IEffect? sourceEffect = null, string? zMove = null, bool externalMove = false,
            string? maxMove = null, Pokemon? originalTarget = null)
        {
            throw new NotImplementedException();
        }

        // Helper struct for UseMove method
        public struct UseMoveOptions
        {
            public Pokemon? Target { get; set; }
            public IEffect? SourceEffect { get; set; }
            public string? ZMove { get; set; }
            public string? MaxMove { get; set; }
        }

        public bool UseMove(Move move, Pokemon pokemon, UseMoveOptions? options = null)
        {
            throw new NotImplementedException("UseMove method is not implemented yet.");
        }

        public bool UseMove(string moveName, Pokemon pokemon, UseMoveOptions? options = null)
        {
            throw new NotImplementedException("UseMove method is not implemented yet.");
        }

        public bool UseMoveInner(Move move, Pokemon pokemon, UseMoveOptions? options = null)
        {
            throw new NotImplementedException("UseMoveInner method is not implemented yet.");
        }

        public bool UseMoveInner(string moveName, Pokemon pokemon, UseMoveOptions? options = null)
        {
            throw new NotImplementedException("UseMoveInner method is not implemented yet.");
        }

        public bool TrySpreadMoveHit(List<Pokemon> targets, Pokemon pokemon, ActiveMove move, bool? notActive = null)
        {
            throw new NotImplementedException("TrySpreadMoveHit method is not implemented yet.");
        }

        public object[] HitStepInvulnerabilityEvent(List<Pokemon> targets, Pokemon pokemon, ActiveMove move)
        {
            throw new NotImplementedException("HitStepInvulnerabilityEvent method is not implemented yet.");
        }

        public object HitStepTryHitEvent(List<Pokemon> targets, Pokemon pokemon, ActiveMove move)
        {
            throw new NotImplementedException("HitStepTryHitEvent method is not implemented yet.");
        }

        public bool[] HitStepTypeImmunity(List<Pokemon> targets, Pokemon pokemon, ActiveMove move)
        {
            throw new NotImplementedException("HitStepTypeImmunity method is not implemented yet.");
        }

        public bool[] HitStepTryImmunity(List<Pokemon> targets, Pokemon pokemon, ActiveMove move)
        {
            throw new NotImplementedException("HitStepTypeImmunity method is not implemented yet.");
        }

        public bool[] HitStepAccuracy(List<Pokemon> targets, Pokemon pokemon, ActiveMove move)
        {
            throw new NotImplementedException("HitStepTypeImmunity method is not implemented yet.");
        }
        public bool[] HitStepBreakProtect(List<Pokemon> targets, Pokemon pokemon, ActiveMove move)
        {
            throw new NotImplementedException("HitStepTypeImmunity method is not implemented yet.");
        }
        public bool[] HitStepStealBoosts(List<Pokemon> targets, Pokemon pokemon, ActiveMove move)
        {
            throw new NotImplementedException("HitStepTypeImmunity method is not implemented yet.");
        }
        public bool[] AfterMoveSecondaryEvent(List<Pokemon> targets, Pokemon pokemon, ActiveMove move)
        {
            throw new NotImplementedException("HitStepTypeImmunity method is not implemented yet.");
        }

        public object TryMoveHit(List<Pokemon> targetOrTargets, Pokemon pokemon, ActiveMove move)
        {
            throw new NotImplementedException("TryMoveHit method is not implemented yet.");
        }
        public object HitStepMoveHitLoop(List<Pokemon> targets, Pokemon pokemon, ActiveMove move)
        {
            throw new NotImplementedException("HitStepTypeImmunity method is not implemented yet.");
        }
        public (SpreadMoveDamage, SpreadMoveTargets) SpreadMoveHit(SpreadMoveTargets targets,
            Pokemon pokemon, ActiveMove moveOrMoveName,
            IHitEffect? hitEffect = null, bool? isSecondary = null, bool? isSelf = false)
        {
            throw new NotImplementedException("SpreadMoveHit method is not implemented yet.");
        }

        public SpreadMoveDamage TryPrimaryHitEvent(SpreadMoveDamage damage,
            SpreadMoveTargets targets, Pokemon pokemon,
            ActiveMove move, ActiveMove moveData, bool? isSecondary = null)
        {
            throw new NotImplementedException("TryPrimaryHitEvent method is not implemented yet.");
        }

        public SpreadMoveDamage GetSpreadDamage(SpreadMoveDamage damage,
            SpreadMoveTargets targets, Pokemon source,
            ActiveMove move, ActiveMove moveData, bool? isSecondary = null, bool? isSelf = null)
        {
            throw new NotImplementedException("GetSpreadDamage method is not implemented yet.");

        }

        public SpreadMoveDamage RunMoveEffects(SpreadMoveDamage damage,
            SpreadMoveTargets targets, Pokemon source,
            ActiveMove move, ActiveMove moveData, bool? isSecondary = null, bool? isSelf = null)
        {
            throw new NotImplementedException("RunMoveEffects method is not implemented yet.");
        }

        public void SelfDrops(SpreadMoveTargets targets, Pokemon source,
            ActiveMove move, ActiveMove moveData, bool? isSecondary = null)
        {
            throw new NotImplementedException("SelfDrops method is not implemented yet.");
        }

        public void Secondaries(SpreadMoveTargets targets, Pokemon source,
            ActiveMove move, ActiveMove moveData, bool? isSelf = null)
        {
            throw new NotImplementedException("Secondaries method is not implemented yet.");
        }

        public SpreadMoveDamage ForceSwitch(SpreadMoveDamage damage,
            SpreadMoveTargets targets, Pokemon source, ActiveMove move)
        {
            throw new NotImplementedException("ForceSwitch method is not implemented yet.");
        }

        public int? MoveHit(List<Pokemon> targets, Pokemon pokemon, ActiveMove moveOrMoveName,
            IHitEffect? moveData = null, bool? isSecondary = null, bool? isSelf = null)
        {
            throw new NotImplementedException("MoveHit method is not implemented yet.");
        }

        public int CalcRecoilDamage(int damageDealt, Move move, Pokemon pokemon)
        {
            throw new NotImplementedException();
        }

        public string? GetZMove(Move move, Pokemon pokemon, bool? skipChecks = null)
        {
            throw new NotImplementedException("GetZMove method is not implemented yet.");
        }

        public ActiveMove GetActiveZMove(Move move, Pokemon pokemon)
        {
            throw new NotImplementedException("GetActiveZMove method is not implemented yet.");
        }

        public ZMoveOptions? CanZMove(Pokemon pokemon)
        {
            throw new NotImplementedException("CanZMove method is not implemented yet.");
        }

        public Move? GetMaxMove(Move move, Pokemon pokemon)
        {
            throw new NotImplementedException("GetMaxMove method is not implemented yet.");
        }

        public ActiveMove GetActiveMaxMove(Move move, Pokemon pokemon)
        {
            throw new NotImplementedException("GetActiveMaxMove method is not implemented yet.");
        }

        public void RunZPower(ActiveMove move, Pokemon pokemon)
        {
            throw new NotImplementedException("RunZPower method is not implemented yet.");
        }

        public bool TargetTypeChoices(string targetType)
        {
            throw new NotImplementedException("TargetTypeChoices method is not implemented yet.");
        }

        public static T CombineResults<T>(T left, T right) where T : struct
        {
            throw new NotImplementedException("CombineResults method is not implemented yet.");
        }

        /**
	     * 0 is a success dealing 0 damage, such as from False Swipe at 1 HP.
	     *
	     * Normal PS return value rules apply:
	     * undefined = success, null = silent failure, false = loud failure
	     */
        public int? GetDamage(Pokemon source, Pokemon target, object move,
            bool suppressMessages = false)
        {
            throw new NotImplementedException("GetDamage method is not implemented yet.");
        }

        public int ModifyDamage(int baseDamage, Pokemon pokemon, Pokemon target, ActiveMove move, bool suppressMessages = false)
        {
            throw new NotImplementedException("ModifyDamage method is not implemented yet.");
        }

        public int GetConfusionDamage(Pokemon pokemon, int basePower)
        {
            throw new NotImplementedException("GetConfusionDamage method is not implemented yet.");
        }

        public string? CanMegaEvolve(Pokemon pokemon)
        {
            throw new NotImplementedException("CanMegaEvolve method is not implemented yet.");
        }

        public string? CanUltraBurst(Pokemon pokemon)
        {
            throw new NotImplementedException("CanUltraBurst method is not implemented yet.");
        }

        public bool RunMegaEvo(Pokemon pokemon)
        {
            throw new NotImplementedException("RunMegaEvo method is not implemented yet.");
        }

        public Func<BattleActions, Pokemon, string?>? CanMegaEvoX { get; set; }
        public Func<BattleActions, Pokemon, string?>? CanMegaEvoY { get; set; }
        public Func<BattleActions, Pokemon, bool>? RunMegaEvoX { get; set; }
        public Func<BattleActions, Pokemon, bool>? RunMegaEvoY { get; set; }

        public string? CanTerastallize(Pokemon pokemon)
        {
            throw new NotImplementedException("CanTerastallize method is not implemented yet.");
        }

        public void Terastallize(Pokemon pokemon)
        {
            throw new NotImplementedException("Terastallize method is not implemented yet.");
        }

    }
}