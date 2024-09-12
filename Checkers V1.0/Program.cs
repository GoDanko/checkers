using System;
using System.IO;
using System.Net;

class Cheeckers
{    
    // More or less important variables
    static bool victory = false;                            // Controls the game loop
    static bool drawBoard = false;                          // Controls if the gameboard is drawn
    static bool drawMenu = true;                            // Controls if the menu is drawn
    static bool drawGUI = true;                             // Controls if the GUI be drawn (debug)
    static bool player1Moved;                               // Controls the turns of a player
    static bool? isPremiumSelected;                         // Controls the type of unit that is chosen
    static string victoryPrompt = "Game ended undisputed."; // Endgame message
    static string basePath = AppDomain.CurrentDomain.BaseDirectory;// Directory where the game is
    static (int, int) inputCoordinates = (0,0);             // Holds the player Input (trusted reference: assign once per game loop, and only copy)
    static int playerCoordinateX;                           // Holds the second memory layer for coordinate X
    static int playerCoordinateY;                           // Holds the second memory layer for coordinate Y
    static int targetCoordinateX;                           // Holds the actual coordinate X of the player
    static int targetCoordinateY;                           // Holds the actual coordinate Y of the player
    static int behindTargetCoordinateX;                     // Holds the coordinate X of value behind the TargetCoordinate
    static int behindTargetCoordinateY;                     // Holds the coordinate Y of value behind the TargetCoordinate
    static (int, int) possibleCoordinateX = (0,0);          // Holds the actual coordinate Y of the player
    static (int, int) possibleCoordinateY = (0,0);          // Holds the actual coordinate Y of the player
    static int highestX = 16;                               // Holds the variable of the highest possible coordinate X
    static int lowestY = 5;                                 // Holds the variable of the lowest possible coordinate Y
    static int highestY = lowestY + 14;                     // Holds the variable of the highest possible coordinate Y
    static int lowestX = highestX - 14;                     // Holds the variable of the lowest possible coordinate X
    static List<(int,int)> player1Coordinates = new List<(int,int)> ();
    static List<(int,int)> player2Coordinates = new List<(int,int)> ();
    
    // Various pieces that make the board
    static string whiteTile = " ";
    static string blackTile = " ";
    static string border = "#";
    static string border2 = "=";
    static string wallY = "|";
    static string wallx = "-";
    static string wallJoint = "+";
    static string player1 = "x";
    static string player2 = "o";
    static string selectedPlayer1 = "X";
    static string selectedPlayer2 = "O";
    static string premium1 = "m";
    static string premium2 = "w";
    static string selectedPremium1 = "M";
    static string selectedPremium2 = "W";
    static string nRendered = "C";

    static string[,] board = new string[20,22] // Chessboard
    {
        {nRendered, nRendered, nRendered, nRendered, nRendered, nRendered, nRendered, nRendered, nRendered, nRendered, nRendered, nRendered, nRendered, nRendered, nRendered, nRendered, nRendered, nRendered, nRendered, nRendered, nRendered, nRendered},
        {" ", " ", " ", nRendered, border, border2, border2, border2, border2, border2, border2, border2, border2, border2, border2, border2, border2, border2, border2, border2, border, nRendered},
        {"8", " ", " ", nRendered, border, whiteTile, wallY, player1, wallY, whiteTile, wallY, player2, wallY, whiteTile, wallY, player2, wallY, whiteTile, wallY, player2, border, nRendered},
        {" ", " ", " ", nRendered, border, wallx, wallJoint, wallx, wallJoint, wallx, wallJoint, wallx, wallJoint, wallx, wallJoint, wallx, wallJoint, wallx, wallJoint, wallx, border, nRendered},
        {"7", " ", " ", nRendered, border, player2, wallY, whiteTile, wallY, player2, wallY, whiteTile, wallY, player2, wallY, whiteTile, wallY, player2, wallY, whiteTile, border, nRendered},
        {" ", " ", " ", nRendered, border, wallx, wallJoint, wallx, wallJoint, wallx, wallJoint, wallx, wallJoint, wallx, wallJoint, wallx, wallJoint, wallx, wallJoint, wallx, border, nRendered},
        {"6", " ", " ", nRendered, border, whiteTile, wallY, blackTile, wallY, whiteTile, wallY, blackTile, wallY, whiteTile, wallY, blackTile, wallY, whiteTile, wallY, blackTile, border, nRendered},
        {" ", " ", " ", nRendered, border, wallx, wallJoint, wallx, wallJoint, wallx, wallJoint, wallx, wallJoint, wallx, wallJoint, wallx, wallJoint, wallx, wallJoint, wallx, border, nRendered},
        {"5", " ", " ", nRendered, border, blackTile, wallY, whiteTile, wallY, blackTile, wallY, whiteTile, wallY, blackTile, wallY, whiteTile, wallY, blackTile, wallY, whiteTile, border, nRendered},
        {" ", " ", " ", nRendered, border, wallx, wallJoint, wallx, wallJoint, wallx, wallJoint, wallx, wallJoint, wallx, wallJoint, wallx, wallJoint, wallx, wallJoint, wallx, border, nRendered},
        {"4", " ", " ", nRendered, border, whiteTile, wallY, blackTile, wallY, whiteTile, wallY, blackTile, wallY, whiteTile, wallY, blackTile, wallY, whiteTile, wallY, blackTile, border, nRendered},
        {" ", " ", " ", nRendered, border, wallx, wallJoint, wallx, wallJoint, wallx, wallJoint, wallx, wallJoint, wallx, wallJoint, wallx, wallJoint, wallx, wallJoint, wallx, border, nRendered},
        {"3", " ", " ", nRendered, border, blackTile, wallY, whiteTile, wallY, blackTile, wallY, whiteTile, wallY, blackTile, wallY, whiteTile, wallY, blackTile, wallY, whiteTile, border, nRendered},
        {" ", " ", " ", nRendered, border, wallx, wallJoint, wallx, wallJoint, wallx, wallJoint, wallx, wallJoint, wallx, wallJoint, wallx, wallJoint, wallx, wallJoint, wallx, border, nRendered},
        {"2", " ", " ", nRendered, border, whiteTile, wallY, player1, wallY, whiteTile, wallY, player1, wallY, whiteTile, wallY, player1, wallY, whiteTile, wallY, player1, border, nRendered},
        {" ", " ", " ", nRendered, border, wallx, wallJoint, wallx, wallJoint, wallx, wallJoint, wallx, wallJoint, wallx, wallJoint, wallx, wallJoint, wallx, wallJoint, wallx, border, nRendered},
        {"1", " ", " ", nRendered, border, player1, wallY, whiteTile, wallY, player1, wallY, whiteTile, wallY, player1, wallY, whiteTile, wallY, player1, wallY, whiteTile, border, nRendered},
        {" ", " ", " ", nRendered, border, border2, border2, border2, border2, border2, border2, border2, border2, border2, border2, border2, border2, border2, border2, border2, border, nRendered},
        {nRendered, nRendered, nRendered, nRendered, nRendered, nRendered, nRendered, nRendered, nRendered, nRendered, nRendered, nRendered, nRendered, nRendered, nRendered, nRendered, nRendered, nRendered, nRendered, nRendered, nRendered, nRendered},
        {" ", " ", " ", nRendered, " ", "A", " ", "B", " ", "C", " ", "D", " ", "E", " ", "F", " ", "G", " ", "H", " ", nRendered}
    };

    static void Main(string[] args) // MAIN method
    {

        Draw(board);

        do
        {
            if(ScanBoard()) break;
            InstructionController(RequestInput());
            Draw(board);
        }
        while (!victory);

        Console.WriteLine($"\n{victoryPrompt}\n");
    }

    public static int RequestInput() // Request an input and verify its compliance
    {
        string? stringInput = Console.ReadLine();
        inputCoordinates = InputController(stringInput);

        if(inputCoordinates.Item2 == 0)
        {
            return inputCoordinates.Item1;
        }
        else return 0;
    }

    public static (int, int) InputController(string? stringInput) // Assignes token based on the input
    {
        if(stringInput?.Length == 2 && drawBoard)
        {
            inputCoordinates = CheckIfIsCoordinate(stringInput);
        }
        else if(stringInput == "run" || stringInput == "start")
        {
            inputCoordinates = (20, 0);
        }
        else if(stringInput == "back" || stringInput == "cancel")
        {
            inputCoordinates = (21, 0);
        }
        else if(stringInput == "quit" || stringInput == "exit" || stringInput == "leave")
        {
            inputCoordinates = (22, 0);
        }
        else if(stringInput == "cls")
        {
            inputCoordinates = (23, 0);
        }
        else if(stringInput == "scan")
        {
            inputCoordinates = (24, 0);
        }

        string logFormula = Convert.ToString(inputCoordinates.Item1) + ":" + Convert.ToString(inputCoordinates.Item2);
        LoggerOut(logFormula);

        return inputCoordinates;
    }

    public static void LoggerOut(string toBeLogged)
    {
        if(!File.Exists(Path.Combine(basePath, "log.txt")))
        {
            using (File.CreateText(Path.Combine(basePath, "log.txt"))) {}
        }
        
        using (StreamWriter logs = File.AppendText(Path.Combine(basePath, "log.txt")))
        {
            logs.WriteLine($"{toBeLogged}");
        }
    }

    public static (int, int) CheckIfIsCoordinate(string stringInput) // translates the input string into a clean token
    {
        for(int i = 0; i < 2; i++)
        {
            switch(stringInput[i])
            {
                case '1':
                    targetCoordinateX = highestX;
                    break;
                case '2':
                    targetCoordinateX = highestX - 2;
                    break;
                case '3':
                    targetCoordinateX = highestX - 4;
                    break;
                case '4':
                    targetCoordinateX = highestX - 6;
                    break;
                case '5':
                    targetCoordinateX = highestX - 8;
                    break;
                case '6':
                    targetCoordinateX = highestX - 10;
                    break;
                case '7':
                    targetCoordinateX = highestX - 12;
                    break;
                case '8':
                    targetCoordinateX = lowestX;
                    break;
                case 'a':
                    targetCoordinateY = lowestY;
                    break;
                case 'b':
                    targetCoordinateY = highestY - 12;
                    break;
                case 'c':
                    targetCoordinateY = highestY - 10;
                    break;
                case 'd':
                    targetCoordinateY = highestY - 8;
                    break;
                case 'e':
                    targetCoordinateY = highestY - 6;
                    break;
                case 'f':
                    targetCoordinateY = highestY - 4;
                    break;
                case 'g':
                    targetCoordinateY = highestY - 2;
                    break;
                case 'h':
                    targetCoordinateY = highestY;
                    break;
                default:
                    targetCoordinateX = 20;
                    targetCoordinateY = 0;
                    break;
            }
        }

        return (targetCoordinateX, targetCoordinateY);
    }

    public static void InstructionController(int input) // Applies changes
    {
        if(input == 0)
        {
            if(drawBoard && !drawMenu)
            {
                if(!player1Moved)
                {
                    GameMaster(player1, selectedPlayer1, premium1, selectedPremium1, player2, premium2);
                }
                else
                {
                    GameMaster(player2, selectedPlayer2, premium2, selectedPremium2, player1, premium1);
                }
            }
        }

        if(input == 20) // Start the game
        {
            drawBoard = true;
            drawMenu = false;
        }
        else if(input == 21) // Move back to the menu
        {
            drawMenu = true;
            drawBoard = false;
        }
        else if(input == 22) // Quit the game
        {
            victory = true;
            drawGUI = false;
        }
        else if(input == 23) // Quit and clear
        {
            victory = true;
            drawGUI = false;
            Console.Clear();
        }
        else if(input == 24)
        {
            ScanBoard();
        }
    }

    public static void GameMaster(string thePlayer, string theSelectedPlayer, string premiumFigure, string SelectedPremiumFigure, string theOtherPlayer, string theOtherPremium) // Contains the game logic and rules
    {
        if(board[targetCoordinateX, targetCoordinateY] == thePlayer || board[targetCoordinateX, targetCoordinateY] == premiumFigure)
        {
            if(board[targetCoordinateX, targetCoordinateY] == thePlayer)
            {
                board[targetCoordinateX, targetCoordinateY] = theSelectedPlayer;
                isPremiumSelected = false;
            }
            else if(board[targetCoordinateX, targetCoordinateY] == premiumFigure)
            {
                board[targetCoordinateX, targetCoordinateY] = SelectedPremiumFigure;
                isPremiumSelected = true;
            }

            playerCoordinateX = inputCoordinates.Item1;
            playerCoordinateY = inputCoordinates.Item2;

            possibleCoordinateX.Item2 = playerCoordinateX + 2;
            possibleCoordinateX.Item1 = playerCoordinateX - 2;
            possibleCoordinateY.Item2 = playerCoordinateY + 2;
            possibleCoordinateY.Item1 = playerCoordinateY - 2;
        }
        else if(board[targetCoordinateX , targetCoordinateY] == blackTile)
        {
            if(isPremiumSelected == false)
            {
                if(CheckIfValidInput(false, false, theOtherPlayer, theOtherPremium))
                {
                    board[playerCoordinateX, playerCoordinateY] = blackTile;
                    board[targetCoordinateX, targetCoordinateY] = thePlayer;
                    player1Moved = !player1Moved;
                }
            }
            else
            {
                if(CheckIfValidInput(false, true, theOtherPlayer, theOtherPremium))
                {
                    board[playerCoordinateX, playerCoordinateY] = blackTile;
                    board[targetCoordinateX, targetCoordinateY] = premiumFigure;
                    player1Moved = !player1Moved;
                }
            }

            possibleCoordinateX = (0,0);
            possibleCoordinateX = (0,0);
            isPremiumSelected = null;
        }
        else if(board[targetCoordinateX, targetCoordinateY] == theOtherPlayer || board[targetCoordinateX, targetCoordinateY] == theOtherPremium)
        {
            if(isPremiumSelected == false)
            {
                if(CheckIfValidInput(true, false, theOtherPlayer, theOtherPremium))
                {
                    board[playerCoordinateX, playerCoordinateY] = blackTile;
                    board[targetCoordinateX, targetCoordinateY] = blackTile;
                    board[behindTargetCoordinateX, behindTargetCoordinateY] = thePlayer;
                    player1Moved = !player1Moved;
                }
            }
            else
            {
                if(CheckIfValidInput(true, true, theOtherPlayer, theOtherPremium))
                {
                    board[playerCoordinateX, playerCoordinateY] = blackTile;
                    board[targetCoordinateX, targetCoordinateY] = blackTile;
                    board[behindTargetCoordinateX, behindTargetCoordinateY] = premiumFigure;
                    player1Moved = !player1Moved;
                }
            }
            
            possibleCoordinateX = (0,0);
            possibleCoordinateX = (0,0);
            isPremiumSelected = null;
        }
    }

    public static bool CheckIfValidInput(bool checkBehind, bool isAllowedBack, string theOtherPlayer, string theOtherPremium) // checks if the changes are applicable
    {
        if((targetCoordinateX == possibleCoordinateX.Item1 || targetCoordinateX == possibleCoordinateX.Item2) && (targetCoordinateY == possibleCoordinateY.Item1 || targetCoordinateY == possibleCoordinateY.Item2))
        {
            if(targetCoordinateX == possibleCoordinateX.Item2) behindTargetCoordinateX = possibleCoordinateX.Item2 + 2;
            else if(targetCoordinateX == possibleCoordinateX.Item1) behindTargetCoordinateX = possibleCoordinateX.Item1 - 2;
            
            if(targetCoordinateY == possibleCoordinateY.Item2) behindTargetCoordinateY = possibleCoordinateY.Item2 + 2;
            else if(targetCoordinateY == possibleCoordinateY.Item1) behindTargetCoordinateY = possibleCoordinateY.Item1 - 2;

            if(!isAllowedBack)
            {
                if(!player1Moved)
                {
                    if(targetCoordinateX == possibleCoordinateX.Item1)
                    {
                        if(!checkBehind)
                        {
                            if(board[targetCoordinateX, targetCoordinateY] == blackTile) return true;
                        }
                        else
                        {
                            if((board[targetCoordinateX, targetCoordinateY] == theOtherPlayer || board[targetCoordinateX, targetCoordinateY] == theOtherPremium) && board[behindTargetCoordinateX, behindTargetCoordinateY] == blackTile) return true;
                        }
                    }
                }
                else
                {              
                    if(targetCoordinateX == possibleCoordinateX.Item2)
                    {      
                        if(!checkBehind)
                        {
                            if(board[targetCoordinateX, targetCoordinateY] == blackTile) return true;
                        }
                        else
                        {
                            if((board[targetCoordinateX, targetCoordinateY] == theOtherPlayer || board[targetCoordinateX, targetCoordinateY] == theOtherPremium) && board[behindTargetCoordinateX, behindTargetCoordinateY] == blackTile) return true;
                        }
                    }
                }
            }
            else return true;
        }

        return false;
    }

    public static void Draw(string[,] board) // Universal method used to draw the board or the menu
    {
        if(drawGUI)
        {
            Console.Clear();

            if(drawBoard)
            {
                for(int i = 1; i < 20; i++)
                {
                    for(int j = 0; j < 21; j++)
                    {
                        if(i == 18 || j == 3){ // This exception is created so the nonRendered value won't be drawn
                            continue;
                        }
                        Console.Write(board[i, j]);
                    }
                    Console.Write("\n");
                }
            }
            else if(drawMenu)
            {
                Console.WriteLine($"-----------------------/      /-------------------/      /------");
                Console.WriteLine($"------| GAME |--------/      /-------------------/      /-------");
                Console.WriteLine($"------|  TO  |-------/      /-------------------/      /--------");
                Console.WriteLine($"------| PLAY |------/      /-------------------/      /---------");
                Console.WriteLine($"-------------------/      /-------------------/      /----------\n\n");
                Console.WriteLine($"-----------------------------------------------");
                Console.WriteLine($"|                                             |");
                Console.WriteLine($"|   RUN | START - Play the game               |");
                Console.WriteLine($"|   QUIT | EXIT | LEAVE - quit the game       |");
                Console.WriteLine($"|                                             |");
                Console.WriteLine($"-----------------------------------------------"); // Here's the menu in full scale
            }
        }
    }

    public static bool ScanBoard() // Scans the board and saves the positions of the pawns
    {
        if(drawBoard)
        {
            GetPlayersPositions();

            if(player1Coordinates.Count == 0)
            {
                victoryPrompt = "Player O has won. Congratulations!";
                return true;
            }
            else if(player2Coordinates.Count == 0)
            {
                victoryPrompt = "Player X has won. Congratulations!";
                return true;
            }
        }
        return false;
    }

    public static void GetPlayersPositions() // finds the positions of the pawns
    {
        
        player1Coordinates.Clear();
        player2Coordinates.Clear();

        for(int i = lowestX; i <= highestX; i += 2)
        {
            for(int j = highestY; j >= lowestY; j -= 2)
            {
                if(board[i,j] == player1 || board[i,j] == selectedPlayer1 || board[i,j] == premium1 || board[i,j] == selectedPremium1)
                {
                    if(board[i,j] == selectedPlayer1) board[i,j] = player1;
                    else if(board[i,j] == selectedPremium1) board[i,j] = premium1;

                    if(i == lowestX) board[i,j] = premium1;

                    player1Coordinates.Add((i,j));
                    
                    // Console.WriteLine($"player1 has {board[i,j]} on ({i},{j})"); // Useful for debug
                }
                else if(board[i,j] == player2 || board[i,j] == selectedPlayer2 || board[i,j] == premium2 || board[i,j] == selectedPremium2)
                {
                    if(board[i,j] == selectedPlayer2) board[i,j] = player2;
                    else if(board[i,j] == selectedPremium2) board[i,j] = premium2;

                    if(i == highestX) board[i,j] = premium2;

                    player2Coordinates.Add((i,j));

                    // Console.WriteLine($"player2 has {board[i,j]} on ({i},{j})"); // Useful for debug
                }
            }
        }
    }
}
