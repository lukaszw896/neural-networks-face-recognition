using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Luxand;
using Microsoft.Win32;
using FaceRecognition1.Content;

namespace FaceRecognition1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public int Image ;

        public MainWindow()
        {
            FSDK.ActivateLibrary("GTNWg1l8Zs+7uJixJ+eBiTF9s1Iofc2pc6UYstMf2/l/MRBagDqX8gzNqXtX64KspTPaszn6+/WwtSHOVDPBQ/WRYTeUTlNJmu9p8tFSCEGDsPodYiISTxA4uoAGtS1iZ3eTbqWkrupH0dCKEdTQzLatWNz7QaCBLaTmdZvn+zU=");
            FSDK.InitializeLibrary(); 
            InitializeComponent();
        }

        private void Load_Pic_Click(object sender, RoutedEventArgs e)
        {
            Face twarz = new Face();

            String pic_adr = "";
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                pic_adr = openFileDialog.FileName;
                if (pic_adr.Length > 1)
                    Console.WriteLine("Zdjecie wybrane");
            }

            if (FSDK.LoadImageFromFile(ref Image, pic_adr) == FSDK.FSDKE_OK)
                Console.WriteLine("Wczytano zdjecie");

            FSDK.TFacePosition FacePosition = new FSDK.TFacePosition();

            if (FSDK.DetectFace(Image, ref FacePosition) == FSDK.FSDKE_OK)
                Console.WriteLine("wykryto twarz");

            FSDK.TPoint[] FacialFeatures;

            if (FSDK.DetectFacialFeatures(Image, out FacialFeatures) == FSDK.FSDKE_OK)
            {
                twarz.name = openFileDialog.SafeFileName;
                Console.WriteLine(twarz.name);
                twarz.index = 0;
                twarz.networkIndex = 0;
                List<float> faceFeatures = FeatureConverter.GetFeatures(FacialFeatures);
                twarz.features = faceFeatures;

                if (twarz.ValidateFace() == 1)
                    Console.WriteLine("Wygenerowano dane twarzy");
                else
                    Console.WriteLine("Blad twarzy "+twarz.name);

            }
        }
    }
}
