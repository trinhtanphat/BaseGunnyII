using System.Collections.Concurrent;
using System.Diagnostics;

namespace GunnySocketProxy;

public sealed class ConnectionLimiter
{
    private readonly int _maxConcurrent;
    private readonly int _maxAttemptsPerMinute;
    private readonly long _windowTicks = checked(Stopwatch.Frequency * 60L);
    private readonly ConcurrentDictionary<string, ClientState> _clients = new(StringComparer.Ordinal);

    public ConnectionLimiter(int maxConcurrent, int maxAttemptsPerMinute)
    {
        if (maxConcurrent <= 0) throw new ArgumentOutOfRangeException(nameof(maxConcurrent));
        if (maxAttemptsPerMinute <= 0) throw new ArgumentOutOfRangeException(nameof(maxAttemptsPerMinute));
        _maxConcurrent = maxConcurrent;
        _maxAttemptsPerMinute = maxAttemptsPerMinute;
    }

    public bool TryAcquire(string clientKey, out IDisposable lease)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(clientKey);
        var state = _clients.GetOrAdd(clientKey, static _ => new ClientState());
        var now = Stopwatch.GetTimestamp();
        lock (state)
        {
            PurgeExpired(state, now);
            if (state.Active >= _maxConcurrent || state.Attempts.Count >= _maxAttemptsPerMinute)
            {
                lease = NullLease.Instance;
                return false;
            }
            state.Active++;
            state.Attempts.Enqueue(now);
            lease = new Lease(this, clientKey, state);
            return true;
        }
    }

    private void PurgeExpired(ClientState state, long now)
    {
        var cutoff = now - _windowTicks;
        while (state.Attempts.Count > 0 && state.Attempts.Peek() <= cutoff)
            state.Attempts.Dequeue();
    }

    private void Release(string clientKey, ClientState state)
    {
        lock (state)
        {
            if (state.Active > 0) state.Active--;
            PurgeExpired(state, Stopwatch.GetTimestamp());
            if (state.Active == 0 && state.Attempts.Count == 0)
                _clients.TryRemove(new KeyValuePair<string, ClientState>(clientKey, state));
        }
    }

    private sealed class ClientState
    {
        public int Active;
        public Queue<long> Attempts { get; } = new();
    }
    private sealed class Lease : IDisposable
    {
        private ConnectionLimiter? _owner;
        private readonly string _clientKey;
        private readonly ClientState _state;

        public Lease(ConnectionLimiter owner, string clientKey, ClientState state)
        {
            _owner = owner;
            _clientKey = clientKey;
            _state = state;
        }

        public void Dispose() => Interlocked.Exchange(ref _owner, null)?.Release(_clientKey, _state);
    }

    private sealed class NullLease : IDisposable
    {
        public static readonly NullLease Instance = new();
        public void Dispose() { }
    }
}
