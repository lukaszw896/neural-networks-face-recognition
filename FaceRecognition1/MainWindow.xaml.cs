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
using FaceRecognition1.Helper;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace FaceRecognition1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public int Image ;
        public List<Face> faces;
        public MainWindow()
        {
            FSDK.ActivateLibrary("GTNWg1l8Zs+7uJixJ+eBiTF9s1Iofc2pc6UYstMf2/l/MRBagDqX8gzNqXtX64KspTPaszn6+/WwtSHOVDPBQ/WRYTeUTlNJmu9p8tFSCEGDsPodYiISTxA4uoAGtS1iZ3eTbqWkrupH0dCKEdTQzLatWNz7QaCBLaTmdZvn+zU=");
            FSDK.InitializeLibrary(); 
            InitializeComponent();
            faces = new List<Face>();         
        }

        private void Load_Pic_Click(object sender, RoutedEventArgs e)
        {
            List<List<string>> imageList = ImageLoader.GetImages();

            for(int i = 0 ; i < imageList.Count; i ++)
            {
                List<string> pictures = imageList[i];
                string folderName = imageList[i][0];
                var folderNames = folderName.Split('\\').ToArray();
                folderName = folderNames[folderNames.Count() - 2];
                for(int j=0; j<pictures.Count; j++)
                {
                    addSingleFace(pictures[j], pictures[j].Substring(pictures[j].LastIndexOf('\\') + 1), folderName, j);
                }
            }
            
            Console.WriteLine("DONE" + faces.Count);
        }

        /// <summary>
        /// Metoda dodaje jedna twarz do listy. 1 jak sie uda
        /// -1 jak sie nie uda
        /// </summary>
        private int addSingleFace(String picDir, String _name, String _folderName, int _index)
        {
            Face twarz = new Face();
            twarz = InputHelper.FacePreparation( picDir, _name, _folderName, _index, twarz);
                       
            faces.Add(twarz);
            return 1;
        }

        private void Save_Pic_Data_Click(object sender, RoutedEventArgs e)
        {
            if (InputHelper.SaveBinary(faces) == 1)
                Console.WriteLine("zapisano do binarki");
        }

        private void Load_Pic_Data_Click(object sender, RoutedEventArgs e)
        {
            faces.Clear();
            faces = InputHelper.LoadBinary();
            Console.WriteLine("wczytano z binarki "+ faces.Count + " danych");
        }
    }
}
