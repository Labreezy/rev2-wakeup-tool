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
            CharacterMoves = new[]
            {
                new CharacterMove { Name = "Blitz High", Input = "5SH" },
                new CharacterMove { Name = "Blitz Low", Input = "2SH" }
            }
        },
        new Preset
        {
            Character = Character.Sol,
            CharacterMoves = new[]
            {
                new CharacterMove { Name = "Gun Flame", Input = "2,3,6P" },
                new CharacterMove { Name = "Gun Flame (Feint)", Input = "2,1,4P" },
                new CharacterMove { Name = "Volcanic Viper S", Input = "6,2,3S" },
                new CharacterMove { Name = "Volcanic Viper H", Input = "6,2,3H" }
            }
        },
        new Preset
        {
            Character = Character.Ky,
            CharacterMoves = new[]
            {
                new CharacterMove { Name = "Stun Edge", Input = "2,3,6S" },
                new CharacterMove { Name = "Charged Stun Edge", Input = "2,3,6H" }
            }
        },
        new Preset
        {
            Character = Character.May
        },
        new Preset
        {
            Character = Character.Millia
        },
        new Preset
        {
            Character = Character.Zato
        },
        new Preset
        {
            Character = Character.Potemkin
        },
        new Preset
        {
            Character = Character.Chipp
        },
        new Preset
        {
            Character = Character.Faust
        },
        new Preset
        {
            Character = Character.Axl
        },
        new Preset
        {
            Character = Character.Venom
        },
        new Preset
        {
            Character = Character.Slayer
        },
        new Preset
        {
            Character = Character.I_No
        },
        new Preset
        {
            Character = Character.Bedman
        },
        new Preset
        {
            Character = Character.Ramlethal
        },
        new Preset
        {
            Character = Character.Sin
        },
        new Preset
        {
            Character = Character.Elphelt
        },
        new Preset
        {
            Character = Character.Leo
        },
        new Preset
        {
            Character = Character.Johnny
        },
        new Preset
        {
            Character = Character.Jack_O
        },
        new Preset
        {
            Character = Character.Jam
        },
        new Preset
        {
            Character = Character.Haehyun,
            CharacterMoves = new[]
            {
                new CharacterMove { Name = "Blue Tuning Ball", Input = "2,3,6S" },
                new CharacterMove { Name = "Red Tuning Ball", Input = "2,3,6H" },
                new CharacterMove { Name = "Four Tigers Sword", Input = "6,2,3K" },
                new CharacterMove { Name = "Four Tigers Sword [Max]", Input = "6,2,3K,5K*56" },
                new CharacterMove { Name = "Four Tigers Sword (Reverse)", Input = "6,2,3K,4K*6" },
                new CharacterMove { Name = "Four Tigers Sword (Reverse) [Max]", Input = "6,2,3K,4K*19" },
                new CharacterMove { Name = "Blue Shi-Shiinken", Input = "6,2,3k*10,5*7,4k*12" },
                new CharacterMove { Name = "Red Shi-Shiinken", Input = "6,2,3k*10,5*7,4k*25" },
                new CharacterMove { Name = "Falcon Dive", Input = "2,1,4K" },
                new CharacterMove { Name = "Falcon Dive [Max]", Input = "2,1,4K" },
                new CharacterMove { Name = "Falcon Dive (Reverse)", Input = "2,1,4K*7" },
                new CharacterMove { Name = "Falcon Dive (Reverse) [Max]", Input = "2,1,4K*7,5k*59" },
                new CharacterMove { Name = "Air Falcon Dive", Input = "2,1,4,7*5,7k" },
                new CharacterMove { Name = "Enlightened 3000 Palm Strike", Input = "2,3,6,2,3,6H" },
                new CharacterMove { Name = "Enlightened 3000 Palm Strike [Max]", Input = "2,3,6,2,3,6H,5*80,5H*195" },
                new CharacterMove { Name = "Celestial Tuning Ball", Input = "2,3,6,2,3,6S" }
            }
        },
        new Preset
        {
            Character = Character.Raven
        },
        new Preset
        {
            Character = Character.Dizzy
        },
        new Preset
        {
            Character = Character.Baiken
        },
        new Preset
        {
            Character = Character.Answer
        }
    };
}

