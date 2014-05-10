using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Tao.OpenGl;
using Tao.Sdl;
using Tao.FreeGlut;

namespace NoobZone
{
    class Player : Camera_FPS
    {
        string s_name;
        public int point;
        MD2 weapon;
        int nb_munit;
        int nb_rech;
        int id;
        public const int munitMax = 50;
        float shootfreq;
        float weapofs;

        Son tir;

        public int MunitCount
        {
            get { return nb_munit; }
            set { nb_munit = value; }
        }
        public int ChargerCount
        {
            get { return nb_rech; }
            set { nb_rech = value; }
        }
        public string Name
        {
            get { return s_name; }
        }

        public Player(string name,int id)
            : base()
        {
            this.s_name = name;
            this.id = id;
            this.weapon = new MD2("Models/sniper.md2","Textures/sniper.jpg");

            Camera.Position = World.Map.SpawnPoints[this.id].position;
            this.f_phi = World.Map.SpawnPoints[this.id].phi;
            this.f_teta = World.Map.SpawnPoints[this.id].teta;

            Camera.Update();
            tir = new Son("Sons/tir.wav");
            shootfreq = 0;

            nb_munit = munitMax;
            nb_rech = 10;
        }
        public override void Update()
        {
            base.Update();

            weapofs -= 2f * World.dt;
            if (weapofs < 0) weapofs = 0;

            shootfreq -= World.dt;
            if (shootfreq < 0) shootfreq = 0;

            if (EventMgr.Mouse.Button[Sdl.SDL_BUTTON_LEFT] && shootfreq == 0 && nb_munit > 0)
            {

                Vector3 startpos = Camera.Position;
                Vector3 endpos = Camera.Position + 900 * Camera.Forward;
                Vector3 emmitpos = World.Map.Level.TraceRay(startpos,endpos).collisionPoint;
                if (TargetMgr.ActiveTarget != -1 && Collision.IsColliding(TargetMgr.Targets[TargetMgr.ActiveTarget].Poly, new Vector3[] { startpos, emmitpos }, 4))
                {
                    TargetMgr.ChooseTarget();
                    point++;
                    TargetMgr.nextTimeup += 10;
                }
                else
                {
                    Emitter emit = new Emitter(emmitpos, 100);
                    emit.SetColor(0, 0, 0);
                    EmitterMgr.emitters.Add(emit);
                }
                tir.Play();
                nb_munit--;
                weapofs = 0.5f;
                shootfreq = 0.1f;
            }
            if ((EventMgr.Keyboard['r'] && nb_munit < munitMax) ||  nb_munit == 0)
            {
                if (nb_rech > 0)
                {
                    nb_rech--;
                    nb_munit = munitMax;
                    shootfreq = 1.2f;
                }
            }
            AABB cambox = new AABB();

            cambox.AddVertex(new Vector3(-10, -10, -60));
            cambox.AddVertex(new Vector3(10, 10, 20));
            cambox.Min += Camera.Position;
            cambox.Max += Camera.Position;
            DropboxMgr.TryToDrop(cambox, s_name);
        }

        public void Render()
        {
            Gl.glPushMatrix();
            Vector3 trans = Camera.Position;
            Gl.glTranslatef(Camera.Position.X, Camera.Position.Y, Camera.Position.Z);
            Gl.glRotatef(Camera.FPSCam.Teta - 90, 0, 0, 1);
            Gl.glRotatef(Camera.FPSCam.Phi, 1, 0, 0);
            Gl.glTranslatef(1.5f, 1f - weapofs, -2.5f);
            weapon.Matrice = Matrix.GetGLModelView();
            Gl.glPopMatrix();
            weapon.Render(0);

            Gl.glMatrixMode(Gl.GL_PROJECTION);
            Gl.glLoadIdentity();
            Texture2D ball = TextureMgr.Load("Textures/bullet.png");
            Gl.glEnable(Gl.GL_BLEND);
            Gl.glBlendFunc(Gl.GL_SRC_ALPHA, Gl.GL_ONE_MINUS_SRC_ALPHA);
            ball.Bind();
            for (int i = 0; i < nb_rech + 1; i++)
            {
                float coeff = 0.05f;
                Gl.glPushMatrix();
                Gl.glTranslatef(0, -0.45f, 0);
                Gl.glBegin(Gl.GL_QUADS);
                Gl.glTexCoord2f(0, 0);
                Gl.glVertex2f(-1f + i * coeff, -0.3f);
                Gl.glTexCoord2f(1, 0);
                Gl.glVertex2f(-0.95f + i * coeff, -0.3f);
                Gl.glTexCoord2f(1, 1);
                Gl.glVertex2f(-0.95f + i * coeff, -0.5f);
                Gl.glTexCoord2f(0, 1);
                Gl.glVertex2f(-1f + i * coeff, -0.5f);
                Gl.glEnd();
                Gl.glPopMatrix();
            }
            ball.Unbind();


            Texture2D seringue = TextureMgr.Load("Textures/target.jpg");

            seringue.Bind();
            Gl.glPushMatrix();
            Gl.glTranslatef(1.5f, -0.5f, 0);
            Gl.glBegin(Gl.GL_QUADS);
            Gl.glTexCoord2f(0, 0);
            Gl.glVertex2f(-1f, -0.3f);
            Gl.glTexCoord2f(1, 0);
            Gl.glVertex2f(-0.95f, -0.3f);
            Gl.glTexCoord2f(1, 1);
            Gl.glVertex2f(-0.95f, -0.5f);
            Gl.glTexCoord2f(0, 1);
            Gl.glVertex2f(-1f, -0.5f);
            Gl.glEnd();
            Gl.glPopMatrix();
            seringue.Unbind();

            Gl.glDisable(Gl.GL_BLEND);

            Engine.Write((nb_rech+1)*0.025f + 0.01f, 0.05f, Glut.GLUT_BITMAP_TIMES_ROMAN_24, "" + nb_munit, new Color(1, 1, 1, 1));
            Engine.Write(0.78f, 0.048f, Glut.GLUT_BITMAP_TIMES_ROMAN_24, "" + point, new Color(1, 1, 1, 1));
        }
    }
}
