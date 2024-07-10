using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace HyperDigger
{
    internal class Player : Sprite, ICollision
    {
        /*
         * JUMP TODO:
         * Anti-gravity Apex --DONE
         * Jump Buffering --DONE
         * Clamp Fall Speed --DONE
         * Coyote Time
         * Early Fall
         * Sticky Feet on Land
         * Speedy Apex
         * Catch Missed Jump
         * Bumped Head on Corner
         */

        const float GRAVITY = 20f;
        const float WALK_SPEED = 2f;
        const float MAX_YSPEED = 10f;
        const float WALK_ACCEL = 20f;
        const float DEACCEL = 15f;
        const float JUMP_FORCE = 6f;
        const float COYOTE_TIME = 0.1f;
        const float APEX_RANGE = 1.0f;
        const float APEX_MULTIPLIER = 0.6f;
        private const float JUMP_BUFFER_TIME = 0.1f;

        Vector2 BoundarySize;
        Vector2 Speed;
        bool IsGrounded = false;
        float jumpRequest = 0;
        float coyoteTime = 0;

        Animations.AnimEntry animEntry;
        Animations.EAnimType moveState = Animations.EAnimType.IDLE;
        Animations.EAnimType overlayState = Animations.EAnimType.IDLE;
        float animFrame = 0;

        public Player(Container container) : base(container) {
            Depth = 1;
            Name = "Ari";
            BoundarySize = new Vector2(1,7);
            //Position = new Vector2(Globals.Graphics.Width / 2, Globals.Graphics.Height / 2);
            Texture = Globals.Cache.LoadTexture("Graphics/Character/ari");
            animEntry = Globals.Database.Animations.Get("ari");
            SourceRect = new Rectangle(0, 0, animEntry.frameWidth, animEntry.frameHeight);
            Origin = new Vector2(SourceRect.Width / 2, SourceRect.Height-1);
        }

        public override void Update(GameTime gameTime)
        {
            float d = (float)gameTime.ElapsedGameTime.TotalSeconds;
            // Read input
            var horz = Globals.Input.GetHorz();
            var jump = Globals.Input.IsTriggered(Input.Button.JUMP);
            if (jump) jumpRequest = JUMP_BUFFER_TIME;
            var prevState = GetCurrentAnimState();
            // Advance/reset coyote time
            if (IsGrounded) coyoteTime = COYOTE_TIME;
            else if (coyoteTime > 0) coyoteTime -= d;
            // Apply gravity
            if (!IsGrounded && Math.Abs(Speed.Y) < APEX_RANGE)
            {
                Speed.Y += d * GRAVITY * APEX_MULTIPLIER;
            }
            else Speed.Y += d * GRAVITY;

            // Apply movement
            if (coyoteTime>0 && jumpRequest>0)
            {
                Speed.Y = -JUMP_FORCE;
                coyoteTime = 0;
                jumpRequest = 0;
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
            if (IsColliding((int)Position.X, (int)Position.Y))
            {
                Position.Y -= 1;
            }
            Vector2 NextPosition = Position + Speed;
            
            while (IsColliding((int)Position.X, (int)NextPosition.Y))
            {
                if (Speed.Y > 0) IsGrounded = true;
                Speed.Y = 0;
                
                NextPosition.Y = HyperMath.MoveTowards(NextPosition.Y, Position.Y, 1);
                if (NextPosition.Y == Position.Y) break;
            }
            while (IsColliding((int)NextPosition.X, (int)NextPosition.Y))
            {
                Speed.X = 0;

                NextPosition.X = HyperMath.MoveTowards(NextPosition.X, Position.X, 1);
                if (NextPosition.X == Position.X) break;
            }
            // Apply
            Position.X = NextPosition.X;
            Position.Y = NextPosition.Y;
            //
            if (prevState != GetCurrentAnimState())
            {
                animFrame = 0;
                _lastFrame = -1;
            }
            // Update animation
            AdvanceAnimation(horz, d);
            //
            jumpRequest -= d;
        }

        private Animations.EAnimType GetCurrentAnimState()
        {
            if (overlayState != Animations.EAnimType.IDLE) return overlayState;
            return moveState;
        }

        private int _lastFrame = 0;
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
            var floorAnimFrame = (int)animFrame;
            var currFrame = state.frames[floorAnimFrame];
            SourceRect.X = currFrame.X * animEntry.frameWidth;
            SourceRect.Y = currFrame.Y * animEntry.frameWidth;
            if (_lastFrame != floorAnimFrame)
            {
                _lastFrame = floorAnimFrame;
                if (state.events.TryGetValue(_lastFrame, out var ev))
                {
                    Globals.Audio.PlaySFXAt(ev.name, ev.volume, ev.pitch, GlobalPosition);
                    System.Console.WriteLine("Play sound {0}", ev.name);
                }
            }
        }

        public bool IsColliding(int tx, int ty)
        {
            if (World == null) return false;

            return World.CheckCollisionWithWorld(this, tx, ty, BoundarySize);
        }

        public bool CollidesWith(int x, int y, Vector2 boundarySize)
        {
            float otherSizeX = boundarySize.X * ICollision.BOUND_UNIT;
            float otherSizeY = boundarySize.Y * ICollision.BOUND_UNIT / 2;
            float xOther = x;
            float yOther = y - otherSizeY;
            float thisSizeX = BoundarySize.X * ICollision.BOUND_UNIT;
            float thisSizeY = BoundarySize.Y * ICollision.BOUND_UNIT / 2;
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
