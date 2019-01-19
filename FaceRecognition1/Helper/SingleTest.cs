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
        public int neuronyNaWarstwe { get; set; }
        public int warstwy { get; set; }
        public int obciazenie { get; set; }
        public int testSet { get; set; }
        public int iteracje { get; set; }
        public string testError { get; set; }
        public string learnError { get; set; }
        public string time { get; set; }
        public double LearningFactor { get; set; }
        public double Momentum { get; set; }

        public SingleTest()
        {

        }
        public SingleTest(int peopleCount, int _NNW, int _warstwy, int _obciaz, int _testSet, int _iteracje, double learningFactor = 0.01, double momentum = 0.4)
        {
            this.PeopleCount = peopleCount;
            this.neuronyNaWarstwe = _NNW;
            this.warstwy = _warstwy;
            this.obciazenie = _obciaz;
            this.testSet = _testSet;
            this.iteracje = _iteracje;
            this.LearningFactor = learningFactor;
            this.Momentum = momentum;
        }
        public void RunTest(List<Face> faces, bool[] activeFeatures = null)
        {
            InputClass inputData = new InputClass();
            inputData.ValidateInput((this.warstwy).ToString(), (this.neuronyNaWarstwe).ToString(), new ActivationSigmoid(), this.obciazenie,
                (this.iteracje).ToString(), (this.LearningFactor).ToString(), (this.Momentum).ToString(), 0, this.testSet, this.PeopleCount);

            this.PerformCalculation(inputData, faces, activeFeatures);
            this.learnError = inputData.LearningError.ToString();
            this.testError = inputData.TestingError.ToString();
            this.time = inputData.ElapsedTime.ToString();
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
            tekst = "ludzie:" + this.PeopleCount.ToString() + "| warswty:" + this.warstwy.ToString() + "| neurony w warstwie:" + this.neuronyNaWarstwe.ToString() + "| bias :" + this.obciazenie.ToString()
              + "| zbiory rozlaczne:" + this.testSet.ToString() + "| iteracje:" + this.iteracje.ToString() + "##### Error learningowy:" + this.learnError.ToString() + "| Error testowy:" + this.testError.ToString() + "| time:" + this.time.ToString();
            return tekst;
        }
    }
}
