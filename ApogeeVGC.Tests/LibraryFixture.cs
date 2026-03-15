using ApogeeVGC.Data;

namespace ApogeeVGC.Tests;

/// <summary>
/// Shared fixture that provides a single <see cref="Library"/> instance
/// for all test classes in the collection. Library initialization is expensive
/// (loads all game data), so sharing avoids repeated startup costs.
/// </summary>
public sealed class LibraryFixture
{
    public Library Library { get; } = new();
}

[CollectionDefinition(Name)]
public class LibraryCollection : ICollectionFixture<LibraryFixture>
{
    public const string Name = "Library";
}
