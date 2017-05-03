using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
namespace CardGame
{
    class Player
    {
        public List<Card> cards { get; set; }
        public bool BUST { get; set; }
    };
    class Computer
    {
        public List<Card> cards { get; set; }
    };

    public enum Suit { Hearts = 1, Diamonds = 2, Spades = 3, Clubs = 4 };

    public enum CardVal { Ace = 1, Two = 2, Three = 3, 
                                            Four = 4, Five = 5, Six = 6, 
                                            Seven = 7, Eight = 8, Nine = 9, 
                                            Ten = 10, Jack = 10, Queen = 10, 
                                            King = 10 };

    public class Card
    {
        public Suit suit { get; set; }
        public CardVal cardVal { get; set; }

    };
    public class Deck
    {
        public List<Card> cards = new List<Card>();
    };


    class BlackJack
    {

        private static Random rng = new Random();
        public static bool Playing { get; set; }
        static void Shuffle(List<Card> list)
        {
            int n = list.Count;
            while ( n > 1 )
            {
                n--;
                int k = rng.Next(n + 1);
                Card value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        static Deck GetDeck(Deck d)
        {
            Card c = new Card();
            foreach ( Suit s in Enum.GetValues(typeof(Suit)) )
            {
                foreach ( CardVal cV in Enum.GetValues(typeof(CardVal)) )
                {
                    c = new Card();
                    c.suit = s;
                    c.cardVal = cV;
                    if ( !d.cards.Contains(c) )
                        d.cards.Add(c);
                }
            }
            Shuffle(d.cards);


            return d;
        }

        static void Deal(Deck d, Player p, Computer c, int cardsToDeal)
        {
            bool playersGo = true;
            int start = d.cards.Count - 1;
            int fin = start - cardsToDeal;
            while ( start > fin )
            {
                switch ( playersGo )
                {
                    case false:
                        c.cards.Add(d.cards[start]);
                        break;
                    case true:
                        p.cards.Add(d.cards[start]);
                        break;
                }
                d.cards.RemoveAt(start);
                playersGo = !playersGo;
                start--;
            }
        }
        static void ShowPlayerHand(Player p)
        {
            Console.WriteLine("\n\nYour hand: ");
            for ( int i = 0; i < p.cards.Count; i++ )
            {
                Console.WriteLine("Card Numer " + i.ToString() + ": " + p.cards[i].cardVal + " of " + p.cards[i].suit);
            }
        }
        static void ShowComputerHand(Computer c)
        {
            Console.WriteLine("\n\nComputers hand: ");
            for ( int i = 0; i < c.cards.Count; i++ )
            {
                Console.WriteLine("Card Numer " + i.ToString() + ": " + c.cards[i].cardVal + " of " + c.cards[i].suit);
            }
        }
        static int GetHandSum(Player p, Computer c, bool isPlayer)
        {

            int currentVal = 0;
            if ( isPlayer )
            {
                for ( int i = 0; i < p.cards.Count; i++ )
                {
                    currentVal += (int)p.cards[i].cardVal;
                }
            }
            else
            {
                for ( int i = 0; i < c.cards.Count; i++ )
                {
                    currentVal += (int)c.cards[i].cardVal;
                }
            }

            return currentVal;
        }
        static void Hit(Deck d, Player p, Computer c)
        {
            int currentVal = GetHandSum(p, c, true);
            if ( currentVal <= 21 )
            {
                int from = d.cards.Count - 1;
                p.cards.Add(d.cards[from]);
                d.cards.RemoveAt(from);
            }
            else
            {
                Console.WriteLine("-----------------Unfortunately you are bust.-----------------");
                p.BUST = true;
                Stick(p, c);
            }

        }
        static void Stick(Player p, Computer c)
        {
            ShowPlayerHand(p);
            ShowComputerHand(c);
            int playerVal = GetHandSum(p, c, true);

            int compVal = GetHandSum(p, c, false);


            bool win = playerVal > compVal  && !p.BUST ? true : false;

            if ( win )
            {
                Console.WriteLine("-->Woohoo, you win!");
            }
            else
            {

                Console.WriteLine("-->Unlucky, you lost this one.");

            }
            Playing = false;
        }
        static void PlayGame(Deck d, Player p, Computer c)
        {
            int choice = 0;
            choice = Convert.ToInt32(Console.ReadLine());

            switch ( choice )
            {
                case 1:
                    Hit(d, p, c);
                    ShowPlayerHand(p);
                    int playerNew = GetHandSum(p, c, true);
                    if(playerNew > 21)
                    {
                        Console.WriteLine("-----------------Unfortunately you are bust.-----------------");
                        p.BUST = true;
                        Stick(p, c);
                    }
                    break;
                case 2:
                    Stick(p, c);
                    break;
            }

        }

        static void Main(string[] args)
        {
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT UserName FROM Win32_ComputerSystem");
            ManagementObjectCollection collection = searcher.Get();
            string username = (string)collection.Cast<ManagementBaseObject>().First()["UserName"];
            username = username.Substring(username.IndexOf("\\") + 1);
            Console.WriteLine("Hello " + username + ". Welcome to Card Game, a simple poker game created using C#.");
            Console.WriteLine("The aim of the game is to get 21!");
            Console.WriteLine("Let me shuffle the deck.");
            Deck currentDeck = new Deck();
            Player p = new Player();
            p.cards = new List<Card>();
            Computer c = new Computer();
            c.cards = new List<Card>();
            currentDeck = GetDeck(currentDeck);
            Console.WriteLine("Okay, the deck is shuffled.");
            Console.WriteLine("Dealing everyone in.");
            Deal(currentDeck, p, c, 4);
            Console.WriteLine("\n\n\nNow we're ready.");
            ShowPlayerHand(p);

            Playing = true;
            while ( Playing )
            {
                Console.WriteLine("\n\nWhat do you want to do..?");
                Console.WriteLine("\n\n\t(1): Hit.\n\t(2): Stick.");
                PlayGame(currentDeck, p, c);
            }



            Console.ReadLine();
        }
    }
}
