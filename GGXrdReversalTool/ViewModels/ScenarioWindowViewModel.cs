using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using GGXrdReversalTool.Commands;
using GGXrdReversalTool.Library.Configuration;
using GGXrdReversalTool.Library.Memory;
using GGXrdReversalTool.Library.Memory.Implementations;
using GGXrdReversalTool.Library.Models;
using GGXrdReversalTool.Library.Models.Inputs;
using GGXrdReversalTool.Library.Scenarios;
using GGXrdReversalTool.Library.Scenarios.Action;
using GGXrdReversalTool.Library.Scenarios.Action.Implementations;
using GGXrdReversalTool.Library.Scenarios.Event;
using GGXrdReversalTool.Library.Scenarios.Event.Implementations;
using GGXrdReversalTool.Library.Scenarios.Frequency;
using GGXrdReversalTool.Library.Scenarios.Frequency.Implementations;

namespace GGXrdReversalTool.ViewModels;

public class ScenarioWindowViewModel : ViewModelBase
{
    private readonly Process _process;
    private readonly IMemoryReader _memoryReader;
    private Scenario? _scenario;
    public ScenarioWindowViewModel()
    {
        _process = Process.GetProcessesByName("GuiltyGearXrd").FirstOrDefault()!;

        if (_process == null)
        {
            //TODO explicit error
            throw new NotImplementedException();
        }

        _memoryReader = new MemoryReader(_process);

        _scenarioEvents = new ObservableCollection<IScenarioEvent>(new IScenarioEvent[]
        {
            new ComboEvent(),
            new AnimationEvent()
        });


        _selectedScenarioAction = new PlayReversalAction() { Input = new SlotInput("6,2,!3H") };
        _selectedScenarioFrequency = new PercentageFrequency();

    }

    #region Window

    #region WindowLoadedCommand
    public RelayCommand WindowLoadedCommand =>  new(Init);
    #endregion
    
    #region WindowClosedCommand

    public RelayCommand WindowClosedCommand => new(Dispose);
    #endregion

    #endregion
    
    #region ReplayTrigger

    public bool IsAsmReplayTypeChecked => ReversalToolConfiguration.Get("ReplayTriggerType") == ReplayTriggerTypes.AsmInjection.ToString();
    public bool IsKeyStrokeReplayTypeChecked => ReversalToolConfiguration.Get("ReplayTriggerType") == ReplayTriggerTypes.Keystroke.ToString();



    public RelayCommand<string> ChangeReplayTypeCommand => new(ChangeReplayTypeCommandExecute, ChangeReplayTypeCommandCanExecute);
    private void ChangeReplayTypeCommandExecute(string parameter)
    {
        ReplayTriggerTypes replayTriggerType =
            (ReplayTriggerTypes) Enum.Parse(typeof(ReplayTriggerTypes), parameter);

        //TODO Implement
        // this._reversalTool.ChangeReplayTrigger(replayTriggerType);

        OnPropertyChanged(nameof(IsAsmReplayTypeChecked));
        OnPropertyChanged(nameof(IsKeyStrokeReplayTypeChecked));
    }
    private bool ChangeReplayTypeCommandCanExecute(string parameter)
    {
        //TODO Implement
        return true;
        // return this._reversalTool.ReplayTriggerType.ToString() != parameter;
    }

        

    #endregion
    
    #region CheckUpdatesCommand

    public RelayCommand CheckUpdatesCommand =>  new(CheckUpdates, CanCheckUpdates);

    private bool CanCheckUpdates()
    {
        return true;
        //TODO Implement 
        // return !IsWakeupReversalStarted && !IsBlockstunReversalStarted && !IsRandomBurstStarted;
    }
    private void CheckUpdates()
    {
        // this.UpdateProcess(true);
    }



    #endregion
    public string Title => $"GGXrd Rev 2 Reversal Tool v{ReversalToolConfiguration.Get("CurrentVersion")}";

    public bool AutoUpdate
    {
        get => ReversalToolConfiguration.Get("AutoUpdate") == "True";
        set
        {
            ReversalToolConfiguration.Set("AutoUpdate", value ? "1" : "0");


            OnPropertyChanged();
        }
    }


    private ObservableCollection<IScenarioEvent> _scenarioEvents;
    public ObservableCollection<IScenarioEvent> ScenarioEvents
    {
        get => _scenarioEvents;
        set
        {
            if (Equals(value, _scenarioEvents)) return;
            _scenarioEvents = value;
            OnPropertyChanged();
        }
    }

    private IScenarioEvent? _selectedScenarioEvent;
    public IScenarioEvent? SelectedScenarioEvent
    {
        get => _selectedScenarioEvent;
        set
        {
            if (Equals(value, _selectedScenarioEvent)) return;
            _selectedScenarioEvent = value;
            OnPropertyChanged();
        }
    }


    private IScenarioAction? _selectedScenarioAction;
    public IScenarioAction? SelectedScenarioAction
    {
        get => _selectedScenarioAction;
        set
        {
            if (Equals(value, _selectedScenarioAction)) return;
            _selectedScenarioAction = value;
            OnPropertyChanged();
        }
    }

    private IScenarioFrequency? _selectedScenarioFrequency;

    public IScenarioFrequency? SelectedScenarioFrequency
    {
        get => _selectedScenarioFrequency;
        set
        {
            if (Equals(value, _selectedScenarioFrequency)) return;
            _selectedScenarioFrequency = value;
            OnPropertyChanged();
        }
    }


    #region EnableCommand

    public RelayCommand EnableCommand => new(Enable, CanEnable);

    private void Enable()
    {
        this._scenario = new Scenario(this._memoryReader, SelectedScenarioEvent, SelectedScenarioAction, SelectedScenarioFrequency);
        
        this._scenario.Run();
    }

    private bool CanEnable()
    {
        return _selectedScenarioEvent != null &&
               _selectedScenarioAction != null &&
               _selectedScenarioFrequency != null &&
               ((_selectedScenarioEvent is AnimationEvent && _selectedScenarioAction.Input.IsReversalValid) || (_selectedScenarioEvent is ComboEvent && _selectedScenarioAction.Input.IsValid)) &&
               _scenario is not { IsRunning: true };
    }

    #endregion

    #region DisableCommand

    public RelayCommand DisableCommand => new(Disable, CanDisable);

    private void Disable()
    {
        this._scenario?.Stop();
    }

    private bool CanDisable()
    {
        return this._scenario?.IsRunning ?? false;
        
        
        //TODO Implement
    }

    #endregion

    private void Init()
    {
        // var process = Process.GetProcessesByName("GuiltyGearXrd").FirstOrDefault();
        // var memoryReader = new MemoryReader(process);
        // // var scenarioEvent = new WakeupEvent();
        // // var scenarioEvent = new ComboEvent();
        // var scenarioEvent = new AnimationEvent();
        // var scenarioAction = new PlayReversalAction();
        // var scenarioFrequency = new PercentageFrequency(100);
        //
        // var scenario = new Scenario(memoryReader, scenarioEvent, scenarioAction, scenarioFrequency);
        //
        // scenario.Run();
    }

    private void Dispose()
    {
        _scenario?.Stop();
    }
}