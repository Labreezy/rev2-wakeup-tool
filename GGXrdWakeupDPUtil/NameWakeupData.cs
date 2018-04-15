using System;

namespace GGXrdWakeupDPUtil
{
    public class NameWakeupData : IEquatable<NameWakeupData>
    {
        public String CharName { get; }
        public int FaceUpFrames { get; }
        public int FaceDownFrames { get; }

        public NameWakeupData(string charName, int faceUpFrames, int faceDownFrames)
        {
            CharName = charName;
            FaceUpFrames = faceUpFrames;
            FaceDownFrames = faceDownFrames;
        }

        public bool Equals(NameWakeupData other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }
            if (ReferenceEquals(this, other))
            {
                return true;
            }
            return string.Equals(CharName, other.CharName) &&
                   FaceUpFrames == other.FaceUpFrames &&
                   FaceDownFrames == other.FaceDownFrames;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }
            if (obj.GetType() != this.GetType())
            {
                return false;
            }
            return Equals((NameWakeupData)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (CharName != null ? CharName.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ FaceUpFrames;
                hashCode = (hashCode * 397) ^ FaceDownFrames;
                return hashCode;
            }
        }
    }
}
