

// Author: FreeDOOM#4231 on Discord


using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ASD___6
{
	// To czy algorytm produkuje rozwiązanie optymalne można sprawdzić poprzez porównanie studenta podchodzącego do
	//  największej ilości egzaminów z ilością kolorów użytych przez algorytm (student tworzy graf regularny co oznacza,
	//  że wszystkie wierzchołki tego podgrafu muszą być innych kolorów).

	// Wszystkie algorytmy nie powodują zbytniego narzutu pamięciowego.

	// Właściwym algorytmem heurystycznym proponowanym jako rozwiązanie jest HeuristicSolution_PriorityQueue
	//  HeuristicSolution jest tutaj tylko dla prezentacji i porównania.

	class Program
	{
		readonly static string[] _fileName_ = new string[] {
			"Sesja_Pietrzeniuk_1", // 0
			"Sesja_test",         // 1
			"CrownGraph",        // 2
			"PetersonGraph",    // 3
			"GeneratedGraph",  // 4
			};
		static string fileName;
		static string fileNameIN  => $"{fileName}_in.txt";
		static string fileNameOUT => $"{fileName}_out.txt";

		static void Main(string[] args)
		{
			fileName = AskUserForFileChoice(4);

			DisplayAllSolutions(new Tuple<string, List<List<int>>>[]
				{
					new Tuple<string, List<List<int>>>("Heuristic", HeuristicSolution.Solve(fileNameIN)),
					new Tuple<string, List<List<int>>>("Heuristic+PriorityQueue", HeuristicSolution_PriorityQueue.Solve(fileNameIN)),
					new Tuple<string, List<List<int>>>("BackTracking", BackTrackingSolution.Solve(fileNameIN))
				});

			SaveResults(fileNameOUT, BackTrackingSolution.Solve(fileNameIN), true);
		}


		#region >>> Obsługa plików <<<

		public static TExam[] ConstructGraph<TExam>(string fileName__) where TExam : ICommonExam
		{
			Console.WriteLine($">>>Started loading file \"{fileName__}\"...");

			if (!File.Exists(fileName__))
				throw new FileNotFoundException($"There is no file {fileName__} in program directory.");

			string[] lines = File.ReadAllLines(fileName__);

			int examCount = Convert.ToInt32(lines[0].Split(" ")[0]);
			int studentCount = Convert.ToInt32(lines[0].Split(" ")[1]);

			TExam[] resultArr = new TExam[examCount];
			for (int i = 0; i < examCount; i++)
				resultArr[i].Setup();

			string[] line;
			int[] tempArr;
			for (int i = 1; i <= studentCount; i++)
			{
				tempArr = lines[i].Split(' ').Select(int.Parse).ToArray();
				foreach (int i0 in tempArr)
					foreach (int i1 in tempArr)
						if (i0 != i1)
							resultArr[i0-1].AddEdge(i1 - 1);
			}

			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine($">>>Compleated loading file \"{fileName__}\"!");
			Console.ResetColor();

			return resultArr;
		}

		/// <summary>
		/// Saves only data specified in exercise instruction.
		/// </summary>
		/// <param name="fileName__"></param>
		/// <param name="toDisplay"></param>
		public static void SaveResults(string fileName__, List<List<int>> toDisplay, bool readToConsole = false)
		{
			Console.WriteLine($">>>Started saving to file \"{fileName__}\"...");

			if (File.Exists(fileName__))
				File.Delete(fileName__);

			var builder = new StringBuilder($"{toDisplay.Count}");
			foreach (List<int> list in toDisplay)
				builder.Append($"\n{string.Join(" ", list)}");

			File.WriteAllText(fileName__, builder.ToString());

			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine($">>>Compleated saving to file \"{fileName__}\"!");
			Console.ForegroundColor = ConsoleColor.Gray;

			if (readToConsole)
			{
				Console.WriteLine($"\n{fileName__} contents:\n");
				Console.WriteLine(File.ReadAllText(fileName__));
			}
		}

		#endregion

		public static void DisplayAllSolutions(Tuple<string, List<List<int>>>[] toDisplay)
		{
			foreach (Tuple<string, List<List<int>>> solution in toDisplay)
			{
				Console.WriteLine($"{solution.Item1} solution:");
				Console.Write(solution.Item2.Count);
				foreach (List<int> list in solution.Item2)
					Console.Write($"\n{string.Join(" ", list)}");
				Console.WriteLine("\n");
			}
        }



		static string AskUserForFileChoice(int manualChoice = -1)
		{
			if (manualChoice != -1)
				return _fileName_[manualChoice];
			Console.WriteLine("Choose file");
			Console.WriteLine(string.Join("\n", _fileName_));
			return _fileName_[Convert.ToInt32(Console.ReadLine())];
		}
	}


	// Wszystkie obiekty tego interfejsu korzystają z lokalnych właściwości i najłatwiej jest to zrobić w ten sposób. :/
	//  Chyba, a przynajmniej jestem leniwy i nikt tu nie będzie przecież zaglądał, działa tak samo optymalnie, a nawet
	//  odrobinę optymalniej bo nie wymaga ciągłego przesyłania referencji do lokalnych zmiennych. :P
	public interface ICommonExam
    {
		public void Setup();
		public void AddEdge(int edge);
    }
}
/*
Rozwiązanie można zacząć od zeksplorowania problemu kolorowania grafu, bo wygląda na to, że jeśli potraktować zadanie
  jako zadanie grafowe to studenci i egzaminy tworzą 2 grupy wewnętrznie nie stykających się wierzchołków, co sugeruje,
  że można rozpatrywać kolorowanie z perspektywy egzaminów.
Zadanie rozważamy z punktu widzenia egzaminów i pilnujemy by nie ustawić w jednym dniu kolidujących ze sobą egzaminów,
  egzaminy kolidują ze sobą poprzez to, że dzielą ze sobą studenta, czyli możemy iterować przez zbiór egzaminów i dla
  każdego jeszcze nie pokolorowanego (color == 0) ustawiamy najniższy kolor z którym nie koliduje.
Tak, jak teraz z tym troche pochodziłem to doszłem do wniosku, że faktycznie jest to zwykły problem kolorowania grafu,
  studenci tak naprawde są zbiorem wierzchołków w klice, czyli jeśli student zdaje 3 egzaminy to oznacza, że wierzchołki
  w tym grafie (reprezentujące egzaminy) tworzą trójkąt. Algorytm ma znaleźć liczbę chromatyczną takiego grafu.
 */
