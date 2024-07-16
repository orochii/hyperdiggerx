using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperDigger.Source.Game.UI
{
    class LevelNameLabel : Label
    {
        const float DELTA_SHOW = 1f;
        const float DELTA_HIDE = 2f;
        const float WAIT_TIME = 2f;

        bool showing = false;
        float status;
        float wait;

        public LevelNameLabel(Container container) : base(container) {
            Font = Global.Cache.LoadFont("Fonts/Title");
            HAlign = Label.EHAlign.Middle;
            Position = new Vector2(Global.Graphics.Width / 2, 64);
            Text = "";
            Shadow = new Color(16,16,16,128);
        }

        public void Set(string text)
        {
            System.Console.WriteLine("SET: " + text);
            if (Text != text)
            {
                Text = text;
                showing = true;
                status = 0;
                wait = 0;
                RefreshVisibility();
                System.Console.WriteLine("LevelName SET to: " + text);
            }
        }

        public override void Update(GameTime gameTime)
        {
            float d = (float)gameTime.ElapsedGameTime.TotalSeconds;
            // Wait
            if (wait > 0)
            {
                wait -= d;
                return;
            }
            // Update state
            if (showing == true)
            {
                if (status < 1f)
                {
                    status = HyperMath.MoveTowards(status, 1f, d/DELTA_SHOW);
                    RefreshVisibility();
                } else
                {
                    showing = false;
                    wait = WAIT_TIME;
                }
            }
            else
            {
                status = HyperMath.MoveTowards(status, 0f, d/DELTA_HIDE);
                RefreshVisibility();
            }
        }

        private void RefreshVisibility()
        {
            byte c = (byte)(status * 255);
            if (c == 0)
            {
                Visible = false;
            }
            else
            {
                Visible = true;
                Color = new Color(c, c, c, c);
            }
        }
    }
}
