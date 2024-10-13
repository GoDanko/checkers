using System;
using System.Diagnostics.Metrics;
using System.IO;
using System.Net;
using System.Net.Mime;
using System.Transactions;

class Cheeckers
{    
    // Configuration Variables
    static string basePath = AppDomain.CurrentDomain.BaseDirectory; // Directory where the game is
    static string logPath = ManageLogFiles();
    static string savePath = ManageSaveFiles();
    static bool clearConsole = false;                        // Controls if the draw function deletes previous frames 
    static bool drawBoard = false;                          // Controls if the gameboard is drawn
    static bool drawMenu = true;                            // Controls if the menu is drawn
    static bool drawGUI = true;                             // Controls if the GUI be drawn (debug)

    // More or less important variables
    static bool victory = false;                            // Controls the game loop
    static bool player1Moved;                               // Controls the turns of a player
    static bool? isPremiumSelected;                         // Controls the type of unit that is chosen
    static string log = "";
    static string victoryPrompt = "Game ended undisputed."; // Endgame message
    static (int, int) inputCoordinates = (0,0);             // Holds the player Input (trusted reference: assign once per game loop, and only copy)
    static byte counter = 0;
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

    static string[,] board = new string[20,22] { // Chessboard
        {nRendered, nRendered, nRendered, nRendered, nRendered, nRendered, nRendered, nRendered, nRendered, nRendered, nRendered, nRendered, nRendered, nRendered, nRendered, nRendered, nRendered, nRendered, nRendered, nRendered, nRendered, nRendered},
        {" ", " ", " ", nRendered, border, border2, border2, border2, border2, border2, border2, border2, border2, border2, border2, border2, border2, border2, border2, border2, border, nRendered},
        {"8", " ", " ", nRendered, border, whiteTile, wallY, player2, wallY, whiteTile, wallY, player2, wallY, whiteTile, wallY, player2, wallY, whiteTile, wallY, player2, border, nRendered},
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


    static void Main(string[] args) { // MAIN method
        
        Draw();

        do {
            log = "";
            ExecuteInstruction(RequestInstruction().ToLower());
            LogIt(logPath, log);
            Draw();
            
            if (CheckForVictory()) break;
        } while (!victory);

        LogIt(logPath, "\n           ----------------------------------------------------\n");
        LogIt(logPath, " RESULT:   " + victoryPrompt + "\n");
        if (drawBoard) Console.WriteLine($"\n{victoryPrompt}\n");
    }

    static string ManageSaveFiles() {
        if (!Directory.Exists(Path.Combine(basePath, "Saves"))) Directory.CreateDirectory(Path.Combine(basePath, "Saves"));
        return Path.Combine(basePath, "Saves");
    }

    static void SaveIt() {
        Console.WriteLine("Choose a save name:");
        string? saveName = Console.ReadLine();

        if (saveName != null && saveName.Length > 4) {
            string filePath = Path.Combine(savePath, saveName.ToLower() + ".txt");

            if (!File.Exists(filePath)) {
                using (File.CreateText(filePath)) {}
                LogIt(filePath, ScanBoard(true));
                Console.WriteLine($"the Game has been saved as '{saveName}'");
            } else {
                Console.WriteLine($"File '{saveName}' already exists. Overwrite it? (y/n)");
                string? answer = Console.ReadLine();
                if (answer == "y") Console.WriteLine("Game Saved");
            }
        }
    }

    static void LoadIt() {
        Console.WriteLine("Type the save to load:");
        string? saveName = Console.ReadLine();
        string? saveData = "";

        if (saveName != null && saveName.Length > 4) {
            string loadPath = Path.Combine(basePath, "Saves", saveName.ToLower() + ".txt");
            if (File.Exists(loadPath)) {
                using (StreamReader SaveString = File.OpenText(loadPath)) {
                    saveData = SaveString.ReadLine();
                    saveData += "";
                }
                ScanBoard(false, true, saveData);
                Console.WriteLine($"Loading {saveName}...");
            } else {
                Console.WriteLine("Invalid file name");
            }
        }
    }

    public static string RequestInstruction() { // Request an input and verify its compliance
        string? stringInput = Console.ReadLine();
        if (stringInput == null) stringInput = "";
        return stringInput;
    }

    public static void ExecuteInstruction(string stringInput) { // Assignes token based on the input
        targetCoordinateX = 0;
        targetCoordinateY = 0;
        inputCoordinates = (0,0);

        if (stringInput.Length == 2 && EncodeInstruction(stringInput) && (inputCoordinates.Item1 != 0 || inputCoordinates.Item2 != 0)) {
            if (!player1Moved) GameMaster(player1, selectedPlayer1, premium1, selectedPremium1, player2, premium2, stringInput);
            else GameMaster(player2, selectedPlayer2, premium2, selectedPremium2, player1, premium1, stringInput);
        } else if (stringInput == "run" || stringInput == "start") {
            drawMenu = false;
            drawBoard = true;
        }
        else if (stringInput == "back" || stringInput == "cancel") {
            drawMenu = true;
            drawBoard = false;
        }
        else if (stringInput == "quit" || stringInput == "exit" || stringInput == "leave") {
            victory = true;
            drawGUI = false;
        }
        else if (stringInput == "cls") {
            victory = true;
            drawGUI = false;
            Console.Clear();
        }
        else if (stringInput == "save") {
            SaveIt();
        }
        else if (stringInput == "load") {
            LoadIt();
        }
    }

    public static bool EncodeInstruction(string stringInput) { // translates the input string into a clean token
        for (int i = 0; i < 2; i++) {
            switch (stringInput[i]) {
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
                    targetCoordinateX = 0;
                    targetCoordinateY = 0;
                    break;
            }
        }
        inputCoordinates.Item1 = targetCoordinateX;
        inputCoordinates.Item2 = targetCoordinateY;

        if (inputCoordinates.Item1 == 0 && inputCoordinates.Item2 == 0) return false;
        else return true;
    }

    public static void GameMaster(string thePlayer, string theSelectedPlayer, string premiumFigure, string SelectedPremiumFigure, string theOtherPlayer, string theOtherPremium, string stringInput) { // Contains the game logic and rules
        if (board[targetCoordinateX, targetCoordinateY] == thePlayer || board[targetCoordinateX, targetCoordinateY] == premiumFigure) {
            if (board[targetCoordinateX, targetCoordinateY] == thePlayer) {
                board[targetCoordinateX, targetCoordinateY] = theSelectedPlayer;
                isPremiumSelected = false;
                counter++;
                log = $"\n ENTRY {FormatString(counter, 3, ' ', true)} |{stringInput}            |({FormatString(inputCoordinates.Item1, 2, ' ', false)},{FormatString(inputCoordinates.Item2, 2, ' ', true)})            |Piece Chosen   |";
            } else if (board[targetCoordinateX, targetCoordinateY] == premiumFigure) {
                board[targetCoordinateX, targetCoordinateY] = SelectedPremiumFigure;
                isPremiumSelected = true;
                counter++;
                log = $"\n ENTRY {FormatString(counter, 3, ' ', true)} |{stringInput}            |({FormatString(inputCoordinates.Item1, 2, ' ', false)},{FormatString(inputCoordinates.Item2, 2, ' ', true)})            |Piece Chosen   |";
            }

            playerCoordinateX = inputCoordinates.Item1;
            playerCoordinateY = inputCoordinates.Item2;

            possibleCoordinateX.Item2 = playerCoordinateX + 2;
            possibleCoordinateX.Item1 = playerCoordinateX - 2;
            possibleCoordinateY.Item2 = playerCoordinateY + 2;
            possibleCoordinateY.Item1 = playerCoordinateY - 2;
        }
        else if (board[targetCoordinateX , targetCoordinateY] == blackTile) {
            if (isPremiumSelected == false) {
                if (CheckIfValidInput(false, false, theOtherPlayer, theOtherPremium)) {
                    board[playerCoordinateX, playerCoordinateY] = blackTile;
                    board[targetCoordinateX, targetCoordinateY] = thePlayer;
                    player1Moved = !player1Moved;
                    counter++;
                    log = $"\n ENTRY {FormatString(counter, 3, ' ', true)} |{stringInput}            |({FormatString(inputCoordinates.Item1, 2, ' ', false)},{FormatString(inputCoordinates.Item2, 2, ' ', true)})            |Piece Moved    |";
                }
            } else {
                if (CheckIfValidInput(false, true, theOtherPlayer, theOtherPremium)) {
                    board[playerCoordinateX, playerCoordinateY] = blackTile;
                    board[targetCoordinateX, targetCoordinateY] = premiumFigure;
                    player1Moved = !player1Moved;
                    counter++;
                    log = $"\n ENTRY {FormatString(counter, 3, ' ', true)} |{stringInput}            |({FormatString(inputCoordinates.Item1, 2, ' ', false)},{FormatString(inputCoordinates.Item2, 2, ' ', true)})            |Piece Moved   |";
                }
            }

            possibleCoordinateX = (0,0);
            possibleCoordinateX = (0,0);
            isPremiumSelected = null;

        } else if (board[targetCoordinateX, targetCoordinateY] == theOtherPlayer || board[targetCoordinateX, targetCoordinateY] == theOtherPremium) {
            if (isPremiumSelected == false) {
                if (CheckIfValidInput(true, false, theOtherPlayer, theOtherPremium)) {
                    board[playerCoordinateX, playerCoordinateY] = blackTile;
                    board[targetCoordinateX, targetCoordinateY] = blackTile;
                    board[behindTargetCoordinateX, behindTargetCoordinateY] = thePlayer;
                    player1Moved = !player1Moved;
                    counter++;
                    log = $"\n ENTRY {FormatString(counter, 3, ' ', true)} |{stringInput}            |({FormatString(inputCoordinates.Item1, 2, ' ', false)},{FormatString(inputCoordinates.Item2, 2, ' ', true)})            |{thePlayer} ate {theOtherPlayer}        |";
                }
            } else {
                if (CheckIfValidInput(true, true, theOtherPlayer, theOtherPremium)) {
                    board[playerCoordinateX, playerCoordinateY] = blackTile;
                    board[targetCoordinateX, targetCoordinateY] = blackTile;
                    board[behindTargetCoordinateX, behindTargetCoordinateY] = premiumFigure;
                    player1Moved = !player1Moved;
                    counter++;
                    log = $"\n ENTRY {FormatString(counter, 3, ' ', true)} |{stringInput}            |({FormatString(inputCoordinates.Item1, 2, ' ', false)},{FormatString(inputCoordinates.Item2, 2, ' ', true)})            |{thePlayer} ate {theOtherPlayer}        |";
                }
            }
            
            possibleCoordinateX = (0,0);
            possibleCoordinateX = (0,0);
            isPremiumSelected = null;
        }
    }

    public static bool CheckIfValidInput(bool checkBehind, bool isAllowedBack, string theOtherPlayer, string theOtherPremium) { // checks if the changes are applicable
        if ((targetCoordinateX == possibleCoordinateX.Item1 || targetCoordinateX == possibleCoordinateX.Item2) && (targetCoordinateY == possibleCoordinateY.Item1 || targetCoordinateY == possibleCoordinateY.Item2)) {
            if (targetCoordinateX == possibleCoordinateX.Item2) behindTargetCoordinateX = possibleCoordinateX.Item2 + 2;
            else if (targetCoordinateX == possibleCoordinateX.Item1) behindTargetCoordinateX = possibleCoordinateX.Item1 - 2;
            
            if (targetCoordinateY == possibleCoordinateY.Item2) behindTargetCoordinateY = possibleCoordinateY.Item2 + 2;
            else if (targetCoordinateY == possibleCoordinateY.Item1) behindTargetCoordinateY = possibleCoordinateY.Item1 - 2;

            if (!isAllowedBack) {
                if (!player1Moved) {
                    if (targetCoordinateX == possibleCoordinateX.Item1) {
                        if (!checkBehind) {
                            if (board[targetCoordinateX, targetCoordinateY] == blackTile) return true;
                        } else {
                            if ((board[targetCoordinateX, targetCoordinateY] == theOtherPlayer || board[targetCoordinateX, targetCoordinateY] == theOtherPremium) && board[behindTargetCoordinateX, behindTargetCoordinateY] == blackTile) return true;
                        }
                    }
                } else {              
                    if (targetCoordinateX == possibleCoordinateX.Item2) {      
                        if (!checkBehind) {
                            if (board[targetCoordinateX, targetCoordinateY] == blackTile) return true;
                        } else {
                            if ((board[targetCoordinateX, targetCoordinateY] == theOtherPlayer || board[targetCoordinateX, targetCoordinateY] == theOtherPremium) && board[behindTargetCoordinateX, behindTargetCoordinateY] == blackTile) return true;
                        }
                    }
                }
            }
            else return true;
        }

        return false;
    }

    public static void Draw() { // Universal method used to draw the board or the menu
        if (drawGUI) {
            if (clearConsole) Console.Clear(); // For debugging
            if (drawBoard) {
                for (int i = 1; i < 20; i++) {
                    for (int j = 0; j < 21; j++) {
                        if (i == 18 || j == 3) continue; // This exception is created so the nonRendered value won't be drawn
                        Console.Write(board[i,j]);
                    }
                    Console.Write("\n");
                }
            } else if (drawMenu) {
                Console.WriteLine($"-----------------------/      /-------------------/      /------");
                Console.WriteLine($"------| GAME |--------/      /-------------------/      /-------");
                Console.WriteLine($"------|  TO  |-------/      /-------------------/      /--------");
                Console.WriteLine($"------| PLAY |------/      /-------------------/      /---------");
                Console.WriteLine($"-------------------/      /-------------------/      /----------\n\n");
                Console.WriteLine($"-----------------------------------------------");
                Console.WriteLine($"|                                             |");
                Console.WriteLine($"|         RUN | START - Play the game         |");
                Console.WriteLine($"|         QUIT | EXIT - quit the game         |");
                Console.WriteLine($"|                                             |");
                Console.WriteLine($"-----------------------------------------------"); // Here's the menu in full scale
            }
        }
    }

    public static bool CheckForVictory() { // Scans the board and saves the positions of the pawns
        if (drawBoard) {
            ScanBoard();

            if (player1Coordinates.Count == 0) {
                victoryPrompt = "Player O has won. Congratulations!";
                return true;
            } else if (player2Coordinates.Count == 0) {
                victoryPrompt = "Player X has won. Congratulations!";
                return true;
            }
        }
        return false;
    }

    public static string ScanBoard(bool saveGame = false, bool loadGame = false, string saveData = "") { // finds the positions of the pawns
        player1Coordinates.Clear();
        player2Coordinates.Clear();
        string result = "";
        byte counter = 0;

        for (int i = lowestX; i <= highestX; i += 2) {
            for (int j = highestY; j >= lowestY; j -= 2) {

                if (loadGame) {
                    board[i,j] = saveData[counter + 8].ToString();
                    counter += 10;
                }

                if (board[i,j] == player1 || board[i,j] == selectedPlayer1 || board[i,j] == premium1 || board[i,j] == selectedPremium1) {
                    if (board[i,j] == selectedPlayer1 || board[i,j] == player1) {
                        if (saveGame) {
                            result += $"({FormatString(i,2,'_',true)},{FormatString(j,2,'_',true)})={player1};\n"; // (xx,xx)=x;
                        } else if (board[i,j] == selectedPlayer1) {
                            board[i,j] = player1;
                        }
                    } else if (board[i,j] == selectedPremium1 || board[i,j] == premium1) {
                        if (saveGame) {
                            result += $"({FormatString(i,2,'_',true)},{FormatString(j,2,'_',true)})={premium1};\n";
                        } else if (board[i,j] == selectedPremium1) {
                            board[i,j] = premium1;
                        }
                    }

                    if (i == lowestX) board[i,j] = premium1;
                    player1Coordinates.Add((i,j));

                } else if (board[i,j] == player2 || board[i,j] == selectedPlayer2 || board[i,j] == premium2 || board[i,j] == selectedPremium2) {
                    if (board[i,j] == selectedPlayer2 || board[i,j] == player2) {
                        if (saveGame) {
                            result += $"({FormatString(i,2,'_',true)},{FormatString(j,2,'_',true)})={player2};\n";
                        } else if (board[i,j] == selectedPlayer2) {
                            board[i,j] = player2;
                        }
                    }
                    else if (board[i,j] == selectedPremium2 || board[i,j] == premium2) {
                        if (saveGame) {
                            result += $"({FormatString(i,2,'_',true)},{FormatString(j,2,'_',true)})={premium2};\n";
                        } else if (board[i,j] == selectedPremium2) {
                            board[i,j] = premium2;
                        }
                    }
                    if (i == highestX) board[i,j] = premium2;
                    player2Coordinates.Add((i,j));

                } else if (board[i,j] == blackTile && saveGame) {
                    result += $"({FormatString(i,2,'_',true)},{FormatString(j,2,'_',true)})={blackTile};\n";
                }
            }
        }
        
        return result;
    }

    static string ManageLogFiles() {
        if (!Directory.Exists(Path.Combine(basePath, "Logs"))) Directory.CreateDirectory(Path.Combine(basePath, "Logs"));
        string filePath;
        
        for (int i = 1; i <= 256; i++) {
            filePath = basePath + @"Logs\log" + i + ".txt";

            if (File.Exists(filePath)) {
                continue;

            } else {
                using (File.CreateText(filePath)) {};
                
                using (StreamWriter logs = File.AppendText(filePath)) {
                    logs.WriteLine("\n                        --- Log File ---                        \n");
                    logs.WriteLine("            INPUT          INSTRUCTION         OUTCOME          ");
                    logs.WriteLine("           ____________________________________________________ ");
                    logs.Write(" ENTRY 0   |RUN / START   |(20,0 )            |Game started   | ");
                }
                
                return filePath;
            }
        }

        filePath = basePath + @"Logs/LogXXX.txt";
        if (File.Exists(filePath)) File.Delete(filePath);
        using (File.CreateText(filePath)) {}
        return filePath;
    }

    public static void LogIt(string targetPath, string content) {
        using (StreamWriter logs = File.AppendText(targetPath)) {
            logs.Write(content);
        }
    }

    public static string FormatString(int input, byte maxLength, char filler, bool justifyToLeft) {
        string result = input.ToString();

        if (justifyToLeft) {
            for (int i = maxLength - result.Length; i >= 1; i--) {
                result = result + filler;
            }
        } else if (!justifyToLeft) {
            for (int i = maxLength - result.Length; i >= 1; i--) {
                result = filler + result;
            }
        }

        return result;
    }
}
