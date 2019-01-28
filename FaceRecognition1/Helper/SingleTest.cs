using Encog.Engine.Network.Activation;
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
        public void RunTest(List<List<Face>> faces, string calcStartDate, TimeSpan timeFromStart, bool[] activeFeatures = null)
        {
            InputClass inputData = new InputClass();
            inputData.ValidateInput((this.HiddenLayersCount).ToString(), (this.HiddenNeuronsCount).ToString(), new ActivationSigmoid(), this.IsBiased,
                (this.IterationsCount).ToString(), (this.LearningFactor).ToString(), (this.Momentum).ToString(), 0,this.PeopleCount);

            this.PerformCalculation(inputData, faces, calcStartDate, timeFromStart, activeFeatures);
            this.LearningError = inputData.LearningError;
            this.TestingError = inputData.TestingError;
            this.ElapsedTime = inputData.ElapsedTime;
        }

        public void PerformCalculation(InputClass inputData, List<List<Face>> faces, string calcStartDate, TimeSpan timeFromStart, bool[] activeFeatures = null)
        {
            Console.WriteLine("Szykuje dane zbioru uczacego");
            var networkLearningInput = NetworkHelper.CreateNetworkInputDataSet(faces, 12, 5, DataSetType.Learning, 12, activeFeatures);
            var networkLearningOutput = NetworkHelper.CreateNetworkOutputDataSet(faces, 12, 5, DataSetType.Learning, 15, activeFeatures);

            var networkValidationInput = NetworkHelper.CreateNetworkInputDataSet(faces, 12, 5, DataSetType.Validation, 12, activeFeatures);
            var networkValidationOutput = NetworkHelper.CreateNetworkOutputDataSet(faces, 12, 5, DataSetType.Validation, 15, activeFeatures);

            var networkTestingInput = NetworkHelper.CreateNetworkInputDataSet(faces, 12, 5, DataSetType.Testing, 12, activeFeatures);
            var networkTestingOutput = NetworkHelper.CreateNetworkOutputDataSet(faces, 12, 5, DataSetType.Testing, 15, activeFeatures);

            var learningSet = NetworkHelper.NormaliseDataSet(networkLearningInput, networkLearningOutput);
            var validationSet = NetworkHelper.NormaliseDataSet(networkValidationInput, networkValidationOutput);
            var testingSet = NetworkHelper.NormaliseDataSet(networkTestingInput, networkTestingOutput);

            NetworkHelper.LearnNetwork(learningSet, testingSet, faces[0][0].features.Count, this.PeopleCount, inputData, validationSet);
            Task.Factory.StartNew(() =>
                            XmlFileWriter.WriteDataToFile("Genetic" + calcStartDate + ".xml", inputData.LearningError, inputData.ValidationError, inputData.TestingError, inputData.ElapsedTime, inputData.IterationsCount,
                                inputData.LearningFactor, inputData.Momentum,inputData.HiddenLayers, inputData.HiddenNeurons,inputData.Bias, timeFromStart,activeFeatures)
                            );
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
