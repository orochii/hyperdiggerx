using LDtk;
using LDtk.Renderer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace HyperDigger
{
    public class TilemapLevel
    {
        public TilemapWorld World { get; private set; }
        LDtkLevel Level;
        TilemapRenderer Renderer;
        Sprite[] Layers = new Sprite[0];
        List<GameObject> Entities = new List<GameObject>();
        List<EntityInstance> EntityInstances = new List<EntityInstance>();
        Texture2D _minimap;
        List<GameObject> _toDestroy = new List<GameObject>();
        public string DisplayName {get;private set;}

        bool IsActive;
        Color MMAP_EMPTY = new Color(32, 32, 32);
        Color MMAP_BLOCK = new Color(64, 64, 64);
        Color MMAP_WATER = new Color(16, 32, 96);

        public string Identifier { get { return Level.Identifier; } }

        public TilemapLevel(TilemapWorld world, LDtkLevel level, TilemapRenderer renderer) {
            World = world;
            Level = level;
            Renderer = renderer;
            DisplayName = "";
            foreach (var f in Level.FieldInstances)
            {
                switch (f._Identifier)
                {
                    case "DisplayName":
                        if (f._Type=="String")
                        {
                            DisplayName = f._Value.ToString();
                        }
                        break;
                }
            }
            System.Console.WriteLine(DisplayName);
            //Level.LayerInstances[0]._Identifier;
            Layers = new Sprite[Level.LayerInstances.Length];
            //l.Layers;
            for (int i = 0; i < Level.LayerInstances.Length; i++)
            {
                var layer = Level.LayerInstances[i];
                var sprite = new Sprite(null);
                sprite.Name = layer._Identifier;
                sprite.Position.X = Level.WorldX;
                sprite.Position.Y = Level.WorldY;
                var layerParams = layer._Identifier.Split('_');
                for (int j = 1; j < layerParams.Length; j++)
                {
                    string p = layerParams[j];
                    char pID = p[0];
                    switch (pID)
                    {
                        case 'Z':
                            if (p.Length >= 3 && p[1]=='m')
                            {
                                int.TryParse(p.Substring(2, p.Length - 2), out int value);
                                sprite.Depth = -value;
                            } else if (p.Length >= 2)
                            {
                                int.TryParse(p.Substring(1, p.Length - 1), out int value);
                                sprite.Depth = value;
                            }
                            break;
                    }
                }
                Layers[i] = sprite;
                // Register entities
                if (layer._Type == LayerType.Entities)
                {
                    EntityInstances.AddRange(layer.EntityInstances);
                }
            }
            GenerateMinimap();
        }

        public void Set(Container v)
        {
            // Prerender
            var l = Renderer.PrerenderLevel(Level);
            for (int i = 0; i < l.Layers.Length; i++)
            {
                Layers[i].Texture = l.Layers[i];
                Layers[i].Container = v;
            }
            bool newState = (v != null);
            // Set or destroy entities
            if (newState == IsActive) return;
            if (newState) // Create
            {
                foreach (var e in EntityInstances)
                {
                    if (e._Identifier == "EntitySpawn")
                    {
                        var spawn = EntityFactory.CreateEntity(v, World, e);
                        if (spawn != null) Entities.Add(spawn);
                    }
                }
            }
            else // Destroy
            {
                foreach (var e in Entities)
                {
                    e.Container = null;
                }
                Entities.Clear();
            }
            IsActive = newState;
        }

        public bool CheckIfInBoundaries(int x, int y)
        {
            var cx = Level.WorldX; 
            var cy = Level.WorldY;
            var cw = cx + Level.PxWid;
            var ch = cy + Level.PxHei;

            return (x >= cx && y > cy) && (x < cw && y < ch);
        }

        public List<GameObject> CheckCollisionWithAllEntities(int cx, int cy, Collider other)
        {
            var ary = new List<GameObject>();

            foreach (var entity in Entities)
            {
                if (entity == other.Parent) continue;
                if (entity is PhysicsBody)
                {
                    var collider = entity as PhysicsBody;
                    if (collider.CollidesWith(cx, cy, other)) ary.Add(entity);
                }
            }

            return ary;
        }
        internal GameObject CheckCollisionWithEntities(int cx, int cy, Collider other)
        {
            foreach (var entity in Entities)
            {
                if (entity == other.Parent) continue;
                if (entity is PhysicsBody)
                {
                    var collider = entity as PhysicsBody;
                    if (collider.CollidesWith(cx, cy, other)) return entity;
                }
            }
            return null;
        }

        public bool CheckCollisionWithTileAtPosition(int x, int y)
        {
            int localX = x - Level.WorldX;
            int localY = y - Level.WorldY;

            foreach (var layer in Level.LayerInstances)
            {
                if (layer._Type == LayerType.IntGrid)
                {
                    var tileId = GetTileId(layer, localX, localY);

                    switch (tileId)
                    {
                        case 1: // GROUND
                        case 3: // WALL
                            return true;
                        case 5: // SLOPE
                            var d = GetTileId(layer, localX, localY + 16);
                            if (d == 1) // Ground
                            {
                                var l = GetTileId(layer, localX - 16, localY);
                                var r = GetTileId(layer, localX + 16, localY);

                                if (l == 1 && r == 5) // 22.5deg Up-Down LeftSide
                                {
                                    return CheckSlope225(localX, localY, 0);
                                }
                                if (l == 5 && r == 1) // 22.5deg Down-Up RightSide
                                {
                                    return CheckSlope225(localX, localY, 0, true);
                                }
                                if (l == 5 && r <= 0) // 22.5deg Up-Down RightSide
                                {
                                    return CheckSlope225(localX, localY, 8);
                                }
                                if (l <= 0 && r == 5) // 22.5deg Down-Up LeftSide
                                {
                                    return CheckSlope225(localX, localY, 8, true);
                                }
                                if (l == 1 && r != 1) // 45deg Up-Down
                                {
                                    return CheckSlope45(localX, localY);
                                }
                                if (l != 1 && r == 1) // 45deg Dn-Up
                                {
                                    return CheckSlope45(localX, localY, true);
                                }
                            }
                            
                            return false;
                    }
                }
            }
            return false;
        }

        private bool CheckSlope45(int localX, int localY, bool invertX=false)
        {
            int x = localX % 16;
            int y = localY % 16;
            if (invertX) x = 16 - x;
            return y > x;
        }
        private bool CheckSlope225(int localX, int localY, int offY, bool invertX = false)
        {
            int x = localX % 16;
            int y = localY % 16;
            if (invertX) x = 16 - x;
            return (y - offY) > x / 2;
        }

        private int GetTileId(LayerInstance layer, int localX, int localY)
        {
            // Return 0 if off bounds
            if (localX < 0 || localY < 0) return -1;
            if (localX >= Level.PxWid || localY >= Level.PxHei) return -1;
            // Transform to tile coordinates
            int tileX = localX / layer._GridSize;
            int tileY = localY / layer._GridSize;
            int mapWidth = Level.PxWid / layer._GridSize;
            int tileIdx = tileY * mapWidth + tileX;
            // If off bounds of the array.
            if (tileIdx < 0 || tileIdx >= layer.IntGridCsv.Length)
            {
                return -1;
            }
            return layer.IntGridCsv[tileIdx];
        }

        internal Rectangle GetBoundaries()
        {
            return new Rectangle(Level.WorldX, Level.WorldY, Level.PxWid, Level.PxHei);
        }

        internal void Update(GameTime gameTime)
        {
            foreach(var entity in Entities)
            {
                if (entity.Destroyed)
                {
                    _toDestroy.Add(entity);
                }
                else
                {
                    entity.Update(gameTime);
                }
            }
            foreach(var d in _toDestroy) Entities.Remove(d);
            _toDestroy.Clear();
#if DEBUG
            if (Global.Input.IsTriggered(Input.Button.ESCAPE))
            {
                var n = Global.Input.GetDir8();
                if (n == 0) n = 5;
                n -= 1;
                if (n < Layers.Length)
                {
                    Layers[n].Visible = !Layers[n].Visible;
                    System.Console.WriteLine("Layer {0} set to {1} - {2}", Layers[n].Name, Layers[n].Visible, Layers[n].Texture);
                }
            }
#endif
        }

        internal int[] GetTileIdAt(Vector2 worldPos)
        {
            int localX = (int)worldPos.X - Level.WorldX;
            int localY = (int)worldPos.Y - Level.WorldY;

            int[] ids = new int[Level.LayerInstances.Length];
            for (int i = 0; i < Level.LayerInstances.Length; i++)
            {
                ids[i] = GetTileId(Level.LayerInstances[i], localX, localY);
            }
            return ids;
        }

        private void GenerateMinimap()
        {
            _minimap = new Texture2D(Global.Graphics.SpriteBatch.GraphicsDevice, Level.Size.X / 16, Level.Size.Y / 16);
            Color[] _data = new Color[_minimap.Width * _minimap.Height];
            _minimap.GetData(_data);

            for (int i = 0; i < _data.Length; i++)
            {
                bool _drawn = false;
                foreach (var l in Level.LayerInstances)
                {
                    if (l._Type == LayerType.IntGrid)
                    {
                        if (i < l.IntGridCsv.Length)
                        {
                            var t = l.IntGridCsv[i];
                            switch (t)
                            {
                                case 1:
                                case 3:
                                    _data[i] = MMAP_BLOCK;
                                    _drawn = true;
                                    break;
                                case 2:
                                    _data[i] = MMAP_WATER;
                                    _drawn = true;
                                    break;
                            }
                        }
                    }
                }
                if (!_drawn) _data[i] = MMAP_EMPTY;
            }
            _minimap.SetData(_data);
        }

        public Texture2D GetMinimap() { return _minimap; }

        internal Vector2 GetPosition()
        {
            return new Vector2(Level.WorldX, Level.WorldY);
        }
    }
}
