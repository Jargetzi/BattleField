using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BattleField.ViewModel
{
    class MainWindowViewModel : ViewModelBase
    {
        #region Fields
        private bool _BoardVisible = false;
        private bool _MenuVisible = true;
        private bool _RulesVisible = false;
        private BoardViewModel _BoardViewModelMain = new BoardViewModel();
        #endregion
        #region Properties
        public Visibility BoardVisibility
        {
            get
            {
                if (BoardVisible)
                    return Visibility.Visible;
                else
                    return Visibility.Hidden;
            }
        }

        public bool MenuVisible
        {
            get { return _MenuVisible; }
            set
            {
                if(_MenuVisible != value)
                {
                    _MenuVisible = value;
                    OnPropertyChanged();
                    OnPropertyChanged("Menu");
                }
            }
        }

        public Visibility Menu
        {
            get
            {
                if (MenuVisible)
                    return Visibility.Visible;
                else
                    return Visibility.Hidden;
            }

        }

        public bool BoardVisible
        {
            get { return _BoardVisible; }
            set
            {
                if(_BoardVisible!=value)
                {
                    _BoardVisible = value;
                    OnPropertyChanged();
                    OnPropertyChanged("Menu");
                    OnPropertyChanged("BoardVisibility");
                }
            }
        }
        public BoardViewModel BoardViewModelMain
        {
            get { return _BoardViewModelMain; }
            set
            {
                if (_BoardViewModelMain != value)
                {
                    _BoardViewModelMain = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool RulesVisible
        {
            get { return _RulesVisible; }
            set
            {
                if (_RulesVisible != value)
                {
                    _RulesVisible = value;
                    OnPropertyChanged();
                }
            }
        }

        public Visibility VisibilityOfRules
        {
            get
            {
                if (RulesVisible)
                    return Visibility.Visible;
                else
                    return Visibility.Hidden;
            }

        }
        #endregion

        #region Commands
        public RelayCommand NewGameCommand { get; set; }
        public RelayCommand RulesCommand { get; set; }
        #endregion

        #region Constructor
        public MainWindowViewModel()
        {
            NewGameCommand = new RelayCommand(NewGame);
            RulesCommand = new RelayCommand(Rules);
        }
        #endregion

        #region Methods
        public void NewGame()
        {
            BoardVisible = !BoardVisible;
            MenuVisible = false;

            BoardViewModelMain.StartGame();
            BoardViewModelMain.mwvm = this;
            MenuVisible = BoardViewModelMain.GameOver;
            
        }

        public void Rules()
        {
            MenuVisible = false;
        }
        #endregion
    }
}
