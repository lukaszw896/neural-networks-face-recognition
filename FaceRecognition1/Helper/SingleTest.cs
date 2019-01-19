using Encog.Neural.Activation;
using Encog.Neural.Networks.Training;
using Encog.Neural.NeuralData;
using FaceRecognition1.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaceRecognition1.Helper
{
    public class SingleTest
    {
        public int PeopleCount { get; set; }
        public int HiddenNeuronsCount { get; set; }
        public int HiddenLayersCount { get; set; }
        public int IsBiased { get; set; }
        public int TestDataSet { get; set; }
        public int IterationsCount { get; set; }
        public double TestingError { get; set; }
        public double LearningError { get; set; }
        public TimeSpan ElapsedTime { get; set; }
        public double LearningFactor { get; set; }
        public double Momentum { get; set; }

        public SingleTest()
        {

        }
        public SingleTest(int peopleCount, int hiddenNeuronsCount, int hiddenLayersCount, int isBiased, int testDataSet, int iterationsCount, double learningFactor = 0.01, double momentum = 0.4)
        {
            this.PeopleCount = peopleCount;
            this.HiddenNeuronsCount = hiddenNeuronsCount;
            this.HiddenLayersCount = hiddenLayersCount;
            this.IsBiased = isBiased;
            this.TestDataSet = testDataSet;
            this.IterationsCount = iterationsCount;
            this.LearningFactor = learningFactor;
            this.Momentum = momentum;
        }
        public void RunTest(List<Face> faces, bool[] activeFeatures = null)
        {
            InputClass inputData = new InputClass();
            inputData.ValidateInput((this.HiddenLayersCount).ToString(), (this.HiddenNeuronsCount).ToString(), new ActivationSigmoid(), this.IsBiased,
                (this.IterationsCount).ToString(), (this.LearningFactor).ToString(), (this.Momentum).ToString(), 0, this.TestDataSet, this.PeopleCount);

            this.PerformCalculation(inputData, faces, activeFeatures);
            this.LearningError = inputData.LearningError;
            this.TestingError = inputData.TestingError;
            this.ElapsedTime = inputData.ElapsedTime;
        }

        public void PerformCalculation(InputClass inputData, List<Face> faces, bool[] activeFeatures = null)
        {
            int multipleOutput = inputData.multipleNeurons ? this.PeopleCount : 0;

            Console.WriteLine("Szykuje dane zbioru uczacego");
            double[][] neuralLearningInput = NetworkHelper.CreateLearningInputDataSet(faces, false, inputData.learningtesting, activeFeatures);
            double[][] neuralLearningOutput = NetworkHelper.CreateLearningOutputDataSet(faces, false, multipleOutput, inputData.learningtesting);
            double[][] neuralTestingInput = NetworkHelper.CreateLearningInputDataSet(faces, true, inputData.learningtesting, activeFeatures);
            double[][] neuralTestingOutput = NetworkHelper.CreateLearningOutputDataSet(faces, true, multipleOutput, inputData.learningtesting);
            INeuralDataSet learningSet, testingSet;

            learningSet = NetworkHelper.NormaliseDataSet(neuralLearningInput, neuralLearningOutput, multipleOutput);
            testingSet = NetworkHelper.NormaliseDataSet(neuralTestingInput, neuralTestingOutput, multipleOutput);

            ITrain network = NetworkHelper.LearnNetwork(learningSet, testingSet, faces[0].features.Count, neuralTestingOutput.Count(), multipleOutput, inputData);

        }
        public string ToText()
        {
            string tekst = "";
            tekst = "ludzie:" + this.PeopleCount.ToString() + "| warswty:" + this.HiddenLayersCount.ToString() + "| neurony w warstwie:" + this.HiddenNeuronsCount.ToString() + "| bias :" + this.IsBiased.ToString()
              + "| zbiory rozlaczne:" + this.TestDataSet.ToString() + "| iteracje:" + this.IterationsCount.ToString() + "##### Error learningowy:" + this.LearningError.ToString() + "| Error testowy:" + this.TestingError.ToString() + "| time:" + this.ElapsedTime.ToString();
            return tekst;
        }
    }
}
