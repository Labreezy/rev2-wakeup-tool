using System.Collections.Concurrent;
using System.Timers;
using Timer = System.Timers.Timer;

namespace GGXrdReversalTool.Library.Logging;

public class LogManager
{


    #region Singleton
    private static readonly Lazy<LogManager> Lazy = new(() => new LogManager());
    public static LogManager Instance => Lazy.Value;

    private LogManager()
    {
        _queueTimer.Elapsed += QueueTimer_Elapsed;

        _queueTimer.Enabled = true;
    }
    #endregion


    public string FileName => "Log.txt";

    public event EventHandler<string>? MessageDequeued;
    private readonly ConcurrentQueue<string> _messageQueue = new();
    private readonly Timer _queueTimer = new (1000);

    private void QueueTimer_Elapsed(object sender, ElapsedEventArgs e)
    {
        DequeueMessage();
    }

    private void DequeueMessage()
    {
        while (_messageQueue.TryDequeue(out var message))
        {
            ProcessMessage(message);
        }
    }


    private void ProcessMessage(string message)
    {
        Console.WriteLine(message);

        MessageDequeued?.Invoke(null, message);


#if !DEBUG
            try
            {
                using var writer = File.AppendText(FileName);
                writer.WriteLine(message);
            }
            catch (Exception)
            {
                // ignored
            }
#endif
    }

    public void WriteLine(string message)
    {
        string line = $"[{DateTimeOffset.Now.ToUniversalTime():yyyy-MM-dd HH:mm:ss}] {message}";
        _messageQueue.Enqueue(line);

    }

    public void WriteException(Exception exception)
    {
        WriteLine($"{exception.GetType().Name}: {exception.Message}\r\n{exception.StackTrace}");
    }
}