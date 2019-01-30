using Encog.Engine.Network.Activation;
using FaceRecognition1.Content;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaceRecognition1.Helper
{
    public class FinalTest
    {
        //Momentum = 0.003 or 0.001
        private readonly InputClass id = new InputClass()
        {
            Momentum = 0.01,
            HiddenLayers = 1,
            HiddenNeurons = 30,
            Bias = true,
            ActivationFunction = new ActivationSigmoid(),
            IterationsCount = 800000,
            PeopleCount = 15
        };
        private readonly int[] seeds = new int[10] { 54235432, 123423432, 9231231, 5645643, 83241231, 3453453, 62362345, 345634543, 27243712, 43259624 };
        public void PerformFinalTest(List<List<Face>> faces)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var date = DateTime.Now.ToString("yyyy-dd-M-HH-mm-ss");
            var networkLearningInput = NetworkHelper.CreateNetworkInputDataSet(faces, 12, 5, DataSetType.Learning, 12);
            var networkLearningOutput = NetworkHelper.CreateNetworkOutputDataSet(faces, 12, 5, DataSetType.Learning, 15);

            var networkValidationInput = NetworkHelper.CreateNetworkInputDataSet(faces, 12, 5, DataSetType.Validation, 12);
            var networkValidationOutput = NetworkHelper.CreateNetworkOutputDataSet(faces, 12, 5, DataSetType.Validation, 15);

            var networkTestingInput = NetworkHelper.CreateNetworkInputDataSet(faces, 12, 5, DataSetType.Testing, 12);
            var networkTestingOutput = NetworkHelper.CreateNetworkOutputDataSet(faces, 12, 5, DataSetType.Testing, 15);

            var learningSet = NetworkHelper.NormaliseDataSet(networkLearningInput, networkLearningOutput);
            var validationSet = NetworkHelper.NormaliseDataSet(networkValidationInput, networkValidationOutput);
            var testingSet = NetworkHelper.NormaliseDataSet(networkTestingInput, networkTestingOutput);
            foreach (double learningRate in new double[] { 0.001, 0.003 })
            {
                Parallel.ForEach(seeds, (x) =>
            {
                var inputDataCopy = new InputClass(learningRate, id.Momentum, id.HiddenLayers, id.HiddenNeurons, id.Bias, id.PeopleCount, id.ActivationFunction, id.IterationsCount);
                NetworkHelper.LearnNetwork(learningSet, testingSet, faces[0][0].features.Count, 15, inputDataCopy, validationSet, x);
                Task.Factory.StartNew(() =>
                XmlFileWriter.WriteDataToFile("FinalTest" + date + ".xml", inputDataCopy.LearningError, inputDataCopy.ValidationError, inputDataCopy.TestingError, inputDataCopy.ElapsedTime, inputDataCopy.IterationsCount,
                    inputDataCopy.LearningFactor, inputDataCopy.Momentum, inputDataCopy.HiddenLayers, inputDataCopy.HiddenNeurons, inputDataCopy.Bias, stopwatch.Elapsed, null, x)
                );

            });
            }
        }
    }
}
