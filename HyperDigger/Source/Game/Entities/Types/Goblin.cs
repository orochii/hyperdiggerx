using Microsoft.Xna.Framework;

namespace HyperDigger.Entities;

class Goblin : MovingBody
{
    public Goblin(Container container, TilemapWorld world, LDtk.EntityInstance entity) : base(container)
    {
        World = world;
        Vector2 position = new Vector2((float)entity._WorldX + 8, (float)entity._WorldY);
        Position = position;

        Texture = Global.Cache.LoadTexture("Graphics/Character/troll");
        SourceRect = new Rectangle(0, 0, 32, 32);
        Depth = 1;
        Name = "Goblin";
        Origin = new Vector2(16, 32 + 1);
        Collider.BoundarySize = new Vector2(6, 10);
        Collider.Offset = new Vector2(0, -10);

        System.Console.WriteLine("{0} : pos {1}", Name, Position);
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        System.Console.WriteLine("{0} : pos {1}", Name, Position);
    }
}
