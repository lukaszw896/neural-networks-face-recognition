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
            if (faces.Count < 1)
            {
                MessageBox.Show("Nie ma danych do zapisania");
                return;
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
        }

        private void Load_Pic_Data_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            open.Title = "Open File...";
            open.Filter = "Binary File (*.bin)|*.bin";
            if (open.ShowDialog() == true)
            {
                FileStream fs = new FileStream(open.FileName, FileMode.Open);
                BinaryFormatter bf = new BinaryFormatter();
                BinaryReader br = new BinaryReader(fs);

                faces.Clear();
                faces = (List<Face>)bf.Deserialize(fs);

                fs.Close();
                br.Close();
            }
            Console.WriteLine("wczytano z binarki");
        }
    }
}
