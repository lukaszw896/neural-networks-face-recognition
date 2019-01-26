using Encog.Engine.Network.Activation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FaceRecognition1.Content
{
    public class InputClass
    {
        public int HiddenLayers { get; set; }
        public int HiddenNeurons { get; set; }
        public IActivationFunction ActivationFunction { get; set; }
        public bool Bias { get; set; }
        public int IterationsCount { get; set; }
        public double LearningFactor { get; set; }
        public double Momentum { get; set; }
        public bool multipleNeurons { get; set; }
        public bool learningtesting { get; set; }
        public double LearningError { get; set; }
        public double TestingError { get; set; }
        public int PeopleCount { get; set; }
        public TimeSpan ElapsedTime { get; set; }
        public List<double> Errors { get; set; }

        public InputClass()
        {

        }
        public bool ValidateInput(string TBLayers, string TBNeuronsInLayer, IActivationFunction _activationFunction,
            int _bias, string TBIteracje, string TBWspUczenia, string TBWspBezwladnosci, int _multipleNeurons, int _learningtesting, int _peopleNumber)
        {
            bool isCorrect = true;
            int _hiddenLayers = 0;
            int _hiddenNeurons = 0;
            int _iterations = 0;
            double _learningFactor = 0.0;
            double _momentum = 0.0;

            bool int1 = int.TryParse(TBLayers, out _hiddenLayers);
            if (int1 == false || _hiddenLayers < 1)
            {
                MessageBox.Show("Error ! Network need at least one hidden layer.");
                return false;
            }
            int1 = int.TryParse(TBNeuronsInLayer, out _hiddenNeurons);
            if (int1 == false || _hiddenNeurons < 1)
            {
                MessageBox.Show("Error ! Network need at least one neuron in hidden layer.");
                return false;
            }
            int1 = int.TryParse(TBIteracje, out _iterations);
            if (int1 == false || _iterations < 1)
            {
                MessageBox.Show("Error ! Network need at least one iteration.");
                return false;
            }
            int1 = double.TryParse(TBWspUczenia, out _learningFactor);
            if (int1 == false || _learningFactor < 0 || _learningFactor > 1)
            {
                MessageBox.Show("Error ! Learning factor has to be from range [0; 1]");
                return false;
            }
            int1 = double.TryParse(TBWspBezwladnosci, out _momentum);
            if (int1 == false || _momentum < 0 || _momentum > 0.5)
            {
                MessageBox.Show("Error ! Momentm has to be from range [0; 0,5]");
                return false;
            }
            this.HiddenLayers = _hiddenLayers;
            this.HiddenNeurons = _hiddenNeurons;
            this.ActivationFunction = _activationFunction;
            this.Bias = _bias == 0 ? false : true;
            this.IterationsCount = _iterations;
            this.LearningFactor = _learningFactor;
            this.Momentum = _momentum;
            this.multipleNeurons = _multipleNeurons == 0 ? true : false;
            this.learningtesting = _learningtesting == 0 ? false : true;
            this.PeopleCount = _peopleNumber;
            return isCorrect;
        }
    }
}
