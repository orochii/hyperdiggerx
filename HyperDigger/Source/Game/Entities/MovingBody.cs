using Microsoft.Xna.Framework;
using SharpFont.TrueType;
using System;

namespace HyperDigger
{
    internal class MovingBody : PhysicsBody
    {
        public const float GRAVITY = 980f;
        const float MAX_YSPEED = 480f;
        const int MAX_SLOPE = 4;

        public float GravityScale = 1.0f;
        public Vector2 Speed;

        protected bool IsGrounded = false;

        public MovingBody(Container container) : base(container) { }

        private void ApplyGravity(float d)
        {
            Speed.Y += d * GRAVITY * GravityScale;
        }

        public override void Update(GameTime gameTime)
        {
            float d = (float)gameTime.ElapsedGameTime.TotalSeconds;

            ApplyGravity(d);

            Speed.Y = Math.Clamp(Speed.Y, -MAX_YSPEED, MAX_YSPEED);

            // Check collisions
            bool _willBeGrounded = false;
            if (IsColliding((int)Position.X, (int)Position.Y))
            {
                Position.Y = (int)Position.Y;
                if (IsColliding((int)Position.X, (int)Position.Y)) Position.Y -= 1;
                _willBeGrounded = true;
            }
            Vector2 NextPosition = Position + (Speed * d);

            while (IsColliding((int)Position.X, (int)NextPosition.Y))
            {
                if (Speed.Y > 0) _willBeGrounded = true;
                Speed.Y = 0;

                NextPosition.Y = HyperMath.MoveTowards(NextPosition.Y, Position.Y, 1);
                if (NextPosition.Y == Position.Y) break;
            }
            // Slope support
            if (IsGrounded)
            {
                if (IsColliding((int)NextPosition.X, (int)NextPosition.Y))
                {
                    var maxSlope = 0;
                    while (maxSlope < MAX_SLOPE)
                    {
                        if (!IsColliding((int)NextPosition.X, (int)NextPosition.Y - maxSlope))
                        {
                            NextPosition.Y = NextPosition.Y - maxSlope;
                            if (Speed.Y > 0) _willBeGrounded = true;
                            Speed.Y = 0;
                            break;
                        }
                        else
                        {
                            maxSlope += 1;
                        }
                    }
                }
                else if (Speed.Y > 0)
                {
                    var maxSlope = MAX_SLOPE;
                    while (maxSlope > 0)
                    {
                        if (!IsColliding((int)NextPosition.X, (int)NextPosition.Y + maxSlope))
                        {
                            if (IsColliding((int)NextPosition.X, (int)NextPosition.Y + maxSlope + 1))
                            {
                                NextPosition.Y = NextPosition.Y + maxSlope;
                                if (Speed.Y > 0) _willBeGrounded = true;
                                Speed.Y = 0;
                            }
                            break;
                        }
                        else
                        {
                            maxSlope -= 1;
                        }
                    }
                }
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
            IsGrounded = _willBeGrounded;
        }

        public bool IsColliding(int tx, int ty)
        {
            if (World == null) return false;

            return World.CheckCollisionWithWorld(tx, ty, Collider);
        }

    }
}
