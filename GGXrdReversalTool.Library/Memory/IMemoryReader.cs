using GGXrdReversalTool.Library.Models;
using GGXrdReversalTool.Library.Models.Inputs;

namespace GGXrdReversalTool.Library.Memory;

public interface IMemoryReader
{
    string ReadAnimationString(int player);
    int FrameCount();
    NameWakeupData GetCurrentDummy();
    bool WriteInputInSlot(int slotNumber, SlotInput slotInput);
    int GetComboCount(int player);
}