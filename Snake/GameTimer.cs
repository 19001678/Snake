using System;
using System.Windows.Threading;

public class GameTimer
{
    private readonly DispatcherTimer dispatcherTimer;
    public int TimeInSeconds { get; private set; }

    public event EventHandler TimerElapsed;

    public GameTimer(int initialTimeInSeconds)
    {
        TimeInSeconds = initialTimeInSeconds;

        dispatcherTimer = new DispatcherTimer();
        dispatcherTimer.Interval = TimeSpan.FromSeconds(1);
        dispatcherTimer.Tick += DispatcherTimer_Tick;
    }

    private void DispatcherTimer_Tick(object sender, EventArgs e)
    {
        TimeInSeconds--;

        if (TimeInSeconds <= 0)
        {
            TimeInSeconds = 0;
            dispatcherTimer.Stop();
            TimerElapsed?.Invoke(this, EventArgs.Empty);
            
        }
    }

    public void Start()
    {
        dispatcherTimer.Start();
    }

    public void Stop()
    {
        dispatcherTimer.Stop();
    }

    public void AddSeconds(int seconds)
    {
        TimeInSeconds += seconds;
    }

    public int GetTimeInSeconds()
    {
        return TimeInSeconds;
    }
}
