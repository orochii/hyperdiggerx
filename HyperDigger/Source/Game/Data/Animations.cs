using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace HyperDigger
{
    class Animations
    {
        public enum EAnimType { IDLE, WALK, JUMP, FALL }

        public class AnimEvent
        {
            public string name;
            public float volume;
            public float pitch;

            public AnimEvent(string name, float volume, float pitch)
            {
                this.name = name;
                this.volume = volume;
                this.pitch = pitch;
            }
        }
        public class AnimState {
            public List<Point> frames;
            public Dictionary<int, AnimEvent> events;
            public int framesPerSecond;

            public AnimState(List<Point> frames, int framesPerSecond)
            {
                this.frames = frames;
                this.framesPerSecond = framesPerSecond;
                this.events = new Dictionary<int, AnimEvent>();
            }
            public AnimState(List<Point> frames, int framesPerSecond, Dictionary<int, AnimEvent> events)
            {
                this.frames = frames;
                this.framesPerSecond = framesPerSecond;
                this.events = events;
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
            ari.entries.Add(EAnimType.IDLE, 
                new AnimState(new List<Point> { new Point(0,0), new Point(1, 0), new Point(2, 0), new Point(3, 0), new Point(4, 0), new Point(5, 0) }, 
                8));
            ari.entries.Add(EAnimType.WALK, 
                new AnimState(new List<Point> { new Point(0,1), new Point(1,1), new Point(2,1), new Point(3,1), new Point(4,1), new Point(5,1) }, 
                16, 
                new Dictionary<int, AnimEvent>() { {0, new AnimEvent("step.ogg",0.15f,1f) },{3, new AnimEvent("step.ogg", 0.20f, .8f) } }));
            ari.entries.Add(EAnimType.JUMP, 
                new AnimState(new List<Point> { new Point(2, 1) }, 
                1));
            ari.entries.Add(EAnimType.FALL, 
                new AnimState(new List<Point> { new Point(0, 1) }, 
                1));
        }

        internal AnimEntry Get(string v)
        {
            animations.TryGetValue(v, out var entry);
            return entry;
        }
    }
}
