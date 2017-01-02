using FaceRecognition1.Content;
using Luxand;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FaceRecognition1.Helper
{
    /// <summary>
    /// Klasa ktora oblsuguje wszystkie wazne metody
    /// zwiazane z obsluga i analiza zdjec
    /// </summary>
    public class InputHelper
    {
        public static Face FacePreparation(String picDir, String _name, String _folderName, int _index, Face twarz, int _folderIndex)
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
                twarz.networkIndex = _folderIndex;
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

        public static int SaveBinary(List<Face> faces)
        {
            if (faces.Count < 1)
            {
                MessageBox.Show("Nie ma danych do zapisania");
                return -1;
            }

            SaveFileDialog save = new SaveFileDialog();
            save.FileName = "ZdjeciaInput"; // Default file name
            save.DefaultExt = ".bin"; // Default file extension
            save.Title = "Save As...";
            save.Filter = "Binary File (*.bin)|*.bin";
            save.RestoreDirectory = true;
            save.InitialDirectory = System.Reflection.Assembly.GetExecutingAssembly().Location;

            Nullable<bool> result = save.ShowDialog();
            if (result == true)
            {
                string filename = save.FileName;
                FileStream fs = new FileStream(filename, FileMode.Create);
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs, faces);
                BinaryWriter w = new BinaryWriter(fs);
                w.Close();
                fs.Close();
            }
            return 1;
        }

        public static List<Face> LoadBinary()
        {
            List<Face> faces = new List<Face>();
            OpenFileDialog open = new OpenFileDialog();
            open.Title = "Open File...";
            open.Filter = "Binary File (*.bin)|*.bin";
            if (open.ShowDialog() == true)
            {
                FileStream fs = new FileStream(open.FileName, FileMode.Open);
                BinaryFormatter bf = new BinaryFormatter();
                BinaryReader br = new BinaryReader(fs);

                faces = (List<Face>)bf.Deserialize(fs);

                fs.Close();
                br.Close();
            }
            return faces;
        }
        public static int ChooseMode(bool flag, int peopleCount)
        {
            if (flag == true)
                return peopleCount;
            return 0;
        }
    }
}
