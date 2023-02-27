﻿using GGXrdReversalTool.Library.Characters;
using GGXrdReversalTool.Library.Logging;
using GGXrdReversalTool.Library.Memory;
using GGXrdReversalTool.Library.Models;
using GGXrdReversalTool.Library.Models.Inputs;
using GGXrdReversalTool.Library.Scenarios.Action;
using GGXrdReversalTool.Library.Scenarios.Event;
using GGXrdReversalTool.Library.Scenarios.Frequency;

namespace GGXrdReversalTool.Library.Scenarios;

public class Scenario : IDisposable
{
    private readonly IMemoryReader _memoryReader;
    
    private readonly IScenarioEvent _scenarioEvent;
    private readonly IScenarioAction _scenarioAction;
    private readonly IScenarioFrequency _scenarioFrequency;


    private static bool _runThread;
    private static readonly object RunThreadLock = new object();

    public bool IsRunning => _runThread;



    public Scenario(
        IMemoryReader memoryReader,
        IScenarioEvent scenarioEvent, 
        IScenarioAction scenarioAction, 
        IScenarioFrequency scenarioFrequency)
    {
        _memoryReader = memoryReader; 
        _scenarioEvent = scenarioEvent;
        _scenarioAction = scenarioAction;
        _scenarioFrequency = scenarioFrequency;
    }


    private void Init()
    {
        
        //TODO Inject via factory
        _scenarioEvent.MemoryReader = _memoryReader;
        _scenarioAction.MemoryReader = _memoryReader;
        _scenarioFrequency.MemoryReader = _memoryReader;

        _scenarioAction.Init();
    }

    public void Run()
    {
        lock (RunThreadLock)
        {
            _runThread = true;
        }
        
        Init();
        
        

        Thread scenarioThread = new Thread(() =>
        {
            LogManager.Instance.WriteLine("Scenario Thread start");
            bool localRunThread = true;


            while (localRunThread)
            {
                var eventType = _scenarioEvent.CheckEvent();
                if (eventType != ScenarioEventTypes.None)
                {
                    LogManager.Instance.WriteLine("Event Occured");
                    
                    //TODO should remove from loop?
                    var currentDummy = _memoryReader.GetCurrentDummy();
                    

                    var timing = GetTiming(eventType, currentDummy, _scenarioAction.Input);

                    Wait(timing);

                    //TODO fix event frquency
                    //var shouldExecuteAction = _scenarioFrequency.ShouldHappen();

                    var shouldExecuteAction = true;

                    if (shouldExecuteAction)
                    {
                        _scenarioAction.Execute();
                        
                        LogManager.Instance.WriteLine("Action Executed");
                    }
                }
                



                lock (RunThreadLock)
                {
                    localRunThread = _runThread;
                }

                Thread.Sleep(1);
            }


            LogManager.Instance.WriteLine("Scenario Thread ended");
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

    
    private int GetTiming(ScenarioEventTypes eventType, Character currentDummy, SlotInput scenarioActionInput)
    {
        //TODO fix why - 2 ?
        switch (eventType)
        {
            case ScenarioEventTypes.KDFaceUp:
                return currentDummy.FaceUpFrames - scenarioActionInput.ReversalFrameIndex - 2;
            case ScenarioEventTypes.KDFaceDown:
                return currentDummy.FaceDownFrames - scenarioActionInput.ReversalFrameIndex - 2;
            case ScenarioEventTypes.WallSplat:
                return currentDummy.WallSplatWakeupTiming - scenarioActionInput.ReversalFrameIndex - 2;
            case ScenarioEventTypes.Blocking:
                return 0;
            case ScenarioEventTypes.Combo:
                return 0;
            case ScenarioEventTypes.Tech:
                //TODO tech reversal recovery = 6?
                return 6 - 2;
            default:
                throw new ArgumentOutOfRangeException(nameof(eventType), eventType, null);
        }
    }
    private void Wait(int frames)
    {
        if (frames > 0)
        {
            int startFrame = _memoryReader.FrameCount();
            int frameCount;

            do
            {
                Thread.Sleep(10);
                frameCount = _memoryReader.FrameCount() - startFrame;
            } while (frameCount < frames);
        }
    }


    public void Dispose()
    {
        Stop();
    }
}