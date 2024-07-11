﻿using LDtk;
using LDtk.Renderer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace HyperDigger
{
    class TilemapLevel
    {
        public TilemapWorld World { get; private set; }
        LDtkLevel Level;
        TilemapRenderer Renderer;
        Sprite[] Layers = new Sprite[0];
        List<GameObject> Entities = new List<GameObject>();

        public string Identifier { get { return Level.Identifier; } }

        public TilemapLevel(TilemapWorld world, LDtkLevel level, TilemapRenderer renderer) {
            World = world;
            Level = level;
            Renderer = renderer;
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
            }
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
        }

        public bool CheckIfInBoundaries(int x, int y)
        {
            var cx = Level.WorldX; 
            var cy = Level.WorldY;
            var cw = cx + Level.PxWid;
            var ch = cy + Level.PxHei;

            return (x >= cx && y > cy) && (x < cw && y < ch);
        }

        internal bool CheckCollisionWithEntities(int cx, int cy, Collider other)
        {
            foreach (var entity in Entities)
            {
                if (entity == other.Parent) continue;
                if (entity is PhysicsBody)
                {
                    var collider = entity as PhysicsBody;
                    if (collider.CollidesWith(cx, cy, other)) return true;
                }
            }
            return false;
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
                    }
                }
            }
            return false;
        }

        private int GetTileId(LayerInstance layer, int localX, int localY)
        {
            int tileX = localX / layer._GridSize;
            int tileY = localY / layer._GridSize;
            int mapWidth = Level.PxWid / layer._GridSize;
            int tileIdx = tileY * mapWidth + tileX;

            //System.Console.WriteLine(string.Format("tileX:{0} tileY:{1} mapWidth:{2} tileIdx:{3}", tileX, tileY, mapWidth, tileIdx));

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
                entity.Update(gameTime);
            }
#if DEBUG
            if (Globals.Input.IsTriggered(Input.Button.ESCAPE))
            {
                var n = Globals.Input.GetDir8();
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
    }
}
