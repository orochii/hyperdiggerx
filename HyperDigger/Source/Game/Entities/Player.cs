using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Runtime.CompilerServices;

namespace HyperDigger
{
    internal class Player : MovingBody
    {
        /*
         * JUMP TODO:
         * Anti-gravity Apex --DONE
         * Jump Buffering --DONE
         * Clamp Fall Speed --DONE
         * Coyote Time --DONE
         * Early Fall --DONE
         * Sticky Feet on Land
         * Speedy Apex
         * Catch Missed Jump
         * Bumped Head on Corner
         */

        const float WALK_SPEED = 2f;
        const float WALK_ACCEL = 20f;
        const float DEACCEL = 15f;
        const float JUMP_FORCE = 6f;
        const float COYOTE_TIME = 0.1f;
        const float APEX_RANGE = 1.0f;
        const float APEX_MULTIPLIER = 0.6f;
        private const float JUMP_BUFFER_TIME = 0.1f;

        bool IsJumping = false;
        float jumpRequest = 0;
        float coyoteTime = 0;

        Animations.AnimEntry animEntry;
        Animations.EAnimType moveState = Animations.EAnimType.IDLE;
        Animations.EAnimType overlayState = Animations.EAnimType.IDLE;
        float animFrame = 0;

        public Player(Container container) : base(container) {
            Texture = Globals.Cache.LoadTexture("Graphics/Character/ari");
            animEntry = Globals.Database.Animations.Get("ari");
            Depth = 1;
            Name = "Ari";
            SourceRect = new Rectangle(0, 0, animEntry.frameWidth, animEntry.frameHeight);
            Origin = new Vector2(animEntry.frameWidth / 2, animEntry.frameHeight - 1);
            Collider.BoundarySize = new Vector2(4,14);
            Collider.Offset = new Vector2(0,-14);
        }

        public override void Update(GameTime gameTime)
        {
            float d = (float)gameTime.ElapsedGameTime.TotalSeconds;
            // Read input
            var horz = Globals.Input.GetHorz();
            var jump = Globals.Input.IsTriggered(Input.Button.JUMP);
            var jumpHeld = Globals.Input.IsPressed(Input.Button.JUMP);
            if (jump) jumpRequest = JUMP_BUFFER_TIME;
            var prevState = GetCurrentAnimState();
            // Advance/reset coyote time
            if (IsGrounded) coyoteTime = COYOTE_TIME;
            else if (coyoteTime > 0) coyoteTime -= d;
            
            // Apply movement
            if (coyoteTime>0 && jumpRequest>0)
            {
                Speed.Y = -JUMP_FORCE;
                coyoteTime = 0;
                jumpRequest = 0;
                IsJumping = true;
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
                if (Speed.Y < 0)
                {
                    moveState = Animations.EAnimType.JUMP;
                    if (IsJumping && !jumpHeld)
                    {
                        IsJumping = false;
                        Speed.Y = 0;
                    }
                }
                else
                {
                    moveState = Animations.EAnimType.FALL;
                    IsJumping = false;
                }
            }
            UpdateGravity();
            //
            base.Update(gameTime);
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

        public void UpdateGravity()
        {
            // Apply gravity
            if (!IsGrounded && Math.Abs(Speed.Y) < APEX_RANGE)
            {
                GravityScale = APEX_MULTIPLIER;
            }
            else GravityScale = 1f;
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

        public override void Draw()
        {
            base.Draw();
            // Draw collider
            if (Collider != null)
            {
                var sx = Collider.OffsetX(GlobalPosition.X) - Collider.BoundarySize.X;
                var sy = Collider.OffsetY(GlobalPosition.Y) - Collider.BoundarySize.Y;
                var w = Collider.BoundarySize.X * 2;
                var h = Collider.BoundarySize.Y * 2;
                var rect = new Rectangle((int)sx, (int)sy, (int)w, (int)h);
                Globals.Graphics.DrawRectangle(rect, new Color(64, 160, 224, 64));
            }
            // Draw origin
            Globals.Graphics.DrawDot(GlobalPosition, Color.IndianRed);
        }
    }
}
