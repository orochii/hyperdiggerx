using FmodForFoxes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace HyperDigger
{
    class Cache
    {
        public ContentManager content;

        Dictionary<string, Texture2D> cachedTextures;
        Dictionary<string, Sound> cachedSounds;
        Dictionary<string, SpriteFont> cachedFonts;

        public Cache() {
            cachedTextures = new Dictionary<string, Texture2D>();
            cachedSounds = new Dictionary<string, Sound>();
            cachedFonts = new Dictionary<string, SpriteFont>();
        }

        public Texture2D LoadTexture(string path)
        {
            if (cachedTextures.TryGetValue(path, out Texture2D texture)) {  return texture; }
            var newTexture = content.Load<Texture2D>(path);
            cachedTextures.Add(path, newTexture);
            return newTexture;
        }

        public Sound LoadSound(string path)
        {
            if (cachedSounds.TryGetValue(path, out var sound)) {  return sound; }
            var newSound = CoreSystem.LoadStreamedSound(path);
            cachedSounds.Add(path, newSound);
            return newSound;
        }

        public SpriteFont LoadFont(string path)
        {
            if (cachedFonts.TryGetValue(path, out var font)) {  return font; }
            var newFont = content.Load<SpriteFont>(path);
            cachedFonts.Add(path, newFont);
            return newFont;
        }

        public void FlushAll()
        {
            foreach (var texture in cachedTextures.Values)
            {
                texture.Dispose();
            }
            cachedTextures.Clear();
            foreach (var sound in cachedSounds.Values)
            {
                sound.Dispose();
            }
            cachedSounds.Clear();
        }
    }
}
