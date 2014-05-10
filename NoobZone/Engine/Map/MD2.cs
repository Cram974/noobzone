using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.IO;

using Tao.OpenGl;

namespace NoobZone
{
    class MD2
    {
        int triangleCount;
        int frameCount;
        int vertexCount;
        int textCoordCount;
        Matrix transform;
          
        public int FrameCount
        {
            get { return frameCount; }
        }

        public Matrix Matrice
        {
            get { return transform; }
            set { transform = value; }
        }

        int[] trianglesIndexBuffer;
        int[] texturesIndexBuffer;
        Vector2[] texturesCoordBuffer;
        Frame[] framesBuffer;
        Texture2D tex;

        public MD2(string fileName, string textureName)
        {
            Texture2D.EnableMipMaping = false;
            tex =  TextureMgr.Load(textureName);
            Texture2D.EnableMipMaping = true;

            transform = new Matrix();

            loadMD2(fileName);
        }

        public AABB getAABB(int frame)
        {
            AABB ret = new AABB();
            Vector3 v1, v2, v3;
            for (int i = 0; i < triangleCount; i++)
            {
                v1 = resize(framesBuffer[frame][trianglesIndexBuffer[i * 3]]);
                v2 = resize(framesBuffer[frame][trianglesIndexBuffer[i * 3 + 1]]);
                v3 = resize(framesBuffer[frame][trianglesIndexBuffer[i * 3 + 2]]);
                ret.AddVertex(v1);
                ret.AddVertex(v2);
                ret.AddVertex(v3);
            }
            return ret;
        }

        public void Render(int frame)
        {
            for (int i = 0; i < triangleCount; i++)
            {
                tex.Bind();
                Gl.glBegin(Gl.GL_TRIANGLES);

                int si1 = trianglesIndexBuffer[i * 3];
                int si2 = trianglesIndexBuffer[i * 3 + 1];
                int si3 = trianglesIndexBuffer[i * 3 + 2];
                
                int ti1 = texturesIndexBuffer[i * 3];
                int ti2 = texturesIndexBuffer[i * 3 + 1];
                int ti3 = texturesIndexBuffer[i * 3 + 2];

                Vector3 v1 = resize(framesBuffer[frame][si1]);
                Vector3 v2 = resize(framesBuffer[frame][si2]);
                Vector3 v3 = resize(framesBuffer[frame][si3]);

                Gl.glTexCoord2fv(texturesCoordBuffer[ti1].V);
                Gl.glVertex3fv(v1.V);

                Gl.glTexCoord2fv(texturesCoordBuffer[ti2].V);
                Gl.glVertex3fv(v2.V);

                Gl.glTexCoord2fv(texturesCoordBuffer[ti3].V);
                Gl.glVertex3fv(v3.V);

                Gl.glEnd();
                tex.Unbind();

            }
        }

        Vector3 resize(Vector3 v)
        {
            return (v * transform);
        }

        unsafe public void loadMD2(string fileName)
        {
            md2_header header;

            FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            BinaryReader br = new BinaryReader(fs);

            //Lecture de l'entête
            byte[] buff = br.ReadBytes(Marshal.SizeOf(typeof(md2_header)));

            GCHandle handle = GCHandle.Alloc(buff, GCHandleType.Pinned);
            header = (md2_header)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(md2_header));
            handle.Free();


            //Chargement des triangles
            md2_triangle trt;

            br.BaseStream.Seek(header.ofs_tris, SeekOrigin.Begin);//Aller au debut de la déclaration des triangles

            triangleCount = header.num_tris;

            trianglesIndexBuffer = new int[triangleCount * 3];
            texturesIndexBuffer = new int[triangleCount * 3];
            for (int i = 0; i < triangleCount; i++)
            {
                buff = br.ReadBytes(Marshal.SizeOf(typeof(md2_triangle)));

                handle = GCHandle.Alloc(buff, GCHandleType.Pinned);
                trt = (md2_triangle)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(md2_triangle));
                handle.Free();

                trianglesIndexBuffer[i * 3] = trt.index_xyz[2];
                trianglesIndexBuffer[i * 3 + 1] = trt.index_xyz[1];
                trianglesIndexBuffer[i * 3 + 2] = trt.index_xyz[0];

                texturesIndexBuffer[i * 3] = trt.index_st[2];
                texturesIndexBuffer[i * 3 + 1] = trt.index_st[1];
                texturesIndexBuffer[i * 3 + 2] = trt.index_st[0];
            }




            //Chargement des "images"
            frameCount = header.num_frames;
            vertexCount = header.num_xyz;
            framesBuffer = new Frame[frameCount];

            br.BaseStream.Seek(header.ofs_frames, SeekOrigin.Begin);

            for (int i = 0; i < frameCount; i++)//frameCount-1
            {
                buff = br.ReadBytes(Marshal.SizeOf(typeof(md2_frame)));

                handle = GCHandle.Alloc(buff, GCHandleType.Pinned);
                md2_frame ffrm = (md2_frame)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(md2_frame));
                handle.Free();

                framesBuffer[i] = new Frame(vertexCount);
                framesBuffer[i].Name = MD2utils.getStrFromCharArray(ffrm.name);

                md2_vertex tvrt;
                float tx, ty, tz;

                for (int j = 0; j < vertexCount; j++)
                {
                    buff = br.ReadBytes(Marshal.SizeOf(typeof(md2_vertex)));

                    handle = GCHandle.Alloc(buff, GCHandleType.Pinned);
                    tvrt = (md2_vertex)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(md2_vertex));
                    handle.Free();

                    tx = tvrt.coords[0] * ffrm.scale[0] + ffrm.translate[0];
                    ty = tvrt.coords[1] * ffrm.scale[1] + ffrm.translate[1];
                    tz = tvrt.coords[2] * ffrm.scale[2] + ffrm.translate[2];

                    framesBuffer[i][j] = new Vector3(tx, ty, tz);
                }
            }

            //Chargement des textures
            md2_compressed_texture ctex;

            br.BaseStream.Seek(header.ofs_st, SeekOrigin.Begin);

            textCoordCount = header.num_st;

            texturesCoordBuffer = new Vector2[textCoordCount];
            for (int i = 0; i < textCoordCount; i++)
            {
                buff = br.ReadBytes(Marshal.SizeOf(typeof(md2_compressed_texture)));

                handle = GCHandle.Alloc(buff, GCHandleType.Pinned);
                ctex = (md2_compressed_texture)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(md2_compressed_texture));
                handle.Free();

                float x = ctex.s;
                x /= header.skinwidth;

                float y = ctex.t;
                y /= header.skinheight;

                texturesCoordBuffer[i] = new Vector2(x, y);
            }
            fs.Close();
        }
    }
}
