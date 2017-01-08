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
        public int ludzie { get; set; }
        public int neuronyNaWarstwe { get; set; }
        public int warstwy { get; set; }
        public int obciazenie { get; set; }
        public int testSet { get; set; }
        public int iteracje { get; set; }
        public string testError { get; set; }
        public string learnError { get; set; }
        public string time { get; set; }

        public SingleTest()
        {

        }
        public SingleTest(int _ludzie, int _NNW, int _warstwy, int _obciaz, int _testSet, int _iteracje)
        {
            this.ludzie = _ludzie;
            this.neuronyNaWarstwe = _NNW;
            this.warstwy = _warstwy;
            this.obciazenie = _obciaz;
            this.testSet = _testSet;
            this.iteracje = _iteracje;
        }
        public void RunTest(List<Face> faces)
        {
            InputClass inputData = new InputClass();
            
            inputData.ValidateInput((this.warstwy).ToString(), (this.neuronyNaWarstwe).ToString(), new ActivationSigmoid(), this.obciazenie,
                (this.iteracje).ToString(), (0.01).ToString(), (0.4).ToString(), 0, this.testSet, this.ludzie);

            PerformCalculation(inputData,faces);
            this.learnError = inputData.learningError.ToString();
            this.testError = inputData.testingError.ToString();
            this.time = inputData.timeElapsed.ToString();           
        }

        public void PerformCalculation(InputClass inputData, List<Face> faces)
        {
            int multipleOutput = 0;
            multipleOutput = InputHelper.ChooseMode(inputData.multipleNeurons, this.ludzie);

            Console.WriteLine("Szykuje dane zbioru uczacego");
            double[][] neuralLearningInput = NetworkHelper.CreateLearningInputDataSet(faces, false, inputData.learningtesting);
            double[][] neuralLearningOutput = NetworkHelper.CreateLearningOutputDataSet(faces, false, multipleOutput, inputData.learningtesting);
            double[][] neuralTestingInput = NetworkHelper.CreateLearningInputDataSet(faces, true, inputData.learningtesting);
            double[][] neuralTestingOutput = NetworkHelper.CreateLearningOutputDataSet(faces, true, multipleOutput, inputData.learningtesting);
            INeuralDataSet learningSet, testingSet;

            if (multipleOutput == 0)
            {
                learningSet = NetworkHelper.NormaliseDataSet(neuralLearningInput, neuralLearningOutput, 0);
                testingSet = NetworkHelper.NormaliseDataSet(neuralTestingInput, neuralTestingOutput, 0);
            }
            else
            {
                learningSet = NetworkHelper.NormaliseDataSet(neuralLearningInput, neuralLearningOutput, 1);
                testingSet = NetworkHelper.NormaliseDataSet(neuralTestingInput, neuralTestingOutput, 1);
            }

            ITrain network = NetworkHelper.LearnNetwork(learningSet, testingSet, faces[0].features.Count, neuralTestingOutput.Count(), multipleOutput, inputData);
            
        }
        public string toText()
        {
            string tekst = "";
            tekst = "ludzie:"+ this.ludzie.ToString() + "| warswty:"+ this.warstwy.ToString()+ "| neurony w warstwie:"+ this.neuronyNaWarstwe.ToString()+ "| bias :"+ this.obciazenie.ToString()
              + "| zbiory rozlaczne:" + this.testSet.ToString() + "| iteracje:" + this.iteracje.ToString() + "##### Error learningowy:" + this.learnError.ToString() +"| Error testowy:" + this.testError.ToString() + "| time:" + this.time.ToString();
      
            return tekst;
        }
    }
}
