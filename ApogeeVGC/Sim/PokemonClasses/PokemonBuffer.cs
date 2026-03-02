using System.Runtime.CompilerServices;

namespace ApogeeVGC.Sim.PokemonClasses;

/// <summary>
/// A stack-allocated, fixed-capacity buffer that holds up to 4 <see cref="Pokemon"/> references.
/// Designed to replace <c>List&lt;Pokemon&gt;</c> in hot-path methods like
/// <c>Side.Allies()</c> and <c>Pokemon.Foes()</c> where the result is always
/// small (≤ 2 in VGC Doubles, ≤ 4 in potential FFA) and immediately iterated.
/// Eliminates tens of thousands of <c>List&lt;&gt;</c> heap allocations per battle.
/// </summary>
public struct PokemonBuffer
{
    private Pokemon? _p0, _p1, _p2, _p3;
    private byte _count;

    public readonly int Count
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _count;
    }

    public readonly Pokemon this[int index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => index switch
        {
            0 => _p0!,
            1 => _p1!,
            2 => _p2!,
            3 => _p3!,
            _ => throw new ArgumentOutOfRangeException(nameof(index)),
        };
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Add(Pokemon p)
    {
        switch (_count)
        {
            case 0: _p0 = p; break;
            case 1: _p1 = p; break;
            case 2: _p2 = p; break;
            case 3: _p3 = p; break;
            default: throw new InvalidOperationException("PokemonBuffer is full (max 4).");
        }
        _count++;
    }

    public readonly Pokemon? FirstOrDefault()
    {
        return _count > 0 ? _p0 : null;
    }

    public readonly bool Any()
    {
        return _count > 0;
    }

    public readonly bool Any(Func<Pokemon, bool> predicate)
    {
        for (int i = 0; i < _count; i++)
        {
            if (predicate(this[i])) return true;
        }
        return false;
    }

    public readonly void AddTo(List<Pokemon> list)
    {
        for (int i = 0; i < _count; i++)
            list.Add(this[i]);
    }

    public readonly List<Pokemon> ToList()
    {
        var list = new List<Pokemon>(_count);
        for (int i = 0; i < _count; i++)
            list.Add(this[i]);
        return list;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly Enumerator GetEnumerator() => new(this);

    public struct Enumerator
    {
        private readonly PokemonBuffer _buffer;
        private int _index;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal Enumerator(PokemonBuffer buffer)
        {
            _buffer = buffer;
            _index = -1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool MoveNext() => ++_index < _buffer._count;

        public readonly Pokemon Current
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _buffer[_index];
        }
    }
}
