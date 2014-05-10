using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Tao.OpenGl;

namespace NoobZone
{
    class Texture2D
    {
        int i_Id;
        int i_Width;
        int i_Height;
        int i_Bpp;
        string path;

        static bool mipmaps = true;

        public int ID
        {
            get { return i_Id; }
        }
        public int Width
        {
            get { return i_Width; }
        }
        public int Height
        {
            get { return i_Height; }
        }
        public int Bpp
        {
            get { return i_Bpp; }
        }
        public int Type
        {
            get { return Gl.GL_TEXTURE_2D; }
        }
        public static bool EnableMipMaping
        {
            get { return mipmaps; }
            set { mipmaps = value; } 
        }


        public Texture2D(string filename)
        {
            path = filename;
            Log.Message("Loading texture: " + filename, Log.MessageType.INFO);
            
            byte[] data = Image.Load(filename, ref i_Width, ref i_Height, ref i_Bpp);
            if (data == null)
            {
                Log.Message("Failed to load: " + filename, Log.MessageType.ERROR);
                return;
            }
            Gl.glEnable(Gl.GL_TEXTURE_2D);
            Gl.glGenTextures(1, out i_Id);
            Bind();

            if (mipmaps)
            {
                Glu.gluBuild2DMipmaps(Gl.GL_TEXTURE_2D, i_Bpp, i_Width, i_Height, i_Bpp == 3 ? Gl.GL_RGB : Gl.GL_RGBA, Gl.GL_UNSIGNED_BYTE, data);

                Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MIN_FILTER,
                                Gl.GL_LINEAR_MIPMAP_LINEAR);
            }
            else
            {
                Gl.glTexImage2D(Gl.GL_TEXTURE_2D, 0, i_Bpp == 3 ? Gl.GL_RGB : Gl.GL_RGBA, i_Width,
                                i_Height, 0, i_Bpp == 3 ? Gl.GL_RGB : Gl.GL_RGBA, Gl.GL_UNSIGNED_BYTE, data);
                Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MIN_FILTER, Gl.GL_LINEAR);
            }
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MAG_FILTER, Gl.GL_LINEAR);

            Unbind();
        }
        public Texture2D(string filename, float gamma)
        {
            path = filename;
            Log.Message("Loading texture: " + filename, Log.MessageType.ERROR);

            byte[] data = Image.Load(filename, ref i_Width, ref i_Height, ref i_Bpp);
            if (data == null)
            {
                Log.Message("Failed to load: " + filename, Log.MessageType.ERROR);
                return;
            }

            modifyGamma(ref data, gamma);

            Gl.glEnable(Gl.GL_TEXTURE_2D);
            Gl.glGenTextures(1, out i_Id);
            Bind();

            if (mipmaps)
            {
                Glu.gluBuild2DMipmaps(Gl.GL_TEXTURE_2D, i_Bpp, i_Width, i_Height, i_Bpp == 3 ? Gl.GL_RGB : Gl.GL_RGBA, Gl.GL_UNSIGNED_BYTE, data);

                Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MIN_FILTER,
                                Gl.GL_LINEAR_MIPMAP_LINEAR);
            }
            else
            {
                Gl.glTexImage2D(Gl.GL_TEXTURE_2D, 0, i_Bpp == 3 ? Gl.GL_RGB : Gl.GL_RGBA, i_Width,
                                i_Height, 0, i_Bpp == 3 ? Gl.GL_RGB : Gl.GL_RGBA, Gl.GL_UNSIGNED_BYTE, data);
                Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MIN_FILTER, Gl.GL_LINEAR);
            }
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MAG_FILTER, Gl.GL_LINEAR);

            Unbind();
        }
        public Texture2D(int width, int height, int bpp, byte[] data)
        {
            i_Width = width;
            i_Height = height;
            i_Bpp = bpp;

            if (data == null)
            {
                //Log.Message("Failed to load: " + filename, Log.MessageType.ERROR);
                return;
            }
            Gl.glEnable(Gl.GL_TEXTURE_2D);
            Gl.glGenTextures(1, out i_Id);
            Bind();

            if (mipmaps)
            {
                Glu.gluBuild2DMipmaps(Gl.GL_TEXTURE_2D, i_Bpp, i_Width, i_Height, i_Bpp == 3 ? Gl.GL_RGB : Gl.GL_RGBA, Gl.GL_UNSIGNED_BYTE, data);

                Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MIN_FILTER,
                                Gl.GL_LINEAR_MIPMAP_LINEAR);
            }
            else
            {
                Gl.glTexImage2D(Gl.GL_TEXTURE_2D, 0, i_Bpp == 3 ? Gl.GL_RGB : Gl.GL_RGBA, i_Width,
                                i_Height, 0, i_Bpp == 3 ? Gl.GL_RGB : Gl.GL_RGBA, Gl.GL_UNSIGNED_BYTE, data);
                Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MIN_FILTER, Gl.GL_LINEAR);
            }
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MAG_FILTER, Gl.GL_LINEAR);

            Unbind();
        }
        public Texture2D(int width, int height, int bpp, byte[] data, float gamma)
        {
            i_Width = width;
            i_Height = height;
            i_Bpp = bpp;

            if (data == null)
            {
                //Log.Message("Failed to load: " + filename, Log.MessageType.ERROR);
                return;
            }

            modifyGamma(ref data, gamma);

            Gl.glEnable(Gl.GL_TEXTURE_2D);
            Gl.glGenTextures(1, out i_Id);
            Bind();

            if (mipmaps)
            {
                Glu.gluBuild2DMipmaps(Gl.GL_TEXTURE_2D, i_Bpp, i_Width, i_Height, i_Bpp == 3 ? Gl.GL_RGB : Gl.GL_RGBA, Gl.GL_UNSIGNED_BYTE, data);

                Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MIN_FILTER,
                                Gl.GL_LINEAR_MIPMAP_LINEAR);
            }
            else
            {
                Gl.glTexImage2D(Gl.GL_TEXTURE_2D, 0, i_Bpp == 3 ? Gl.GL_RGB : Gl.GL_RGBA, i_Width,
                                i_Height, 0, i_Bpp == 3 ? Gl.GL_RGB : Gl.GL_RGBA, Gl.GL_UNSIGNED_BYTE, data);
                Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MIN_FILTER, Gl.GL_LINEAR);
            }
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MAG_FILTER, Gl.GL_LINEAR);

            Unbind();
        }
        ~Texture2D()
        {
            Destroy();
        }

        void modifyGamma(ref byte[] data, float gamma)
        {
            for (int j = 0; j < data.Length; j++)
            {
                float f = ((float)data[j] * (gamma / 255f));
                float scale = 1.0f;
                float temp = 1f / f;

                if (f > 1.0f && temp < scale)
                    scale = temp;

                data[j] = (byte)(f * scale * 255f);
            }
        }

        public void Destroy()
        {
            //Gl.glDeleteTextures(1, ref i_Id);
        }

        public void Bind()
        {
            Gl.glEnable(Gl.GL_TEXTURE_2D);
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, i_Id);
        }
        public void Unbind()
        {
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0);
            Gl.glDisable(Gl.GL_TEXTURE_2D);
        }

        public void Bind(int slot)
        {
	        Gl.glActiveTexture(Gl.GL_TEXTURE0_ARB+slot);
	        Gl.glEnable(this.Type);
	        Gl.glBindTexture(this.Type, i_Id);
        }
        public void Unbind(int slot)
        {
            Gl.glActiveTexture(Gl.GL_TEXTURE0_ARB + slot);
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0);
            Gl.glDisable(Gl.GL_TEXTURE_2D);
        }
    }
}
