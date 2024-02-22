public class Position
{
    public int X { get; set; }
    public int Y { get; set; }

    public Position(int x, int y)
    {
        X = x;
        Y = y;
    }
}

public class Player
{
    public string Name { get; }
    public Position Position { get; set; }
    public int GemCount { get; set; }
    private Game game;

    public Player(string name, Position position, Game game)
    {
        Name = name;
        Position = position;
        GemCount = 0;
        this.game = game;
    }

    public void Move(char direction)
    {
        int newX = Position.X;
        int newY = Position.Y;

        switch (direction)
        {
            case 'U':
                newX--;
                break;
            case 'D':
                newX++;
                break;
            case 'L':
                newY--;
                break;
            case 'R':
                newY++;
                break;
            default:
                Console.WriteLine("Invalid direction");
                return; // Exit the method if the direction is invalid
        }

        // Check if the new position is within the bounds of the board
        if (newX < 0 || newX >= 6 || newY < 0 || newY >= 6)
        {
            Console.WriteLine("Cannot move outside the board.");
            return; // Exit the method if the move is outside the board
        }

        // Check if the new position contains an obstacle
        if (game.board.Grid[newX, newY].Occupant == "O")
        {
            Console.WriteLine("Cannot move into an obstacle.");
            return; // Exit the method if the move is into an obstacle
        }

        // Update the player's position
        Position.X = newX;
        Position.Y = newY;

        // Update the grid with the new player position
        UpdateGrid();
    }


    private void UpdateGrid()
    {
        game.board.Grid[Position.X, Position.Y].Occupant = Name;
        for (int i = 0; i < 6; i++)
        {
            for (int j = 0; j < 6; j++)
            {
                if (game.board.Grid[i, j].Occupant == Name && (i != Position.X || j != Position.Y))
                {
                    game.board.Grid[i, j].Occupant = "-";
                    return;
                }
            }
        }
    }
}

public class Cell
{
    public string Occupant { get; set; }
}

public class Board
{
    public Cell[,] Grid { get; }

    public Board()
    {
        Grid = new Cell[6, 6];
        InitializeBoard();
    }

    private void InitializeBoard()
    {
        // Initialize all cells as empty
        for (int i = 0; i < 6; i++)
        {
            for (int j = 0; j < 6; j++)
            {
                Grid[i, j] = new Cell { Occupant = "-" };
            }
        }

        // Place players
        Grid[0, 0].Occupant = "P1";
        Grid[5, 5].Occupant = "P2";

        // Place gems (randomly for demonstration)
        Random random = new Random();
        for (int i = 0; i < 6; i++)
        {
            int x = random.Next(6);
            int y = random.Next(6);
            if (Grid[x, y].Occupant == "-")
            {
                Grid[x, y].Occupant = "G";
            }
        }

        // Place obstacles (randomly for demonstration)
        for (int i = 0; i < 4; i++)
        {
            int x = random.Next(6);
            int y = random.Next(6);
            if (Grid[x, y].Occupant == "-")
            {
                Grid[x, y].Occupant = "O";
            }
        }
    }

    public void Display()
    {
        for (int i = 0; i < 6; i++)
        {
            for (int j = 0; j < 6; j++)
            {
                Console.Write(Grid[i, j].Occupant + " ");
            }
            Console.WriteLine();
        }
    }

    public bool IsValidMove(Player player, char direction)
    {
        int newX = player.Position.X;
        int newY = player.Position.Y;

        switch (direction)
        {
            case 'U':
                newX--;
                break;
            case 'D':
                newX++;
                break;
            case 'L':
                newY--;
                break;
            case 'R':
                newY++;
                break;
            default:
                return false;
        }

        if (newX < 0 || newX >= 6 || newY < 0 || newY >= 6)
            return false;

        if (Grid[newX, newY].Occupant == "O")
        {
            return false;
        }

        return true;
    }

    public void CollectGem(Player player, char direction)
    {
        int newX = player.Position.X;
        int newY = player.Position.Y;

        switch (direction)
        {
            case 'U':
                newX--;
                break;
            case 'D':
                newX++;
                break;
            case 'L':
                newY--;
                break;
            case 'R':
                newY++;
                break;
            default:
                Console.WriteLine("Invalid direction");
                return; // Exit the method if the direction is invalid
        }

        // Check if the new position is within the bounds of the board
        if (newX < 0 || newX >= 6 || newY < 0 || newY >= 6)
        {
            Console.WriteLine("Cannot move outside the board.");
            return; // Exit the method if the move is outside the board
        }

        Cell currentCell = Grid[newX, newY];

        if (currentCell.Occupant == "G")
        {
            player.GemCount++;
            currentCell.Occupant = "-"; // Remove the collected gem from the board
        }
        else if (currentCell.Occupant == "O")
        {
            // If the current cell contains an obstacle, inform the player
            Console.WriteLine("Cannot collect gem. Obstacle in the way.");
        }
    }
}

public class Game
{
    public Board board; // Change to public
    public Player player1;
    public Player player2;
    public Player currentTurn;
    private int totalTurns;

    public Game()
    {
        board = new Board();
        player1 = new Player("P1", new Position(0, 0), this); // Pass reference to the game
        player2 = new Player("P2", new Position(5, 5), this); // Pass reference to the game
        currentTurn = player1;
        totalTurns = 0;
    }

    public void Start()
    {
        while (!IsGameOver())
        {
            Console.WriteLine($"Turn {totalTurns + 1}");
            board.Display();
            Console.WriteLine($"Current Player: {currentTurn.Name}");
            Console.Write("Enter direction (U/D/L/R): ");
            char direction = char.ToUpper(Console.ReadKey().KeyChar);
            Console.WriteLine();

            if (board.IsValidMove(currentTurn, direction))
            {
                board.CollectGem(currentTurn, direction);
                currentTurn.Move(direction);
                totalTurns++;
                SwitchTurn(); // Switch the turn only after a valid move
            }
            else
            {
                Console.WriteLine("Invalid move!");
            }
        }

        AnnounceWinner();
    }





    private void SwitchTurn()
    {
        if (currentTurn == player1)
            currentTurn = player2;
        else
            currentTurn = player1;
    }

    private bool IsGameOver()
    {
        // Count the remaining gems on the board
        int remainingGems = 0;
        foreach (Cell cell in board.Grid)
        {
            if (cell.Occupant == "G")
            {
                remainingGems++;
            }
        }

        // Game ends if either player has collected all the gems or there are no more gems left
        return totalTurns >= 30 || remainingGems == 0;
    }

    private void AnnounceWinner()
    {
        Console.WriteLine("Game Over!");
        Console.WriteLine($"Player 1 gems: {player1.GemCount}");
        Console.WriteLine($"Player 2 gems: {player2.GemCount}");

        if (player1.GemCount > player2.GemCount)
            Console.WriteLine("Player 1 wins!");
        else if (player2.GemCount > player1.GemCount)
            Console.WriteLine("Player 2 wins!");
        else
            Console.WriteLine("It's a tie!");
    }
}

public class Program
{
    public static Game game;

    static void Main(string[] args)
    {
        game = new Game();
        Player player1 = new Player("P1", new Position(0, 0), game);
        Player player2 = new Player("P2", new Position(5, 5), game);
        game.Start();
    }
}