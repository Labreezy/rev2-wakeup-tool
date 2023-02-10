using GGXrdReversalTool.Library.Characters;

namespace GGXrdReversalTool.Library.Presets;

public class Preset
{
    public Character? Character { get; set; }

    public IEnumerable<CharacterMove> CharacterMoves { get; private init; } = Enumerable.Empty<CharacterMove>();
    
    public static IEnumerable<Preset> Presets => new[]
    {
        new Preset
        {
            CharacterMoves = new []
            {
                new CharacterMove { Name = "Blitz High", Input = "5SH"},
                new CharacterMove { Name = "Blitz Low", Input = "2SH"}
            }
        },
        new Preset
        {
            Character = Character.Sol,
            CharacterMoves = new []
            {
                new CharacterMove { Name = "Gun Flame", Input = "2,3,6P"},
                new CharacterMove { Name = "Gun Flame (Feint)", Input = "2,1,4P"},
                new CharacterMove { Name = "Volcanic Viper S", Input = "6,2,3S"},
                new CharacterMove { Name = "Volcanic Viper H", Input = "6,2,3H"}
            }
        },
        new Preset
        {
            Character = Character.Ky,
            CharacterMoves = new []
            {
                new CharacterMove { Name = "Stun Edge", Input = "2,3,6S"},
                new CharacterMove { Name = "Charged Stun Edge", Input = "2,3,6H"}
            }
        }
    };
}

