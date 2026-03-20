using ApogeeVGC.Mcts.Ensemble;
using ApogeeVGC.Mcts.Ensemble.MiniModels;
using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Core;
using ApogeeVGC.Sim.Player;

namespace ApogeeVGC.Mcts;

/// <summary>
/// Factory for creating an MCTS player with ensemble-informed root priors.
/// Returns a PlayerMctsStandalone backed by MctsSearchEnsemble,
/// reusing the proven player infrastructure to avoid code duplication.
/// </summary>
public static class PlayerMctsEnsemble
{
    public static PlayerMctsStandalone Create(
        SideId sideId,
        PlayerOptions options,
        IBattleController battleController)
    {
        IMiniModel[] models =
        [
            new DamageMaxMiniModel(),
            new KOSeekingMiniModel(),
            new KOAvoidanceMiniModel(),
            new TypePositioningMiniModel(),
            new DamageMinMiniModel(),
            new SpeedAdvantageMiniModel(),
            new StatusSpreadingMiniModel(),
            new ProtectPredictionMiniModel(),
        ];

        string configPath = Environment.GetEnvironmentVariable("APOGEE_ENSEMBLE_CONFIG")
                            ?? Path.Combine("Tools", "DLModel", "ensemble_config.json");
        EnsembleConfig config = EnsembleConfig.Load(configPath);

        var ensemble = new EnsembleEvaluator(models, config);

        OpponentInference? opponentModel = null;
        StateEncoder? stateEncoder = null;
        if (MctsResources.IsInitialized && MctsResources.OpponentModel != null)
        {
            opponentModel = MctsResources.OpponentModel;
            stateEncoder = MctsResources.Encoder;
        }

        MctsConfig mctsConfig = options.MctsConfig ?? new MctsConfig();
        var search = new MctsSearchEnsemble(
            mctsConfig, ensemble, opponentModel, stateEncoder);

        return new PlayerMctsStandalone(sideId, options, battleController, search);
    }
}
