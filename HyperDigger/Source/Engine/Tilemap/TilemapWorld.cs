using LDtk;
using LDtk.Renderer;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace HyperDigger
{
    internal class TilemapWorld
    {
        LDtkFile File;
        LDtkWorld World;
        TilemapRenderer Renderer;
        Viewport _viewport;
        TilemapLevel[] Levels;
        public TilemapLevel CurrentLevel {  get; private set; }
        List<TilemapLevel> ActiveLevels = new List<TilemapLevel>();
        
        public Viewport Viewport { 
            get { return _viewport; } 
            set { 
                _viewport = value;
                
            }
        }

        public TilemapWorld(Viewport viewport, String filename, int worldIdx = 0) {
            Viewport = viewport;
            // Load file
            File = LDtkFile.FromFile(filename, Globals.Cache.content);

            // Load world
            if (worldIdx >= File.Worlds.Length) worldIdx = 0;
            World = File.LoadWorld(File.Worlds[worldIdx].Iid);

            // Create renderer
            Renderer = new TilemapRenderer(Globals.Graphics.SpriteBatch, Globals.Cache.content);

            // Load levels here.
            Levels = new TilemapLevel[World.Levels.Length];
            for (int i = 0; i < World.Levels.Length; i++)
            {
                LDtkLevel level = World.Levels[i];
                var tilemapLevel = new TilemapLevel(level, Renderer);
                Levels[i] = tilemapLevel;
            }
        }

        public int FindIdxByIdentifier(string identifier)
        {
            var foundIdx = -1;
            for (int i = 0; i < Levels.Length; i++)
            {
                if (World.Levels[i].Identifier.CompareTo(identifier) == 0)
                {
                    foundIdx = i; 
                    break;
                }
            }
            return foundIdx;
        }
        public int FindIdxByIid(Guid iid)
        {
            var foundIdx = -1;
            for (int i = 0; i < Levels.Length; i++)
            {
                if (World.Levels[i].Iid == iid)
                {
                    foundIdx = i;
                    break;
                }
            }
            return foundIdx;
        }

        public bool CheckCollisionWithWorld(int cx, int cy, Vector2 boundarySize)
        {
            foreach (var l in ActiveLevels)
            {
                if (l.CheckCollisionWithEntities(cx, cy, boundarySize)) return true;
            }

            for (int x = 0; x <= boundarySize.X; x++)
            {
                int xx = x * IEntities.BOUND_UNIT;
                int xl = cx + xx;
                int xr = cx - xx;
                for (int y = 0; y <= boundarySize.Y; y++)
                {
                    int yy = y * IEntities.BOUND_UNIT;
                    int yc = cy - yy;

                    var ll = GetActiveLevelInBoundaries(xl, yc);
                    if (ll != null)
                    {
                        if (ll.CheckCollisionWithTileAtPosition(xl, yc))
                            return true;
                    }
                    var lr = GetActiveLevelInBoundaries(xr, yc);
                    if (lr != null)
                    {
                        if (lr.CheckCollisionWithTileAtPosition(xr, yc))
                            return true;
                    }
                }
            }
            
            return false;
        }

        public bool SetActiveLevelByBoundaries(int x, int y)
        {
            var l = GetActiveLevelInBoundaries(x, y);
            if (l != null && l != CurrentLevel)
            {
                ActivateLevel(l.Identifier);
                return true;
            }
            return false;
        }

        public TilemapLevel GetActiveLevelInBoundaries(int x, int y)
        {
            foreach (var l in ActiveLevels)
            {
                if (l.CheckIfInBoundaries(x, y))
                {
                    return l;
                }
            }
            return null;
        }

        public void ActivateLevel(string identifier)
        {
            // Look for level by identifier.
            var foundIdx = FindIdxByIdentifier(identifier);
            if (foundIdx < 0) return;
            // Set current level.
            CurrentLevel = Levels[foundIdx];
            // Set new list of active levels.
            List<TilemapLevel> newActiveLevels = new List<TilemapLevel>();
            newActiveLevels.Add(Levels[foundIdx]);
            foreach(var neighbour in World.Levels[foundIdx]._Neighbours)
            {
                var idx = FindIdxByIid(neighbour.LevelIid);
                if (idx < 0) continue;
                newActiveLevels.Add(Levels[idx]);
            }
            SetActiveLevels(newActiveLevels);
        }

        public void SetActiveLevels(List<TilemapLevel> newActiveLevels)
        {
            foreach (var l in ActiveLevels)
            {
                l.Set(null);
            }
            ActiveLevels = newActiveLevels;
            foreach (var l in ActiveLevels)
            {
                l.Set(_viewport);
            }
        }

        internal Rectangle GetCurrentBoundaries()
        {
            if (CurrentLevel != null)
                return CurrentLevel.GetBoundaries();

            return new Rectangle();
        }

        internal void Update(GameTime gameTime)
        {
            CurrentLevel.Update(gameTime);
        }
    }
}
