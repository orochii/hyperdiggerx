using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Runtime.CompilerServices;

namespace HyperDigger
{
    internal class Player : MovingBody, IDamageable
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

        const float WALK_SPEED = 128f;
        const float WALK_ACCEL = 880f;
        const float DEACCEL = 760f;
        const float JUMP_FORCE = 320f;
        const float COYOTE_TIME = 0.1f;
        const float APEX_RANGE = 32.0f;
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
            Texture = Global.Cache.LoadTexture("Graphics/Character/ari");
            animEntry = Global.Database.Animations.Get("ari");
            Depth = 1;
            Name = "Ari";
            SourceRect = new Rectangle(0, 0, animEntry.frameWidth, animEntry.frameHeight);
            Origin = new Vector2(animEntry.frameWidth / 2, animEntry.frameHeight + 1);
            Collider.BoundarySize = new Vector2(4,14);
            Collider.Offset = new Vector2(0,-14);
        }

        public override void Update(GameTime gameTime)
        {
            float d = (float)gameTime.ElapsedGameTime.TotalSeconds;
            
            // Read input
            var horz = Global.Input.GetHorz();
            var jump = Global.Input.IsTriggered(Input.Button.JUMP);
            var jumpHeld = Global.Input.IsPressed(Input.Button.JUMP);
            if (jump) jumpRequest = JUMP_BUFFER_TIME;
            var prevState = GetCurrentAnimState();

            if (Global.Input.IsTriggered(Input.Button.SKILL_L)) UseCard(0);
            else if (Global.Input.IsTriggered(Input.Button.SKILL_U)) UseCard(1);
            else if (Global.Input.IsTriggered(Input.Button.SKILL_D)) UseCard(2);
            else if (Global.Input.IsTriggered(Input.Button.SKILL_R)) UseCard(3);

            if (Global.Input.IsTriggered(Input.Button.DRAW))
            {
                // Draw.
                Global.State.Deck.PullCard();
            }

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
                    Global.Audio.PlaySFXAt(ev.name, ev.volume, ev.pitch, GlobalPosition);
                }
            }
        }

        private void UseCard(int idx)
        {
            var hand = Global.State.Deck.Hand;
            if (hand.Count > idx)
            {
                var cardIdx = hand[idx];
                Global.State.Deck.Mill(idx);

                Card card = Global.Database.Cards.Get(cardIdx);
                if (card != null)
                {
                    int dir = (Effects == SpriteEffects.FlipHorizontally) ? -1 : 1;
                    Vector2 position = Position + new Vector2(card.Offset.X * dir, card.Offset.Y);
                    var c = EntityFactory.CreateCardEntity(card.EntityName, this, position);

                    Global.Audio.PlaySFXAt("step.ogg", 1, 1, GlobalPosition);
                }
            }
        }

        public void DoDamage(GameObject source, int damage)
        {
            System.Console.WriteLine("Player receives {0} damage", damage);
        }
    }
}
