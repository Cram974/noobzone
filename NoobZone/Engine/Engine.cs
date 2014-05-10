using System;
using System.Linq;
using System.Text;
using System.Threading;

using Tao.OpenGl;
using Tao.Sdl;
using Tao.FreeGlut;
using Tao.DevIl;

namespace NoobZone
{
    class Engine
    {
        static int i_fps;
        public static bool IsExtentionSupported(string ext)
        {
            return Gl.IsExtensionSupported(ext);
        }

        static void Update()
        {
            World.Update();
            EventMgr.Update();
            Camera.Update();
            EmitterMgr.Update();
            DropboxMgr.Update();
            TargetMgr.Update();

            if (EventMgr.Keyboard[Sdl.SDLK_ESCAPE])
                EventMgr.Quit = true;

            if (EventMgr.Keyboard['f'] && Camera.CameraType != Camera.Type.FreeFly)
            {
                Camera.SetFreeFly();
                TargetMgr.Stop();
            }
            if (EventMgr.Keyboard[' '] && Camera.CameraType != Camera.Type.FPS)
            {
                Camera.FPSCam = new Player("Thorin", 0);
                Camera.SetFPS();
                Camera.FPSCam.UpdateAngles();
                Camera.Position = World.Map.SpawnPoints[0].position;
                TargetMgr.Start();
            }

            if (EventMgr.Keyboard['g'] && Camera.CameraType != Camera.Type.Driven)
            {
                Camera.SetDriven();
                TargetMgr.Stop();
            }
            if (World.dt != 0 && Sdl.SDL_GetTicks() % 10 == 0)
                i_fps = (int)(1f/World.dt);
        }
        static void Display()
        {
            Gl.glClear(Gl.GL_COLOR_BUFFER_BIT|Gl.GL_DEPTH_BUFFER_BIT);

            Gl.glViewport(0, 0, VarMgr.Geti("WindowWidth"), VarMgr.Geti("WindowHeight"));
            Camera.LookAt();

            Gl.glMatrixMode(Gl.GL_MODELVIEW);
            Gl.glLoadIdentity();

            World.Map.Render();

            EmitterMgr.Render();

            SceneMgr.Render();

            if (TargetMgr.ActiveTarget != -1)
                TargetMgr.Targets[TargetMgr.ActiveTarget].Render();
            
            if (Camera.CameraType == Camera.Type.FPS)
                ((Player)Camera.FPSCam).Render();

            Gl.glMatrixMode(Gl.GL_PROJECTION);
            Gl.glLoadIdentity();
            
            if(!TargetMgr.hasloose && TargetMgr.hasStarted)
                Write(0f, 0.95f, Glut.GLUT_BITMAP_TIMES_ROMAN_24, "Time Remaining: " + TargetMgr.nextTimeup, new Color(1, 1, 1, 1));

            if(TargetMgr.hasloose)
                Write(0.45f, 0.55f, Glut.GLUT_BITMAP_TIMES_ROMAN_24, "Time Over! Score:" + ((Player)Camera.FPSCam).point, new Color(1, 1, 1, 1));

            Gl.glFlush();
            Sdl.SDL_GL_SwapBuffers();

            int err = Gl.glGetError();
            if (err != Gl.GL_NO_ERROR)
            {
                Log.Message("Erreur GL : " + Glu.gluErrorString(err), Log.MessageType.ERROR);
            }
        }
        public static void Write(float x, float y, IntPtr font, string text, Color c)
        {
            if (text == null || text.Length == 0) 
                return;

            Gl.glEnable(Gl.GL_BLEND);
            Gl.glColor4fv(c.V);
            x = (x * 2 - 1);
            y = (y * 2 - 1);
            Gl.glRasterPos2f(x, y);
            for (int i = 0; i < text.Length; i++)
            {
                Glut.glutBitmapCharacter(font, (int)text[i]);
            }
            Gl.glColor3f(1, 1, 1);
            Gl.glDisable(Gl.GL_BLEND);
        }  
        public static void Init(string[] args)
        {
            if (args.Length == 3)
                VarMgr.Clear();
            VarMgr.Seti("WindowWidth", 1280);
            VarMgr.Seti("WindowHeight", 800);
            VarMgr.Seti("WindowBpp", 32);
            VarMgr.Setf("WindowRatio", 1280f/800f);
            VarMgr.Setb("WindowFullscreen", true);

            CreateWindow();
            
            Gl.glEnable(Gl.GL_DEPTH_TEST);
            Gl.glDepthFunc(Gl.GL_LESS);
            //Gl.glPolygonMode(Gl.GL_FRONT_AND_BACK, Gl.GL_LINE);

            Glut.glutInit();
            EventMgr.Init();
            Camera.Init();
            EmitterMgr.Init();
            AudioMgr.Init();
            DropboxMgr.Init();

            World.Map = new Map("Maps/Abbey.txt");

            World.Map.drivsound.PlayLoop();

            Camera.SetDriven();


            int err = Gl.glGetError();
            if (err != Gl.GL_NO_ERROR)
            {
                Log.Message("Erreur GL : " + Glu.gluErrorString(err), Log.MessageType.ERROR);
            }
        }

        static void CreateWindow()
        {
            Sdl.SDL_Init(Sdl.SDL_INIT_VIDEO);

            Sdl.SDL_WM_SetCaption("NoobZone", null);
            if (VarMgr.Getb("WindowFullscreen"))
                Sdl.SDL_SetVideoMode(VarMgr.Geti("WindowWidth"),
                                     VarMgr.Geti("WindowHeight"),
                                     VarMgr.Geti("WindowBpp"),
                                     Sdl.SDL_OPENGL|Sdl.SDL_FULLSCREEN);
            else
                Sdl.SDL_SetVideoMode(VarMgr.Geti("WindowWidth"),
                                     VarMgr.Geti("WindowHeight"),
                                     VarMgr.Geti("WindowBpp"),
                                     Sdl.SDL_OPENGL);

            Sdl.SDL_WM_GrabInput(Sdl.SDL_TRUE);
            Sdl.SDL_ShowCursor(Sdl.SDL_FALSE);
        }

        public static void Launch(string[] args)
        {
            Log.Create("NoobZone.log");
            
            Init(args);

            while (!EventMgr.Quit)
            {
                Update();
                Display();
                if (World.dt < 1f / 60f)
                    Thread.Sleep((int)(((1f / 60f) - World.dt) * 1000f));
            }
            AudioMgr.Quit();
            Sdl.SDL_Quit();
            Log.Destroy();
        }

        
    }
}
