using Encog.Neural.Activation;
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
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FaceRecognition1.Helper
{
    public class NetworkHelper
    {

        public static INeuralDataSet CombineTrainingSet(double[][] dane, double[][] odpowiedzi)
        {
            return new BasicNeuralDataSet(dane, odpowiedzi);
        }

        public static double[][] CreateLearningInputDataSet(List<Face>faces)
        {
            double[][] neuralInput = new double[faces.Count][];
            for (int i = 0; i < faces.Count; i++)
            {
                neuralInput[i] = new double[faces[i].features.Count];
                for (int j = 0; j < faces[i].features.Count; j++)
                {
                    neuralInput[i][j] = (double)faces[i].features[j];
                }
            }
            return neuralInput;
        }

        public static double[][] CreateLearningOutputDataSet(List<Face> faces)
        {
            double[][] neuralOutput = new double[faces.Count][];
            for (int i = 0; i < faces.Count; i++)
            {
                neuralOutput[i] = new double[1];
                neuralOutput[i][0] = (double)faces[i].networkIndex;
            }
            return neuralOutput;
        }

        public static void LearnNetwork(INeuralDataSet learningSet, int inputSize)
        {
            int iteracje = 2000;
            List<double> errors = new List<double>();
            Console.WriteLine("Tworze siec...");
            ITrain Network = CreateNeuronNetwork(learningSet, inputSize);

            int iteracja = 0;
            do
            {
                Network.Iteration();
                Console.WriteLine("Epoch #" + iteracja + " Error:" + Network.Error);
                errors.Add(Network.Error);
                iteracja++;
            } while ((iteracja < iteracje) && (Network.Error > 0.0005));


            /// TUTAJ SIEC SIE TEORETYCZNIE NAUCZYLA
            /// TERAZ ZBIOR TESTOWY, WYNIKI
            /// I WYKRES ERRORA

        }


        public static INeuralDataSet NormaliseDataSet(double[][] input, double[][] ideal)
        {
            Console.WriteLine("Normalizuje...");
            double[][] norm_input = new double[input.Length][];
            double[][] norm_ideal = new double[input.Length][];

            double maxInput = input[0][0], minInput = input[0][0];
            double maxOutput = input[0][0], minOutput = input[0][0];

            for (int i = 0; i < input.Length; i++)
            {
                for (int j = 0; j < input[i].Count(); j++)
                {
                    if (input[i][j] < minInput)
                        minInput = input[i][j];

                    if (input[i][j] > maxInput)
                        maxInput = input[i][j];
                }

                if (ideal[i][0] < minInput)
                    maxOutput = ideal[i][0];

                if (ideal[i][0] > maxInput)
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
            INeuralDataSet dataset = CombineTrainingSet(norm_input, norm_ideal);
            return dataset;
        }


        public static ITrain CreateNeuronNetwork(INeuralDataSet learningSet, int inputSize)
        {
            BasicNetwork network = new BasicNetwork();
            //------------------------------------------------------------------------------------------

            int szerokosc = inputSize + 4;
            int dlugosc = 4;
            bool bias = true;
            IActivationFunction ActivationFunction =  new ActivationSigmoid();
            double learning = 0.01;
            double momentum = 0.01;
            //-----------------------------------------------------------------------------------------

            network.AddLayer(new BasicLayer(ActivationFunction, bias, inputSize));

            for (int i = 0; i < dlugosc; i++)
                network.AddLayer(new BasicLayer(ActivationFunction, bias, szerokosc));

            network.AddLayer(new BasicLayer(new ActivationLinear(), false, 1));

            network.Structure.FinalizeStructure();
            network.Reset();
            ITrain train = new Backpropagation(network, learningSet, learning, momentum);
            return train;
        }
    }
}
