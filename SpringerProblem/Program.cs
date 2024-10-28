namespace SpringerProblem;

internal static class Program
{
    // Board size
    private const int N = 8;

    // Possible moves for the knight
    private static readonly int[] xMove = { 2 , 1 , -1 , -2 , -2 , -1 , 1 , 2 };
    private static readonly int[] yMove = { 1 , 2 , 2 , 1 , -1 , -2 , -2 , -1 };

    // The solution board
    private static int[,] solution = new int[N , N];

    // Console colors for the chess board
    private static readonly ConsoleColor LightSquareColor = ConsoleColor.White;
    private static readonly ConsoleColor DarkSquareColor = ConsoleColor.DarkGray;
    private static readonly ConsoleColor NumberColor = ConsoleColor.Black;
    private static readonly ConsoleColor BorderColor = ConsoleColor.DarkYellow;

    private static void Main( string[] args )
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.Title          = "Knight's Tour Solver";

        // Initialize solution board with -1
        for ( var i = 0 ; i < N ; i++ )
        for ( var j = 0 ; j < N ; j++ )
            solution[ i , j ] = -1;

        PrintTitle();
        Console.WriteLine( "\nPlease enter the starting position (1-8 for both row and column):" );

        int startX , startY;

        while ( true )
        {
            Console.Write( "Enter starting row (1-8): " );
            if ( !int.TryParse( Console.ReadLine() , out startX ) || startX < 1 || startX > 8 )
            {
                Console.WriteLine( "Invalid input. Please enter a number between 1 and 8." );
                continue;
            }

            Console.Write( "Enter starting column (1-8): " );
            if ( !int.TryParse( Console.ReadLine() , out startY ) || startY < 1 || startY > 8 )
            {
                Console.WriteLine( "Invalid input. Please enter a number between 1 and 8." );
                continue;
            }

            break;
        }

        // Convert to 0-based indexing
        startX--;
        startY--;

        Console.Clear();
        PrintTitle();

        if ( SolveKnightsTour( startX , startY ) )
        {
            Console.WriteLine( "\nSolution found! Here's the knight's tour:\n" );
            PrintSolution();
            PrintLegend( startX , startY );
        }
        else
        {
            Console.WriteLine( "\nNo solution exists from the given starting position." );
        }

        Console.WriteLine( "\nPress any key to exit..." );
        Console.ReadKey();
    }

    private static void PrintTitle()
    {
        Console.WriteLine( @"
╔═══════════════════════════════════╗
║         Springer Problem          ║
║     (Warnsdorff's Algorithm)      ║
╚═══════════════════════════════════╝" );
    }

    private static void PrintLegend( int startX , int startY )
    {
        Console.WriteLine( "\nLegend:" );
        Console.WriteLine( $"• Starting position: ({startX + 1}, {startY + 1})" );
        Console.WriteLine( "• Numbers indicate the order of knight's moves" );
        Console.WriteLine( "• Move 0 is the starting position" );
        Console.WriteLine( $"• Final position: {GetLastMove()}" );
    }

    private static string GetLastMove()
    {
        for ( var i = 0 ; i < N ; i++ )
        {
            for ( var j = 0 ; j < N ; j++ )
            {
                if ( solution[ i , j ] == N * N - 1 )
                    return $"({i + 1}, {j + 1})";
            }
        }

        return "not found";
    }

    private static bool SolveKnightsTour( int startX , int startY )
    {
        var currentX  = startX;
        var currentY  = startY;
        var moveCount = 0;

        solution[ currentX , currentY ] = moveCount;

        while ( moveCount < N * N - 1 )
        {
            var nextMoves = GetNextMovesOrdered( currentX , currentY ).ToList();

            if ( !nextMoves.Any() )
                return false;

            var nextMove = nextMoves.First();
            currentX = nextMove.Item1;
            currentY = nextMove.Item2;
            moveCount++;
            solution[ currentX , currentY ] = moveCount;
        }

        return true;
    }

    private static IEnumerable< Tuple< int , int > > GetNextMovesOrdered( int x , int y )
    {
        var possibleMoves = new List< Tuple< int , int , int > >();

        for ( var i = 0 ; i < 8 ; i++ )
        {
            var nextX = x + xMove[ i ];
            var nextY = y + yMove[ i ];

            if ( IsValidMove( nextX , nextY ) )
            {
                var accessibility = CalculateAccessibility( nextX , nextY );
                possibleMoves.Add( new Tuple< int , int , int >( nextX , nextY , accessibility ) );
            }
        }

        return possibleMoves
              .OrderBy( m => m.Item3 )
              .Select( m => new Tuple< int , int >( m.Item1 , m.Item2 ) );
    }

    private static int CalculateAccessibility( int x , int y )
    {
        var count = 0;

        for ( var i = 0 ; i < 8 ; i++ )
        {
            var nextX = x + xMove[ i ];
            var nextY = y + yMove[ i ];

            if ( IsValidMove( nextX , nextY ) )
                count++;
        }

        return count;
    }

    private static bool IsValidMove( int x , int y ) => x >= 0 && x < N && y >= 0 && y < N && solution[ x , y ] == -1;

    private static void PrintSolution()
    {
        var defaultColor = Console.ForegroundColor;

        // Print column letters
        Console.ForegroundColor = BorderColor;
        Console.Write( "      A   B   C   D   E   F   G   H" );
        Console.WriteLine();

        // Print top border
        Console.Write( "   ┌──" );
        for ( var i = 0 ; i < N + 2 ; i++ )
            Console.Write( "───" );
        Console.WriteLine( "──┐" );

        // Print board
        for ( var i = 0 ; i < N ; i++ )
        {
            Console.ForegroundColor = BorderColor;
            Console.Write( $" {i + 1} │ " ); // Row numbers

            for ( var j = 0 ; j < N ; j++ )
            {
                // Set background color for chess pattern
                Console.BackgroundColor = ( i + j ) % 2 == 0 ? LightSquareColor : DarkSquareColor;
                Console.ForegroundColor = NumberColor;

                // Add padding for better alignment
                var number = solution[ i , j ].ToString().PadLeft( 2 , ' ' );
                Console.Write( $" {number} " );
            }

            // Reset colors and print right border
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = BorderColor;
            Console.WriteLine( " │" );
        }

        // Print bottom border
        Console.Write( "   └──" );
        for ( var i = 0 ; i < N + 2 ; i++ )
            Console.Write( "───" );
        Console.WriteLine( "──┘" );

        // Reset colors
        Console.BackgroundColor = ConsoleColor.Black;
        Console.ForegroundColor = defaultColor;
    }
}