using GGXrdReversalTool.Library.Memory;

namespace GGXrdReversalTool.Library.Scenarios.Event.Implementations;

public class AnimationEvent : IScenarioEvent
{
    private const string FaceDownAnimation = "CmnActFDown2Stand";
    private const string FaceUpAnimation = "CmnActBDown2Stand";
    private const string WallSplatAnimation = "CmnActWallHaritsukiGetUp";
    private const string TechAnimation = "CmnActUkemi";
    private const string CrouchBlockingAnimation = "CmnActCrouchGuard";
    private const string StandBlockingAnimation = "CmnActMidGuardLoop";
    private const string HighBlockingAnimation = "CmnActHighGuardLoop";

    public IMemoryReader MemoryReader { get; set; }

    public bool ShouldCheckWakingUp { get; set; } = true;
    public bool ShouldCheckWallSplat { get; set; } = true;
    public bool ShouldCheckAirTech { get; set; } = false;

    public bool ShouldCheckStartBlocking { get; set; } = false;


    private string _oldAnimation = "";
    public ScenarioEventTypes CheckEvent()
    {
        var animationString = MemoryReader.ReadAnimationString(2);

        _oldAnimation = animationString;

        return animationString switch
        {
            FaceDownAnimation when ShouldCheckWakingUp => ScenarioEventTypes.KDFaceDown,
            FaceUpAnimation when ShouldCheckWakingUp => ScenarioEventTypes.KDFaceUp,
            WallSplatAnimation when ShouldCheckWallSplat => ScenarioEventTypes.WallSplat,
            TechAnimation when ShouldCheckAirTech => ScenarioEventTypes.Tech,
            CrouchBlockingAnimation or StandBlockingAnimation or HighBlockingAnimation when ShouldCheckStartBlocking => ScenarioEventTypes.Blocking,
            _ => ScenarioEventTypes.None
        };
    }
    
}