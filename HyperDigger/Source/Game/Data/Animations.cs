using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace HyperDigger
{
    class Animations
    {
        public enum EAnimType { IDLE, WALK, JUMP, FALL }

        public class AnimState {
            public List<Point> frames;
            public int framesPerSecond;

            public AnimState(List<Point> frames, int framesPerSecond)
            {
                this.frames = frames;
                this.framesPerSecond = framesPerSecond;
            }
        }
        public class AnimEntry
        {
            public int frameWidth;
            public int frameHeight;
            public Dictionary<EAnimType, AnimState> entries = new Dictionary<EAnimType, AnimState>();

            public AnimState Get(EAnimType type)
            {
                if (entries.TryGetValue(type, out AnimState animState)) return animState;
                if (entries.TryGetValue(EAnimType.IDLE, out AnimState idleState)) return animState;
                return null;
            }
        }

        Dictionary<string, AnimEntry> animations = new Dictionary<string, AnimEntry>();

        public Animations() {
            var ari = new AnimEntry();
            animations.Add("ari", ari);
            ari.frameWidth = 32;
            ari.frameHeight = 32;
            ari.entries.Add(EAnimType.IDLE, new AnimState(new List<Point> { new Point(0,0), new Point(1, 0), new Point(2, 0), new Point(3, 0), new Point(4, 0), new Point(5, 0) }, 8));
            ari.entries.Add(EAnimType.WALK, new AnimState(new List<Point> { new Point(0,1), new Point(1,1), new Point(2,1), new Point(3,1), new Point(4,1), new Point(5,1) }, 8));
            ari.entries.Add(EAnimType.JUMP, new AnimState(new List<Point> { new Point(2, 1) }, 8));
            ari.entries.Add(EAnimType.FALL, new AnimState(new List<Point> { new Point(0, 1) }, 8));
        }

        internal AnimEntry Get(string v)
        {
            animations.TryGetValue(v, out var entry);
            return entry;
        }
    }
}
