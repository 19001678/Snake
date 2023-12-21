using System;
using System.Windows.Threading;

public class GameTimer
{
    private readonly DispatcherTimer dispatcherTimer;
    public int TimeInMilliseconds { get; set; }

    public event EventHandler TimerElapsed;

    public GameTimer(int initialTimeInMilliseconds)
    {
        TimeInMilliseconds = initialTimeInMilliseconds;

        dispatcherTimer = new DispatcherTimer();
        dispatcherTimer.Interval = TimeSpan.FromMilliseconds(100);
        dispatcherTimer.Tick += DispatcherTimer_Tick;
    }

    private void DispatcherTimer_Tick(object sender, EventArgs e)
    {
        TimeInMilliseconds-=100;

        if (TimeInMilliseconds <= 0)
        {
            TimeInMilliseconds = 0;
            dispatcherTimer.Stop();
            //TimerElapsed?.Invoke(this, EventArgs.Empty);
            
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

    public void AddMilliseconds(int milliseconds)
    {
            TimeInMilliseconds += milliseconds;
    }

    public int GetTimeInMilliseconds()
    {
        return TimeInMilliseconds;
    }
}
