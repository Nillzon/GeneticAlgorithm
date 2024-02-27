using GeneticAlgorithm;

string targetString = "Trying out a really long string for this Genetic Algorithm program that i just created, hope it doesn't break!";
Population problem = new(1000, 0.0005f, targetString);
problem.GenerateSolution();

namespace GeneticAlgorithm
{
    public class DNA
    {
        private readonly string target;
        private readonly Random rnd = new();
        public char[] genes;

        public DNA(string target)
        {
            this.target = target;
            genes = new char[target.Length];
            for (int i = 0; i < genes.Length; i++)
            {
                //32, 128 representerar ASCII kodernas start och slut för bokstäverna så att individernas DNA kan tilldelas bokstäver från a-z
                //använd 32, 246 om du vill använda bokstäver från a-ö (OBS! detta innebär väldigt många special characters)
                genes[i] = (char)rnd.Next(32, 128);
            }
        }

        //Beräknar fitnesspoäng, ju högre fitnesspoäng betyder att generna i detta DNA träffar fler rätt bokstäver från frasen vi försöker generera
        public float EvaluateFitness()
        {
            int score = 0;
            for (int i = 0; i < genes.Length; i++)
            {
                if (genes[i] == target[i])
                {
                    score++;
                }
            }

            //Fitness: 0 = ingen bokstav rätt
            //Fitness: 1 = alla bokstäver rätt
            return (float)score / target.Length;
        }

        //Muterar generna (bokstäverna) i individernas DNA
        public void Mutate(float mutationRate)
        {
            for (int i = 0; i < genes.Length; i++)
            {
                if (rnd.NextDouble() < mutationRate)
                {
                    //32, 246 representerar ASCII kodernas start och slut för bokstäverna så att individernas DNA kan "muteras" med värden från a-ö
                    //32, 128 representerar ASCII kodernas start och slut för bokstäverna så att individernas DNA kan "muteras" med värden från a-z
                    genes[i] = (char)rnd.Next(32, 128);
                }
            }
        }

        public override string ToString()
        {
            return new string(genes);
        }
    }

    public class Population
    {
        private readonly int totalPopulation;
        private readonly float mutationRate;
        private readonly string target;
        private DNA[] population;
        private readonly Random rnd = new();
        private int generationCount = 1;

        /// <summary>
        /// <para>totalPopulation = antalet individer i varje generation</para>
        /// <para>mutationRate (float) = mutationsgraden på dess barn</para>
        /// <para>target = frasen som vi vill generera</para>
        /// </summary>
        /// <param name="totalPopulation"></param>
        /// <param name="mutationRate"></param>
        /// <param name="target"></param>
        public Population(int totalPopulation, float mutationRate, string target)
        {
            this.totalPopulation = totalPopulation;
            this.mutationRate = mutationRate;
            this.target = target;
            InitializePopulation();
        }

        private void InitializePopulation()
        {
            //Här lägger vi till totala antalet individer till befolkningen (de olika generationerna)
            population = new DNA[totalPopulation];
            for (int i = 0; i < population.Length; i++)
            {
                population[i] = new DNA(target);
            }
        }

        //Själva motorn i programmet som ska hitta lösningen
        public void GenerateSolution()
        {
            int random = rnd.Next(population.Length);

            //Loopar igenom generationerna tills en generation hittat den korrekta frasen
            while (true)
            {
                EvaluatePopulationFitness();
                if (CheckForSolution())
                {
                    Console.WriteLine("\nGeneration: " + generationCount + " lyckades få fram frasen: {0}", population[0]);
                    break;
                }
                Console.Write("Generation: " + generationCount + " Har fått fram denna slumpartade fras genom mutation: ");
                Console.WriteLine(population[random].ToString());
                CreateNextGeneration();
                generationCount++;

            }
        }

        //utvärderar varje individs Fitnesspoäng för att se hur nära de är att hitta den korrekta frasen
        private void EvaluatePopulationFitness()
        {
            //Fitness: 0 = ingen bokstav rätt
            //Fitness: 1 = alla bokstäver rätt
            foreach (var individual in population)
            {
                individual.EvaluateFitness();
            }
            Array.Sort(population, (a, b) => b.EvaluateFitness().CompareTo(a.EvaluateFitness()));
        }

        //Kollar om vi hittat den korrekta frasen
        private bool CheckForSolution()
        {
            return population[0].ToString() == target;
        }

        //Parar slumpmässigt föräldrarna och muterar dess barn så att de "utvecklas" 
        //Detta för att barnen förhoppningsvis ska få en bättre fitness score än föregående föräldrar
        private void CreateNextGeneration()
        {
            List<DNA> newPopulation = new();
            for (int i = 0; i < totalPopulation; i++)
            {
                DNA parentA = ChooseParent();
                DNA parentB = ChooseParent();
                DNA child = Crossover(parentA, parentB);
                child.Mutate(mutationRate);
                newPopulation.Add(child);
            }
            population = newPopulation.ToArray();
        }

        //metod för att slumpmässigt välja en förälder till barnet
        private DNA ChooseParent()
        {
            int index = (int)(rnd.NextDouble() * rnd.Next(1, totalPopulation));
            return population[index];
        }

        //här tilldelas föräldrarnas gener (bokstäver) till barnet
        private DNA Crossover(DNA parentA, DNA parentB)
        {
            DNA child = new(target);
            int midpoint = rnd.Next(child.genes.Length);
            for (int i = 0; i < child.genes.Length; i++)
            {
                child.genes[i] = i > midpoint ? parentA.genes[i] : parentB.genes[i];
            }
            return child;
        }
    }
}