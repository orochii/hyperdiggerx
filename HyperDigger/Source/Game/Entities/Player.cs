using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.ComponentModel.Design;
using System.Xml.Linq;
using static HyperDigger.Animations;

namespace HyperDigger
{
    internal class Player : Sprite, IEntities
    {
        const float GRAVITY = 20f;
        const float WALK_SPEED = 2f;
        const float MAX_YSPEED = 10f;
        const float WALK_ACCEL = 20f;
        const float DEACCEL = 5f;
        const float JUMP_FORCE = 6f;

        public TilemapWorld World;
        Vector2 BoundarySize;
        Vector2 Speed;
        bool IsGrounded = false;

        Animations.AnimEntry animEntry;
        Animations.EAnimType moveState = Animations.EAnimType.IDLE;
        Animations.EAnimType overlayState = Animations.EAnimType.IDLE;
        float animFrame = 0;

        public Player(Viewport viewport) : base(viewport) {
            Depth = 1;
            Name = "Ari";
            BoundarySize = new Vector2(1,7);
            //Position = new Vector2(Globals.Graphics.Width / 2, Globals.Graphics.Height / 2);
            Texture = Globals.Cache.content.Load<Texture2D>("Graphics/Character/ari");
            animEntry = Globals.Database.Animations.Get("ari");
            SourceRect = new Rectangle(0, 0, animEntry.frameWidth, animEntry.frameHeight);
            Origin = new Vector2(SourceRect.Width / 2, SourceRect.Height);
        }

        public override void Update(GameTime gameTime)
        {
            float d = (float)gameTime.ElapsedGameTime.TotalSeconds;
            // Read input
            var horz = Globals.Input.GetHorz();
            var jump = Globals.Input.IsTriggered(Input.Button.JUMP);
            var prevState = GetCurrentAnimState();
            // Apply gravity
            Speed.Y += d * GRAVITY;
            // Apply movement
            if (IsGrounded && jump)
            {
                Speed.Y = -JUMP_FORCE;
            }
            if (horz != 0)
            {
                Speed.X = HyperMath.MoveTowards(Speed.X, horz * WALK_SPEED, WALK_ACCEL * d);
                moveState = Animations.EAnimType.WALK;
            } else
            {
                Speed.X = HyperMath.MoveTowards(Speed.X, 0, DEACCEL * d);
                moveState = Animations.EAnimType.IDLE;
            }
            if (!IsGrounded)
            {
                if (Speed.Y < 0) moveState = Animations.EAnimType.JUMP;
                else moveState = Animations.EAnimType.FALL;
            }
            Speed.Y = Math.Clamp(Speed.Y, -MAX_YSPEED, MAX_YSPEED);

            // Check collisions
            IsGrounded = false;
            Vector2 NextPosition = Position + Speed;
            while (IsColliding((int)Position.X, (int)NextPosition.Y))
            {
                //NextPosition.X = HyperMath.MoveTowards(NextPosition.X, Position.X, 1);
                if (NextPosition.Y == Position.Y)
                {
                    Position.Y -= 1;
                    NextPosition.Y = Position.Y;
                } else
                {
                    NextPosition.Y = HyperMath.MoveTowards(NextPosition.Y, Position.Y, 1);
                }
                Speed.Y = 0;
                IsGrounded = true;
            }
            while (IsColliding((int)NextPosition.X, (int)Position.Y))
            {
                //NextPosition.X = HyperMath.MoveTowards(NextPosition.X, Position.X, 1);
                NextPosition.X = HyperMath.MoveTowards(NextPosition.X, Position.X, 1);
                Speed.X = 0;
            }
            // Apply
            Position.X = NextPosition.X;
            Position.Y = NextPosition.Y;
            //
            if (prevState != GetCurrentAnimState()) animFrame = 0;
            // Update animation
            AdvanceAnimation(horz, d);
        }

        private Animations.EAnimType GetCurrentAnimState()
        {
            if (overlayState != Animations.EAnimType.IDLE) return overlayState;
            return moveState;
        }

        private void AdvanceAnimation(float horz, float delta)
        {
            if (horz != 0)
            {
                if (horz < 0) Effects = SpriteEffects.FlipHorizontally;
                else Effects = SpriteEffects.None;
            }
            // 
            var curr = GetCurrentAnimState();
            var state = animEntry.Get(curr);
            animFrame += (state.framesPerSecond * delta);
            while (animFrame >= state.frames.Count)
            {
                animFrame -= state.frames.Count;
            }
            var currFrame = state.frames[(int)animFrame];
            SourceRect.X = currFrame.X * animEntry.frameWidth;
            SourceRect.Y = currFrame.Y * animEntry.frameWidth;
        }

        public bool IsColliding(int tx, int ty)
        {
            if (World == null) return false;

            return World.CheckCollisionWithWorld(tx, ty, BoundarySize);
        }

        public bool CollidesWith(int x, int y, Vector2 boundarySize)
        {
            float otherSizeX = boundarySize.X * IEntities.BOUND_UNIT;
            float otherSizeY = boundarySize.Y * IEntities.BOUND_UNIT / 2;
            float xOther = x;
            float yOther = y - otherSizeY;
            float thisSizeX = BoundarySize.X * IEntities.BOUND_UNIT;
            float thisSizeY = BoundarySize.Y * IEntities.BOUND_UNIT / 2;
            float xThis = Position.X;
            float yThis = Position.Y - thisSizeY;
            float deltaX = xOther - xThis;
            float deltaY = yOther - yThis;
            return (deltaX < otherSizeX+thisSizeX) || (deltaY < otherSizeY + thisSizeY);
        }

        public void DoUpdate(GameTime gameTime)
        {
            Update(gameTime);
        }
    }
}
