using System.Runtime.CompilerServices;

namespace ApogeeVGC.Sim.BattleClasses;

/// <summary>
/// Custom interpolated string handler that short-circuits string construction
/// when <see cref="Battle.DebugMode"/> is false. This eliminates all string
/// allocations from <c>Battle.Debug($"...")</c> calls in non-debug mode.
/// </summary>
[InterpolatedStringHandler]
public ref struct DebugInterpolatedStringHandler
{
    private DefaultInterpolatedStringHandler _inner;
    private readonly bool _enabled;

    public DebugInterpolatedStringHandler(
        int literalLength,
        int formattedCount,
        Battle battle,
        out bool handlerIsValid)
    {
        _enabled = battle.DebugMode;
        handlerIsValid = _enabled;
        if (_enabled)
        {
            _inner = new DefaultInterpolatedStringHandler(literalLength, formattedCount);
        }
    }

    public void AppendLiteral(string value)
    {
        if (_enabled) _inner.AppendLiteral(value);
    }

    public void AppendFormatted<T>(T value)
    {
        if (_enabled) _inner.AppendFormatted(value);
    }

    public void AppendFormatted<T>(T value, string? format)
    {
        if (_enabled) _inner.AppendFormatted(value, format);
    }

    public void AppendFormatted<T>(T value, int alignment)
    {
        if (_enabled) _inner.AppendFormatted(value, alignment);
    }

    public void AppendFormatted<T>(T value, int alignment, string? format)
    {
        if (_enabled) _inner.AppendFormatted(value, alignment, format);
    }

    public void AppendFormatted(ReadOnlySpan<char> value)
    {
        if (_enabled) _inner.AppendFormatted(value);
    }

    public void AppendFormatted(ReadOnlySpan<char> value, int alignment = 0, string? format = null)
    {
        if (_enabled) _inner.AppendFormatted(value, alignment, format);
    }

    public void AppendFormatted(string? value)
    {
        if (_enabled) _inner.AppendFormatted(value);
    }

    public void AppendFormatted(string? value, int alignment = 0, string? format = null)
    {
        if (_enabled) _inner.AppendFormatted(value, alignment, format);
    }

    internal string ToStringAndClear()
    {
        return _enabled ? _inner.ToStringAndClear() : string.Empty;
    }
}
