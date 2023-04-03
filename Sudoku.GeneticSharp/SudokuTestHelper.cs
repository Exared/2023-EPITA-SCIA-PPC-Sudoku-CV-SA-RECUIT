using GeneticSharp;
using GeneticSharp.Extensions;
using System;
using System.Linq;

namespace Sudoku.GeneticSharp
{
    public enum SudokuTestDifficulty
    {
        VeryEasy,
        Easy,
        Medium
    }
    public static class SudokuTestHelper
    {
        private static readonly string _veryEasySudokuString =
            "9.2..54.31...63.255.84.7.6..263.9..1.57.1.29..9.67.53.24.53.6..7.52..3.4.8..4195.";

        private static readonly string _easySudokuString =
            "..48...1767.9.....5.8.3...43..74.1...69...78...1.69..51...8.3.6.....6.9124...15..";

        private static readonly string _mediumSudokuString =
            "..6.......8..542...4..9..7...79..3......8.4..6.....1..2.3.67981...5...4.478319562";


        public static SudokuBoard CreateBoard(SudokuTestDifficulty difficulty)
        {
            string sudokuToParse;
            switch (difficulty)
            {
                case SudokuTestDifficulty.VeryEasy:
                    sudokuToParse = _veryEasySudokuString;
                    break;
                case SudokuTestDifficulty.Easy:
                    sudokuToParse = _easySudokuString;
                    break;
                case SudokuTestDifficulty.Medium:
                    sudokuToParse = _mediumSudokuString;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(difficulty), difficulty, null);
            }

            return SudokuBoard.Parse(sudokuToParse);
        }

        public static SudokuBoard Eval(IChromosome sudokuChromosome, SudokuBoard sudokuBoard, int populationSize,
            double fitnessThreshold, int generationNb)
        {
            SudokuFitness fitness = new SudokuFitness(sudokuBoard);
            EliteSelection selection = new EliteSelection();
            // UniformCrossover crossover = new UniformCrossover();
            // UniformMutation mutation = new UniformMutation();
            OrderedCrossover crossover = new OrderedCrossover();
            PartialShuffleMutation mutation = new PartialShuffleMutation();


            Population population = new Population(populationSize, populationSize, sudokuChromosome);
            GeneticAlgorithm ga = new GeneticAlgorithm(population, fitness, selection, crossover, mutation)
            {
                Termination = new OrTermination(new ITermination[]
                {
                    new FitnessThresholdTermination(fitnessThreshold),
                    new GenerationNumberTermination(generationNb)
                })
            };

            ga.Start();

            ISudokuChromosome bestIndividual = (ISudokuChromosome)ga.Population.BestChromosome;
            IList<SudokuBoard> solutions = bestIndividual.GetSudokus();

            Console.WriteLine($"Best fitness: {solutions.Min(solutionSudoku => fitness.Evaluate(solutionSudoku))}");
            // Return the best solution
            return solutions.First(solutionSudoku => fitness.Evaluate(solutionSudoku) == solutions.Min(solutionSudoku => fitness.Evaluate(solutionSudoku)));
        }
    }
}