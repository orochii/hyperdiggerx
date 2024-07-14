using FmodForFoxes;
using Microsoft.Xna.Framework;

namespace HyperDigger
{
    class Audio
    {
        INativeFmodLibrary _nativeLibrary;

        string currentBGMName;
        bool isPlaying = false;
        Channel PlayingBGM;
        bool isFading = false;
        Channel FadingBGM;

        ChannelGroup BGMGroup;
        ChannelGroup SFXGroup;

        public Audio() {
            _nativeLibrary = new DesktopNativeFmodLibrary();
        }

        internal void Initialize()
        {
            FmodManager.Init(_nativeLibrary, FmodInitMode.CoreAndStudio, "Content");
            BGMGroup = new ChannelGroup("BGM");
            SFXGroup = new ChannelGroup("SFX");
        }

        public void Update(GameTime gameTime)
        {
            float d = (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (isFading)
            {
                FadingBGM.Volume = HyperMath.MoveTowards(FadingBGM.Volume, 0, d);
                if (FadingBGM.Volume <= 0)
                {
                    FadingBGM.Stop();
                    isFading = false;
                }
            }
        }

        public Channel PlaySFX(string sfxName, float volume, float pitch)
        {
            var sound = Global.Cache.LoadSound("Audio/SFX/" + sfxName);
            var channel = sound.Play(SFXGroup);
            channel.Volume = volume;
            channel.Pitch = pitch;
            return channel;
        }

        public Channel PlaySFXAt(string sfxName, float volume, float pitch, Vector2 pos)
        {
            var channel = PlaySFX(sfxName, volume, pitch);
            channel.Is3D = true;
            var halfScreen = new Vector2(Global.Graphics.Width / 2, Global.Graphics.Height / 2);
            var scaledPos = (pos - halfScreen) * 0.01f;
            channel.Position3D = new Vector3(scaledPos.X, scaledPos.Y, 0);
            return channel;
        }

        public void PlayBGM(string bgmName)
        {
            // Check if currently playing.
            if (bgmName.CompareTo(currentBGMName) == 0) return;
            currentBGMName = bgmName;
            // Fade current song, if any.
            if (isPlaying)
            {
                if (isFading) FadingBGM.Stop();
                FadingBGM = PlayingBGM;
                isFading = true;
            }
            // Play if name is not empty
            if (bgmName.Length > 0)
            {
                var sound = Global.Cache.LoadSound("Audio/BGM/" + currentBGMName);
                PlayingBGM = sound.Play(BGMGroup);
                PlayingBGM.Looping = true;
                isPlaying = true;
            }
            else
            {
                PlayingBGM = default;
                isPlaying = false;
            }
        }

    }
}
