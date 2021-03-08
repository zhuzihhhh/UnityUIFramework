using System;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;

public class AsyncMessagebroker<T> : IDisposable {
    private Channel<T> channel;
    private IConnectableUniTaskAsyncEnumerable<T> multicastSource;
    private IDisposable connection;

    public AsyncMessagebroker() {
        channel = Channel.CreateSingleConsumerUnbounded<T>();
        multicastSource = channel.Reader.ReadAllAsync().Publish();
        connection = multicastSource.Connect();
    }

    public void Publish(T value) {
        channel.Writer.TryWrite(value);
    }

    public IUniTaskAsyncEnumerable<T> Subscribe() {
        return multicastSource;
    }

    public void Dispose() {
        channel.Writer.TryComplete();
        connection.Dispose();
    }
}