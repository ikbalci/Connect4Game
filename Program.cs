class Program
{
    static int[,]? board;
    static void Main(string[] args)
    {
        int width = 9;
        int height = 9;

        // Connect4Game\bin\Debug\net7.0 <- where .txt files are located
        string fileName = "Hamle.txt";
        string filePath = Path.GetFullPath(fileName);
        var f = new FileInfo(filePath);

        string[] colors = { "White", "Red" };

        string[] players = { "Kenan", "ibrahim" };

        string[] playerColors = { colors[0], colors[1] };

        board = new int[width, height];

        int currentPlayer = 0;

        string? answer = "";

        while (File.Exists(fileName) && f.Length != 0)
        {
            Console.WriteLine("An incomplete game found. Do you want to continue the game y/n?");
            answer = Console.ReadLine();

            if (answer?.ToLower() == "y")
            {
                ClearBoard(board);
                ContinueGame(board);
                break;

            }
            else if (answer?.ToLower() == "n")
            {
                // clear Hamle.txt and Tahta.txt
                File.WriteAllText("Hamle.txt", string.Empty);
                File.WriteAllText("Tahta.txt", string.Empty);
                break;

            }
            else
            {
                Console.WriteLine("Invalid input!\nPlease enter \"y\" or \"n\".\n");
            }
        }

        while (true)
        {
            // display board
            DisplayBoard(board);

            // set next player
            string playerName = players[currentPlayer];
            string playerColor = playerColors[currentPlayer];

            // get player move
            int column = GetPlayerMove(playerName, playerColor, board);

            // add player move into the board
            int row = AddPiece(column, playerColor, board);

            // write player move into the file
            WriteMoveToFile(column, row, playerColor);

            // write the board info into the file
            WriteBoardToFile(board);

            // check if players win
            if (CheckWin(column, row, playerColor, board))
            {
                Console.WriteLine($"{playerName} wins!");
                File.WriteAllText("Hamle.txt", string.Empty);
                File.WriteAllText("Tahta.txt", string.Empty);
                break;
            }

            // check if players draw
            if (CheckDraw(board))
            {
                Console.Write("Game Over! ");
                Console.WriteLine("Players have drawn.");
                File.WriteAllText("Hamle.txt", string.Empty);
                File.WriteAllText("Tahta.txt", string.Empty);
                break;
            }

            // switch players
            currentPlayer = (currentPlayer + 1) % 2;
        }
        Console.ReadKey();
    }

    // displays the game board in console
    static void DisplayBoard(int[,] board)
    {
        int numRows = board.GetLength(1);
        int numCols = board.GetLength(0);

        // display column numbers
        Console.Write("  ");
        for (int col = 0; col < numCols; col++)
        {
            Console.Write($"  {col + 1}");
        }
        Console.WriteLine();

        // display the board
        for (int row = 0; row < numRows; row++)
        {
            Console.Write(row + 1 + "|");
            for (int col = 0; col < numCols; col++)
            {
                if (board[col, row] == 0)
                {
                    Console.Write("  ");
                }
                else if (board[col, row] == 1)
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write("X ");
                    Console.ResetColor();
                }
                else if (board[col, row] == 2)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write("O ");
                    Console.ResetColor();
                }
                Console.Write("|");
            }
            Console.WriteLine();
            Console.WriteLine(" " + new string('-', (numCols * 3) + 1));
        }
    }

    // gets player moves
    static int GetPlayerMove(string playerName, string playerColor, int[,] board)
    {
        while (true)
        {
            Console.Write(playerName + " (" + playerColor + "), which column would you like to play? ");
            string? input = Console.ReadLine();
            int column;
            if (int.TryParse(input, out column) && column >= 1 && column <= board.GetLength(0))
            {
                int row = GetNextOpenRow(column - 1, board);
                if (row == -1)
                {
                    Console.WriteLine("Column is full. Please choose another column.");
                    continue;
                }
                return column - 1;
            }
            Console.WriteLine("Invalid move. Please enter a number between 1 and " + board.GetLength(0));
        }
    }

    static int GetNextOpenRow(int column, int[,] board)
    {
        for (int row = board.GetLength(1) - 1; row >= 0; row--)
        {
            if (board[column, row] == 0)
            {
                return row;
            }
        }
        return -1;
    }

    // adds player moves 
    static int AddPiece(int column, string color, int[,] board)
    {
        for (int i = board.GetLength(1) - 1; i >= 0; i--)
        {
            if (board[column, i] == 0)
            {
                board[column, i] = (color == "White") ? 1 : 2;
                return i;
            }
        }
        // if column is full, return -1
        return -1;
    }

    // writes player moves to file
    static void WriteMoveToFile(int column, int row, string color)
    {
        // opens Hamle.txt file or creates
        using (StreamWriter file = new StreamWriter("Hamle.txt", true))
        {
            string move = $"{row + 1}{column + 1} {color}";
            file.WriteLine(move);
        }
    }

    // writes the board to file
    static void WriteBoardToFile(int[,] board)
    {
        using (StreamWriter file = new StreamWriter("Tahta.txt", false))
        {
            for (int row = 0; row < board.GetLength(1); row++)
            {
                for (int column = 0; column < board.GetLength(0); column++)
                {
                    if (board[column, row] == 0)
                    {
                        file.Write("_ ");
                    }
                    else if (board[column, row] == 1)
                    {
                        file.Write("W ");
                    }
                    else
                    {
                        file.Write("R ");
                    }
                }
                file.WriteLine();
            }
        }
    }

    // continues the game if did not end
    static void ContinueGame(int[,] board)
    {
        if (File.Exists("Hamle.txt"))
        {
            string[] moves = File.ReadAllLines("Hamle.txt");
            foreach (string move in moves)
            {
                // parse the move info from the file
                int row = int.Parse(move.Substring(0, 1)) - 1;
                int column = int.Parse(move.Substring(1, 1)) - 1;
                string color = move.Substring(3);

                // add the pieces to the board
                AddPiece(column, color, board);
            }
        }
    }

    // clears the board
    static void ClearBoard(int[,] board)
    {
        for (int i = 0; i < board.GetLength(0); i++)
        {
            for (int j = 0; j < board.GetLength(1); j++)
            {
                board[i, j] = 0;
            }
        }
    }

    // checks if players win
    static bool CheckWin(int column, int row, string color, int[,] board)
    {
        int piece = (color == "White") ? 1 : 2;

        // horizontal check
        int count = 0;
        for (int i = 0; i < board.GetLength(0); i++)
        {
            if (i <= column && board[i, row] == piece)
            {
                count++;
                if (count == 4)
                {
                    DisplayBoard(board);
                    return true;
                }
            }
            else
            {
                count = 0;
            }
        }

        // vertical check
        count = 0;
        for (int i = 0; i < board.GetLength(1); i++)
        {
            if (board[column, i] == piece)
            {
                count++;
                if (count == 4)
                {
                    DisplayBoard(board);
                    return true;
                }
            }
            else
            {
                count = 0;
            }
        }

        // right diagonal check
        count = 0;
        int x1 = column;
        int y1 = row;
        while (x1 > 0 && y1 < board.GetLength(1) - 1)
        {
            x1--;
            y1++;
        }
        while (x1 < board.GetLength(0) && y1 >= 0)
        {
            if (board[x1, y1] == piece)
            {
                count++;
                if (count == 4)
                {
                    DisplayBoard(board);
                    return true;
                }
            }
            else
            {
                count = 0;
            }
            x1++;
            y1--;
        }

        // left diagonal check
        count = 0;
        int x2 = column;
        int y2 = row;
        while (x2 < board.GetLength(0) - 1 && y2 < board.GetLength(1) - 1)
        {
            x2++;
            y2++;
        }
        while (x2 >= 0 && y2 >= 0)
        {
            if (board[x2, y2] == piece)
            {
                count++;
                if (count == 4)
                {
                    DisplayBoard(board);
                    return true;
                }
            }
            else
            {
                count = 0;
            }
            x2--;
            y2--;
        }
        return false;
    }

    // checks if players draw
    static bool CheckDraw(int[,] board)
    {
        int totalMoves = 0;
        for (int i = 0; i < board.GetLength(0); i++)
        {
            for (int j = 0; j < board.GetLength(1); j++)
            {
                if (board[i, j] != 0)
                {
                    totalMoves++;
                }
            }
        }
        if (totalMoves == 80)
        {
            DisplayBoard(board);
            return true;
        }
        return false;
    }
}