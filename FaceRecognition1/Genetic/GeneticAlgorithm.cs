using FaceRecognition1.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaceRecognition1.Genetic
{
    public class GeneticAlgorithm
    {
        public List<DNA> Population { get; private set; }
        public int Generation { get; private set; }
        public double BestFitness { get; private set; }
        private double[] IndividualTicketTable { get; set; }
        public DNA BestSpecimen;
        public int Elitism;
        public double MutationRate;
        private List<Face> faces;
        private List<DNA> newPopulation;
        private Random random;
        private double fitnessSum;

        public GeneticAlgorithm(int populationSize, Random random, int elitism, List<Face> faces, double mutationRate = 0.01f)
        {
            Generation = 1;
            Elitism = elitism;
            MutationRate = mutationRate;
            Population = new List<DNA>(populationSize);
            newPopulation = new List<DNA>(populationSize);
            this.random = random;
            this.faces = faces;
            this.IndividualTicketTable = new double[populationSize];
            this.GenerateTickets();
            for (int i = 0; i < populationSize; i++)
            {
                Population.Add(new DNA(random, faces));
            }
        }

        public void NewGeneration(int numNewDNA = 0, bool crossoverNewDNA = false)
        {
            int finalCount = Population.Count + numNewDNA;

            if (finalCount <= 0)
            {
                return;
            }

            if (Population.Count > 0)
            {
                CalculateFitness();
                Population.Sort(CompareDNA);
            }
            newPopulation.Clear();
            if (numNewDNA != 0)
                this.GenerateTickets();
            for (int i = 0; i < finalCount; i++)
            {
                if (i < Elitism && i < Population.Count)
                {
                    newPopulation.Add(Population[i]);
                }
                else if (i < Population.Count || crossoverNewDNA)
                {
                    DNA parent1 = ChooseParent();
                    DNA parent2 = ChooseParent();

                    DNA child = parent1.Crossover(parent2);

                    child.Mutate(MutationRate);
                    newPopulation.Add(child);
                }
                else
                {
                    newPopulation.Add(new DNA(random, this.faces));
                }
            }
            List<DNA> tmpList = Population;
            Population = newPopulation;
            newPopulation = tmpList;
            Generation++;
        }
        private DNA ChooseParent()
        {
            var treshold = (Population.Count - 1) * (Population.Count - 1) * random.NextDouble();
            int index = Population.Count - 3 - (int)Math.Ceiling(Math.Sqrt(treshold));
            if (index < 0)
                index = 0;
            return Population[index];
            //double randomNumber = random.NextDouble() * fitnessSum;

            //for (int i = 0; i < Population.Count; i++)
            //{
            //    if (randomNumber < Population[i].GetFitnessValue())
            //    {
            //        return Population[i];
            //    }
            //    randomNumber -= Population[i].GetFitnessValue();
            //}
            //return null;
        }
        //More tickets -> more chance to become a parent
        //Bigger fitness function -> more tickets
        //If population count is stable than it can be run only once
        private void GenerateTickets()
        {
            for (int i = 0; i < this.Population.Count; i++)
            {
                this.IndividualTicketTable[i] = (Population.Count - i) ^ 2;
            }
        }

        private void CalculateFitness()
        {
            fitnessSum = 0;
            DNA best = Population[0];
            //for(int i=0;i< Population.Count; i++)
            //{
            //    Population[i].CalculateFitness();
            //}
            Parallel.For(0, Population.Count, (x) => Population[x].CalculateFitness());
            for (int i = 0; i < Population.Count; i++)
            {
                fitnessSum += Population[i].GetFitnessValue();

                if (Population[i].GetFitnessValue() > best.GetFitnessValue())
                {
                    best = Population[i];
                }
            }
            BestFitness = best.GetFitnessValue();
            this.BestSpecimen = new DNA(best);
        }

        private int CompareDNA(DNA a, DNA b)
        {
            if (a.GetFitnessValue() > b.GetFitnessValue())
                return -1;
            else if (a.GetFitnessValue() < b.GetFitnessValue())
                return 1;
            else
                return 0;
        }

    }
}
