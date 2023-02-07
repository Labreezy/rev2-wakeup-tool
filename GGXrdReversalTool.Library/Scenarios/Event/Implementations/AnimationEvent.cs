using GGXrdReversalTool.Library.Memory;

namespace GGXrdReversalTool.Library.Scenarios.Event.Implementations;

public class AnimationEvent : IScenarioEvent
{
    private const string FaceDownAnimation = "CmnActFDown2Stand";
    private const string FaceUpAnimation = "CmnActBDown2Stand";
    private const string WallSplatAnimation = "CmnActWallHaritsukiGetUp";
    private const string TechAnimation = "CmnActUkemi";

    public IMemoryReader MemoryReader { get; set; }

    public bool ShouldCheckWakingUp { get; set; } = true;
    public bool ShouldCheckWallSplat { get; set; } = true;
    public bool ShouldCheckAirTech { get; set; } = false;


    private string _oldAnimation = "";
    public ScenarioEventTypes CheckEvent()
    {
        //TODO Implement
        
        var animationString = MemoryReader.ReadAnimationString(2);

        var shouldInvoke = animationString is FaceDownAnimation or FaceUpAnimation or WallSplatAnimation &&
                           _oldAnimation != animationString;

        _oldAnimation = animationString;


        if (animationString is FaceDownAnimation && ShouldCheckWakingUp)
        {
            return ScenarioEventTypes.KDFaceDown;
        }

        if (animationString is FaceUpAnimation && ShouldCheckWakingUp)
        {
            return ScenarioEventTypes.KDFaceUp;
        }

        if (animationString is WallSplatAnimation && ShouldCheckWallSplat)
        {
            return ScenarioEventTypes.WallSplat;
        }

        if (animationString is TechAnimation && ShouldCheckAirTech)
        {
            return ScenarioEventTypes.Tech;
        }

        return ScenarioEventTypes.None;


    }
    
}