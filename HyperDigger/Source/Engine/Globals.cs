using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace HyperDigger
{
    class Globals
    {
        public static Graphics Graphics;
        public static Audio Audio;
        public static Cache Cache;
        public static Input Input;
        public static SceneStack SceneStack;
        public static Database Database;

        internal static void Initialize(Game g)
        {
            Database = new Database();
            Graphics = new Graphics(g);
            Audio = new Audio();
            Cache = new Cache();
            Input = new Input();
            SceneStack = new SceneStack();
        }
    }
}
