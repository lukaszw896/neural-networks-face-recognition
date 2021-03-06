﻿using FaceRecognition1.Content;
using FaceRecognition1.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FaceRecognition1.Genetic
{
    public class DNA
    {
        //remember that passed random has to have double value in range 0-1
        private Random random;
        private readonly int[] hNeuronsCountBounds = { 2, 90 };
        private readonly int[] hLayersCountBounds = { 0, 3 };
        private readonly int[] iterationCountBouns = { 20000, 30000 };
        private readonly double[] learningFactorBounds = { 0.005, 1.00 };
        private readonly double[] momentumBounds = { 0.005, 0.5 };
        private List<List<Face>> FacesList { get; set; }
        private double fitness = 0.0;
        private SingleTest neuralNetworkData;
        Type type = null;
        public int HNeuronsCount { get; set; }
        public int HLayersCount { get; set; }
        public bool IsBiased { get; set; }
        public int IterationCount { get; set; }
        public double LearningFactor { get; set; }
        public double Momentum { get; set; }
        public bool[] ActiveFeatures { get; set; }
        //TODO
        public DNA(DNA original)
        {
            this.random = original.random;
            this.FacesList = original.FacesList;
            this.HNeuronsCount = original.HNeuronsCount;
            this.HLayersCount = original.HLayersCount;
            this.IsBiased = original.IsBiased;
            this.IterationCount = original.IterationCount;
            this.LearningFactor = original.LearningFactor;
            this.Momentum = original.Momentum;
            this.ActiveFeatures = original.ActiveFeatures;
            this.neuralNetworkData = original.neuralNetworkData;
        }
        public DNA(Random random, List<List<Face>> faces)
        {
            this.type = typeof(DNA);
            this.FacesList = faces;
            this.random = random;
            //12 is a number of features...
            this.ActiveFeatures = new bool[12];
            foreach (PropertyInfo property in this.type.GetProperties())
            {
                property.SetValue(this, this.GetRandomGene(property,true));
            }
        }
        //TODO Save test info to file?
        public double CalculateFitness(string calcStartDate, TimeSpan timeFromStart)
        {
            Console.WriteLine("Fitness calc START");
            this.neuralNetworkData = new SingleTest(/*number of faces...*/15, HNeuronsCount, HLayersCount, IsBiased ? 1 : 0, 1, 60000, LearningFactor, Momentum);
            this.neuralNetworkData.RunTest(FacesList, calcStartDate, timeFromStart, this.ActiveFeatures);
            this.fitness = 100 - neuralNetworkData.TestingError;
            Console.WriteLine("Fitness calc FINISH");
            return this.fitness;
        }

        public DNA Crossover(DNA secondParent)
        {
            var child = new DNA(this.random,this.FacesList);
            var type = typeof(DNA);
            foreach (PropertyInfo property in type.GetProperties())
            {
                if (property.Name == "ActiveFeatures")
                {
                    child.ActiveFeatures = (bool[])this.ActiveFeatures.Clone();
                    for(int i = 0; i < this.ActiveFeatures.Count(); i++)
                    {
                        if (this.random.NextDouble() < 0.5)
                            child.ActiveFeatures[i] = secondParent.ActiveFeatures[i];
                    }
                }
                else
                {
                    property.SetValue(child, this.random.NextDouble() < 0.5 ? property.GetValue(this) : property.GetValue(secondParent));
                }
            }
            return child;
        }
        public void Mutate(double mutationRate)
        {
            foreach (PropertyInfo property in this.type.GetProperties())
            {
                if (this.random.NextDouble() < mutationRate)
                {
                    property.SetValue(this, this.GetRandomGene(property));
                }
            }
            for (int i = 0; i < ActiveFeatures.Length; i++)
                if (this.random.NextDouble() < mutationRate)
                    ActiveFeatures[i] = this.random.NextDouble() > 0.5 ? true : false;
        }
        //0 index lower bound
        //1 index upper bound
        private object GetRandomGene(PropertyInfo property, bool isNewIndividual = false)
        {
            var rand = this.random.NextDouble();
            switch (property.Name)
            {
                case "HNeuronsCount":
                    return hNeuronsCountBounds[0] + (int)Math.Ceiling((double)(hNeuronsCountBounds[1] - hNeuronsCountBounds[0]) * rand);
                case "HLayersCount":
                    return hLayersCountBounds[0] + (int)Math.Ceiling((double)(hLayersCountBounds[1] - hLayersCountBounds[0]) * rand);
                case "IsBiased":
                    return rand > 0.5 ? true : false;
                case "IterationCount":
                    return iterationCountBouns[0] + (int)Math.Ceiling((double)(iterationCountBouns[1] - iterationCountBouns[0]) * rand);
                case "LearningFactor":
                    return learningFactorBounds[0] + (learningFactorBounds[1] - learningFactorBounds[0]) * rand;
                case "Momentum":
                    return momentumBounds[0] + (momentumBounds[1] - momentumBounds[0]) * rand;
                case "ActiveFeatures":
                    if (isNewIndividual == true)
                        for (int i = 0; i < ActiveFeatures.Length; i++)
                            ActiveFeatures[i] = this.random.NextDouble() > 0.5 ? true : false;
                    return ActiveFeatures;
                default:
                    throw new Exception("Given gene was not found during random gene value generation.");
            }
        }
        public double GetFitnessValue()
        {
            return this.fitness;
        }
        public double GetLearningFitnessValue()
        {
            return this.neuralNetworkData.LearningError;
        }
    }
}
