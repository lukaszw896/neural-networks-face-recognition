using Encog.Engine.Network.Activation;
using Encog.Neural.Data;
using Encog.Neural.Data.Basic;
using Encog.Neural.Networks;
using Encog.Neural.Networks.Layers;
using Encog.Neural.Networks.Training;
using Encog.Neural.Networks.Training.Propagation.Back;
using Encog.Neural.NeuralData;
using FaceRecognition1.Content;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace FaceRecognition1.Helper
{
    public enum DataSetType
    {
        Learning = 0,
        Validation = 1,
        Testing = 2
    }
    public class NetworkHelper
    {
        public static double maxOutput = 0.0;
        public static double minOutput = 0.0;


        public static INeuralDataSet CombineTrainingSet(double[][] dane, double[][] odpowiedzi)
        {
            return new BasicNeuralDataSet(dane, odpowiedzi);
        }
        public static double[][] CreateNetworkInputDataSet(List<List<Face>> faceList, int learnPhotosCount, int validationPhotosCount, DataSetType dataSetType,int featuresCount, bool[] activeFeatures = null)
        {
            var dataSetCount = GetDataSetCount(faceList, learnPhotosCount, validationPhotosCount, dataSetType);
            double[][] networkInput = new double[dataSetCount][];
            int startingIndex = dataSetType == DataSetType.Learning ? 0 : learnPhotosCount;
            int currentFaceIndex = 0;
            for (int i = 0; i < faceList.Count; i++)
            {
                int finishIndex = GetFaceFinishIndex(faceList[i].Count, learnPhotosCount, validationPhotosCount, dataSetType);
                for (int j = startingIndex; j < finishIndex; j++)
                {
                    networkInput[currentFaceIndex] = new double[featuresCount];
                    for (int k = 0; k < featuresCount; k++)
                    {
                        networkInput[currentFaceIndex][k] = faceList[i][j].features[k];
                    }
                    if (activeFeatures != null)
                        networkInput[currentFaceIndex] = MultiplyDoubleVec(networkInput[currentFaceIndex], activeFeatures);
                    currentFaceIndex++;
                }
            }
            return networkInput;
        }
        public static double[][] CreateNetworkOutputDataSet(List<List<Face>> faceList, int learnPhotosCount, int validationPhotosCount, DataSetType dataSetType, int outputSize, bool[] activeFeatures = null)
        {
            var dataSetCount = GetDataSetCount(faceList, learnPhotosCount, validationPhotosCount, dataSetType);
            double[][] networkOutput = new double[dataSetCount][];
            int startingIndex = dataSetType == DataSetType.Learning ? 0 : learnPhotosCount;
            int currentFaceIndex = 0;
            for (int i = 0; i < faceList.Count; i++)
            {
                int finishIndex = GetFaceFinishIndex(faceList[i].Count, learnPhotosCount, validationPhotosCount, dataSetType);
                for (int j = startingIndex; j < finishIndex; j++)
                {
                    networkOutput[currentFaceIndex] = new double[outputSize];
                    for (int k = 0; k < outputSize; k++)
                    {
                        if (k == faceList[i][j].networkIndex)
                            networkOutput[currentFaceIndex][k] = 1.0;
                        else
                            networkOutput[currentFaceIndex][k] = 0.0;
                    }
                    currentFaceIndex++;
                }
            }
            return networkOutput;
        }
        private static int GetFaceFinishIndex(int picturesCount, int learnPhotosCount, int validationPhotosCount, DataSetType dataSetType)
        {
            switch (dataSetType)
            {
                case DataSetType.Learning:
                    return learnPhotosCount;
                case DataSetType.Validation:
                    return learnPhotosCount + validationPhotosCount;
                case DataSetType.Testing:
                    return picturesCount;
                default:
                    throw new Exception("Invalid data set type");
            }
        }
        /// <summary>
        /// In my assumption Learning set is disjoint with Validation and Testing sets.
        /// Validation set is a subset of testing set. It's because during creation of this project I had very limited number of facial pictures per person.
        /// </summary>
        /// <param name="faceList"></param>
        /// <param name="learnPhotosCount"></param>
        /// <param name="validationPhotosCount"></param>
        /// <param name="dataSetType"></param>
        /// <returns></returns>
        private static int GetDataSetCount(List<List<Face>> faceList, int learnPhotosCount, int validationPhotosCount, DataSetType dataSetType)
        {
            switch (dataSetType)
            {
                case DataSetType.Learning:
                    return faceList.Count * learnPhotosCount;
                case DataSetType.Validation:
                    return faceList.Count * validationPhotosCount;
                case DataSetType.Testing:
                    return faceList.Sum(x => x.Count - learnPhotosCount);
                default:
                    throw new Exception("Invalid data set type");
            }
        }
        public static double[][] CreateLearningInputDataSet(List<Face> faces, bool test, bool rozlacznosc, bool[] activeFeatures = null)
        {
            double picNum = 1.0;
            if (test == false || rozlacznosc == true)
                picNum = 2.0;

            double[][] neuralInput = new double[(int)(faces.Count * (1.0 / picNum))][];
            int counter = 0;
            if (rozlacznosc == true && test == true)
            {
                for (int i = 1; i < faces.Count(); i += (int)picNum)
                {
                    neuralInput[counter] = new double[faces[i].features.Count];
                    for (int j = 0; j < faces[i].features.Count; j++)
                    {
                        neuralInput[counter][j] = faces[i].features[j];
                    }
                    if (activeFeatures != null)
                        neuralInput[counter] = MultiplyDoubleVec(neuralInput[counter], activeFeatures);
                    counter++;
                }
            }
            else
            {
                for (int i = 0; i < faces.Count(); i += (int)picNum)
                {
                    neuralInput[counter] = new double[faces[i].features.Count];
                    for (int j = 0; j < faces[i].features.Count; j++)
                    {
                        neuralInput[counter][j] = (double)faces[i].features[j];
                    }
                    if (activeFeatures != null)
                        neuralInput[counter] = MultiplyDoubleVec(neuralInput[counter], activeFeatures);
                    counter++;
                }
            }
            return neuralInput;
        }

        private static double[] MultiplyDoubleVec(double[] features, bool[] isActive)
        {
            if (features.Length != isActive.Length)
                throw new Exception("Multiplied vectors length cannot be different!");
            for (int i = 0; i < features.Length; i++)
                features[i] = isActive[i] ? features[i] : 0;
            return features;
        }

        public static double[][] CreateLearningOutputDataSet(List<Face> faces, bool test, int outputSize, bool rozlacznosc)
        {
            double picNum = 1.0;
            if (test == false || rozlacznosc == true)
                picNum = 2.0;

            double[][] neuralOutput = new double[(int)(faces.Count * (1.0 / picNum))][];
            int counter = 0;
            if (rozlacznosc == true && test == true)
            {
                for (int i = 1; i < faces.Count(); i += (int)picNum)
                {
                    if (outputSize == 0)
                    {
                        neuralOutput[counter] = new double[1];
                        neuralOutput[counter][0] = (double)faces[i].networkIndex;
                    }
                    else
                    {
                        neuralOutput[counter] = new double[outputSize];
                        for (int j = 0; j < outputSize; j++)
                        {
                            if (j == faces[i].networkIndex)
                                neuralOutput[counter][j] = 1.0;
                            else
                                neuralOutput[counter][j] = 0.0;
                        }
                    }
                    counter++;
                }
            }
            else
            {
                for (int i = 0; i < faces.Count(); i += (int)picNum)
                {
                    if (outputSize == 0)
                    {
                        neuralOutput[counter] = new double[1];
                        neuralOutput[counter][0] = (double)faces[i].networkIndex;
                    }
                    else
                    {
                        neuralOutput[counter] = new double[outputSize];
                        for (int j = 0; j < outputSize; j++)
                        {
                            if (j == faces[i].networkIndex)
                                neuralOutput[counter][j] = 1.0;
                            else
                                neuralOutput[counter][j] = 0.0;
                        }
                    }
                    counter++;
                }
            }

            return neuralOutput;
        }

        public static ITrain LearnNetwork(INeuralDataSet learningSet, INeuralDataSet testingSet, int inputSize, int testingSize, int answersSize, InputClass inputData, INeuralDataSet validationSet = null, int validationInputSize = 0)
        {
            int iteracje = inputData.IterationsCount;
            List<double> errors = new List<double>();
            Console.WriteLine("Tworze siec...");
            var network = CreateNeuronNetwork(learningSet, inputSize, answersSize, inputData);

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            double lowestValidationError = 100.0;
            double currentValidationError = 100.0;
            int growingErrorCount = 0;
            int iteracja = 0;
            do
            {
                network.Iteration();
                //if (validationSet != null)
                //{
                //    currentValidationError = GetNetworkTestError(network, validationSet, answersSize);
                //}
                //Console.WriteLine("Epoch #" + iteracja + " Error:" + network.Error);
                errors.Add(network.Error);
                iteracja++;
            } while ((iteracja < iteracje) && (network.Error > 0.0001) && (network.Error < 10000));
            stopwatch.Stop();

            /// TUTAJ SIEC SIE TEORETYCZNIE NAUCZYLA
            /// TERAZ ZBIOR TESTOWY, WYNIKI
            /// I WYKRES ERRORA
            /// 

            inputData.LearningError = GetNetworkTestError(network, learningSet, answersSize);
            inputData.TestingError = GetNetworkTestError(network, testingSet, answersSize);
            inputData.ElapsedTime = stopwatch.Elapsed;
            inputData.IterationsCount = iteracja;
            inputData.Errors = errors;

            Console.WriteLine("Learning error: " + inputData.LearningError +
                validationSet != null ? (" ValidationError: " + currentValidationError) : "" +
                " Testing error: " + inputData.TestingError + " Elapsed: " + inputData.ElapsedTime + " IterCount: " + iteracja);
            return network;

        }

        public static double GetNetworkTestError(Backpropagation network, INeuralDataSet testingSet, int answersSize)
        {
            double[] neuralAnswer = new double[testingSet.Count];
            int i = 0;
            foreach (var pair in testingSet)
            {
                double[] output = new double[answersSize];
                network.Network.Flat.Compute(pair.Input, output);
                if (answersSize != 0)
                {
                    double small = 0.0;
                    for (int r = 0; r < answersSize; r++)
                    {
                        if (output[r] >= small)
                        {
                            neuralAnswer[i] = r;
                            small = output[r];
                        }
                    }
                }
                else
                    neuralAnswer[i] = output[0];
                i++;
            }
            int[] answers = DenormaliseAnswers(neuralAnswer, answersSize);
            Console.WriteLine("Neural Network Results");
            double calculateError = CalculateFinalError(answers, testingSet, answersSize);
            return calculateError;
        }

        public static void CreateErrorFile(List<double> errors)
        {
            string line = "";
            // Write the string to a file.
            System.IO.StreamWriter file = new System.IO.StreamWriter("errors.R");

            NumberFormatInfo nfi = new NumberFormatInfo();
            nfi.NumberDecimalSeparator = ".";

            line = "points<- c(";
            int i = 0;
            for (i = 0; i < errors.Count - 1; i++)
            {
                line += errors[i].ToString(nfi) + ",";
            }
            line += errors[i].ToString(nfi) + ")";
            file.WriteLine(line);
            file.WriteLine(@"plot(points , type= ""o"", col= ""red"")");
            file.WriteLine(@"title(main= ""Error"", col.main= ""black"", font.main= 4)");

            file.Close();
        }

        public static double CalculateFinalError(int[] answers, INeuralDataSet testingSet, int answersSize)
        {
            int properAnswer = 0;
            double[] neuralAnswers = new double[answers.Count()];
            int j = 0;
            if (answersSize == 0)
            {
                foreach (var pair in testingSet)
                {
                    neuralAnswers[j] = pair.Ideal[0];
                    j++;
                }
            }
            else
            {
                foreach (var pair in testingSet)
                {
                    for (int r = 0; r < answersSize; r++)
                    {
                        if ((double)(pair.Ideal[r]) >= 0.66)
                            neuralAnswers[j] = (double)r;
                    }
                    j++;
                }
            }
            Console.WriteLine("test");
            int[] idealAnswers = DenormaliseAnswers(neuralAnswers, answersSize);
            for (int i = 0; i < answers.Count(); i++)
            {
                if (idealAnswers[i] == answers[i])
                    properAnswer++;
            }
            double error = 100.0;
            error = 100.0 - ((properAnswer * 100.0) / answers.Count());
            return error;
        }

        public static int[] DenormaliseAnswers(double[] answers, int answersSize)
        {
            Console.WriteLine("Denormalizuje wynik...");
            int[] denorm_answers = new int[answers.Count()];
            if (answersSize == 0)
            {
                for (int i = 0; i < answers.Count(); i++)
                {
                    denorm_answers[i] = (int)Math.Round(((answers[i] * (maxOutput - minOutput)) + minOutput));
                }
            }
            else
                denorm_answers = ConvertDoubleArrayToIntArray(answers);

            Console.WriteLine("Zdenormalizowano");
            return denorm_answers;
        }

        public static int[] ConvertDoubleArrayToIntArray(double[] adDoubleArray)
        {
            return adDoubleArray.Select(d => (int)d).ToArray();
        }

        public static INeuralDataSet NormaliseDataSet(double[][] input, double[][] ideal, int multipleOutput)
        {
            Console.WriteLine("Normalizuje...");
            double[][] norm_input = new double[input.Length][];
            double[][] norm_ideal = new double[input.Length][];

            if (multipleOutput == 0)
            {
                double maxInput = input[0][0], minInput = input[0][0];
                maxOutput = ideal[0][0];
                minOutput = ideal[0][0];

                for (int i = 0; i < input.Length; i++)
                {
                    for (int j = 0; j < input[i].Count(); j++)
                    {
                        if (input[i][j] < minInput)
                            minInput = input[i][j];

                        if (input[i][j] > maxInput)
                            maxInput = input[i][j];
                    }

                    if (ideal[i][0] < minOutput)
                        minOutput = ideal[i][0];

                    if (ideal[i][0] > maxOutput)
                        maxOutput = ideal[i][0];
                }


                for (int i = 0; i < input.Length; i++)
                {
                    norm_input[i] = new double[input[i].Count()];
                    for (int j = 0; j < input[i].Count(); j++)
                    {
                        norm_input[i][j] = (input[i][j] - minInput) / (maxInput - minInput);
                    }
                    norm_ideal[i] = new double[1];
                    norm_ideal[i][0] = (ideal[i][0] - minOutput) / (maxOutput - minOutput);
                }
                Console.WriteLine("Znormalizowano");
                Thread.Sleep(500);
            }
            else
            {
                norm_input = input;
                norm_ideal = ideal;
            }
            INeuralDataSet dataset = CombineTrainingSet(norm_input, norm_ideal);
            return dataset;
        }


        public static Backpropagation CreateNeuronNetwork(INeuralDataSet learningSet, int inputSize, int answersSize, InputClass inputData)
        {
            var network = new BasicNetwork();
            //------------------------------------------------------------------------------------------

            int szerokosc = inputData.HiddenNeurons;
            int dlugosc = inputData.HiddenLayers;
            bool bias = inputData.Bias;
            IActivationFunction ActivationFunction = inputData.ActivationFunction;

            double learning = inputData.LearningFactor;
            double momentum = inputData.Momentum;
            //-----------------------------------------------------------------------------------------

            network.AddLayer(new BasicLayer(ActivationFunction, bias, inputSize));

            for (int i = 0; i < dlugosc; i++)
                network.AddLayer(new BasicLayer(ActivationFunction, bias, szerokosc));

            if (answersSize == 0)
                network.AddLayer(new BasicLayer(new ActivationLinear(), false, 1));
            else
                network.AddLayer(new BasicLayer(ActivationFunction, false, answersSize));

            network.Structure.FinalizeStructure();
            network.Reset(324523);
            var train = new Backpropagation(network, learningSet, learning, momentum);
            return train;
        }
    }
}
