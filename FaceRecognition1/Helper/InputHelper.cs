using FaceRecognition1.Content;
using Luxand;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaceRecognition1.Helper
{
    /// <summary>
    /// Klasa ktora oblsuguje wszystkie wazne metody
    /// zwiazane z obsluga i analiza zdjec
    /// </summary>
    public class InputHelper
    {
        public static Face FacePreparation(String picDir, String _name, String _folderName, int _index, Face twarz)
        {
            int Image = 0;
            if (FSDK.LoadImageFromFile(ref Image, picDir) != FSDK.FSDKE_OK)
            {
                Console.WriteLine("addSingleFace error !");
            }
            FSDK.TFacePosition FacePosition = new FSDK.TFacePosition();

            if (FSDK.DetectFace(Image, ref FacePosition) != FSDK.FSDKE_OK)
            {
                Console.WriteLine("addSingleFace error !");
            }

            FSDK.TPoint[] FacialFeatures;

            if (FSDK.DetectFacialFeatures(Image, out FacialFeatures) == FSDK.FSDKE_OK)
            {
                twarz.name = _name;
                twarz.folderName = _folderName;
                Console.WriteLine(twarz.name);
                twarz.index = _index;
                List<float> faceFeatures = FeatureConverter.GetFeatures(FacialFeatures);
                twarz.features = faceFeatures;

                if (twarz.ValidateFace() == 1)
                    Console.WriteLine("Wygenerowano dane twarzy");
                else
                    Console.WriteLine("Blad twarzy " + twarz.name);
            }
            else
            {
                Console.WriteLine("addSingleFace error !");
            }
            return twarz;
        }
    }
}
