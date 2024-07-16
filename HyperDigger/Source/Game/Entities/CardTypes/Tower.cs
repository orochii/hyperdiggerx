using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HyperDigger.CardTypes;

class Tower : StaticBody
{
    private GameObject Owner;
    private float Life = 5f;
    private float Speed = 64f;
    private int Damage = 1;

    private Collider Hurtbox;

    public Tower(Container container, GameObject owner, Vector2 position) : base(container)
    {
        Depth = 100;
        World = owner.World;
        Owner = owner;
        Position = position;
        Name = "Tower";
        Texture = Global.Cache.LoadTexture("Graphics/Effects/tower");
        Collider.BoundarySize = new Vector2(10,32);
        Origin = new Vector2(10,32);

        Hurtbox = new Collider(this);
        Hurtbox.BoundarySize = new Vector2(12,8);
        Hurtbox.Offset.Y = -32;

        World.AddEntity(this);
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        var d = (float)gameTime.ElapsedGameTime.TotalSeconds;

        Position.Y -= Speed * d;
        Speed = HyperMath.MoveTowards(Speed, 0, d * Speed);

        var dmgAry = World.CheckCollisionWithAllEntities((int)Position.X, (int)Position.Y, Hurtbox);
        if (dmgAry.Count > 0)
        {
            foreach (var dmg in dmgAry)
            {
                if (dmg == Owner) continue;
                if (dmg is IDamageable) (dmg as IDamageable).DoDamage(this, Damage);
            }
        }
        
        Life -= d;
        if (Life < 0) Destroyed = true;
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        base.Draw(spriteBatch);

        // Draw debug colliders.
        if (!Global.DebugDraw) return;
        // Draw collider
        if (Hurtbox != null)
        {
            var sx = Hurtbox.OffsetX(GlobalPosition.X) - Hurtbox.BoundarySize.X;
            var sy = Hurtbox.OffsetY(GlobalPosition.Y) - Hurtbox.BoundarySize.Y;
            var w = Hurtbox.BoundarySize.X * 2;
            var h = Hurtbox.BoundarySize.Y * 2;
            var rect = new Rectangle((int)sx, (int)sy, (int)w, (int)h);
            Global.Graphics.DrawRectangle(spriteBatch, rect, new Color(128, 8, 8, 64));
        }
    }
}
