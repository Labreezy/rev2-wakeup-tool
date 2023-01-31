using GGXrdReversalTool.Library.Memory;

namespace GGXrdReversalTool.Library.Scenarios.Event.Implementations;

public class AnimationEvent : IScenarioEvent
{
    private const string FaceDownAnimation = "CmnActFDown2Stand";
    private const string FaceUpAnimation = "CmnActBDown2Stand";
    private const string WallSplatAnimation = "CmnActWallHaritsukiGetUp";
    private const string TechAnimation = "CmnActUkemi";

    public IMemoryReader MemoryReader { get; set; }


    private string _oldAnimation = "";
    public ScenarioEventTypes CheckEvent()
    {
        //TODO Implement
        
        var animationString = MemoryReader.ReadAnimationString(2);

        var shouldInvoke = animationString is FaceDownAnimation or FaceUpAnimation or WallSplatAnimation &&
                           _oldAnimation != animationString;

        _oldAnimation = animationString;
        // if (shouldInvoke)
        // {
        //     Occured?.Invoke(ScenarioEventTypes.KDFaceDown);
        // }


        if (animationString is FaceDownAnimation)
        {
            return ScenarioEventTypes.KDFaceDown;
        }

        if (animationString is FaceUpAnimation)
        {
            return ScenarioEventTypes.KDFaceUp;
        }

        if (animationString is WallSplatAnimation)
        {
            return ScenarioEventTypes.WallSplat;
        }

        if (animationString is TechAnimation)
        {
            return ScenarioEventTypes.Tech;
        }

        return ScenarioEventTypes.None;


    }
    
}