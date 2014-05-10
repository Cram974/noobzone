using System;
using System.Collections.Generic;

using Tao.Sdl;

namespace NoobZone
{
    class EventMgr
    {
        public struct MouseState
        {
            public int X;
            public int Y;
            public int Xrel;
            public int Yrel;
            public bool Moved;
            public Dictionary<int,bool> Button;
        }

        static Dictionary<int, bool> keystate;
        static Dictionary<string, int> keymap;
        static MouseState mousestate;
        static Sdl.SDL_Event evt;
        static bool quit;

        public static void Init()
        {
            keystate = new Dictionary<int,bool>();
            keymap = new Dictionary<string, int>();
            mousestate.Button = new Dictionary<int, bool>();

            for (int i = Sdl.SDLK_FIRST; i < Sdl.SDLK_LAST; i++)
                keystate.Add(i, false);

            mousestate.Button.Add(Sdl.SDL_BUTTON_RIGHT, false);
            mousestate.Button.Add(Sdl.SDL_BUTTON_LEFT, false);
            mousestate.Button.Add(Sdl.SDL_BUTTON_MIDDLE, false);
            Update();
        }

        public static MouseState Mouse
        {
            get { return mousestate; }
        }
        public static Dictionary<int, bool> Keyboard
        {
            get { return keystate; }
        }
        public static bool Quit
        {
            get { return quit; }
            set { quit = value; }
        }

        public static void Update()
        {

            mousestate.Moved = false;
            while (Sdl.SDL_PollEvent(out evt) != 0)
            {
                switch (evt.type)
                {
                    case Sdl.SDL_KEYDOWN:
                        keystate[evt.key.keysym.sym] = true;
                        break;
                    case Sdl.SDL_KEYUP:
                        keystate[evt.key.keysym.sym] = false;
                        break;
                    case Sdl.SDL_MOUSEMOTION:
                        mousestate.Moved = true;
                        mousestate.X = evt.motion.x;
                        mousestate.Y = evt.motion.y;
                        mousestate.Xrel = evt.motion.xrel;
                        mousestate.Yrel = evt.motion.yrel;
                        break;
                    case Sdl.SDL_MOUSEBUTTONDOWN:
                        mousestate.Button[evt.button.button] = true;
                        break;
                    case Sdl.SDL_MOUSEBUTTONUP:
                        mousestate.Button[evt.button.button] = false;
                        break;
                    case Sdl.SDL_QUIT:
                        quit = true;
                        break;
                }
            }
        }
    }
}
