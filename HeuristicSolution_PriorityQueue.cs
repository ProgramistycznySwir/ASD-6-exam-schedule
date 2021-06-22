using System;
using System.Collections.Generic;

namespace ASD___6
{
    // Działanie:
    //  Algorytm przechodzi po wszystkich egzaminach i w kolejności od największego do najmniejszego stopnia przypisuje
    //   im pierwszy możliwy termin.
    // Złożoność czasowa:
    //  Złożoność czasowa tego rozwiązania to O(n).

    // Ta wariancja algorytmu heurystycznego jest odrobinę bardziej precyzyjna.
    // Algorytm także często produkuje odpowiednie rezultaty (jak w przypadku grafu dwudzielnego, w którym zwykły polega)
    //  dlatego, że wierzchołki są przemieszane podczas wrzucania ich do kolejki priorytetowaj, co wprowadza element losowości.
    // Zadziwiająco algorytm ma wysoką precyzję, w próbie 5000 losowych grafów z 5 studentami i 10 egzaminami,
    //  pomylił się tylko 2 razy.

    class HeuristicSolution_PriorityQueue
    {
        static Exam[] graph;
        static PriorityQueue<ExamIndex> priorityQueue;
        static int mostExams;
        public static int MostExams => mostExams;


        // Wrapper
        static public List<List<int>> Solve(string fileName)
        {
            graph = Program.ConstructGraph<Exam>(fileName);
            Color();
            return GroupColors();
        }

        // Właściwa metoda zawierająca algorytm.
        static void Color()
        {
            // Skonstruuj kolejkę priorytetową:
            priorityQueue = new PriorityQueue<ExamIndex>(graph.Length);
            for (int i = 0; i < graph.Length; i++)
                priorityQueue.Add(new ExamIndex(i, graph[i].neighbours.Count));

            // Przejdź po wszystkich wierzchołkach grafu zachłannie zaczynając od tych o największym stopniu.
            while (!priorityQueue.IsEmpty)
                graph[priorityQueue.Poll().index].SetFirstNonCollidingColor();
        }
        static List<List<int>> GroupColors()
        {
            var colors = new List<List<int>>();
            for (int i = 0; i < graph.Length; i++)
            {
                while (colors.Count < graph[i].color)
                    colors.Add(new List<int>());
                colors[graph[i].color - 1].Add(i + 1);
            }
            return colors;
        }

        // Ta struktura jest identyczna z HeuristicSolution.Exam, chcesz to możesz połączyć, mi się nie chciało.
        struct Exam : ICommonExam
        {
            public void Setup()
                { color = 0; neighbours = new HashSet<int>(); }
            public void AddEdge(int edge) => neighbours.Add(edge);

            public int color;
            public HashSet<int> neighbours;

            public void SetFirstNonCollidingColor()
            {
                int color = 0;
            REPEAT:
                color++;
                foreach (int neighbour in neighbours)
                    if (graph[neighbour].color == color)
                        goto REPEAT;
                this.color = color;
            }

            public Exam(int color, HashSet<int> neighbours = null)
                => (this.color, this.neighbours) = (color, neighbours ?? new HashSet<int>());

            public override string ToString()
                => $"color:{color} {{{string.Join(",", neighbours)}}}";
            public string ToString(int index)
                => $"{index}. color:{color} {{{string.Join(",", neighbours)}}}";
        }

        struct ExamIndex : IItemWithPriority<ExamIndex>
        {
            public int Priority => priority;

            public readonly int index;
            public readonly int priority;

            public ExamIndex(int index, int priority)
                => (this.index, this.priority) = (index, priority);
        }
    }
}
