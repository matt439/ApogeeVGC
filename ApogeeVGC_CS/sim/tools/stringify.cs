namespace ApogeeVGC_CS.sim.tools
{
    public static class Stringify
    {
        public static string GetMoveTargetString(MoveTarget target)
        {
            return target switch
            {
                MoveTarget.AdjacentAlly => "adjacentAlly",
                MoveTarget.AdjacentAllyOrSelf => "adjacentAllyOrSelf",
                MoveTarget.AdjacentFoe => "adjacentFoe",
                MoveTarget.All => "all",
                MoveTarget.AllAdjacent => "allAdjacent",
                MoveTarget.AllAdjacentFoes => "allAdjacentFoes",
                MoveTarget.Allies => "allies",
                MoveTarget.AllySide => "allySide",
                MoveTarget.AllyTeam => "allyTeam",
                MoveTarget.Any => "any",
                MoveTarget.FoeSide => "foeSide",
                MoveTarget.Normal => "normal",
                MoveTarget.RandomNormal => "randomNormal",
                MoveTarget.Scripted => "scripted",
                MoveTarget.Self => "self",
                MoveTarget.None => "none",
                _ => "Unknown move target."
            };
        }
    }
}