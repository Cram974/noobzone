using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

using Tao.DevIl;

namespace NoobZone
{
    class Image
    {
        static bool first = true;
        
        static byte[] OpenImageDevIL(string filename, ref int w, ref int h, ref int d)
        {
	        
	        if(first) 
            {
		        first = false;

		        // Initalisation de DevIL
		        Il.ilInit();

		        // On indique que l'origine des images se trouve sur le coin haut-gauche
		        Il.ilOriginFunc(Il.IL_ORIGIN_UPPER_LEFT);
		        Il.ilEnable(Il.IL_ORIGIN_SET);

		        Il.ilEnable(Il.IL_TYPE_SET);
		        Il.ilTypeFunc(Il.IL_UNSIGNED_BYTE);
	        }

            // Génération d'une nouvelle texture
            int ilTexture;
            Il.ilGenImages(1, out ilTexture);
            Il.ilBindImage(ilTexture);

            // Chargement de l'image
	        if (!Il.ilLoadImage(filename))
		        return null;


            // Récupération de la taille de l'image
	        w = Il.ilGetInteger(Il.IL_IMAGE_WIDTH);
	        h = Il.ilGetInteger(Il.IL_IMAGE_HEIGHT);
	        d = Il.ilGetInteger(Il.IL_IMAGE_BPP);

	        if(d==4)
		        Il.ilConvertImage(Il.IL_RGBA, Il.IL_UNSIGNED_BYTE);
            else
                Il.ilConvertImage(Il.IL_RGB, Il.IL_UNSIGNED_BYTE);

            // Récupération des pixels de l'image
            IntPtr Pixels = Il.ilGetData();
            byte[] img = new byte[w*h*d];
            for (int i = 0; i < w * h * d; i++)
                img[i] = Marshal.ReadByte(Pixels, i);

            // Suppression de la texture
            Il.ilBindImage(0);
            Il.ilDeleteImages(1, ref ilTexture);

	        return img;
        }
        public static byte[] Load(string filename, ref int w, ref int h, ref int d)
        {
            return OpenImageDevIL(filename, ref w, ref h, ref d);
        }
    }
}
