using Microsoft.Xna.Framework;

namespace HyperDigger
{
    class Global
    {
        public static bool DebugDraw = true;

        public static Graphics Graphics;
        public static Audio Audio;
        public static Cache Cache;
        public static Input Input;
        public static SceneStack SceneStack;
        public static Database Database;
        public static GameState State;

        internal static void Initialize(Game g)
        {
            Database = new Database();
            Graphics = new Graphics(g);
            Audio = new Audio();
            Cache = new Cache();
            Input = new Input();
            SceneStack = new SceneStack();
            State = new GameState();
        }
    }
}
