using ApogeeVGC.Data;
using ApogeeVGC.Sim.Utils;

namespace ApogeeVGC.Sim.PokemonClasses;

public record ExportOptions
{
    public bool? HideStats { get; init; }
    public bool? RemoveNicknames { get; init; }
}

public static class Teams
{
    public static string Pack(List<PokemonSet>? team)
    {
        throw new NotImplementedException();
    }

    public static List<PokemonSet>? Unpack(string buf)
    {
        throw new NotImplementedException();
    }

    public static string PackName(StringUndefinedUnion? name)
    {
        throw new NotImplementedException();
    }

    public static string UnpackName(string name, Library library)
    {
        throw new NotImplementedException();
    }

    public static string Export(List<PokemonSet> team, ExportOptions? options = null)
    {
        throw new NotImplementedException();
    }

    public static string ExportSet(PokemonSet set, ExportOptions? options = null)
    {
        throw new NotImplementedException();    
    }

    public static void ParseExportedTeamLine(string line, bool isFirstLine, PokemonSet set, bool aggressive = false)
    {
        throw new NotImplementedException();
    }

    public static List<PokemonSet>? Import(string bufffer, bool aggressive = false)
    {
        throw new NotImplementedException();
    }

    // GetGenerator()
    // Generate()
}