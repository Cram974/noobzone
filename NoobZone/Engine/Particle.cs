using System;
using System.Collections.Generic;

using Tao.OpenGl;
using Tao.Sdl;

namespace NoobZone
{
    class Particle
    {
        Vector3 accel;
        Vector3 speed;
        Vector3 pos;
        float lifetime;
        float mass;

        public Vector3 Pos
        {
            get { return pos; }
            set { pos = value; }
        }
        public Vector3 Accel
        {
            set { accel = value; }
            get { return accel; }
        }
        public Vector3 Speed
        {
            get { return speed; }
            set { speed = value; }
        }
        public float Mass
        {
            get { return mass; }
            set { mass = value; }
        }
        public float LifeTime
        {
            get { return lifetime; }
            set { lifetime = value; }
        }
        public Vector3 color;

        public Particle(Vector3 Position, Vector3 Velocity, float lifeTime)
        {
            accel = new Vector3();
            color = new Vector3();
            lifetime = lifeTime;
            pos = new Vector3(Position);
            speed = new Vector3(Velocity);
            mass = 2f;
        }
    }

    class Emitter
    {
        List<Particle> particles;

        public int Count
        {
            get { return particles.Count; }
        }
        public void SetColor(byte r, byte g, byte b)
        {
            for(int i = 0; i<particles.Count; i++)
            {
                particles[i].color.X = (float)(r / 255f);
                particles[i].color.Y = (float)(g / 255f);
                particles[i].color.Z = (float)(b / 255f);
            }
        }
        public Emitter(Vector3 pos, float intensity)
        {
            Vector3 x = new Vector3(1f, 0f, 0f);
            Vector3 y = new Vector3(0f, 1f, 0f);
            Vector3 z = new Vector3(0f, 0f, 1f);

            particles = new List<Particle>();


            particles.Add(new Particle(pos, (x).Normalized * intensity, 5));
            particles.Add(new Particle(pos, (-1 * x).Normalized * intensity, 5));
            particles.Add(new Particle(pos, (y).Normalized * intensity, 5));
            particles.Add(new Particle(pos, (-1*y).Normalized * intensity, 5));
            particles.Add(new Particle(pos, (z).Normalized * intensity, 5));
            particles.Add(new Particle(pos, (-1*z).Normalized * intensity, 5));
            particles.Add(new Particle(pos, (x + y).Normalized * intensity, 5));
            particles.Add(new Particle(pos, (-1 * (x + y)).Normalized * intensity, 5));
            particles.Add(new Particle(pos, (x + z).Normalized * intensity, 5));
            particles.Add(new Particle(pos, (-1 * (x + z)).Normalized * intensity, 5));
            particles.Add(new Particle(pos, (y + z).Normalized * intensity, 5));
            particles.Add(new Particle(pos, (-1 * (y + z)).Normalized * intensity, 5));
            particles.Add(new Particle(pos, (x + y + z).Normalized * intensity, 5));
            particles.Add(new Particle(pos, (-1 * (x + y + z)).Normalized * intensity, 5));
            particles.Add(new Particle(pos, (x + y - z).Normalized * intensity, 5));
            particles.Add(new Particle(pos, (-1 * (x + y - z)).Normalized * intensity, 5));
            particles.Add(new Particle(pos, (x - y + z).Normalized * intensity, 5));
            particles.Add(new Particle(pos, (-1 * (x - y + z)).Normalized * intensity, 5));
            particles.Add(new Particle(pos, (-1 * x + y + z).Normalized * intensity, 5));
            particles.Add(new Particle(pos, (-1 * (-1 * x + y + z)).Normalized * intensity, 5));
        }

        public void Update()
        {
            float dt = World.dt * 1000;

            for (int i = 0; i < particles.Count; i++)
            {
                particles[i].Accel = World.g * -150 * World.Up;
                particles[i].Speed += particles[i].Accel * World.dt;
                particles[i].Pos += particles[i].Speed * World.dt;
                
                particles[i].LifeTime -= World.dt * 20f;
                if (particles[i].LifeTime <= 0)
                {
                    particles.RemoveAt(i);
                }
            }
        }

        public void Render()
        {
            for (int i = 0; i < particles.Count; i++)
            {
                Vector3 pos = particles[i].Pos;

                Glu.GLUquadric param = Glu.gluNewQuadric();
                Gl.glColor3fv(particles[i].color.V);
                Glu.gluQuadricDrawStyle(param, Glu.GLU_FILL);
                Gl.glPushMatrix();
                Gl.glTranslatef(pos.X, pos.Y, pos.Z);
                Glu.gluSphere(param, 1, 20, 20);
                Gl.glPopMatrix();
                Gl.glColor3f(1f, 1f, 1f);
                Glu.gluDeleteQuadric(param);

            }
        }

    }

    class EmitterMgr
    {
        public static List<Emitter> emitters;

        public static void Init()
        {
            emitters = new List<Emitter>();
        }

        public static void Update()
        {
            for (int i = 0; i < emitters.Count; i++)
            {
                if (emitters[i].Count == 0)
                {
                    emitters.RemoveAt(i);
                }
                else
                {
                    emitters[i].Update();
                }
            }
        }
        public static void Render()
        {
            for (int i = 0; i < emitters.Count; i++)
            {
                emitters[i].Render();
            }
        }
    }
}
