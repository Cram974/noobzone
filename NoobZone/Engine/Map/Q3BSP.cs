using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

using System.IO;
using System.Runtime.InteropServices;

using Tao.OpenGl;

namespace NoobZone
{
    class Q3BSP : IRenderable
    {
        #region Attributs
        #region Vertex Buffer Object

        //int IBO_index_id;
        //int[] IBO_index;

        int VBO_id;
        float[] VBO_position;
        float[] VBO_normal;
        float[] VBO_texcoord;
        float[] VBO_lmcoord;
        byte[] VBO_color;

        IntPtr VBO_posOfs;
        IntPtr VBO_norOfs;
        IntPtr VBO_texOfs;
        IntPtr VBO_lmOfs;
        IntPtr VBO_colOfs;
        #endregion

        int[] m_elements;
        Q3Header m_header;
        Q3Lump[] m_lumps;
        string m_entities;
        Q3Shader[] m_shaders;
        Q3Plane[] m_planes;
        Q3Node[] m_nodes;
        Q3Leaf[] m_leaves;
        int[] m_leafFaces;
        int[] m_leafBrushes;
        Q3Model[] m_models;
        Q3Brush[] m_brushes;
        Q3BrushSide[] m_brushSides;
        Q3Face[] m_faces;
        Q3LightMap[] m_lightmaps;
        Q3Vis m_vis;

        Texture2D[] m_lmTextures;
        Texture2D[] m_imgTextures;

        AABB m_box;

        public AABB BoundingBox
        {
            get { return m_box; }
        }
        #endregion

        public Q3BSP(string fileName)
        {
            #region Loading Structures
            BinaryReader br = new BinaryReader(new FileStream(fileName, FileMode.Open));

            #region Load Header
            byte[] buff = br.ReadBytes(Marshal.SizeOf(typeof(Q3Header)));

            GCHandle handle = GCHandle.Alloc(buff, GCHandleType.Pinned);
            m_header = (Q3Header)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(Q3Header));
            handle.Free();
            #endregion

            LoadLumps(br, 8, (int)Q3LumpType.NumLumps);

            #region Load Entities
            LoadEntities(br, m_lumps[(int)Q3LumpType.Entities].Offset,
                             m_lumps[(int)Q3LumpType.Entities].Size);
            #endregion

            #region Load Shaders
            LoadShaders(br, m_lumps[(int)Q3LumpType.Shaders].Offset,
                             m_lumps[(int)Q3LumpType.Shaders].Size /
                             Marshal.SizeOf(typeof(Q3Shader)));
            #endregion

            #region Load Planes
            LoadPlanes(br, m_lumps[(int)Q3LumpType.Planes].Offset,
                              m_lumps[(int)Q3LumpType.Planes].Size /
                              Marshal.SizeOf(typeof(Q3Plane)));
            #endregion

            #region Load Nodes
            LoadNodes(br, m_lumps[(int)Q3LumpType.Nodes].Offset,
                              m_lumps[(int)Q3LumpType.Nodes].Size /
                              Marshal.SizeOf(typeof(Q3Node)));
            #endregion

            #region Load Leaves
            LoadLeaves(br, m_lumps[(int)Q3LumpType.Leaves].Offset,
                              m_lumps[(int)Q3LumpType.Leaves].Size /
                              Marshal.SizeOf(typeof(Q3Leaf)));
            #endregion

            #region Load LeafFaces
            LoadLeafFaces(br, m_lumps[(int)Q3LumpType.LeafFaces].Offset,
                              m_lumps[(int)Q3LumpType.LeafFaces].Size /
                              Marshal.SizeOf(typeof(int)));
            #endregion

            #region Load LeafBrushes
            LoadLeafBrushes(br, m_lumps[(int)Q3LumpType.LeafBrushes].Offset,
                              m_lumps[(int)Q3LumpType.LeafBrushes].Size /
                              Marshal.SizeOf(typeof(int)));
            #endregion

            #region Load Models
            LoadModels(br, m_lumps[(int)Q3LumpType.Models].Offset,
                              m_lumps[(int)Q3LumpType.Models].Size /
                              Marshal.SizeOf(typeof(Q3Model)));
            #endregion

            #region Load Brushes
            LoadBrushes(br, m_lumps[(int)Q3LumpType.Brushes].Offset,
                              m_lumps[(int)Q3LumpType.Brushes].Size /
                              Marshal.SizeOf(typeof(Q3Brush)));
            #endregion

            #region Load BrushSides
            LoadBrushSides(br, m_lumps[(int)Q3LumpType.BrushSides].Offset,
                              m_lumps[(int)Q3LumpType.BrushSides].Size /
                              Marshal.SizeOf(typeof(Q3BrushSide)));
            #endregion

            LoadVertices(br, m_lumps[(int)Q3LumpType.Vertices].Offset, m_lumps[(int)Q3LumpType.Vertices].Size / 44);
            LoadElements(br, m_lumps[(int)Q3LumpType.Elements].Offset, m_lumps[(int)Q3LumpType.Elements].Size / 4);
            LoadFaces(br, m_lumps[(int)Q3LumpType.Faces].Offset, m_lumps[(int)Q3LumpType.Faces].Size / 104);

            #region Load LightMaps
            LoadLightMaps(br, m_lumps[(int)Q3LumpType.Lightmaps].Offset,
                              m_lumps[(int)Q3LumpType.Lightmaps].Size /
                              Marshal.SizeOf(typeof(Q3LightMap)));
            #endregion

            LoadVis(br, m_lumps[(int)Q3LumpType.Visibility].Offset);
            br.Close();
            #endregion

            Create_VBO();
        }

        #region Méthodes de chargement
        void LoadLumps(BinaryReader br, int offset, int n)
        {
            br.BaseStream.Seek(offset, SeekOrigin.Begin);
            m_lumps = new Q3Lump[n];

            for (int i = 0; i < n; i++)
            {
                m_lumps[i] = new Q3Lump(br.ReadInt32(), br.ReadInt32());
            }
        }
        void LoadEntities(BinaryReader br, int offset, int size)
        {
            br.BaseStream.Seek(offset, SeekOrigin.Begin);
            m_entities = Encoding.ASCII.GetString(br.ReadBytes(size));
        }
        void LoadShaders(BinaryReader br, int offset, int n)
        {
            br.BaseStream.Seek(offset, SeekOrigin.Begin);
            m_shaders = new Q3Shader[n];
            m_imgTextures = new Texture2D[n];
            for (int i = 0; i < n; i++)
            {
                byte[] buff = br.ReadBytes(Marshal.SizeOf(typeof(Q3Shader)));

                GCHandle handle = GCHandle.Alloc(buff, GCHandleType.Pinned);
                m_shaders[i] = (Q3Shader)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(Q3Shader));
                handle.Free();
                string texName = (new String(m_shaders[i].Name)).TrimEnd(new char[]{'\0',' '});
                Texture2D.EnableMipMaping = false;

                if (File.Exists(texName + ".jpg"))
                    m_imgTextures[i] = TextureMgr.Load(texName + ".jpg");
                else if (File.Exists(texName + ".tga"))
                    m_imgTextures[i] = TextureMgr.Load(texName + ".tga");
                else
                    m_imgTextures[i] = null;

                Texture2D.EnableMipMaping = true;
            }
        }
        void LoadPlanes(BinaryReader br, int offset, int n)
        {
            br.BaseStream.Seek(offset, SeekOrigin.Begin);
            m_planes = new Q3Plane[n];
            for (int i = 0; i < n; i++)
            {
                byte[] buff = br.ReadBytes(Marshal.SizeOf(typeof(Q3Plane)));

                GCHandle handle = GCHandle.Alloc(buff, GCHandleType.Pinned);
                m_planes[i] = (Q3Plane)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(Q3Plane));
                handle.Free();
            }
        }
        void LoadNodes(BinaryReader br, int offset, int n)
        {
            br.BaseStream.Seek(offset, SeekOrigin.Begin);
            m_nodes = new Q3Node[n];
            for (int i = 0; i < n; i++)
            {
                byte[] buff = br.ReadBytes(Marshal.SizeOf(typeof(Q3Node)));

                GCHandle handle = GCHandle.Alloc(buff, GCHandleType.Pinned);
                m_nodes[i] = (Q3Node)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(Q3Node));
                handle.Free();
            }
        }
        void LoadLeaves(BinaryReader br, int offset, int n)
        {
            br.BaseStream.Seek(offset, SeekOrigin.Begin);
            m_leaves = new Q3Leaf[n];
            for (int i = 0; i < n; i++)
            {
                byte[] buff = br.ReadBytes(Marshal.SizeOf(typeof(Q3Leaf)));

                GCHandle handle = GCHandle.Alloc(buff, GCHandleType.Pinned);
                m_leaves[i] = (Q3Leaf)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(Q3Leaf));
                handle.Free();
            }
        }
        void LoadLeafFaces(BinaryReader br, int offset, int n)
        {
            br.BaseStream.Seek(offset, SeekOrigin.Begin);
            m_leafFaces = new int[n];
            for (int i = 0; i < n; i++)
            {
                byte[] buff = br.ReadBytes(Marshal.SizeOf(typeof(int)));

                GCHandle handle = GCHandle.Alloc(buff, GCHandleType.Pinned);
                m_leafFaces[i] = (int)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(int));
                handle.Free();
            }
        }
        void LoadLeafBrushes(BinaryReader br, int offset, int n)
        {
            br.BaseStream.Seek(offset, SeekOrigin.Begin);
            m_leafBrushes = new int[n];
            for (int i = 0; i < n; i++)
            {
                byte[] buff = br.ReadBytes(Marshal.SizeOf(typeof(int)));

                GCHandle handle = GCHandle.Alloc(buff, GCHandleType.Pinned);
                m_leafBrushes[i] = (int)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(int));
                handle.Free();
            }
        }
        void LoadModels(BinaryReader br, int offset, int n)
        {
            br.BaseStream.Seek(offset, SeekOrigin.Begin);
            m_models = new Q3Model[n];
            for (int i = 0; i < n; i++)
            {
                byte[] buff = br.ReadBytes(Marshal.SizeOf(typeof(Q3Model)));

                GCHandle handle = GCHandle.Alloc(buff, GCHandleType.Pinned);
                m_models[i] = (Q3Model)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(Q3Model));
                handle.Free();
            }
        }
        void LoadBrushes(BinaryReader br, int offset, int n)
        {
            br.BaseStream.Seek(offset, SeekOrigin.Begin);
            m_brushes = new Q3Brush[n];
            for (int i = 0; i < n; i++)
            {
                byte[] buff = br.ReadBytes(Marshal.SizeOf(typeof(Q3Brush)));

                GCHandle handle = GCHandle.Alloc(buff, GCHandleType.Pinned);
                m_brushes[i] = (Q3Brush)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(Q3Brush));
                handle.Free();
            }
        }
        void LoadBrushSides(BinaryReader br, int offset, int n)
        {
            br.BaseStream.Seek(offset, SeekOrigin.Begin);
            m_brushSides = new Q3BrushSide[n];
            for (int i = 0; i < n; i++)
            {
                byte[] buff = br.ReadBytes(Marshal.SizeOf(typeof(Q3BrushSide)));

                GCHandle handle = GCHandle.Alloc(buff, GCHandleType.Pinned);
                m_brushSides[i] = (Q3BrushSide)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(Q3BrushSide));
                handle.Free();
            }
        }
        void LoadVertices(BinaryReader br, int offset, int n)
        {
            br.BaseStream.Seek(offset, SeekOrigin.Begin);
            VBO_position = new float[n * 3];
            VBO_normal = new float[n * 3];
            VBO_texcoord = new float[n * 2];
            VBO_lmcoord = new float[n * 2];
            VBO_color = new byte[n * 4];
            m_box = new AABB();
            for (int i = 0; i < n; i++)
            {
                VBO_position[i * 3] = br.ReadSingle();
                VBO_position[i * 3 + 1] = br.ReadSingle();
                VBO_position[i * 3 + 2] = br.ReadSingle();

                m_box.AddVertex(new Vector3(VBO_position[i * 3], VBO_position[i * 3 + 1], VBO_position[i * 3 + 2]));

                VBO_texcoord[i * 2] = br.ReadSingle();
                VBO_texcoord[i * 2 + 1] = br.ReadSingle();

                VBO_lmcoord[i * 2] = br.ReadSingle();
                VBO_lmcoord[i * 2 + 1] = br.ReadSingle();

                VBO_normal[i * 3] = br.ReadSingle();
                VBO_normal[i * 3 + 1] = br.ReadSingle();
                VBO_normal[i * 3 + 2] = br.ReadSingle();

                VBO_color[i * 4] = br.ReadByte();
                VBO_color[i * 4 + 1] = br.ReadByte();
                VBO_color[i * 4 + 2] = br.ReadByte();
                VBO_color[i * 4 + 3] = br.ReadByte();
            }
        }
        void LoadElements(BinaryReader br, int offset, int n)
        {
            br.BaseStream.Seek(offset, SeekOrigin.Begin);
            m_elements = new int[n];
            for (int i = 0; i < n; i++)
            {
                m_elements[i] = br.ReadInt32();
            }
        }
        void LoadFaces(BinaryReader br, int offset, int n)
        {
            br.BaseStream.Seek(offset, SeekOrigin.Begin);
            m_faces = new Q3Face[n];
            for (int i = 0; i < n; i++)
            {
                byte[] buff = br.ReadBytes(Marshal.SizeOf(typeof(Q3Face)));

                GCHandle handle = GCHandle.Alloc(buff, GCHandleType.Pinned);
                m_faces[i] = (Q3Face)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(Q3Face));
                handle.Free();
            }
        }
        void LoadLightMaps(BinaryReader br, int offset, int n)
        {
            br.BaseStream.Seek(offset, SeekOrigin.Begin);
            m_lightmaps = new Q3LightMap[n];
            m_lmTextures = new Texture2D[n];

            for (int i = 0; i < n; i++)
            {
                byte[] buff = br.ReadBytes(Marshal.SizeOf(typeof(Q3LightMap)));

                GCHandle handle = GCHandle.Alloc(buff, GCHandleType.Pinned);
                m_lightmaps[i] = (Q3LightMap)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(Q3LightMap));
                handle.Free();

                m_lmTextures[i] = new Texture2D(128, 128, 3, m_lightmaps[i].Data, 1f);
            }
        }
        void LoadVis(BinaryReader br, int offset)
        {
            br.BaseStream.Seek(offset, SeekOrigin.Begin);
            m_vis = new Q3Vis();
            m_vis.ClusterCount = br.ReadInt32();
            m_vis.RowSize = br.ReadInt32();
            m_vis.Data = br.ReadBytes(m_lumps[(int)Q3LumpType.Visibility].Size - (Marshal.SizeOf(typeof(int)) * 2));
        }
        #endregion

        #region Méthodes de Rendu
        void Create_VBO()
        {
            VBO_id = 0;
            int posSize = VBO_position.Length * Marshal.SizeOf(typeof(float));
            int norSize = VBO_normal.Length * Marshal.SizeOf(typeof(float));
            int texSize = VBO_texcoord.Length * Marshal.SizeOf(typeof(float));
            int lmSize = VBO_lmcoord.Length * Marshal.SizeOf(typeof(float));
            int colSize = VBO_color.Length * Marshal.SizeOf(typeof(byte));

            VBO_posOfs = new IntPtr(0);
            VBO_norOfs = new IntPtr(posSize);
            VBO_texOfs = new IntPtr(posSize + norSize);
            VBO_lmOfs = new IntPtr(posSize + norSize + texSize);
            VBO_colOfs = new IntPtr(posSize + norSize + texSize + lmSize);

            Gl.glGenBuffersARB(1, out VBO_id);

            Gl.glBindBuffer(Gl.GL_ARRAY_BUFFER, VBO_id);
            Gl.glBufferData(Gl.GL_ARRAY_BUFFER, new IntPtr(posSize + norSize + texSize + lmSize + colSize), null, Gl.GL_STREAM_DRAW);

            Gl.glBufferSubData(Gl.GL_ARRAY_BUFFER_ARB, VBO_posOfs, new IntPtr(posSize), VBO_position);
            Gl.glBufferSubData(Gl.GL_ARRAY_BUFFER_ARB, VBO_norOfs, new IntPtr(norSize), VBO_normal);
            Gl.glBufferSubData(Gl.GL_ARRAY_BUFFER_ARB, VBO_texOfs, new IntPtr(texSize), VBO_texcoord);
            Gl.glBufferSubData(Gl.GL_ARRAY_BUFFER_ARB, VBO_lmOfs, new IntPtr(lmSize), VBO_lmcoord);
            Gl.glBufferSubData(Gl.GL_ARRAY_BUFFER_ARB, VBO_colOfs, new IntPtr(colSize), VBO_color);

            Gl.glBindBuffer(Gl.GL_ARRAY_BUFFER, 0);
        }
        void Start_VBO()
        {
            Gl.glBindBuffer(Gl.GL_ARRAY_BUFFER, VBO_id);

            Gl.glEnableClientState(Gl.GL_VERTEX_ARRAY);
            Gl.glVertexPointer(3, Gl.GL_FLOAT, 0, VBO_posOfs);

            //Gl.glEnableClientState(Gl.GL_NORMAL_ARRAY);
            //Gl.glVertexPointer(3, Gl.GL_FLOAT, 0, VBO_norOfs);

            Gl.glClientActiveTextureARB(Gl.GL_TEXTURE0);
            Gl.glEnableClientState(Gl.GL_TEXTURE_COORD_ARRAY);
            Gl.glTexCoordPointer(2, Gl.GL_FLOAT, 0, VBO_texOfs);

            Gl.glClientActiveTextureARB(Gl.GL_TEXTURE1);
            Gl.glEnableClientState(Gl.GL_TEXTURE_COORD_ARRAY);
            Gl.glTexCoordPointer(2, Gl.GL_FLOAT, 0, VBO_lmOfs);

            Gl.glEnableClientState(Gl.GL_COLOR_ARRAY);
            Gl.glColorPointer(4, Gl.GL_UNSIGNED_BYTE, 0, VBO_colOfs);
        }
        void Stop_VBO()
        {
            Gl.glDisableClientState(Gl.GL_COLOR_ARRAY);

            Gl.glClientActiveTexture(Gl.GL_TEXTURE1);
            Gl.glDisableClientState(Gl.GL_TEXTURE_COORD_ARRAY);

            Gl.glClientActiveTexture(Gl.GL_TEXTURE0);
            Gl.glDisableClientState(Gl.GL_TEXTURE_COORD_ARRAY);

            //Gl.glDisableClientState(Gl.GL_NORMAL_ARRAY);

            Gl.glDisableClientState(Gl.GL_VERTEX_ARRAY);

            Gl.glBindBuffer(Gl.GL_ARRAY_BUFFER, 0);
        }

        int FindLeaf()
        {
            int index = 0;

            while (index >= 0)
            {
                Q3Node node = m_nodes[index];
                Q3Plane plane = m_planes[node.Plane];

                // Distance from point to a plane
                float distance = (new Vector3(plane.Normal)).Dot(Camera.Position) - plane.Distance;

                if (distance >= 0)
                    index = node.Front;
                else
                    index = node.Back;
            }

            return ~index;
        }
        bool IsClusterVisible(int current, int test)
        {
	        if(current < 0) return true;
	        byte visSet = m_vis.Data[(current*m_vis.RowSize) + (test / 8)];
	        int result = visSet & (1 << ((test) & 7));
            return (result != 0);
        }

        void RenderPolygonFace(Q3Face face)
        {
            uint[] indexes = new uint[face.ElemCount];
            for (int i = 0; i < face.ElemCount; i++)
                indexes[i] = (uint)m_elements[face.ElemStart + i] + (uint)face.VertStart;


            Gl.glDrawElements(Gl.GL_TRIANGLES, face.ElemCount, Gl.GL_UNSIGNED_INT, indexes);
        }
        void RenderFace(Q3Face face)
        {
            Q3Shader shader = m_shaders[face.Shader];

            if (!TestSurfaceflag(shader.SurfaceFlags, Q3SurfaceFlags.Sky))
            if (!TestSurfaceflag(shader.SurfaceFlags, Q3SurfaceFlags.NonSolid))
            if (!TestSurfaceflag(shader.SurfaceFlags, Q3SurfaceFlags.NoDraw))
            if (!TestSurfaceflag(shader.SurfaceFlags, Q3SurfaceFlags.Skip))
            if (!TestContentflag(shader.ContentFlags, Q3ContentFlags.Translucent))
            switch (face.Type)
            {
                
                case Q3FaceType.Normal:
                    Gl.glAlphaFunc(Gl.GL_GREATER, 0f);
                    RenderPolygonFace(face);
                    break;
                case Q3FaceType.Mesh:
                    Gl.glAlphaFunc(Gl.GL_GREATER, 0.4f);
                    Gl.glDisable(Gl.GL_CULL_FACE);
                    RenderPolygonFace(face);
                    Gl.glEnable(Gl.GL_CULL_FACE);
                    break;
                case Q3FaceType.Flare:
                    break;
                case Q3FaceType.Patch:
                    break;
            }

        }

        bool TestSurfaceflag(ValueType totest, ValueType flag)
        {
            return ((int)totest & (int)flag) == (int)flag;
        }
        bool TestContentflag(ValueType totest, ValueType flag)
        {
            return ((uint)totest & (uint)flag) == (uint)flag;
        }

        public override void Render()
        {
            int CameraLeaf = FindLeaf();
            int CameraCluster = m_leaves[CameraLeaf].Cluster;

            bool[] visibleFaces = new bool[m_faces.Length];
            ArrayList facesToDraw = new ArrayList();

            for (int i = 0; i < m_leaves.Length; i++)
                if (IsClusterVisible(CameraCluster, m_leaves[i].Cluster))
                    if (Camera.Frustrum.BoxInFrustrum(new AABB(m_leaves[i].BBox)))
                        for (int j = 0; j < m_leaves[i].FaceCount; j++)
                        {
                            int faceIndex = m_leafFaces[m_leaves[i].FaceStart + j];
                            if (!visibleFaces[faceIndex])
                            {
                                facesToDraw.Add(m_faces[faceIndex]);
                                visibleFaces[faceIndex] = true;
                            }
                        }

            facesToDraw.Sort(new Q3FaceComparer());
            int texIndex = -999;
            int lmIndex = -999;

            
            

            

            
            Start_VBO();
            Gl.glShadeModel(Gl.GL_SMOOTH);
            Gl.glEnable(Gl.GL_ALPHA_TEST);
            Gl.glEnable(Gl.GL_CULL_FACE);
            Gl.glCullFace(Gl.GL_BACK);
            Gl.glFrontFace(Gl.GL_CW);
            foreach (Q3Face f in facesToDraw)
            {
                if (f.Shader != texIndex)
                {
                    texIndex = f.Shader;
                    Gl.glActiveTexture(Gl.GL_TEXTURE0_ARB);
                    Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0);
                    if (m_imgTextures[texIndex] != null)
                    {
                        m_imgTextures[texIndex].Bind(0);
                    }
                }
                if (f.LmTexture != lmIndex)
                {
                    lmIndex = f.LmTexture;
                    Gl.glActiveTexture(Gl.GL_TEXTURE1_ARB);
                    Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0);

                    Gl.glEnableClientState(Gl.GL_COLOR_ARRAY);
                    Gl.glColorPointer(4, Gl.GL_UNSIGNED_BYTE, 0, VBO_colOfs);

                    if (0 <= lmIndex && lmIndex <= m_lmTextures.Length)
                    {
                        if (m_lmTextures[(uint)lmIndex] != null)
                        {
                            m_lmTextures[lmIndex].Bind(1);
                            Gl.glDisableClientState(Gl.GL_COLOR_ARRAY);
                        }
                    }
                }
                RenderFace(f);
            }
            Gl.glActiveTexture(Gl.GL_TEXTURE1_ARB);
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0);

            Gl.glActiveTexture(Gl.GL_TEXTURE0_ARB);
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0);

            Gl.glDisable(Gl.GL_TEXTURE_2D);
            Gl.glDisable(Gl.GL_ALPHA_TEST);
            Gl.glDisable(Gl.GL_CULL_FACE);
            Gl.glShadeModel(Gl.GL_FLAT);

            Stop_VBO();
            
            
            
            

            

            
            
        }
        #endregion

        #region Méthodes de Collision
        public Q3CollisionData TraceRay(Vector3 startPosition, Vector3 endPosition)
        {
            Q3CollisionData collision = new Q3CollisionData();
            collision.type = Q3CollisionType.Ray;
            return Trace(startPosition, endPosition, ref collision);
        }
        public Q3CollisionData TraceSphere(Vector3 startPosition, Vector3 endPosition, float sphereRadius)
        {
            Q3CollisionData collision = new Q3CollisionData();
            collision.type = Q3CollisionType.Sphere;
            collision.sphereRadius = sphereRadius;
            return Trace(startPosition, endPosition, ref collision);
        }
        public Q3CollisionData TraceBox(Vector3 startPosition, Vector3 endPosition, AABB bbox)
        {
            Q3CollisionData collision = new Q3CollisionData();
            Vector3 boxMinimums = bbox.Min;
            Vector3 boxMaximums = bbox.Max;

            if (boxMinimums.X == 0 && boxMinimums.Y == 0 && boxMinimums.Z == 0 && boxMaximums.X == 0 && boxMaximums.Y == 0 && boxMaximums.Z == 0)
            {
                collision.type = Q3CollisionType.Ray;
                return Trace(startPosition, endPosition, ref collision);
            }

            if (boxMaximums.X < boxMinimums.X)
            {
                float x = boxMaximums.X;
                boxMaximums.X = boxMinimums.X;
                boxMinimums.X = x;
            }
            if (boxMaximums.Y < boxMinimums.Y)
            {
                float y = boxMaximums.Y;
                boxMaximums.Y = boxMinimums.Y;
                boxMinimums.Y = y;
            }
            if (boxMaximums.Z < boxMinimums.Z)
            {
                float z = boxMaximums.Z;
                boxMaximums.Z = boxMinimums.Z;
                boxMinimums.Z = z;
            }

            Vector3 boxExtents = new Vector3();
            boxExtents.X = Math.Max(Math.Abs(boxMaximums.X), Math.Abs(boxMinimums.X));
            boxExtents.Y = Math.Max(Math.Abs(boxMaximums.Y), Math.Abs(boxMinimums.Y));
            boxExtents.Z = Math.Max(Math.Abs(boxMaximums.Z), Math.Abs(boxMinimums.Z));

            collision.type = Q3CollisionType.Box;
            collision.boxMinimums = boxMinimums;
            collision.boxMaximums = boxMaximums;
            collision.boxExtents = boxExtents;
            return Trace(startPosition, endPosition, ref collision);
        }
        private Q3CollisionData Trace(Vector3 startPosition, Vector3 endPosition, ref Q3CollisionData collision)
        {
            collision.startOutside = true;
            collision.inSolid = false;
            collision.ratio = 1.0f;
            collision.startPosition = startPosition;
            collision.endPosition = endPosition;
            collision.collisionPoint = startPosition;

            WalkNode(0, 0.0f, 1.0f, startPosition, endPosition, ref collision);

            if (1.0f == collision.ratio)
            {
                collision.collisionPoint = endPosition;
            }
            else
            {
                collision.collisionPoint = startPosition + (collision.ratio - 0.002f) * (endPosition - startPosition);
            }

            return collision;
        }
        private void WalkNode(int nodeIndex, float startRatio, float endRatio, Vector3 startPosition, Vector3 endPosition, ref Q3CollisionData cd)
        {
            // Is this a leaf?
            if (0 > nodeIndex)
            {
                Q3Leaf leaf = m_leaves[~nodeIndex];
                for (int i = 0; i < leaf.BrushCount; i++)
                {
                    Q3Brush brush = m_brushes[m_leafBrushes[leaf.BrushStart + i]];
                    if (0 < brush.NumSides &&
                        1 == ((int)m_shaders[brush.ShaderIndex].ContentFlags & 1))
                    {
                        CheckBrush(ref brush, ref cd);
                    }
                }

                return;
            }

            // This is a node
            Q3Node thisNode = m_nodes[nodeIndex];
            Q3Plane thisPlane = m_planes[thisNode.Plane];
            float startDistance = startPosition.Dot( new Vector3(thisPlane.Normal) ) - thisPlane.Distance;
            float endDistance = endPosition.Dot( new Vector3(thisPlane.Normal) ) - thisPlane.Distance;
            float offset = 0;

            // Set offset for sphere-based collision
            if (cd.type == Q3CollisionType.Sphere)
            {
                offset = cd.sphereRadius;
            }

            // Set offest for box-based collision
            if (cd.type == Q3CollisionType.Box)
            {
                offset = Math.Abs(cd.boxExtents.X * (new Vector3(thisPlane.Normal)).X) + Math.Abs(cd.boxExtents.Y * (new Vector3(thisPlane.Normal)).Y) + Math.Abs(cd.boxExtents.Z * (new Vector3(thisPlane.Normal)).Z);
            }

            if (startDistance >= offset && endDistance >= offset)
            {
                // Both points are in front
                WalkNode(thisNode.Front, startRatio, endRatio, startPosition, endPosition, ref cd);
            }
            else if (startDistance < -offset && endDistance < -offset)
            {
                WalkNode(thisNode.Back, startRatio, endRatio, startPosition, endPosition, ref cd);
            }
            else
            {
                // The line spans the splitting plane
                int side = 0;
                float fraction1 = 0.0f;
                float fraction2 = 0.0f;
                float middleFraction = 0.0f;
                Vector3 middlePosition = new Vector3();

                if (startDistance < endDistance)
                {
                    side = 1;
                    float inverseDistance = 1.0f / (startDistance - endDistance);
                    fraction1 = (startDistance - offset + Q3Constants.Epsilon) * inverseDistance;
                    fraction2 = (startDistance + offset + Q3Constants.Epsilon) * inverseDistance;
                }
                else if (endDistance < startDistance)
                {
                    side = 0;
                    float inverseDistance = 1.0f / (startDistance - endDistance);
                    fraction1 = (startDistance + offset + Q3Constants.Epsilon) * inverseDistance;
                    fraction2 = (startDistance - offset - Q3Constants.Epsilon) * inverseDistance;
                }
                else
                {
                    side = 0;
                    fraction1 = 1.0f;
                    fraction2 = 0.0f;
                }

                if (fraction1 < 0.0f) fraction1 = 0.0f;
                else if (fraction1 > 1.0f) fraction1 = 1.0f;
                if (fraction2 < 0.0f) fraction2 = 0.0f;
                else if (fraction2 > 1.0f) fraction2 = 1.0f;

                middleFraction = startRatio + (endRatio - startRatio) * fraction1;
                middlePosition = startPosition + fraction1 * (endPosition - startPosition);

                int side1;
                int side2;
                if (0 == side)
                {
                    side1 = thisNode.Front;
                    side2 = thisNode.Back;
                }
                else
                {
                    side1 = thisNode.Back;
                    side2 = thisNode.Front;
                }

                WalkNode(side1, startRatio, middleFraction, startPosition, middlePosition, ref cd);

                middleFraction = startRatio + (endRatio - startRatio) * fraction2;
                middlePosition = startPosition + fraction2 * (endPosition - startPosition);

                WalkNode(side2, middleFraction, endRatio, middlePosition, endPosition, ref cd);
            }
        }
        private void CheckBrush(ref Q3Brush brush, ref Q3CollisionData cd)
        {
            float startFraction = -1.0f;
            float endFraction = 1.0f;
            bool startsOut = false;
            bool endsOut = false;

            for (int i = 0; i < brush.NumSides; i++)
            {
                Q3BrushSide brushSide = m_brushSides[brush.FirstSide + i];
                Q3Plane plane = m_planes[brushSide.PlaneNum];

                float startDistance = 0, endDistance = 0;

                if (cd.type == Q3CollisionType.Ray)
                {
                    startDistance = cd.startPosition.Dot(new Vector3( plane.Normal) ) - plane.Distance;
                    endDistance = cd.endPosition.Dot(new Vector3(plane.Normal) ) - plane.Distance;
                }

                else if (cd.type == Q3CollisionType.Sphere)
                {
                    startDistance = cd.startPosition.Dot(new Vector3( plane.Normal)) - (plane.Distance + cd.sphereRadius);
                    endDistance = cd.endPosition.Dot(new Vector3( plane.Normal)) - (plane.Distance + cd.sphereRadius);
                }

                else if (cd.type == Q3CollisionType.Box)
                {
                    Vector3 offset = new Vector3();
                    if ((new Vector3(plane.Normal)).X < 0)
                        offset.X = cd.boxMaximums.X;
                    else
                        offset.X = cd.boxMinimums.X;

                    if ((new Vector3(plane.Normal)).Y < 0)
                        offset.Y = cd.boxMaximums.Y;
                    else
                        offset.Y = cd.boxMinimums.Y;

                    if ((new Vector3(plane.Normal)).Z < 0)
                        offset.Z = cd.boxMaximums.Z;
                    else
                        offset.Z = cd.boxMinimums.Z;

                    startDistance = (cd.startPosition.X + offset.X) * (new Vector3(plane.Normal)).X +
                                    (cd.startPosition.Y + offset.Y) * (new Vector3(plane.Normal)).Y +
                                    (cd.startPosition.Z + offset.Z) * (new Vector3(plane.Normal)).Z -
                                    plane.Distance;

                    endDistance = (cd.endPosition.X + offset.X) * (new Vector3(plane.Normal)).X +
                                  (cd.endPosition.Y + offset.Y) * (new Vector3(plane.Normal)).Y +
                                  (cd.endPosition.Z + offset.Z) * (new Vector3(plane.Normal)).Z -
                                  plane.Distance;
                }

                if (startDistance > 0)
                    startsOut = true;
                if (endDistance > 0)
                    endsOut = true;

                if (startDistance > 0 && endDistance > 0)
                {
                    return;
                }

                if (startDistance <= 0 && endDistance <= 0)
                {
                    continue;
                }

                if (startDistance > endDistance)
                {
                    float fraction = (startDistance - Q3Constants.Epsilon) / (startDistance - endDistance);
                    if (fraction > startFraction)
                        startFraction = fraction;
                }
                else
                {
                    float fraction = (startDistance + Q3Constants.Epsilon) / (startDistance - endDistance);
                    if (fraction < endFraction)
                        endFraction = fraction;
                }
            }

            if (false == startsOut)
            {
                cd.startOutside = false;
                if (false == endsOut)
                    cd.inSolid = true;

                return;
            }

            if (startFraction < endFraction)
            {
                if (startFraction > -1.0f && startFraction < cd.ratio)
                {
                    if (startFraction < 0)
                        startFraction = 0;
                    cd.ratio = startFraction;
                }
            }
        }
        #endregion
    }

    #region Structures Quake III
    public enum Q3LumpType
    {
        Entities = 0,
        Shaders,
        Planes,
        Nodes,
        Leaves,
        LeafFaces,
        LeafBrushes,
        Models,
        Brushes,
        BrushSides,
        Vertices,
        Elements,
        Fog,
        Faces,
        Lightmaps,
        LightVolumes,
        Visibility,
        NumLumps
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Q3Header
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public char[] Magic;
        public int Version;
    }

    public class Q3Lump
    {
        public Q3Lump(int ofs, int size)
        {
            Offset = ofs;
            Size = size;
        }
        public int Offset;
        public int Size;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Q3Plane
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public float[] Normal;//Normal Du Plan
        public float Distance;//Distance plan/origine
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Q3Model
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
        public float[] BBox;//Bounding Box
        public int FaceStart;//Premiere face du tableau
        public int FaceCount;//Nombre de face du tableau
        public int BrushStart;//Premier brush du tableau
        public int BrushCount;//Nombre de Brush
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Q3Node
    {
        public int Plane;//Plan Separateur
        public int Front;//Fils gauche: indice du plan en face du separateur
        public int Back;//Fils droit: indice du plan deriere le separateur
        //Les indices négatifs indiquent les leaves

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
        public int[] BBox;//Bounding Box
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Q3Leaf
    {
        public int Cluster;
        public int Area;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
        public int[] BBox;
        public int FaceStart;
        public int FaceCount;
        public int BrushStart;
        public int BrushCount;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Q3Shader
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
        public char[] Name;
        public Q3SurfaceFlags SurfaceFlags;
        public Q3ContentFlags ContentFlags;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Q3Vis
    {
        public int ClusterCount;
        public int RowSize;
        public byte[] Data;
    }

    [StructLayout(LayoutKind.Sequential)]
    public class Q3Face
    {
        public int Shader;
        public int Unknown;
        public Q3FaceType Type;
        public int VertStart;
        public int VertCount;
        public int ElemStart;
        public int ElemCount;
        public int LmTexture;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public int[] LmOffset;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public int[] LmSize;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public float[] Origin;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
        public float[] BBox;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public float[] Normal;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public int[] MeshCtrl;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Q3BrushSide
    {
        public int PlaneNum;
        public int Content;//?Shader
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Q3Brush
    {
        public int FirstSide;
        public int NumSides;
        public int ShaderIndex;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Q3LightMap
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128 * 128 * 3)]
        public byte[] Data;
    }

    [Flags]
    public enum Q3ContentFlags : uint
    {
        /// <summary>An eye is never valid in a solid.</summary>
        Solid = 1,
        Lava = 8,
        Slime = 16,
        Water = 32,
        Fog = 64,

        AreaPortal = 0x8000,
        PlayerClip = 0x10000,
        MonsterClip = 0x20000,

        /// <summary>Bot specific.</summary>
        Teleporter = 0x40000,
        /// <summary>Bot specific.</summary>
        JumpPad = 0x80000,
        /// <summary>Bot specific.</summary>
        ClusterPortal = 0x100000,
        /// <summary>Bot specific.</summary>
        DoNotEnter = 0x200000,

        /// <summary>Removed before bsping an entity.</summary>
        Origin = 0x1000000,

        /// <summary>Should never be on a brush, only in game.</summary>
        Body = 0x2000000,
        Corpse = 0x4000000,
        /// <summary>Brushes not used for the bsp.</summary>
        Detail = 0x8000000,
        /// <summary>Brushes used for the bsp.</summary>
        Structural = 0x10000000,
        /// <summary>Don't consume surface fragments inside.</summary>
        Translucent = 0x20000000,
        Trigger = 0x40000000,
        /// <summary>Don't leave bodies or items (death fog, lava).</summary>
        NoDrop = 0x80000000
    }

    [Flags]
    public enum Q3SurfaceFlags
    {
        /// <summary>Never give falling damage.</summary>
        NoDamage = 0x1,
        /// <summary>Effects game physics.</summary>
        Slick = 0x2,
        /// <summary>Lighting from environment map.</summary>
        Sky = 0x4,
        Ladder = 0x8,
        /// <summary>Don't make missile explosions.</summary>
        NoImpact = 0x10,
        /// <summary>Don't leave missile marks.</summary>
        NoMarks = 0x20,
        /// <summary>Make flesh sounds and effects.</summary>
        Flesh = 0x40,
        /// <summary>Don't generate a drawsurface at all.</summary>
        NoDraw = 0x80,
        /// <summary>Make a primary bsp splitter.</summary>
        Hint = 0x100,
        /// <summary>Completely ignore, allowing non-closed brushes.</summary>
        Skip = 0x200,
        /// <summary>Surface doesn't need a lightmap.</summary>
        NoLightmap = 0x400,
        /// <summary>Generate lighting info at vertexes.</summary>
        PointLight = 0x800,
        /// <summary>Clanking footsteps.</summary>
        MetalSteps = 0x1000,
        /// <summary>No footstep sounds.</summary>
        NoSteps = 0x2000,
        /// <summary>Don't collide against curves with this set.</summary>
        NonSolid = 0x4000,
        /// <summary>Act as a light filter during q3map -light.</summary>
        LightFilter = 0x8000,
        /// <summary>Do per-pixel light shadow casting in q3map.</summary>
        AlphaShadow = 0x10000,
        /// <summary>Don't dlight even if solid (solid lava, skies).</summary>
        NoDLight = 0x20000
    }

    public enum Q3FaceType
    {
        Normal = 1,
        Patch = 2,
        Mesh = 3,
        Flare = 4
    }

    public struct Q3CollisionData
    {
        public Q3CollisionType type;
        public float ratio;
        public Vector3 collisionPoint;
        public bool startOutside;
        public bool inSolid;
        public Vector3 startPosition;
        public Vector3 endPosition;
        public float sphereRadius;
        public Vector3 boxMinimums;
        public Vector3 boxMaximums;
        public Vector3 boxExtents;
    }

    public enum Q3CollisionType
    {
        /// <summary>
        /// Collision test preformed with a 2D ray projecting from the start point.
        /// </summary>
        Ray,
        /// <summary>
        /// Collision test preformed with a sphere centered around the start point.
        /// </summary>
        Sphere,
        /// <summary>
        /// Collision test preformed with a box centered around the start point.
        /// </summary>
        Box
    }
    enum Q3ColliderType
    {
        Ray = 0,
        Sphere = 1,
        Box = 2
    }

    class Q3FaceComparer : IComparer
    {
        #region IComparer Members
        int IComparer.Compare(object x, object y)
        {
            Q3Face face1 = (Q3Face)x;
            Q3Face face2 = (Q3Face)y;

            if (face1.Shader != face2.Shader)
                return (face1.Shader - face2.Shader);
            else
                return (face1.LmTexture - face2.LmTexture);
        }
        #endregion
    }

    class Q3Constants
    {
        public const float Epsilon = 0.03125f;
        public const float scale = 4.0f;
    }

    #endregion
}
