using System;
using System.Collections.Generic;
using System.Text;

namespace ASD___6
{
    // Działanie:
    //  Jest to algorytm z powrotami nakierowany na rozwiązanie poprzez algorytmy heurystyczne i zachłanne.
    //  Można powiedzieć, że algorytm w głównej mierze upewnia się czy algorytm heurystyczny zwrócił najoptymalniejszą wartość.
    // Złożoność czasowa:
    //  Pesymistyczna złożoność czasowa tego rozwiązania to O(c^(n-1)), gdzie n oznacza ilość egzaminów, a c to liczba
    //   chromatyczna oszacowana przez algorytm heurystyczny (najbardziej pesymistycznie c=n).
    //  Dlatego, że jest to algorytm kierowany heurystycznie c zazwyczaj zmniejsza się w czasie wykonania algorytmu i
    //   średnia złożoność czasowa jest mniejsza.


    // Optymalizacje:
    //  Algorytm heurystyczny jest po to by ograniczyć przestrzeń poszukiwań, (nie ma sensu rozważać rozwiązań
    //   wykorzystujących więcej kolorów niż algorytm heurystyczny).
    //  Algorytm także nie poszukuje dalej rozwiązań jeśli znalazł rozwiązanie z ilością kolorów równych liczbie
    //   wierzchołków w największej klice znanej w grafie (słowo klucz "znanej", czasami możliwa jest sytuacja, że ta
    //   liczba jest większa, jednak obliczenie precyzyjnie tej liczby wymaga znania liczby chromatycznej grafu, co już
    //   nie pomogłoby algorytmowi).
    //  Przestrzeń poszukiwań jest także zawężana za każdym razem kiedy algorytm natrafi na lepsze rozwiązanie od dotychczas
    //   znalezionego wtedy nie próbuje użyć nie mniej niż ilość kolorów w obecnie najoptymalniejszym rozwiązaniu.

    // Powyższe zabiegi mają na celu ograniczenie c, co dalej nie zmienia faktu, że algorytm ma złożoność wykładniczą.
    //  Warto jednak zwrócić uwagę, że nawet zmniejszenie c o 1, wyjątkowo pozytywnie wpływa na czas wykonania algorytmu,
    //  nawet dla małych liczb jak c=3, zmniejszenie c o 1 dla 10 egzaminów oznacza ok. 600-krotnie mniejszą przestrzeń
    //  poszukiwań, a ta różnica rośnie wraz ze zwiększeniem się c i n (zwłaszcza n).

    class BackTrackingSolution
    {
        static Exam[] graph;
        static int VertCount => graph.Length;
        static int GraphMaxIndex => graph.Length - 1;
        static int mostExams;

        static List<List<int>> bestSolution;
        // Algorytm szuka koloru mniejszego od tej wartości więc wszędzie niech próbuje używać mniej niż tej liczby.
        static int BestSolutionColorCount => bestSolution.Count;

        // Teoretycznie powinno się tę liczbę określać na podstawie faktycznie użytych kolorów w grafie, załóżmy wrzucać
        //  je do HashSet'u jednak jeśli na początku zamiast 1wszego koloru użyjemy 2gi, to nie wpływa na ostateczną
        //  ilość użytych kolorów, bo użycie innego koloru oznacza tylko inny wariant tej samej permutacji.
        //  W taki sposób ogranicza się przestrzeń poszukiwań.
        static int currentColorCount;
        static public List<List<int>> Solve(string fileName)
        {
            // Najpierw algorytm próbuje heurystycznego rozwiązania.
            //bestSolution = HeuristicSolution_PriorityQueue.Solve(graphData);
            bestSolution = HeuristicSolution_PriorityQueue.Solve(fileName);
            if (BestSolutionColorCount == mostExams)
                return bestSolution;

            // Inicjalizacja poszukiwania optymalnego rozwiązania.
            graph = Program.ConstructGraph<Exam>(fileName);
            currentColorCount = 0;

            // Nie ma to jak zmienić język w środku dokumentacji, z racji, że nie chce mi się tłumaczyć, musi wystarczyć
            //  po angielsku.
            // i means color to check next in solution-tree
            for (int i = 1; i < BestSolutionColorCount; i++)
            {
                bool isColorAddedToCount = i > currentColorCount;
                currentColorCount += isColorAddedToCount ? 1 : 0;

                if (StepIn(0, i))
                    return bestSolution;

                // Erase changes:
                currentColorCount += isColorAddedToCount ? -1 : 0;
            }

            return bestSolution;
        }

        // Bool here indicates that the best posible solution was found and further search is futile.
        static public bool StepIn(int vert, int color)
        {
            graph[vert].color = color;
            //Console.WriteLine($"><> Into vert({vert})) {{currentColorCount: {currentColorCount}}}, BestSolutionColorCount: {BestSolutionColorCount}");
            // Reached end.
            if (vert >= GraphMaxIndex)
            {
                // Verify solution:
                if(currentColorCount < BestSolutionColorCount)
                {
                    bestSolution = GroupColors();
                    // If true, we can't go any lower and/or can't optimise further.
                    return BestSolutionColorCount == mostExams;
                }
                return false;
            }
            // i means color to check next in solution-tree
            for (int i = 1; i < BestSolutionColorCount; i++)
                if (graph[vert + 1].IsValidColor(i))
                {
                    //Console.WriteLine($"><> At vert({vert}), color ({i}) {{currentColorCount: {currentColorCount}}}, BestSolutionColorCount: {BestSolutionColorCount}");
                    bool isColorAddedToCount = i > currentColorCount;
                    currentColorCount += isColorAddedToCount ? 1 : 0;

                    if (StepIn(vert + 1, i))
                        return true;

                    // Erase changes:
                    currentColorCount += isColorAddedToCount ? -1 : 0;
                }

            // Erase this step:
            graph[vert].color = 0;
            return false;
        }


        static List<List<int>> GroupColors()
        {
            //DEBUG_DisplayGraphData();

            var colors = new List<List<int>>();
            for (int i = 0; i < graph.Length; i++)
            {
                if (colors.Count < graph[i].color)
                    colors.Add(new List<int>());
                //Console.WriteLine(graph[i].color - 1);
                colors[graph[i].color - 1].Add(i + 1);
            }
            return colors;
        }

        public static void DEBUG_DisplayGraphData()
        {
            Console.WriteLine("Graph:");
            for(int i = 0; i < VertCount; i++)
                Console.WriteLine(graph[i].ToString(i));
            Console.WriteLine();
        }

        struct Exam : ICommonExam
        {
            public void Setup()
                {color = 0; neighbours = new HashSet<int>(); }
            public void AddEdge(int edge) => neighbours.Add(edge);

            public int color;
            public HashSet<int> neighbours;

            public bool IsValidColor(int color)
            {
                foreach (int neighbour in neighbours)
                    if (graph[neighbour].color == color)
                        return false;
                return true;
            }

            public Exam(int color, HashSet<int> neighbours = null)
                => (this.color, this.neighbours) = (color, neighbours ?? new HashSet<int>());

            public override string ToString()
                => $"color:{color} {{{string.Join(",", neighbours)}}}";
            public string ToString(int index)
                => $"{index}. color:{color} {{{string.Join(",", neighbours)}}}";
        }
    }
}
