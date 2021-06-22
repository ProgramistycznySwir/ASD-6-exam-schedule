using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace ASD___6
{
    // Działanie:
    //  Algorytm zwyczajnie przechodzi po wszystkich egzaminach i ustawia im pierwszy możliwy termin.
    // Złożoność czasowa:
    //  Złożoność czasowa tego rozwiązania to O(n).


    class HeuristicSolution
    {
        static Exam[] graph;
        static int mostExams;
        public static int MostExams => mostExams;


        // Wrapper.
        static public List<List<int>> Solve(string fileName, bool readToConsole = false)
        {
            graph = Program.ConstructGraph<Exam>(fileName);
            Color();
            return GroupColors();
        }

        static void Color()
        {
            for (int i = 0; i < graph.Length; i++)
                graph[i].SetFirstNonCollidingColor();
        }
        static List<List<int>> GroupColors()
        {
            var colors = new List<List<int>>();
            for (int i = 0; i < graph.Length; i++)
            {
                if (colors.Count < graph[i].color)
                    colors.Add(new List<int>());
                colors[graph[i].color - 1].Add(i + 1);
            }
            return colors;
        }

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

            // Kolejna dziwna składnia, googluj "C# null coalescing operator" :)
            public Exam(int color, HashSet<int> neighbours = null)
                => (this.color, this.neighbours) = (color, neighbours ?? new HashSet<int>());

            public override string ToString()
                => $"color:{color} {{{string.Join(",", neighbours)}}}";
            public string ToString(int index)
                => $"{index}. color:{color} {{{string.Join(",", neighbours)}}}";
        }
    }
}
