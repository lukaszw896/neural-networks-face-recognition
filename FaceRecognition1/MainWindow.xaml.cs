﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Luxand;
using FaceRecognition1.Content;
using FaceRecognition1.Helper;
using System.IO;
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
        public List<Face> Faces { get; set; }
        public int peopleNumber;
        public ITrain learnedNetwork;
        public MainWindow()
        {
            FSDK.ActivateLibrary("GTNWg1l8Zs+7uJixJ+eBiTF9s1Iofc2pc6UYstMf2/l/MRBagDqX8gzNqXtX64KspTPaszn6+/WwtSHOVDPBQ/WRYTeUTlNJmu9p8tFSCEGDsPodYiISTxA4uoAGtS1iZ3eTbqWkrupH0dCKEdTQzLatWNz7QaCBLaTmdZvn+zU=");
            FSDK.InitializeLibrary();
            InitializeComponent();
            Faces = new List<Face>();
            InitialSettings();
        }

        public void InitialSettings()
        {
            CBAktywacje.SelectedIndex = 2;
            CBObciazenie.SelectedIndex = 1;
            CBLastLayer.SelectedIndex = 0;
        }

        private async void Load_Pic_Click(object sender, RoutedEventArgs e)
        {
            BlakWait.Visibility = Visibility.Visible;
            Faces.Clear();

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
            Console.WriteLine("DONE" + Faces.Count);
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

            Faces.Add(twarz);
            return 1;
        }

        /// <summary>
        /// Obsluga eventow z guzikow
        /// </summary>
        private void Save_Pic_Data_Click(object sender, RoutedEventArgs e)
        {
            if (InputHelper.SaveBinary(Faces) == 1)
                Console.WriteLine("zapisano do binarki");
        }

        private void Load_Pic_Data_Click(object sender, RoutedEventArgs e)
        {
            Faces.Clear();
            Faces = InputHelper.LoadBinary();
            if (Faces.Count >= 1)
            {
                int peopleCounter = 0;
                peopleCounter = Faces[Faces.Count - 1].networkIndex + 1;
                peopleNumber = peopleCounter;
                Console.WriteLine("wczytano z binarki " + Faces.Count + " danych");
                errorLblGeneticAlgorithm.Visibility = Visibility.Collapsed;
                errorLblGridSearch.Visibility = Visibility.Collapsed;
                errorLblSingleRun.Visibility = Visibility.Collapsed;
            }
        }

        private async void Ucz_Siec_Click(object sender, RoutedEventArgs e)
        {
            BlakWait.Visibility = Visibility.Visible;
            inputData = new InputClass();
            if (Faces.Count < 1 || Faces == null)
            {
                MessageBox.Show("Error ! No data is loaded");
                BlakWait.Visibility = Visibility.Collapsed;
                return;
            }
            if (inputData.ValidateInput(TBLayers.Text, TBNeuronsInLayer.Text, ActivationFunction, CBObciazenie.SelectedIndex,
                TBIteracje.Text, TBWspUczenia.Text, TBWspBezwladnosci.Text, peopleNumber) == false)
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
                    var sortedFaces = InputHelper.TransformIntoListOfLists(Faces);
                    Console.WriteLine("Szykuje dane zbioru uczacego");

                    var neuralLearningInput = NetworkHelper.CreateNetworkInputDataSet(sortedFaces, 12, 5, DataSetType.Learning, 12/*, new bool[] { true,true, false, true,false,true,true,true,true,true,false,false }*/);
                    var neuralLearningOutput = NetworkHelper.CreateNetworkOutputDataSet(sortedFaces, 12, 5, DataSetType.Learning, 15);

                    var neuralValidationInput = NetworkHelper.CreateNetworkInputDataSet(sortedFaces, 12, 5, DataSetType.Validation, 12/*, new bool[] { true, true, false, true, false, true, true, true, true, true, false, false }*/);
                    var neuralValidationOutput = NetworkHelper.CreateNetworkOutputDataSet(sortedFaces, 12, 5, DataSetType.Validation, 15);

                    var neuralTestingInput = NetworkHelper.CreateNetworkInputDataSet(sortedFaces, 12, 5, DataSetType.Testing, 12/*, new bool[] { true, true, false, true, false, true, true, true, true, true, false, false }*/);
                    var neuralTestingOutput = NetworkHelper.CreateNetworkOutputDataSet(sortedFaces, 12, 5, DataSetType.Testing, 15);

                    var learningSet = NetworkHelper.NormaliseDataSet(neuralLearningInput, neuralLearningOutput);
                    var validationSet = NetworkHelper.NormaliseDataSet(neuralValidationInput, neuralValidationOutput);
                    var testingSet = NetworkHelper.NormaliseDataSet(neuralTestingInput, neuralTestingOutput);

                    var network = NetworkHelper.LearnNetwork(learningSet, testingSet, Faces[0].features.Count, peopleNumber, inputData, validationSet);
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
            var ft = new FinalTest();
            //var gs = new GridSearch();
            var sortedPhotos = InputHelper.TransformIntoListOfLists(this.Faces);
            //gs.StartGridSearch(sortedPhotos);
            ft.PerformFinalTest(sortedPhotos);
        }

        private void BtnGeneticAlgorithm_Click(object sender, RoutedEventArgs e)
        {
            var sortedPhotos = InputHelper.TransformIntoListOfLists(this.Faces);
            int populationSize = 90;
            double mutationRate = 0.33;
            int elitism = 4;
            var random = new Random();
            var ga = new GeneticAlgorithm(populationSize, random, elitism, sortedPhotos, mutationRate);
            for (int i = 0; i < 300; i++)
            {
                ga.NewGeneration();
                Console.WriteLine(100 - ga.BestFitness);
                using (StreamWriter sw = new StreamWriter("WYNIKI.txt", true))
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
