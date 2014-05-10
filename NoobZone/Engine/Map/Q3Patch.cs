using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NoobZone
{
    /*
    class Q3BiQuadPatch : IRenderable
    {
        public Q3Vertex[] vertices;
        public int[] indices;
        public int tesLevel;

        public void Tesselate(Q3Vertex[] controls, int level)
        {
            float px, py;
            Q3Vertex[] temp = new Q3Vertex[3];

            tesLevel = level;
            vertices = new Q3Vertex[(level + 1) * (level + 1)];

            for (int v = 0; v <= level; v++)
            {
                px = (float)v / level;
                float a = ((1.0f - px) * (1.0f - px));
                float b = ((1.0f - px) * px * 2);
                float c = px * px;

                vertices[v] = controls[0] * a + controls[3] * b + controls[6] * c;
            }

            for (int u = 1; u <= level; u++)
            {
                py = (float)u / level;
                float a = ((1.0f - py) * (1.0f - py));
                float b = ((1.0f - py) * py * 2);
                float c = py * py;

                temp[0] = controls[0] * a + controls[1] * b + controls[2] * c;
                temp[1] = controls[3] * a + controls[4] * b + controls[5] * c;
                temp[2] = controls[6] * a + controls[7] * b + controls[8] * c;

                for (int v = 0; v <= level; v++)
                {
                    px = (float)v / level;

                    a = (1.0f - px) * (1.0f - px);
                    b = (1.0f - px) * px * 2;
                    c = px * px;

                    vertices[u * (level + 1) + v] = temp[0] * a + temp[1] * b + temp[2] * c;
                }
            }

            indices = new int[level * (level + 1) * 2];
            for (int row = 0; row < level; row++)
            {
                for (int pt = 0; pt <= level; pt++)
                {
                    indices[(row * (level + 1) + pt) * 2 + 1] = row * (level + 1) + pt;
                    indices[(row * (level + 1) + pt) * 2] = (row + 1) * (level + 1) + pt;
                }
            }
        }

        public override void  Render()
        {
            int tris = (tesLevel + 1) * 2;
            for (int i = 0; i < tesLevel; i++)
            {
                graphics.DrawUserIndexedPrimitives<Q3BSPVertex>(
                    PrimitiveType.TriangleStrip,
                    vertices,
                    0,
                    vertices.Length,
                    indices,
                    i * tris,
                    tris - 2);  // num indices - 2 = numtris for trilist
            }
        }
    }

    class Q3Patch
    {
        private int width = 0;
        private int height = 0;
        private int tesselation;
        private Q3BiQuadPatch[] patches = null;

        public void GeneratePatch(Q3Vertex[] vList, int vStart, int nVerts, int size_w, int size_h, int level)
        {
            width = (size_w - 1) / 2;
            height = (size_h - 1) / 2;

            tesselation = level;
            patches = new Q3BiQuadPatch[width * height];

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Q3BiQuadPatch bpatch = new Q3BiQuadPatch();
                    Q3Vertex[] ctrlPoints = new Q3Vertex[9];
                    for (int r = 0; r < 3; r++)
                    {
                        for (int c = 0; c < 3; c++)
                        {
                            ctrlPoints[r * 3 + c] = vList[vStart + (y * 2 * size_w + x * 2) + r * size_w + c];
                        }
                    }
                    bpatch.Tesselate(ctrlPoints, level);
                    patches[(y * width + x)] = bpatch;
                }
            }
        }

        public void Draw(GraphicsDevice graphics)
        {
            foreach (Q3BiQuadPatch patch in patches)
            {
                patch.Draw(graphics);
            }
        }

        #region Properties
        public int Width
        {
            get { return width; }
            set { width = value; }
        }

        public int Height
        {
            get { return height; }
            set { height = value; }
        }

        public int Tesselation
        {
            get { return tesselation; }
            set { tesselation = value; }
        }

        public int PatchCount
        {
            get
            {
                if (null != patches)
                {
                    return patches.Length;
                }

                return 0;
            }
        }
        #endregion


        public Q3Vertex[] GetVertices()
        {
            List<Q3Vertex> vertices = new List<Q3Vertex>();
            foreach (Q3BiQuadPatch patch in patches)
            {
                foreach (Q3Vertex vert in patch.vertices)
                {
                    vertices.Add(vert);
                }
            }
            return vertices.ToArray();
        }

        public short[] GetIndices()
        {
            List<short> indices = new List<short>();
            int indexOffset = 0;

            foreach (Q3BiQuadPatch patch in patches)
            {
                int numberOfVerticesPerLevel = (patch.tesLevel + 1) * 2;

                for (int i = 0; i < patch.tesLevel; ++i)
                {
                    for (int j = 0; j < numberOfVerticesPerLevel - 2; ++j)
                    {
                        int triangleOffset = numberOfVerticesPerLevel * i;

                        // Flip every other triangle
                        if (j % 2 == 0)
                        {
                            indices.Add((short)(indexOffset + patch.indices[triangleOffset + j]));
                            indices.Add((short)(indexOffset + patch.indices[triangleOffset + j + 1]));
                            indices.Add((short)(indexOffset + patch.indices[triangleOffset + j + 2]));
                        }

                        else
                        {
                            indices.Add((short)(indexOffset + patch.indices[triangleOffset + j]));
                            indices.Add((short)(indexOffset + patch.indices[triangleOffset + j + 2]));
                            indices.Add((short)(indexOffset + patch.indices[triangleOffset + j + 1]));
                        }
                    }
                }

                indexOffset += patch.vertices.Length;
            }
            return indices.ToArray();
        }
    }

    class Q3Vertex
    {
        Vector3 pos;
        Vector3 nor;
        Vector2 tex;
        Vector2 lm;
        Color col;

        public Vector3 Position
        {
            get { return pos; }
            set { pos = value; }
        }
        public Vector3 Normal
        {
            get { return nor; }
            set { nor = value; }
        }
        public Vector2 TexCoord
        {
            get { return tex; }
            set { tex = value; }
        }
        public Vector2 LmCoord
        {
            get { return lm; }
            set { lm = value; }
        }
        public Color Color
        {
            get { return col; }
            set { col = value; }
        }

        public Q3Vertex()
        {
            pos = new Vector3();
            nor = new Vector3();
            tex = new Vector2();
            lm = new Vector2();
            col = new Color();
        }
        public Q3Vertex(Vector3 pos, Vector3 nor, Vector2 tex, Vector2 lm, Color col)
        {
            this.pos = pos;
            this.nor = nor;
            this.tex = tex;
            this.lm = lm;
            this.col = col;
        }

        public static Vector2 operator *(float f, Q3Vertex v)
        {
            Q3Vertex res = new Q3Vertex();
            res.pos = v.pos * f;
            res.nor = v.nor;
            res.tex = v.tex * f;
            res.lm = v.lm * f;
            res.col = new Color(v.col.R * f, v.col.G * f, v.col.B * f, v.col.A * f);
            return new Vector2(v.X * f, v.Y * f);
        }
        public static Vector2 operator *(Q3Vertex v, float f)
        {
            Q3Vertex res = new Q3Vertex();
            res.pos = v.pos * f;
            res.nor = v.nor;
            res.tex = v.tex * f;
            res.lm = v.lm * f;
            res.col = new Color(v.col.R * f, v.col.G * f, v.col.B * f, v.col.A * f);
            return new Vector2(v.X * f, v.Y * f);
        }
    }
     */
}
