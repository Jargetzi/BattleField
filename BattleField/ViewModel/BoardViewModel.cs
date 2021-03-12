using BattleField.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BattleField.Common;
using System.Windows;
using BattleField.View;
using System.Windows.Data;
using System.Threading;

namespace BattleField.ViewModel
{
    class BoardViewModel:ViewModelBase
    {
        #region Fields
        private List<Card> _OpponentDeck = new List<Card>();
        private List<Card> _Deck = new List<Card>();
        private List<Card> _Casualties = new List<Card>();
        private static Random rng = new Random();
        private Card[] _BattleField = new Card[6];
        private Card[] _OpponentBattleField = new Card[6];
        private bool _isPlayerTurn = true;
        private string _log;
        private bool _OrdersVisible = false;
        private int _order = 0;
        private Card _reinforcement;
        private Card _opponentReinforcement;
        private List<Card> Pot = new List<Card>();
        private int PotColumn;
        private bool _gameOver = false;
        public MainWindowViewModel mwvm;

        #endregion

        #region Properties
        public List<Card> OpponentDeck
        {
            get { return _OpponentDeck; }
            set
            {
                if(_OpponentDeck != value)
                {
                    _OpponentDeck = value;
                    OnPropertyChanged();
                    OnPropertyChanged("OpponentDeckCount");
                }
            }
        }

        public List<Card> Deck
        {
            get { return _Deck; }
            set
            {
                if (_Deck != value)
                {
                    _Deck = value;
                    OnPropertyChanged();
                    OnPropertyChanged("DeckCount");
                }
            }
        }

        public List<Card> Casualties
        {
            get { return _Casualties; }
            set
            {
                if (_Casualties != value)
                {
                    _Casualties = value;
                    OnPropertyChanged();
                    OnPropertyChanged("CasualtyCard");
                }
            }
        }

        public Card[] BattlefieldLayout
        {
            get { return _BattleField; }
            set
            {
                if (_BattleField != value)
                {
                    _BattleField = value;
                    OnPropertyChanged();
                }
            }
        }

        public Card[] OpponentBattlefield
        {
            get { return _OpponentBattleField; }
            set
            {
                if (_OpponentBattleField != value)
                {
                    _OpponentBattleField = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsPlayerTurn
        {
            get { return _isPlayerTurn; }
            set
            {
                if(_isPlayerTurn != value)
                {
                    _isPlayerTurn = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Log
        {
            get { return _log; }
            set
            {
                if(_log != value)
                {
                    _log = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool OrdersVisible
        {
            get { return _OrdersVisible; }
            set
            {
                if(_OrdersVisible != value)
                {
                    _OrdersVisible = value;
                    OnPropertyChanged();
                }
            }
        }

        public int Order
        {
            get { return _order; }
            set
            {
                if (_order != value)
                {
                    _order = value;
                    OnPropertyChanged();
                    //update animation
                }
            }
        }

        public string CasualtyCard
        {
            get
            {
                if (Casualties.Count > 0)
                    return Casualties[Casualties.Count -1].Source;
                else
                    return "";
            }
        }

        public Card Reinforcement
        {
            get { return _reinforcement; }
            set
            {
                if(_reinforcement != value)
                {
                    _reinforcement = value;
                    OnPropertyChanged();
                }
            }
        }

        public Card OpponentReinforcement
        {
            get { return _opponentReinforcement; }
            set
            {
                if (_opponentReinforcement != value)
                {
                    _opponentReinforcement = value;
                    OnPropertyChanged();
                }
            }
        }

        public string DeckCount
        {
            get
            {
                if (Deck.Count > 0)
                    return Deck.Count.ToString();
                else
                    return "";
            }
        }

        public string OpponentDeckCount
        {
            get
            {
                if (OpponentDeck.Count > 0)
                    return OpponentDeck.Count.ToString();
                else
                    return "";
            }
        }

        public bool GameOver
        {
            get { return _gameOver; }
            set
            {
                if(_gameOver != value)
                {
                    _gameOver = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion


        #region Commands
        public RelayCommand ContinueCommand { get; set; }

        public RelayCommand OrderAttackCommand { get; set; }
        public RelayCommand OrderDefendCommand { get; set; }
        #endregion
        public BoardViewModel()
        {
            ContinueCommand = new RelayCommand(Continue);
            OrderAttackCommand = new RelayCommand(OrderAttack);
            OrderDefendCommand = new RelayCommand(OrderDefend);
            DealCards();
        }

        public void StartGame()
        {
            GameOver = false;
            Log = "";
            DealCards();
            RunGame();
        }

        public void DealCards()
        {
            List<Card> AllCards = new List<Card>();

            CardSuit[] Suits = (CardSuit[])Enum.GetValues(typeof(CardSuit));
            CardValue[] Vals = (CardValue[])Enum.GetValues(typeof(CardValue));

            Deck.Clear();
            OpponentDeck.Clear();
            Casualties.Clear();

            //create cards
            foreach (var suit in Suits)
            {
                foreach(var v in Vals)
                {
                    Card c = new Card();
                    c.Suit = suit;
                    c.Value = v;
                    c.FaceUP = false;

                    if(c.Value != CardValue.Joker)
                        AllCards.Add(c);
                }
            }

            //Add 2 Jokers
            Card c3 = new Card();
            c3.Value = CardValue.Joker;
            c3.FaceUP = false;

            Card c2 = new Card();
            c2.Value = CardValue.Joker;
            c2.FaceUP = false;

            AllCards.Add(c3);
            AllCards.Add(c2);

            //Shuffle the deck
            int n = AllCards.Count;
            while(n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                Card c = AllCards[k];
                AllCards[k] = AllCards[n];
                AllCards[n] = c;
            }

            //Deal
            int i = 0;
            foreach(Card c in AllCards)
            {
                if (i % 2 == 0)
                    Deck.Add(c);
                else
                    OpponentDeck.Add(c);

                i++;
            }

            Log += "Cards have been dealt.\n";
        }

        public async void RunGame()
        {
            int Round = 1;
            while (Deck.Count > 0 && OpponentDeck.Count > 0)
            {
                Log += $"Round {Round}\n";
                SetBattleField();
                await Task.Delay(3000);
                await PayCasualty();

                await Task.Delay(3000);
                await Battle();
                Round++;
            }
            GameOver = true;
            string results = Deck.Count > 0 ? "won" : "lost";
            MessageBox.Show($"Game Over! You {results}");
            mwvm.MenuVisible = GameOver;
            mwvm.BoardVisible = !GameOver;
        }

        private async Task Battle()
        {
            for (int i = 0; i < 3; i++)
            {
                bool PlayerAttack;
                bool OpponentAttack;

                List<Card> BattleCards = new List<Card>();
                List<Card> OpponentBattleCards = new List<Card>();
                //check if the battlefield still has cards
                if (BattlefieldLayout[i] != null)
                    BattleCards.Add(BattlefieldLayout[i]);
                if (BattlefieldLayout[i + 3] != null)
                    BattleCards.Add(BattlefieldLayout[i + 3]);
                if (OpponentBattlefield[i] != null)
                    OpponentBattleCards.Add(OpponentBattlefield[i]);
                if (OpponentBattlefield[i + 3] != null)
                    OpponentBattleCards.Add(OpponentBattlefield[i + 3]);

                if((BattleCards.Count == 0 || OpponentBattleCards.Count == 0) && Deck.Count > 0)
                {
                    ReturnBattlefields(BattleCards,OpponentBattleCards);
                    continue;
                }
                else if((BattleCards.Count == 0 || OpponentBattleCards.Count == 0) && Deck.Count == 0)
                {
                    break;
                }

                if (BattlefieldLayout[i+3] != null)
                    BattlefieldLayout[i + 3].FaceUP = true;

                if (IsPlayerTurn)
                {
                    PlayerAttack = await PlayerOrders(i);
                    OpponentAttack = OpponentOrders(i);
                }
                else
                {
                    OpponentAttack = OpponentOrders(i);
                    PlayerAttack = await PlayerOrders(i);
                }
                await Task.Delay(1000);
                
                await DoBattle(PlayerAttack, OpponentAttack, i);

                IsPlayerTurn = !IsPlayerTurn;
            }
        }

        private async Task DoBattle(bool playerAttack, bool opponentAttack, int column)
        {
            List<Card> BattleCards = new List<Card>();
            List<Card> OpponentBattleCards = new List<Card>();
            int aceCount = 0;

            string order = playerAttack ? "attack" : "defend";
            Log += $"You order to {order}\n";
            order = opponentAttack ? "attack" : "defend";
            Log += $"Opponent orders to {order}\n";
            if (!playerAttack && !opponentAttack)
            {
                //no battle, cards go into pot
                Log += "Both defend so current cards go into spoils\n";
                //Pot.Clear();

                await FlipCards(column);
                Pot.Add(BattlefieldLayout[column]);
                Pot.Add(BattlefieldLayout[column + 3]);
                Pot.Add(OpponentBattlefield[column]);
                Pot.Add(OpponentBattlefield[column + 3]);

                PotColumn = column;
            }
            else
            {
                await FlipCards(column);

                int PlayerTotal = 0;
                int OpponentTotal = 0;

                //manage reinforcements and jokers with the battlecards
                if(BattlefieldLayout[column] != null)
                    BattleCards.Add(BattlefieldLayout[column]);
                if(BattlefieldLayout[column + 3] != null)
                    BattleCards.Add(BattlefieldLayout[column + 3]);
                if(OpponentBattlefield[column] != null)
                    OpponentBattleCards.Add(OpponentBattlefield[column]);
                if(OpponentBattlefield[column+ 3] != null)
                    OpponentBattleCards.Add(OpponentBattlefield[column+3]);

                EvaluateJoker(BattleCards,OpponentBattleCards,out PlayerTotal,out OpponentTotal,out aceCount);
                Log += $"{PlayerTotal} vs {OpponentTotal}\n";
                if (PlayerTotal > OpponentTotal)
                {
                    //player win
                    Log += "You won the Battle!\n";

                    TakeSpoils(true, column, playerAttack, opponentAttack, BattleCards, OpponentBattleCards, aceCount);
                }
                else if (OpponentTotal > PlayerTotal)
                {
                    //Opponent win
                    Log += "Opponent won the Battle!\n";

                    TakeSpoils(false, column, playerAttack, opponentAttack, BattleCards, OpponentBattleCards, aceCount);
                }
                else
                {
                    //Ties will result in reinforcement
                    while (PlayerTotal == OpponentTotal)
                    {
                        Log += "Tie\n";
                        if (Deck.Count > 0)
                        {
                            Reinforcement = Deck[0];
                            Deck.RemoveAt(0);
                            Reinforcement.FaceUP = true;
                            BattleCards.Add(Reinforcement);
                            PlayerTotal += Reinforcement.GetValue();
                            Log += $"You reinforced with {Reinforcement.GetValueStringLog()}\n";
                            await Task.Delay(1000);
                        }
                        if(OpponentDeck.Count > 0)
                        {
                            OpponentReinforcement = OpponentDeck[0];
                            OpponentDeck.RemoveAt(0);
                            OpponentReinforcement.FaceUP = true;
                            OpponentBattleCards.Add(OpponentReinforcement);
                            OpponentTotal += OpponentReinforcement.GetValue();
                            Log += $"Opponent reinforced with {OpponentReinforcement.GetValueStringLog()}\n";
                            await Task.Delay(1000);
                        }

                        EvaluateJoker(BattleCards, OpponentBattleCards, out PlayerTotal, out OpponentTotal, out aceCount);
                    }
                    if (PlayerTotal > OpponentTotal)
                    {
                        Log += "You won with the reinforcement!\n";
                        TakeSpoils(true, column, playerAttack, opponentAttack, BattleCards, OpponentBattleCards, aceCount);
                    }
                    else
                    {
                        Log += "Opponent won with the reinforcement!\n";
                        TakeSpoils(false, column, playerAttack, opponentAttack, BattleCards, OpponentBattleCards, aceCount);
                    }
                    OpponentReinforcement = null;
                    Reinforcement = null;
                }
            }
        }

        private void EvaluateJoker(List<Card> BattleCards, List<Card> OpponentBattleCards,out int PlayerTotal, out int OpponentTotal, out int aceCount )
        {
            aceCount = 0;
            int PlayerJoker = 0;
            int OpponentJoker = 0;
            int PlayerJack = 0;
            int OpponentJack = 0;
            int PlayerQueen = 0;
            int OpponentQueen = 0;
            int PlayerKing = 0;
            int OpponentKing = 0;

            foreach(Card c in BattleCards)
            {
                if (c == null)
                    continue;

                if (c.Value == CardValue.Ace)
                    aceCount += 1;
                if (c.Value == CardValue.Joker)
                    PlayerJoker += 1;
                if (c.Value == CardValue.Jack)
                    PlayerJack += 1;
                if (c.Value == CardValue.Queen)
                    PlayerQueen += 1;
                if (c.Value == CardValue.King)
                    PlayerKing += 1;
            }
            foreach (Card c in OpponentBattleCards)
            {
                if (c == null)
                    continue;

                if (c.Value == CardValue.Ace)
                    aceCount += 1;
                if (c.Value == CardValue.Joker)
                    OpponentJoker += 1;
                if (c.Value == CardValue.Jack)
                    OpponentJack += 1;
                if (c.Value == CardValue.Queen)
                    OpponentQueen += 1;
                if (c.Value == CardValue.King)
                    OpponentKing += 1;
            }

            //gets base totals
            
            PlayerTotal = BattleCards.Sum(item => item.GetValue());
            //doubles get an extra point
            if (BattleCards.Count != 1 && BattleCards[0].GetValueString() == BattleCards[1].GetValueString())
                PlayerTotal += 1;

            OpponentTotal = OpponentBattleCards.Sum(item => item.GetValue());
            //opponent logic for doubles
            if (OpponentBattleCards.Count != 1 && OpponentBattleCards[0].GetValueString() == OpponentBattleCards[1].GetValueString())
                OpponentTotal += 1;

            if (PlayerJoker > 0 || OpponentJoker > 0)
            {
                //players face cards
                if (PlayerKing > 0)
                    PlayerTotal += 2 * PlayerKing;
                if (PlayerQueen > 0)
                    OpponentTotal -= 2 * PlayerQueen;
                if (PlayerJack > 0)
                    PlayerTotal -= 5 * PlayerJack;

                //opponent face cards
                if (OpponentKing > 0)
                    OpponentTotal += 2 * OpponentKing;
                if (OpponentQueen > 0)
                    PlayerTotal -= 2 * OpponentQueen;
                if (OpponentJack > 0)
                    OpponentTotal -= 5 * OpponentJack;
            }
        }

        private void TakeSpoils(bool playerWon,int column, bool playerAttack, bool opponentAttack,List<Card> BattleCards, List<Card> OpponentBattleCards, int aceCount)
        {
            if(playerWon)
            {
                foreach(Card c in BattleCards)
                {
                    c.FaceUP = false;
                    Deck.Add(c);
                }
                foreach(Card c in OpponentBattleCards)
                {
                    c.FaceUP = false;
                    Deck.Add(c);
                }
                BattleCards.Clear();
                OpponentBattleCards.Clear();
                if(Pot.Count > 0)
                {
                    foreach(Card c in Pot)
                    {
                        c.FaceUP = false;
                        Deck.Add(c);
                    }
                    if (BattlefieldLayout.Contains(Pot[0]))
                    {
                        BattlefieldLayout[PotColumn] = null;
                        BattlefieldLayout[PotColumn + 3] = null;
                        OpponentBattlefield[PotColumn] = null;
                        OpponentBattlefield[PotColumn + 3] = null;
                    }
                    Pot.Clear();
                }

                BattlefieldLayout[column] = null;
                BattlefieldLayout[column + 3] = null;
                OpponentBattlefield[column] = null;
                OpponentBattlefield[column + 3] = null;

                Reinforcement = null;
                OpponentReinforcement = null;

                OnPropertyChanged("BattleFieldLayout");
                OnPropertyChanged("OpponentBattlefield");
                int spoils = 0;
                spoils += (playerAttack ? 1 : 0) + (opponentAttack ? 1 : 0);

                if(aceCount > 0)
                {
                    Log += $"Additional {aceCount} spoils for Aces\n";
                    spoils += aceCount;
                }

                if (playerAttack)
                {
                    for (int i = 0; i < spoils; i++)
                    {
                        if (OpponentDeck.Count > 0)
                        {
                            Deck.Add(OpponentDeck[0]);
                            Log += $"Oppenent lost {OpponentDeck[0].GetValueStringLog()} as a spoil\n";
                            OpponentDeck.RemoveAt(0);
                        }
                    }
                }
            }
            else
            {
                foreach (Card c in BattleCards)
                {
                    c.FaceUP = false;
                    OpponentDeck.Add(c);
                }
                foreach (Card c in OpponentBattleCards)
                {
                    c.FaceUP = false;
                    OpponentDeck.Add(c);
                }
                BattleCards.Clear();
                OpponentBattleCards.Clear();
                if (Pot.Count > 0)
                {
                    foreach (Card c in Pot)
                    {
                        c.FaceUP = false;
                        OpponentDeck.Add(c);
                    }
                    if (BattlefieldLayout.Contains(Pot[0]))
                    {
                        BattlefieldLayout[PotColumn] = null;
                        BattlefieldLayout[PotColumn + 3] = null;
                        OpponentBattlefield[PotColumn] = null;
                        OpponentBattlefield[PotColumn + 3] = null;
                    }
                    Pot.Clear();
                }
                BattlefieldLayout[column] = null;
                BattlefieldLayout[column + 3] = null;
                OpponentBattlefield[column] = null;
                OpponentBattlefield[column + 3] = null;
                Reinforcement = null;
                OpponentReinforcement = null;

                //OnPropertyChanged("Reinforcement");
                //OnPropertyChanged("OpponentReinforcement");

                OnPropertyChanged("BattlefieldLayout");
                OnPropertyChanged("OpponentBattlefield");
                int spoils = 0;
                spoils += (playerAttack ? 1 : 0) + (opponentAttack ? 1 : 0);

                if (aceCount > 0)
                {
                    Log += $"Additional {aceCount} spoils for Aces\n";
                    spoils += aceCount;
                }

                if (opponentAttack)
                {
                    for (int i = 0; i < spoils; i++)
                    {
                        if (Deck.Count > 0)
                        {
                            OpponentDeck.Add(Deck[0]);
                            Log += $"You lost {Deck[0].GetValueStringLog()} as a spoil\n";
                            Deck.RemoveAt(0);
                        }
                    }
                }
            }
            OnPropertyChanged("DeckCount");
            OnPropertyChanged("OpponentDeckCount");
        }

        public async Task FlipCards(int column)
        {
            if (BattlefieldLayout[column] != null)
            {
                BattlefieldLayout[column].FaceUP = true;
                await Task.Delay(1000);
            }
            if (BattlefieldLayout[column + 3] != null)
            {
                BattlefieldLayout[column + 3].FaceUP = true;
                if (column == 0)
                {
                    
                }
                await Task.Delay(1000);
            }
            if (OpponentBattlefield[column] != null)
            {
                OpponentBattlefield[column].FaceUP = true;
                await Task.Delay(1000);
            }
            if (OpponentBattlefield[column + 3] != null)
            {
                OpponentBattlefield[column + 3].FaceUP = true;
                await Task.Delay(1000);
            }
        }

        private bool OpponentOrders(int i)
        {
            if (OpponentDeck.Count ==0 || OpponentBattlefield[i + 3] == null || OpponentBattlefield[i + 3].GetValue() >= 5)
            {
                return true;
            }
            else
                return false;
        }

        private async Task<bool> PlayerOrders(int i)
        {
            //return true;
            OrdersVisible = true;
            Order = 0;
            while (OrdersVisible)
            {
                await Task.Delay(1000);
            }

            //will need to add a way to get orders
            if (Order == 1)
            {
                return true;
            }
            else
                return false;
        }

        private async Task PayCasualty()
        {
            if (Deck.Count > 0)
            {
                Deck[0].FaceUP = true;
                Casualties.Add(Deck[0]);
                
                Log += $"You lost {Deck[0].GetValueStringLog()} as a casualty\n";
                Deck.RemoveAt(0);
                OnPropertyChanged("CasualtyCard");
            }
            await Task.Delay(2000);
            if (OpponentDeck.Count > 0)
            {
                OpponentDeck[0].FaceUP = true;
                Casualties.Add(OpponentDeck[0]);

                Log += $"Opponent lost {OpponentDeck[0].GetValueStringLog()} as a casualty\n";
                OpponentDeck.RemoveAt(0);
                OnPropertyChanged("CasualtyCard");
            }
            OnPropertyChanged("DeckCount");
            OnPropertyChanged("OpponentDeckCount");
        }

        private void SetBattleField()
        {
            //start laying out normal for player
            if(Deck.Count >= 6)
            {
                for(int i = 0;i<6;i++)
                {
                    BattlefieldLayout[i] = Deck[0];
                    Deck.RemoveAt(0);
                }
                OnPropertyChanged("BattlefieldLayout");
            }
            else
            {
                //near end of game
                for (int i = 0; i < 3; i++)
                {
                    if (Deck.Count > 0)
                    {
                        BattlefieldLayout[i+3] = Deck[0];
                        Deck.RemoveAt(0);
                    }
                    else
                        BattlefieldLayout[i+3] = null;

                    if (Deck.Count > 0)
                    {
                        BattlefieldLayout[i] = Deck[0];
                        Deck.RemoveAt(0);
                    }
                    else
                        BattlefieldLayout[i] = null;
                }
                OnPropertyChanged("BattlefieldLayout");
            }

            //Layout battlefield of oppenent
            if (OpponentDeck.Count >= 6)
            {
                for (int i = 0; i < 6; i++)
                {
                    OpponentBattlefield[i] = OpponentDeck[0];
                    OpponentDeck.RemoveAt(0);
                }
                OnPropertyChanged("OpponentBattlefield");
            }
            else
            {
                //near end of game
                for(int i = 0; i< 3;i++)
                {
                    if (OpponentDeck.Count > 0)
                    {
                        OpponentBattlefield[i+3] = OpponentDeck[0];
                        OpponentDeck.RemoveAt(0);
                    }
                    else
                        OpponentBattlefield[i+3] = null;

                    if(OpponentDeck.Count > 0)
                    {
                        OpponentBattlefield[i] = OpponentDeck[0];
                        OpponentDeck.RemoveAt(0);
                    }
                    else
                        OpponentBattlefield[i] = null;
                }
                OnPropertyChanged("OpponentBattlefield");
            }
            Log += "Battlefields have been laid down.\n";
            OnPropertyChanged("DeckCount");
            OnPropertyChanged("OpponentDeckCount");
        }

        public void Continue()
        {
            if(Order == 0)
            {
                MessageBox.Show("need to declare an Order");
            }
            else
                OrdersVisible = false;
        }

        public void OrderAttack()
        {
            Order = 1;
            
            System.Media.SoundPlayer player = new System.Media.SoundPlayer(@"Sounds\Swords2.wav");
            player.Play();
            player.Dispose();
        }

        public void OrderDefend()
        {
            Order = -1;

            System.Media.SoundPlayer player2 = new System.Media.SoundPlayer(@"Sounds\DropSword.wav");
            player2.Play();
            player2.Dispose();
        }

        public void ReturnBattlefields(List<Card> BattleCards, List<Card> OpponentBattleCards)
        {
            foreach (Card c in BattleCards)
            {
                c.FaceUP = false;
                Deck.Add(c);
            }
            foreach (Card c in OpponentBattleCards)
            {
                c.FaceUP = false;
                OpponentDeck.Add(c);
            }
        }
    }
}
