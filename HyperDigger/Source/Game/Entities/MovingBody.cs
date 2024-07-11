using Microsoft.Xna.Framework;
using SharpFont.TrueType;
using System;

namespace HyperDigger
{
    internal class MovingBody : PhysicsBody
    {
        public const float GRAVITY = 20f;
        const float MAX_YSPEED = 10f;

        public float GravityScale = 1.0f;
        public Vector2 Speed;

        protected bool IsGrounded = false;

        public MovingBody(Container container) : base(container) { }

        public void ApplyGravity(float d)
        {
            Speed.Y += d * GRAVITY * GravityScale;
        }

        public override void Update(GameTime gameTime)
        {
            float d = (float)gameTime.ElapsedGameTime.TotalSeconds;

            ApplyGravity(d);

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
        }

        public bool IsColliding(int tx, int ty)
        {
            if (World == null) return false;

            return World.CheckCollisionWithWorld(tx, ty, Collider);
        }

    }
}
