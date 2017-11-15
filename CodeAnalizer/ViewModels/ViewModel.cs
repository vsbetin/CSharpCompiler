using CodeAnalizer.Infrastucture;
using CodeAnalizer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CodeAnalizer.ViewModels
{
    class ViewModel : ViewModelBase
    {
        LexicalAnalyzer _analizer;

        public ViewModel()
        {
            _analizer = new LexicalAnalyzer();
            Rows = "1";
            ProgramText = @"prog first
int a, b, c;
{
    read(c);
    a = 2.4E-12;
    for (b = 1; b < 20; 5)
    {
        c = a + b;
    }
    if (c >= 25) 
    {
        c = c + 5;
    }
    write(a, b, c);
}";
            ExecuteAddRow(null);
        }

        private string _rows;

        public string Rows
        {
            get { return _rows; }
            set
            {
                SetProperty(ref _rows, value, "Rows");
            }
        }

        private string _programText;

        public string ProgramText
        {
            get { return _programText; }
            set
            {
                SetProperty(ref _programText, value, "ProgramText");
                Run.RaiseCanExecuteChanged();
            }
        }

        private string _lexemeText;

        public string LexemeText
        {
            get { return _lexemeText; }
            set
            {
                SetProperty(ref _lexemeText, value, "LexemeText");
            }
        }

        private string _constantText;

        public string ConstantText
        {
            get { return _constantText; }
            set
            {
                SetProperty(ref _constantText, value, "ConstantText");
            }
        }

        private string _identifierText;

        public string IdentifierText
        {
            get { return _identifierText; }
            set
            {
                SetProperty(ref _identifierText, value, "IdentifierText");
            }
        }

        RelayCommand _addRow;
        public RelayCommand AddRow
        {
            get
            {
                return _addRow ?? (_addRow = new RelayCommand(ExecuteAddRow));
            }
        }

        private void ExecuteAddRow(object obj)
        {
            int count = ProgramText.Count(ch => ch == '\r');
            StringBuilder builder = new StringBuilder();
            for (int i = 1; i <= count + 1; i++)
            {
                builder.Append(i + Environment.NewLine);
            }
            Rows = builder.ToString();
        }

        RelayCommand _run;
        public RelayCommand Run
        {
            get
            {
                return _run ?? (_run = new RelayCommand(ExecuteRun, CanExecuteRun));
            }
        }

        private bool CanExecuteRun(object obj)
        {
            if (string.IsNullOrWhiteSpace(ProgramText))
                return false;
            return true;
        }

        private void ExecuteRun(object obj)
        {
            var output = _analizer.Run(ProgramText);
            LexemeText = output.lexemeText;
            IdentifierText = output.identifiersText;
            ConstantText = output.constantText;
        }
    }
}
