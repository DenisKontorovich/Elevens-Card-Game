using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

/*Author: Denis Kontorovich 
 File Name: Program.cs
 Project Name: Elevens
 Creation Date: 02/06/2020
 Modified Date: 02/16/2020
 Description: A card game which involves the shuffling of a card deck and dealing out 12 cards to the user.The user has two potential moves which they can make. The
 first move the user can make is to move a face card. This can only be done if the pile that the user chooses to move has a face card and only one card in the pile, the
 face card is moved to the bottom of the deck, and the card on the top of the deck is placed on the pile. The second move is for the user to choose two cards which add
 up to eleven based on their face value. Face cards have a values less of 0, while aces have a value of 1. If there is an eleven pair then a card from the deck is added to the first 
 pile selected and the second pile selected. Game continues until all twelve of a user's cards have a face card such as king, queen, or jacks, or the user runs out of moves to do.
 */
namespace Elevens
{
    class Program
    {
        //Random object allowing for random number generation when shuffling cards
        static Random rng = new Random();

        //Store the twelve piles of cards(gameboard)
        static int[,] playableCards = new int[2, 6];

        //Store the number of cards in each of the card piles
        static int[,] cardsInPile = new int[2, 6];

        //Store the cards in the deck
        static List<int> cardDeck = new List<int>();

        //Store values that represent the different screens in-game
        const int MAIN_MENU = 0;
        const int MOVE_CARD = 1;
        const int ELEVEN_PAIR = 2;

        //Store value used to determine whether a card is a face card
        const int FACE_CARD = 0;

        //Store value used to create four of every card/face card 
        const int CATEGORY = 4;

        //Store the current game state
        static int gameState;

        //Store the user input for which move they would like to perform in main menu
        static int playSelection;

        //Store values for the card pairings that are meant to make an eleven pair
        static int firstCard;
        static int secondCard;

        //Store value for the actual sum of the card pairings 
        static int sumCards = 0;

        //Store locations on the game board for the two cards selected
        //For the two cards which are supposed to have a sum of eleven
        static int firstCardLocX;
        static int firstCardLocY;
        static int secondCardLocX;
        static int secondCardLocY;

        //Store value for the row and column on the gameboard
        static int row;
        static int col;
        
        //Store user input for selecting card piles
        static char firstLetter;
        static char secondLetter;
        static char letterSelection;

        //Store value used to organize and display the cards
        static string spacer = "  ";

        //Store value for the users selection while in the main menu
        static string userMenuSelection;

        //Store value of letters used to label the card piles
        static string[,] designatedLetters = new string[2, 6];

        //Store value for whether user input is valid
        static bool invalidInput = false;

        //Store value to check if the user is still playing the game, if the user won, or if the user lost
        static bool isGamePlaying = true;
        static bool checkWin = false;
        static bool checkLoss = false;
        
        static void Main(string[] args)
        {
            //Set the current game state to main menu
            gameState = MAIN_MENU;

            //Set the values of the letters which represent the card piles
            designatedLetters[0, 0] = "a";
            designatedLetters[0, 1] = "b";
            designatedLetters[0, 2] = "c";
            designatedLetters[0, 3] = "d";
            designatedLetters[0, 4] = "e";
            designatedLetters[0, 5] = "f";
            designatedLetters[1, 0] = "g";
            designatedLetters[1, 1] = "h";
            designatedLetters[1, 2] = "i";
            designatedLetters[1, 3] = "j";
            designatedLetters[1, 4] = "k";
            designatedLetters[1, 5] = "l";

            //Places the cards in the deck
            setDeck();
              
            //Shuffle the Deck
            ShuffleDeck(1000);

            //Set the cards in the appropriate position on the game board
            SetCards();

            //Display the cards
            DealCards();

            //Perform the following tasks as long as the user is still playing the game
            while (isGamePlaying == true)
            {
                //Clear the user display
                Console.Clear();

                //Display the gameboard/cards
                DealCards();

                //Permorm the appropriate tasks depending on which state the game is in
                switch (gameState)
                {
                    case MAIN_MENU:
                        {
                            //User has not yet entered an invalid input
                            invalidInput = false;

                            //Display text for the main menu and receive the user input for what option is selected
                            Console.WriteLine("Choose an option");
                            Console.WriteLine("1) Move a Face Card");
                            Console.WriteLine("2) State an Eleven Pair");
                            userMenuSelection = Console.ReadLine();

                            //When the user's input is a valid option
                            if (Int32.TryParse(userMenuSelection, out playSelection ) == true)
                            {
                                //Convert the users input into a number so that the game state can be determined
                                playSelection = Convert.ToInt32(userMenuSelection);
                            }
                            else
                            {
                                //User entered an ivalid option
                                invalidInput = true;
                            }

                            //Check the option the user has chosen
                            if (playSelection == 1)
                            {
                                //Game is currently allowing for the user to move a card
                                gameState = MOVE_CARD;
                            }
                            else if (playSelection == 2)
                            {
                                //Game is currently allowing for the user to choose an eleven pair
                                gameState = ELEVEN_PAIR;
                            }
                            else
                            {
                                //User entered an invalid option
                                invalidInput = true;
                            }

                            //When user entered an invalid otpion
                            if(invalidInput == true)
                            {
                                //Display an "error" message, giving the user an option to continue
                                Console.WriteLine("Invalid option selected");
                                Console.WriteLine("Press ENTER to continue");
                                Console.ReadLine();
                            }
                        }
                        break;
                    case MOVE_CARD:
                        {
                            //Store value for the location of a card pile
                            Point pileLoc;
                            
                            //Display the option for the user to choose a letter representing a card pile and receive the user's selected letter
                            Console.WriteLine("Choose a pile by its designated letter");
                            letterSelection = Convert.ToChar(Console.ReadLine());
                            
                            //Determine the location of the pile based on the letter selected
                            pileLoc = FindPile(letterSelection);
                            
                            //When the card in the pile is a face card
                            if (playableCards[pileLoc.Y, pileLoc.X] < FACE_CARD)
                            {
                                //When there is only one card in the pile
                                if (cardsInPile[pileLoc.Y, pileLoc.X] == 1)
                                {
                                    //Replace the card in the pile with the card at the top of the deck
                                    cardDeck.Add(playableCards[pileLoc.Y, pileLoc.X]);
                                    playableCards[pileLoc.Y, pileLoc.X] = cardDeck[0];
                                    cardDeck.RemoveAt(0);

                                    //Check whether the user lost and won
                                    CheckLoss();
                                    CheckWin();

                                    //The game is currently in main menu
                                    gameState = MAIN_MENU;
                                }
                                else
                                {
                                    //The letter selected is invalid
                                    invalidInput = true;
                                }
                            }
                            else
                            {
                                //The letter selected is invalid
                                invalidInput = true;
                            }
                            
                            //When the letter selected is invalid
                            if (invalidInput == true)
                            {
                                //Display an "error" message, giving the user an option to continue
                                Console.WriteLine("Sorry, that is an invalid pile letter to modify");
                                Console.WriteLine("Press ENTER to continue");
                                Console.ReadLine();

                                //The game is in main menu
                                gameState = MAIN_MENU;
                            }
                        }
                        break;
                    case ELEVEN_PAIR:
                        {
                            //Store location for the piles of the first card and second card that will be chosen
                            Point pileLoc1;
                            Point pileLoc2;

                            //Display for the user to choose the first pile and receive the first letter chosen
                            Console.WriteLine("Choose the first pile by its designated letter");
                            firstLetter = Convert.ToChar(Console.ReadLine());

                            //Display for the user to choose the second pile and receive the second letter chosen
                            Console.WriteLine("Choose the second pile by its designated letter that is no pile");
                            secondLetter = Convert.ToChar(Console.ReadLine());

                            //Determine the location of the first pile based on the first letter chosen
                            pileLoc1 = FindPile(firstLetter);

                            //When the card in the first pile is not a face card
                            if (playableCards[pileLoc1.Y, pileLoc1.X] > FACE_CARD)
                            {
                                //Set the value of the first card
                                firstCard = playableCards[pileLoc1.Y, pileLoc1.X];

                                //Determine the coordinates or the row and column of the first card
                                firstCardLocX = pileLoc1.Y;
                                firstCardLocY = pileLoc1.X;
                            }
                            else
                            {
                                //The card selected is invalid
                                invalidInput = true;
                            }

                            //Determine the location of the second pile based on the first letter chosen
                            pileLoc2 = FindPile(secondLetter);

                            //When the card in the first pile is not a face card
                            if (playableCards[pileLoc2.Y, pileLoc2.X] > FACE_CARD)
                            {
                                //Set the value of the second card
                                secondCard = playableCards[pileLoc2.Y, pileLoc2.X];

                                //Determine the coordinates or the row and column of the second card
                                secondCardLocX = pileLoc2.Y;
                                secondCardLocY = pileLoc2.X;
                            }
                            else
                            {
                                //The card selected is invalid
                                invalidInput = true;
                            }

                            //Calculate the sum of the two chosen cards by adding the value of the two cards
                            sumCards = firstCard + secondCard;

                            //When the sum of the two cards is 11
                            if (sumCards == 11 )
                            {
                                //Add cards from the deck to the piles of the corresponding cards
                                CardReplacement(firstCardLocX, firstCardLocY);
                                CardReplacement(secondCardLocX, secondCardLocY);

                                //Check whether user has lost or won
                                CheckLoss();
                                CheckWin();
                                
                                //Game is currently in main menu
                                gameState = MAIN_MENU;

                            }
                            else if (sumCards != 11 || invalidInput == true)
                            {
                                //Output an "error" message, giving the user an option to continue
                                Console.WriteLine("Sorry the cards selected do not add up to the value of eleven");
                                Console.WriteLine("Press ENTER to try again");
                                Console.ReadLine();

                                //The game is currently in main menu
                                gameState = MAIN_MENU;
                            }

                        }
                        break;
                }
            }

            //Clear user display and display cards
            Console.Clear();
            DealCards();

            //Check whether user lost or won
            if(checkLoss == true)
            {
                //Display losing message
                Console.WriteLine("You LOST");
            }
            else if(checkWin == true)
            {
                //Display winning message
                Console.WriteLine("You WON");
            }

            //Display friendly message for the user
            Console.WriteLine("Thanks for playing");

            Console.ReadLine();

        }

        //Pre: N/A
        //Post: N/A
        //Description: Creates the card deck by placing the values of the cards in the deck
        private static void setDeck()
        {
            //For every number between 0 and the number of cards in the deck with the same value
            for (int i = 0; i < CATEGORY; i++)
            {
                //For every number between 0 and 10
                for (int j = 1; j <= 10; j++)
                {
                    //Add a value between 1 and 10 to the deck of cards
                    cardDeck.Add(j);
                }
            }

            //For every number between 0 and the number of cards in the deck with the same value
            for (int j = 0; j < CATEGORY; j++)
            {
                //Add a value of -1 to the deck of cards
                //Representing the king card
                cardDeck.Add(-1);
            }

            //For every number between 0 and the number of cards in the deck with the same value
            for (int k = 0; k < CATEGORY; k++)
            {
                //Add a value of -2 to the deck of cards
                //Representing the queen card
                cardDeck.Add(-2);

            }

            //For every number between 0 and the number of cards in the deck with the same value
            for (int l = 0; l < CATEGORY; l++)
            {
                //Add a value of -3 to the deck of cards
                //Representing the jack card
                cardDeck.Add(-3);
            }
        }


        //Pre: number of times the deck will be shuffled
        //Post: N/A
        //Description: Shuffles the card deck by generating two random numbers between 0 and 
        //the number of cards in the deck and rearranging the cards
        private static void ShuffleDeck(int numShuffles)
        {
            //Store the random numbers and placeholders for the random numbers
            int randomNumPlaceholder;
            int randomNum;
            int randomNum2;

            //For every number between 0 and the number of shuffles
            for (int i = 0; i <= numShuffles; i++)
            {
                
                //Generate two random numbers between 0 and the number of cards in the deck
                randomNum = rng.Next(0, cardDeck.Count);
                randomNum2 = rng.Next(0, cardDeck.Count);

                //Set placeholder equal to a card at a random position in the card deck
                randomNumPlaceholder = cardDeck[randomNum];

                //Rearrange the cards between the two positions
                cardDeck[randomNum] = cardDeck[randomNum2];
                cardDeck[randomNum2] = randomNumPlaceholder;

            }
        }

        //Pre:N/A
        //Post:N/A
        //Description: Takes the first twelve cards from the deck and stores it in a 2x6 gameboard
        private static void SetCards()
        {
            //Set counter
            int counter = 0;

            //For every number between 0 and the number of rows in the game board
            for (int row = 0; row < playableCards.GetLength(0); row++)
            {
                //For every number between 0 and the number of columns in the gameboard
                for (int col = 0; col < playableCards.GetLength(1); col++)
                {
                    
                    //Set the number of cards in each pile to 1
                    cardsInPile[row, col] = 1;

                    //Set card from the deck to the position on the gameboard and remove the card from the deck
                    playableCards[row, col] = cardDeck[counter];
                    cardDeck.RemoveAt(counter);

                    //Increase counter
                    counter++;

                }
            }
        }

        //Pre: N/A
        //Post: N/A
        //Description: Create a visual display for the user of the 
        // 2x6 game board of cards
        private static void DealCards()
        {
            //For every number between 0 and the number of rows in the gameboard
            for (int row = 0; row < playableCards.GetLength(0); row++)
            {
                //For every number between 0 and the number of columns in the gameboard
                for (int col = 0; col < playableCards.GetLength(1); col++)
                {
                    
                    //When the first row has been displayed
                    if (row == 1 && col == 0)
                    {
                        //Add a spacer to seperate the first and second row on the gameboard
                        Console.WriteLine(spacer);
                    }

                    //When card is a face card
                    if (playableCards[row, col] < FACE_CARD)
                    {
                        //Change the colour of the text to red
                        Console.ForegroundColor = ConsoleColor.Red;
                    }
                    
                    //Check the value of the card
                    if (playableCards[row, col] == 1)
                    {
                        //Display the card as an ace, and the appropriate letter for the pile, and the number of cards in the pile
                        Console.Write("A(" + designatedLetters[row, col] + ":" + cardsInPile[row, col] + ")" + spacer);
                    }
                    else if (playableCards[row, col] == -1)
                    {
                        //Display the card as a king, and the appropriate letter for the pile, and the number of cards in the pile
                        Console.Write("K(" + designatedLetters[row, col] + ":" + cardsInPile[row, col] + ")" + spacer);
                    }
                    else if (playableCards[row, col] == -2)
                    {
                        //Display the card as a queen, and the appropriate letter for the pile, and the number of cards in the pile
                        Console.Write("Q(" + designatedLetters[row, col] + ":" + cardsInPile[row, col] + ")" + spacer);
                    }
                    else if (playableCards[row, col] == -3)
                    {
                        //Display the card as jack, and the appropriate letter for the pile, and the number of cards in the pile
                        Console.Write("J(" + designatedLetters[row, col] + ":" + cardsInPile[row, col] + ")" + spacer);
                    }
                    else
                    {
                        //Display the appropriate card, letter for the pile, and the number of cards in the pile
                        Console.Write(playableCards[row, col] + "(" + designatedLetters[row, col] + ":" + cardsInPile[row, col] + ")" + spacer);
                    }

                    //Change text colour to white
                    Console.ForegroundColor = ConsoleColor.White;
                }
            }
            
            //Add a spacer between the gameboard and text for organizational purposes
            Console.WriteLine(spacer);
        }

        //Pre: The location of the cards based on the row(x-coordinate) and column(y-coordinate)
        //Post: N/A
        //Description: Places the card from the top of the deck to the corresponding 
        //card pile, for when the user chooses two cards that add up to eleven
        private static void CardReplacement(int locX, int locY)
        {
            //Place the first card in the deck on top of the card pile
            playableCards[locX, locY] = cardDeck[0];
            cardDeck.RemoveAt(0);

            //Increase the number of cards in the pile
            cardsInPile[locX, locY]++;
            
        }

        //Pre: A letter which represents the pile selected
        //Post: the location of the pile
        //Description: Determines the location(row and column) based on which letter is used 
        private static Point FindPile(char letter)
        { 
            //Set the pile character to the chosen letter
            char pile = letter;

            //Set the number of the pile to the difference in value between the pile and the letter "a"
            int pileNum = pile - 'a';

            //Determine the row and column of the gameboard
            row = pileNum / 6;
            col = pileNum % 6;

            //Set the location of the pile to the appropriate row and column
            Point pileLoc = new Point(col, row);

            //Pile location
            return pileLoc;
        }

        //Pre: N/A
        //Post: N/A
        //Description: Determines whether the user has lost by checking whether there are
        //any pairs of cards which add up 11, and if there are any face cards in a pile with only one card
        //Disproves that the user lost
        private static void CheckLoss()
        {
            //User has currently lost
            checkLoss = true;

            //For every number between 0 and the number fo rows in the gameboard
            for (int row = 0; row < playableCards.GetLength(0); row++)
            {
                //For every number between 0 and the number of columns in the gameboard
                for (int col = 0; col < playableCards.GetLength(1); col++)
                {
                    //When the card is a face card 
                    if (playableCards[row, col] < FACE_CARD)
                    {
                        //When there is one card in the pile
                        if (cardsInPile[row, col] == 1)
                        {
                            //User has not lost
                            checkLoss = false;
                        }
                    }
                    else
                    {
                        //For every number between 0 and the number of rows in the gameboard
                        for (int row2 = 0; row2 < playableCards.GetLength(0); row2++)
                        {
                            //For every number between 0 and the number of columns in the gameboard
                            for (int col2 = 0; col2 < playableCards.GetLength(1); col2++)
                            {
                                //Check to make sure that the game is comparing two different cards
                                if (row != row2 && col != col2)
                                {
                                    //When the sum of the two cards is 11
                                    if (playableCards[row, col] + playableCards[row2, col2] == 11)
                                    {
                                        //User has not lost
                                        checkLoss = false;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            //When user has lost
            if(checkLoss == true)
            {
                //User has lost and is not currently playing
                isGamePlaying = false;
            }

        }

        //Pre: N/A
        //Post: N/A
        //Description: Determines whether the user has won by checking
        //whether every card on the game board is a face card
        private static void CheckWin()
        {
            //Set counter to 0
            int counter = 0;

            //For every numbeer between 0 and the number of rows in the game board
            for (int row = 0; row < playableCards.GetLength(0); row++)
            {
                //For every number between 0 and the number of columns in the game board
                for (int col = 0; col < playableCards.GetLength(1); col++)
                {
                    //When the card on the game board is a face card
                    if(playableCards[row,col] < FACE_CARD)
                    {
                        //Increase counter
                        counter++;
                    }
                }
            }
            
            //Whent the counter is at twelve representing every card on the game board
            if(counter == 12)
            {
                //The user has won and the user is no longer playing
                checkWin = true;
                isGamePlaying = false;
            }            
        }
    }
}