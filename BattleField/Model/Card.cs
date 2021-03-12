using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BattleField.Common;

namespace BattleField.Model
{
    class Card: ViewModelBase
    {
        private CardSuit _Suit;

        private CardValue _Value;

        public bool _FaceUP = false;

        private string _source;

        public CardSuit Suit
        {
            get { return _Suit; }
            set
            {
                if(_Suit != value)
                {
                    _Suit = value;
                    OnPropertyChanged();
                    OnPropertyChanged("Source");
                }
            }
        }

        public CardValue Value
        {
            get { return _Value; }
            set
            {
                if (_Value != value)
                {
                    _Value = value;
                    OnPropertyChanged();
                    OnPropertyChanged("Source");
                }
            }
        }

        public bool FaceUP
        {
            get { return _FaceUP; }
            set
            {
                if(_FaceUP != value)
                {
                    _FaceUP = value;
                    OnPropertyChanged();
                    OnPropertyChanged("Source");
                }
            }
        }

        public int GetValue()
        {
            switch (Value)
            {
                case CardValue.Ace:
                    return 11;
                case CardValue.Two:
                    return 2;
                case CardValue.Three:
                    return 3;
                case CardValue.Four:
                    return 4;
                case CardValue.Five:
                    return 5;
                case CardValue.Six:
                    return 6;
                case CardValue.Seven:
                    return 7;
                case CardValue.Eight:
                    return 8;
                case CardValue.Nine:
                    return 9;
                case CardValue.Ten:
                    return 10;
                case CardValue.Jack:
                    return 10;
                case CardValue.Queen:
                    return 10;
                case CardValue.King:
                    return 10;
                case CardValue.Joker:
                    return 10;
                default:
                    return 0;
            }
        }

        public string GetValueString()
        {
            switch (Value)
            {
                case CardValue.Ace:
                    return "ace";
                case CardValue.Two:
                    return "2";
                case CardValue.Three:
                    return "3";
                case CardValue.Four:
                    return "4";
                case CardValue.Five:
                    return "5";
                case CardValue.Six:
                    return "6";
                case CardValue.Seven:
                    return "7";
                case CardValue.Eight:
                    return "8";
                case CardValue.Nine:
                    return "9";
                case CardValue.Ten:
                    return "10";
                case CardValue.Jack:
                    return "jack";
                case CardValue.Queen:
                    return "queen";
                case CardValue.King:
                    return "king";
                case CardValue.Joker:
                    return "joker";
                default:
                    return "";
            }
        }

        public string GetValueStringLog()
        {
            switch (Value)
            {
                case CardValue.Ace:
                    return $"ace of {Suit.ToString()}";
                case CardValue.Two:
                    return $"2 of {Suit.ToString()}";
                case CardValue.Three:
                    return $"3 of {Suit.ToString()}";
                case CardValue.Four:
                    return $"4 of {Suit.ToString()}";
                case CardValue.Five:
                    return $"5 of {Suit.ToString()}";
                case CardValue.Six:
                    return $"6 of {Suit.ToString()}";
                case CardValue.Seven:
                    return $"7 of {Suit.ToString()}";
                case CardValue.Eight:
                    return $"8 of {Suit.ToString()}";
                case CardValue.Nine:
                    return $"9 of {Suit.ToString()}";
                case CardValue.Ten:
                    return $"10 of {Suit.ToString()}";
                case CardValue.Jack:
                    return $"jack of {Suit.ToString()}";
                case CardValue.Queen:
                    return $"queen of {Suit.ToString()}";
                case CardValue.King:
                    return $"king of {Suit.ToString()}";
                case CardValue.Joker:
                    return $"a joker";
                default:
                    return "";
            }
        }

        public string Source
        {
            get {
                if (FaceUP)
                {
                    if(Value != CardValue.Joker)
                        return $"/BattleField;component/Images/{GetValueString().ToLower()}_of_{Suit.ToString().ToLower()}.png";
                    else
                        return $"/BattleField;component/Images/red_joker.png";
                }
                else
                    return $"/BattleField;component/Images/back.png";
            }
        }
    }
}
