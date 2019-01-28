using Encog.Engine.Network.Activation;
using Encog.Neural.Networks.Training;
using Encog.Neural.NeuralData;
using FaceRecognition1.Content;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace FaceRecognition1.Helper
{
    public class GridSearch
    {
        private readonly double[] _learningRate = { 0.001, 0.003, 0.01 };
        private readonly double[] _momentum = { 0.001, 0.003, 0.01, 0.4 };
        private readonly int[] _hiddenLayersCount = { 1, 2, 3 };
        private readonly int[] _neuronsCount = { 30, 50};
        private readonly bool[] _bias = { true, false };

        public void StartGridSearch()
        {
            var faces = GetFacialData();
            PerformCalculations(faces);
        }
        private List<List<Face>> GetFacialData()
        {
            var path = "C:\\Projects\\SIECI NEURONOWE 2019\\Twarze N 15x20\\ZdjeciaInput302.bin";
            var fs = new FileStream(path, FileMode.Open);
            var bf = new BinaryFormatter();
            var br = new BinaryReader(fs);
            var faces = (List<Face>)bf.Deserialize(fs);
            return InputHelper.TransformIntoListOfLists(faces);
        }

        private void PerformCalculations(List<List<Face>> faces)
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

            Parallel.ForEach(_momentum, (x) => MainCalculationLoop(_learningRate,x,_hiddenLayersCount,_bias,_neuronsCount,learningSet,validationSet,testingSet,faces,date,stopwatch.Elapsed));
            stopwatch.Stop();
        }

        private void MainCalculationLoop(double[] _learningRate, double _momentum, int[] _hiddenLayersCount, bool[] _bias, int[] _neuronsCount, INeuralDataSet learningSet,
            INeuralDataSet validationSet, INeuralDataSet testingSet, List<List<Face>> faces, string date, TimeSpan timeFromStart)
        {
            foreach (var learningRate in _learningRate)
            {
                foreach (var neuronsCount in _neuronsCount)
                {
                    foreach (var hiddenLayersCount in _hiddenLayersCount)
                    {
                        foreach (var bias in _bias)
                        {
                            var inputData = new InputClass(learningRate, _momentum, hiddenLayersCount, neuronsCount, bias, 15, new ActivationSigmoid(), 40000);
                            NetworkHelper.LearnNetwork(learningSet, testingSet, faces[0][0].features.Count, 15, inputData, validationSet);
                            Task.Factory.StartNew(() =>
                            XmlFileWriter.WriteDataToFile("GridSearch" + date + ".xml", inputData.LearningError, inputData.ValidationError, inputData.TestingError, inputData.ElapsedTime, inputData.IterationsCount,
                                learningRate, _momentum, hiddenLayersCount, neuronsCount, bias, timeFromStart)
                            );
                        }
                    }
                }
            }
        }
    }
}
