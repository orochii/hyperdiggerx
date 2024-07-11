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
        Container _container;
        Container _innerContainer;
        TilemapLevel[] Levels;
        public TilemapLevel CurrentLevel {  get; private set; }
        List<TilemapLevel> ActiveLevels = new List<TilemapLevel>();
        List<GameObject> GlobalEntities = new List<GameObject>();
        public GameObject ViewTarget;

        public Container Container { 
            get { return _container; } 
            set { 
                _container = value;
                
            }
        }

        public Container InnerContainer { get { return _innerContainer; } }
        
        public TilemapWorld(Container container, String filename, int worldIdx = 0) {
            _innerContainer = new Container(container);
            Container = container;
            // Load file
            File = LDtkFile.FromFile(filename, Globals.Cache.content);

            // Load world
            if (worldIdx >= File.Worlds.Length) worldIdx = 0;
            World = File.LoadWorld(File.Worlds[worldIdx].Iid);

            // Create renderer
            Renderer = new TilemapRenderer(Globals.Graphics.SpriteBatch);

            // Load levels here.
            Levels = new TilemapLevel[World.Levels.Length];
            for (int i = 0; i < World.Levels.Length; i++)
            {
                LDtkLevel level = World.Levels[i];
                var tilemapLevel = new TilemapLevel(this, level, Renderer);
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

        public bool CheckCollisionWithWorld(int cx, int cy, Collider other)
        {
            // Check entities first.
            foreach (var l in ActiveLevels)
            {
                if (l.CheckCollisionWithEntities(cx, cy, other)) return true;
            }

            // Check against levels.
            int offsetCx = (int)other.OffsetX(cx);
            int offsetCy = (int)other.OffsetY(cy);
            
            int xUnits = (int)other.BoundarySize.X / Collider.BOUND_UNIT;
            int yUnits = (int)other.BoundarySize.Y / Collider.BOUND_UNIT;

            for (int x = 0; x <= xUnits; x++)
            {
                int xx = x * Collider.BOUND_UNIT;
                int xl = offsetCx + xx;
                int xr = offsetCx - xx;
                for (int y = 0; y <= yUnits; y++)
                {
                    int yy = y * Collider.BOUND_UNIT;
                    int yt = offsetCy - yy;
                    int yb = offsetCy + yy;

                    var tl = GetActiveLevelInBoundaries(xl, yt);
                    if (tl != null)
                    {
                        if (tl.CheckCollisionWithTileAtPosition(xl, yt))
                            return true;
                    }
                    var tr = GetActiveLevelInBoundaries(xr, yt);
                    if (tr != null)
                    {
                        if (tr.CheckCollisionWithTileAtPosition(xr, yt))
                            return true;
                    }
                    var bl = GetActiveLevelInBoundaries(xl, yb);
                    if (bl != null)
                    {
                        if (bl.CheckCollisionWithTileAtPosition(xl, yb))
                            return true;
                    }
                    var br = GetActiveLevelInBoundaries(xr, yb);
                    if (br != null)
                    {
                        if (br.CheckCollisionWithTileAtPosition(xr, yb))
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
                l.Set(_innerContainer);
            }
        }

        internal void Update(GameTime gameTime)
        {
            foreach(var e in GlobalEntities)
            {
                e.Update(gameTime);
            }
            CurrentLevel.Update(gameTime);
            // Update view
            SetActiveLevelByBoundaries((int)ViewTarget.Position.X, (int)ViewTarget.Position.Y);
            // Follow player
            InnerContainer.Position.X = (Globals.Graphics.Width / 2) - ViewTarget.Position.X;
            InnerContainer.Position.Y = (Globals.Graphics.Height / 2) - ViewTarget.Position.Y;
            ClampViewToBoundaries();
        }

        private void ClampViewToBoundaries()
        {
            var boundaries = GetCurrentBoundaries();
            if (boundaries.Width == 0) return;

            var minX = boundaries.Left;
            var minY = boundaries.Top;
            var maxX = boundaries.Right - Globals.Graphics.Width;
            var maxY = boundaries.Bottom - Globals.Graphics.Height;
            maxX = Math.Max(minX, maxX);
            maxY = Math.Max(minY, maxY);
            InnerContainer.Position.X = Math.Clamp(InnerContainer.Position.X, -maxX, -minX);
            InnerContainer.Position.Y = Math.Clamp(InnerContainer.Position.Y, -maxY, -minY);
        }

        public Rectangle GetCurrentBoundaries()
        {
            if (CurrentLevel != null)
                return CurrentLevel.GetBoundaries();

            return new Rectangle();
        }

        public void AddEntity(GameObject entity)
        {
            GlobalEntities.Add(entity);
            entity.World = this;
        }
    }
}
