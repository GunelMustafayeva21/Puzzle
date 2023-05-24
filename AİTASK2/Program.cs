using System;
using System.Collections.Generic;

class PuzzleNode
{
    public int[,] Board { get; set; }
    public int G { get; set; }
    public int H { get; set; }
    public int F => G + H;
    public PuzzleNode Parent { get; set; }
    public string Move { get; set; }
}

class EightPuzzleGame
{
    private int[,] initialBoard;
    public int[,] goalBoard = { { 1, 2, 3 }, { 4, 5, 6 }, { 7, 8, 0 } };
    private List<PuzzleNode> openList;
    private HashSet<string> closedList;

    public EightPuzzleGame(int[,] initialBoard)
    {
        this.initialBoard = initialBoard;
        openList = new List<PuzzleNode>();
        closedList = new HashSet<string>();
    }

    public void Solve()
    {
        PuzzleNode initialNode = new PuzzleNode
        {
            Board = initialBoard,
            G = 0,
            H = CalculateHeuristic(initialBoard),
            Parent = null,
            Move = ""
        };
        openList.Add(initialNode);

        while (openList.Count > 0)
        {
            openList.Sort((a, b) => a.F.CompareTo(b.F));
            PuzzleNode currentNode = openList[0];
            openList.RemoveAt(0);
            closedList.Add(GetBoardString(currentNode.Board));

            if (IsGoalState(currentNode.Board))
            {
                PrintSolution(currentNode);
                return;
            }

            List<PuzzleNode> nextMoves = GetPossibleMoves(currentNode);
            foreach (PuzzleNode move in nextMoves)
            {
                if (closedList.Contains(GetBoardString(move.Board)))
                    continue;

                int index = openList.FindIndex(n => GetBoardString(n.Board) == GetBoardString(move.Board));
                if (index != -1 && openList[index].F <= move.F)
                    continue;

                openList.Add(move);
            }
        }

        Console.WriteLine("No solution found.");
    }

    private int CalculateHeuristic(int[,] board)
    {
        int distance = 0;
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                int value = board[i, j];
                if (value != 0)
                {
                    int targetRow = (value - 1) / 3;
                    int targetCol = (value - 1) % 3;
                    distance += Math.Abs(i - targetRow) + Math.Abs(j - targetCol);
                }
            }
        }
        return distance;
    }

    private List<PuzzleNode> GetPossibleMoves(PuzzleNode node)
    {
        int emptyRow = -1;
        int emptyCol = -1;

        // Find the coordinates of the empty tile
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if (node.Board[i, j] == 0)
                {
                    emptyRow = i;
                    emptyCol = j;
                    break;
                }
            }
            if (emptyRow != -1)
                break;
        }

        List<PuzzleNode> moves = new List<PuzzleNode>();

        // Move the empty tile in all possible directions (up, down, left, right)
        if (emptyRow > 0)
        {
            int[,] newBoard = (int[,])node.Board.Clone();
            newBoard[emptyRow, emptyCol] = newBoard[emptyRow - 1, emptyCol];
            newBoard[emptyRow - 1, emptyCol] = 0;
            PuzzleNode newNode = new PuzzleNode
            {
                Board = newBoard,
                G = node.G + 1,
                H = CalculateHeuristic(newBoard),
                Parent = node,
                Move = "Up"
            };
            moves.Add(newNode);
        }

        if (emptyRow < 2)
        {
            int[,] newBoard = (int[,])node.Board.Clone();
            newBoard[emptyRow, emptyCol] = newBoard[emptyRow + 1, emptyCol];
            newBoard[emptyRow + 1, emptyCol] = 0;
            PuzzleNode newNode = new PuzzleNode
            {
                Board = newBoard,
                G = node.G + 1,
                H = CalculateHeuristic(newBoard),
                Parent = node,
                Move = "Down"
            };
            moves.Add(newNode);
        }

        if (emptyCol > 0)
        {
            int[,] newBoard = (int[,])node.Board.Clone();
            newBoard[emptyRow, emptyCol] = newBoard[emptyRow, emptyCol - 1];
            newBoard[emptyRow, emptyCol - 1] = 0;
            PuzzleNode newNode = new PuzzleNode
            {
                Board = newBoard,
                G = node.G + 1,
                H = CalculateHeuristic(newBoard),
                Parent = node,
                Move = "Left"
            };
            moves.Add(newNode);
        }

        if (emptyCol < 2)
        {
            int[,] newBoard = (int[,])node.Board.Clone();
            newBoard[emptyRow, emptyCol] = newBoard[emptyRow, emptyCol + 1];
            newBoard[emptyRow, emptyCol + 1] = 0;
            PuzzleNode newNode = new PuzzleNode
            {
                Board = newBoard,
                G = node.G + 1,
                H = CalculateHeuristic(newBoard),
                Parent = node,
                Move = "Right"
            };
            moves.Add(newNode);
        }

        return moves;
    }

    private bool IsGoalState(int[,] board)
    {
        return GetBoardString(board) == GetBoardString(goalBoard);
    }

    private string GetBoardString(int[,] board)
    {
        string boardString = "";
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                boardString += board[i, j].ToString();
            }
        }
        return boardString;
    }

    private void PrintSolution(PuzzleNode node)
    {
        List<PuzzleNode> solutionPath = new List<PuzzleNode>();
        while (node != null)
        {
            solutionPath.Add(node);
            node = node.Parent;
        }
        solutionPath.Reverse();

        foreach (PuzzleNode state in solutionPath)
        {
            Console.WriteLine("Move: " + state.Move);
            PrintBoard(state.Board);
            Console.WriteLine();
        }
    }

    public void PrintBoard(int[,] board)
    {
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                Console.Write(board[i, j] + " ");
            }
            Console.WriteLine();
        }
    }
}

class Program
{
    static void Main(string[] args)
    {
        int[,] initialBoard = GetInitialBoardFromUser();

        EightPuzzleGame game = new EightPuzzleGame(initialBoard);

        Console.WriteLine("Initial Board:");
        game.PrintBoard(initialBoard);

        Console.WriteLine("Final Board:");
        game.PrintBoard(game.goalBoard);

        Console.WriteLine("Step-by-Step Transformation:");
        game.Solve();

        Console.ReadLine();
    }

    static int[,] GetInitialBoardFromUser()
    {
        int[,] initialBoard = new int[3, 3];

        Console.WriteLine("Enter the initial board configuration (0-8):");
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                Console.Write("Enter number at position [" + i + "," + j + "]: ");
                if (!int.TryParse(Console.ReadLine(), out int number) || number < 0 || number > 8)
                {
                    Console.WriteLine("Invalid input. Please enter a number between 0 and 8.");
                    j--;
                    continue;
                }
                initialBoard[i, j] = number;
            }
        }

        return initialBoard;
    }
}
