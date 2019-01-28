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
using Encog.Neural.NeuralData;
using Encog.Neural.Data.Basic;
using Encog.Neural.Networks.Training;
using FaceRecognition1.Genetic;
using Encog.Engine.Network.Activation;

namespace FaceRecognition1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        IActivationFunction ActivationFunction { get; set; }
        public InputClass inputData;
        public int Image;
        public List<Face> faces;
        public int peopleNumber;
        public ITrain learnedNetwork;
        public MainWindow()
        {
            FSDK.ActivateLibrary("GTNWg1l8Zs+7uJixJ+eBiTF9s1Iofc2pc6UYstMf2/l/MRBagDqX8gzNqXtX64KspTPaszn6+/WwtSHOVDPBQ/WRYTeUTlNJmu9p8tFSCEGDsPodYiISTxA4uoAGtS1iZ3eTbqWkrupH0dCKEdTQzLatWNz7QaCBLaTmdZvn+zU=");
            FSDK.InitializeLibrary();
            InitializeComponent();
            faces = new List<Face>();
            InitialSettings();
        }

        public void InitialSettings()
        {
            CBAktywacje.SelectedIndex = 2;
            CBObciazenie.SelectedIndex = 1;
            CBSets.SelectedIndex = 0;
            CBLastLayer.SelectedIndex = 0;
        }

        private async void Load_Pic_Click(object sender, RoutedEventArgs e)
        {
            BlakWait.Visibility = Visibility.Visible;
            faces.Clear();

            List<List<string>> imageList = ImageLoader.GetImages();
            int folderIndex = 0;
            await Task.Run(() =>
            {
                for (int i = 0; i < imageList.Count; i++)
                {
                    folderIndex = i;
                    List<string> pictures = imageList[i];
                    string folderName = imageList[i][0];
                    var folderNames = folderName.Split('\\').ToArray();
                    folderName = folderNames[folderNames.Count() - 2];
                    for (int j = 0; j < pictures.Count; j++)
                    {
                        addSingleFace(pictures[j], pictures[j].Substring(pictures[j].LastIndexOf('\\') + 1), folderName, j, folderIndex);
                    }
                }
                peopleNumber = imageList.Count;
            });
            Console.WriteLine("DONE" + faces.Count);
            BlakWait.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Metoda dodaje jedna twarz do listy. 1 jak sie uda
        /// -1 jak sie nie uda
        /// </summary>
        private int addSingleFace(String picDir, String _name, String _folderName, int _index, int _folderIndex)
        {
            Face twarz = new Face();
            twarz = InputHelper.FacePreparation(picDir, _name, _folderName, _index, twarz, _folderIndex);

            faces.Add(twarz);
            return 1;
        }

        /// <summary>
        /// Obsluga eventow z guzikow
        /// </summary>
        private void Save_Pic_Data_Click(object sender, RoutedEventArgs e)
        {
            if (InputHelper.SaveBinary(faces) == 1)
                Console.WriteLine("zapisano do binarki");
        }

        private void Load_Pic_Data_Click(object sender, RoutedEventArgs e)
        {
            faces.Clear();
            faces = InputHelper.LoadBinary();
            if (faces.Count >= 1)
            {
                int peopleCounter = 0;
                peopleCounter = faces[faces.Count - 1].networkIndex + 1;
                peopleNumber = peopleCounter;
                Console.WriteLine("wczytano z binarki " + faces.Count + " danych");
            }
        }

        private async void Ucz_Siec_Click(object sender, RoutedEventArgs e)
        {
            BlakWait.Visibility = Visibility.Visible;
            inputData = new InputClass();
            if (faces.Count < 1 || faces == null)
            {
                MessageBox.Show("Error ! No data is loaded");
                BlakWait.Visibility = Visibility.Collapsed;
                return;
            }
            if (inputData.ValidateInput(TBLayers.Text, TBNeuronsInLayer.Text, ActivationFunction, CBObciazenie.SelectedIndex,
                TBIteracje.Text, TBWspUczenia.Text, TBWspBezwladnosci.Text, CBLastLayer.SelectedIndex, CBSets.SelectedIndex, peopleNumber) == false)
            {
                BlakWait.Visibility = Visibility.Collapsed;
                return;
            }
            else
            {
                lblLearningError.Content = "...";
                lblTestingError.Content = "...";
                lblPeopleCount.Content = "...";
                lblTimeElapsed.Content = "...";
                lblValidationError.Content = "...";
                lblIterationsCount.Content = "...";
                await PerformCalculation();
                lblLearningError.Content = inputData.LearningError;
                lblTestingError.Content = inputData.TestingError;
                lblPeopleCount.Content = inputData.PeopleCount;
                lblTimeElapsed.Content = inputData.ElapsedTime.ToString();
                lblValidationError.Content = inputData.ValidationError;
                lblIterationsCount.Content = inputData.IterationsCount;
            }
            BlakWait.Visibility = Visibility.Collapsed;
        }

        public async Task PerformCalculation()
        {
            await Task.Run(() =>
                {
                    var sortedFaces = InputHelper.TransformIntoListOfLists(faces);
                    Console.WriteLine("Szykuje dane zbioru uczacego");

                    var neuralLearningInput = NetworkHelper.CreateNetworkInputDataSet(sortedFaces, 12, 5, DataSetType.Learning, 12);
                    var neuralLearningOutput = NetworkHelper.CreateNetworkOutputDataSet(sortedFaces, 12, 5, DataSetType.Learning, 15);

                    var neuralValidationInput = NetworkHelper.CreateNetworkInputDataSet(sortedFaces, 12, 5, DataSetType.Validation, 12);
                    var neuralValidationOutput = NetworkHelper.CreateNetworkOutputDataSet(sortedFaces, 12, 5, DataSetType.Validation, 15);

                    var neuralTestingInput = NetworkHelper.CreateNetworkInputDataSet(sortedFaces, 12, 5, DataSetType.Testing, 12);
                    var neuralTestingOutput = NetworkHelper.CreateNetworkOutputDataSet(sortedFaces, 12, 5, DataSetType.Testing, 15);

                    var learningSet = NetworkHelper.NormaliseDataSet(neuralLearningInput, neuralLearningOutput);
                    var validationSet = NetworkHelper.NormaliseDataSet(neuralValidationInput, neuralValidationOutput);
                    var testingSet = NetworkHelper.NormaliseDataSet(neuralTestingInput, neuralTestingOutput);

                    var network = NetworkHelper.LearnNetwork(learningSet, testingSet, faces[0].features.Count, peopleNumber, inputData, validationSet);
                    learnedNetwork = network;
                });

        }
        private void CBAktywacje_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            ComboBoxItem typeItem = (ComboBoxItem)CBAktywacje.SelectedItem;
            string value = typeItem.Content.ToString();
            switch (value)
            {
                case "Linear":
                    ActivationFunction = new ActivationLinear();
                    break;
                case "LOG":
                    ActivationFunction = new ActivationLOG();
                    break;
                case "Sigmoid":
                    ActivationFunction = new ActivationSigmoid();
                    break;
                case "SIN":
                    ActivationFunction = new ActivationSIN();
                    break;
                case "TANH":
                    ActivationFunction = new ActivationTANH();
                    break;
            }
        }

        private void SaveNetwork_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Stop ! Wersja demo nie obejmuje analizy wynikow");
            return;
            if (InputHelper.SaveNetwork(learnedNetwork) == 1)
                Console.WriteLine("zapisano siec neuronowa do binarki");
        }

        private void TestNetwork_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Stop ! Wersja demo nie obejmuje analizy wynikow");
            return;
            AnswWindow window = new AnswWindow();
            window.Show();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            //TESTY
            BlakWait.Visibility = Visibility.Visible;
            await Task.Run(() =>
                {
                    TestHelper testperforamcje = new TestHelper();
                    testperforamcje.PerformTests();
                });
            BlakWait.Visibility = Visibility.Collapsed;
        }

        private void BtnGridSearch_Click(object sender, RoutedEventArgs e)
        {
            var gs = new GridSearch();
            gs.StartGridSearch();
        }

        private void BtnGeneticAlgorithm_Click(object sender, RoutedEventArgs e)
        {
            List<Face> faces = new List<Face>();
            String path = "C:\\Projects\\SIECI NEURONOWE 2019\\Twarze N 15x20\\ZdjeciaInput302.bin";
            FileStream fs = new FileStream(path, FileMode.Open);
            BinaryFormatter bf = new BinaryFormatter();
            BinaryReader br = new BinaryReader(fs);

            faces = (List<Face>)bf.Deserialize(fs);
            var sortedPhotos = InputHelper.TransformIntoListOfLists(faces);
            int populationSize = 80;
            double mutationRate = 0.30;
            int elitism = 4;
            var random = new Random();
            var ga = new GeneticAlgorithm(populationSize, random, elitism, sortedPhotos, mutationRate);
            for (int i = 0; i < 300; i++)
            {
                ga.NewGeneration();
                Console.WriteLine(100 - ga.BestFitness);
                using (StreamWriter sw = new StreamWriter("C:\\Projects\\SIECI NEURONOWE 2019\\FaceRecognition1\\WYNIKI", true))
                {
                    sw.WriteLine("Error:" + (100 - ga.BestFitness) + "   LearningError: " + ga.BestSpecimen.GetLearningFitnessValue() + "   HLayersCount: " + ga.BestSpecimen.HLayersCount +
                        "   HNeuronsCount: " + ga.BestSpecimen.HNeuronsCount + "   IsBiased" + ga.BestSpecimen.IsBiased +
                        "   IterationsCount" + ga.BestSpecimen.IterationCount + "   LearningFactor: " + ga.BestSpecimen.LearningFactor +
                        "  Momentum: " + ga.BestSpecimen.Momentum + " FeaturesVector: " + String.Concat(ga.BestSpecimen.ActiveFeatures));
                    sw.WriteLine("-------------------------------------------------------------------------------------");
                }
            }
        }
    }
}
