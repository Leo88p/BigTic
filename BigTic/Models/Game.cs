using System.Drawing;

namespace BigTic.Models
{
    public class Cell
    {
        public string value { get; set; }
        public List<Arc> adjesants;
        public Cell()
        {
            value = "empty";
            adjesants = [];
        }

    }
    public class Arc
    {
        public string value { get; set; }
        public List<Cell> adjesants;
        public Arc()
        {
            value = "empty";
            adjesants = [];
        }
        public void Set(string value, Cell adj)
        {
            this.value = value;
            adjesants.Add(adj);
        }
    }
    public class Game
    {
        public List<List<Cell>> field { get; set; }
        public List<List<Arc>> horizontalArcs { get; set; }
        public List<List<Arc>> verticalArcs { get; set; }
        public int player { get; set; }
        public int[] score { get; set; }
        public int maxMoves { get; set; } = -4;
        public int currentMove { get; set; } = 0;
        public bool gameEnded { get; set; } = false;
        public int playerSurrended { get; set; } = 0;

        public string User1 { get; set; }
        public string User2 { get; set; }
        public List<List<Type>> Create2D<Type>(int size) where Type : new()
        {
            List<List<Type>> arr = [];
            for (int i = 0; i < size; i++)
            {
                arr.Add([]);
                for (int j = 0; j < size; j++)
                {
                    arr[i].Add(new Type());
                }
            }
            return arr;
        }
        public Game(int size)
        {
            field = Create2D<Cell>(size);
            horizontalArcs = Create2D<Arc>(size + 1);
            verticalArcs = Create2D<Arc>(size + 1);
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    if (i + j < (size - 1) / 2 || i + j > 1.5 * (size - 1) || i - j < (1 - size) / 2 || j - i < (1 - size) / 2)
                    {
                        field[i][j].value = "empty";
                    }
                    else
                    {
                        field[i][j].value = "0";
                        horizontalArcs[i][j].Set("0", this.field[i][j]);
                        field[i][j].adjesants.Add(horizontalArcs[i][j]);
                        horizontalArcs[i][j + 1].Set("0", this.field[i][j]);
                        field[i][j].adjesants.Add(horizontalArcs[i][j + 1]);
                        verticalArcs[i][j].Set("0", this.field[i][j]);
                        field[i][j].adjesants.Add(verticalArcs[i][j]);
                        verticalArcs[i + 1][j].Set("0", this.field[i][j]);
                        field[i][j].adjesants.Add(this.verticalArcs[i + 1][j]);
                    }
                }
            }
            for (int i = 0; i < size + 1; i++)
            {
                for (int j = 0; j < size + 1; j++)
                {
                    if (verticalArcs[i][j].value != "empty")
                    {
                        if (this.verticalArcs[i][j].adjesants.Count < 2)
                        {
                            verticalArcs[i][j].value = "1";
                        }
                        else
                        {
                            maxMoves++;
                        }
                    }
                    if (this.horizontalArcs[i][j].value != "empty")
                    {
                        if (this.horizontalArcs[i][j].adjesants.Count < 2)
                        {
                            horizontalArcs[i][j].value = "1";
                        }
                        else
                        {
                            maxMoves++;
                        }
                    }
                }
            }
            field[(size - 1) / 2][0].value = "1";
            horizontalArcs[(size - 1) / 2][1].value = "1";
            field[(size - 1) / 2][size - 1].value = "1";
            horizontalArcs[(size - 1) / 2][size - 1].value = "1";

            field[0][(size - 1) / 2].value = "2";
            verticalArcs[1][(size - 1) / 2].value = "1";
            field[size - 1][(size - 1) / 2].value = "2";
            verticalArcs[size - 1][(size - 1) / 2].value = "1";
            player = 0;
            score = [2, 2];
        }
        public double Distance(double x1, double y1, double x2, double y2)
        {
            return Math.Sqrt((Math.Pow((x1 - x2), 2) + Math.Pow((y1 - y2), 2)));
        }
        public bool PointLine(double px, double py, double x1, double y1, double x2, double y2, double buffer)
        {
            return Distance(px, py, x1, y1) + Distance(px, py, x2, y2) < Distance(x1, y1, x2, y2) + buffer;
        }
        public void CheckClick(double x, double y, double r, double w)
        {
            const int size = 9;
            for (int i = 0; i < size + 1; i++)
            {
                for (int j = 0; j < size + 1; j++)
                {
                    List<Cell> adjs = [];
                    if (horizontalArcs[i][j].value != "empty"
                        && PointLine(x, y, w * 0.025 + (i + 0.1) * r, w * 0.025 + j * r, w * 0.025 + (i + 0.9) * r, w * 0.025 + j * r, 5)
                        && horizontalArcs[i][j].value == "0")
                    {
                        horizontalArcs[i][j].value = "1";
                        currentMove++;
                        adjs = horizontalArcs[i][j].adjesants;
                    }
                    else if (verticalArcs[i][j].value != "empty"
                        && PointLine(x, y, w * 0.025 + i * r, w * 0.025 + (j + 0.1) * r, w * 0.025 + i * r, w * 0.025 + (j + 0.9) * r, 5)
                        && verticalArcs[i][j].value == "0")
                    {
                        verticalArcs[i][j].value = "1";
                        currentMove++;
                        adjs = verticalArcs[i][j].adjesants;
                    }
                    if (adjs.Count > 0)
                    {
                        int count = 0;
                        foreach (var adj in adjs)
                        {
                            int arcs = 0;
                            foreach (var a in adj.adjesants)
                            {
                                if (a.value == "1")
                                {
                                    arcs++;
                                }
                            }
                            if (arcs == 4)
                            {
                                adj.value = (player + 1).ToString();
                                count++;
                                score[player]++;
                            }
                        }
                        if (count == 0)
                        {
                            player = 1 - player;
                        }
                    }
                }
            }
        }
    }
}
