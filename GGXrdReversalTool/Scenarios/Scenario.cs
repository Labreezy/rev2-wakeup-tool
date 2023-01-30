using System.Threading;
using GGXrdReversalTool.Scenarios.Action;
using GGXrdReversalTool.Scenarios.Event;
using GGXrdReversalTool.Scenarios.Frequency;

namespace GGXrdReversalTool.Scenarios;

public class Scenario
{
    private readonly IScenarioEvent _scenarioEvent;
    private readonly IScenarioAction _scenarioAction;
    private readonly IScenarioFrequency _scenarioFrequency;


    private static bool _runThread;
    private static readonly object RunThreadLock = new object();



    public Scenario(IScenarioEvent scenarioEvent, IScenarioAction scenarioAction, IScenarioFrequency scenarioFrequency)
    {
        _scenarioEvent = scenarioEvent;
        _scenarioAction = scenarioAction;
        _scenarioFrequency = scenarioFrequency;
    }



    public void Run()
    {
        lock (RunThreadLock)
        {
            _runThread = true;
        }

        Thread scenarioThread = new Thread(() =>
        {
            //TODO Inject LogManager
            // LogManager.Instance.WriteLine("Scenario Thread start");
            bool localRunThread = true;

            _scenarioEvent.Occured += (sender, eventArgs) =>
            {
                var shouldExecuteAction = _scenarioFrequency.ShouldHappen();

                if (shouldExecuteAction)
                {
                    _scenarioAction.Execute();
                }
            };


            while (localRunThread)
            {

                _scenarioEvent.CheckEvent();



                lock (RunThreadLock)
                {
                    localRunThread = _runThread;
                }

                Thread.Sleep(1);
            }


            //TODO Inject LogManager
            // LogManager.Instance.WriteLine("Scenario Thread ended");
        });

        scenarioThread.Start();

    }



    public void Stop()
    {
        lock (RunThreadLock)
        {
            _runThread = false;
        }
    }



}