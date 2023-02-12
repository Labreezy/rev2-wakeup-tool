using GGXrdReversalTool.Library.Input;

namespace GGXrdReversalTool.Library.Replay.Implementations;

public class AsmInjectionReplayTrigger : IReplayTrigger
{
    public void Trigger()
    {
        //TODO Implement
        InputManager.SendKey(DirectXKeyStrokes.DIK_N, false, InputType.Keyboard);
        Thread.Sleep(10);
        InputManager.SendKey(DirectXKeyStrokes.DIK_N, true, InputType.Keyboard);
    }
}