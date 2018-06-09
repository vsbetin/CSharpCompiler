using CodeAnalizer.Infrastucture;
using CodeAnalizer.Models;
using CodeAnalizer.Views;
using Microsoft.Xaml.Interactivity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace CodeAnalizer.ViewModels
{
    class ViewModel : ViewModelBase
    {
        LexicalAnalyzer _lexicalAnalizer;
        Executor exec;

        public ViewModel()
        {
            _lexicalAnalizer = new LexicalAnalyzer();
            Rows = "1";
            VarValue = "";
            ProgramText = @"Prog test
var int x ,y ,i;
{
    read(y);
    x = 12;
    if (y>0)
    {
        x = y*2-1;
        if (x < 3)
        {
            write(x);
        };
    };
    for i = 0 to x step 4
    {
        write(i);
    };
}";
            ExecuteAddRow(null);
        }

        private bool _cond = true;

        public bool Cond
        {
            get { return _cond; }
            set
            {
                SetProperty(ref _cond, value, "Cond");
            }
        }

        private string _varName;

        public string VarName
        {
            get { return _varName; }
            set
            {
                SetProperty(ref _varName, value, "VarName");
            }
        }

        private string _varValue;

        public string VarValue
        {
            get { return _varValue; }
            set
            {
                SetProperty(ref _varValue, value, "VarValue");
            }
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

        private string _syntaxAnalizerText;

        public string SyntaxAnalizerText
        {
            get { return _syntaxAnalizerText; }
            set
            {
                SetProperty(ref _syntaxAnalizerText, value, "SyntaxAnalizerText");
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

        RelayCommand _writeVar;
        public RelayCommand WriteVar
        {
            get
            {
                return _writeVar ?? (_writeVar = new RelayCommand(RunWriteVar, CanExecuteWriteVar));
            }
        }

        private bool CanExecuteWriteVar(object obj)
        {
            return !Cond;
        }

        private async void RunWriteVar(object obj)
        {
            if (!Cond)
            {
                if (int.TryParse(VarValue, out int value))
                {
                    exec.ReadVarValue = value;
                    Cond = exec.process();
                    if(Cond)
                    {
                        VarName = "";
                        WriteVar.RaiseCanExecuteChanged();
                        var dialog = new MessageDialog(exec.Result, "Result");
                        await dialog.ShowAsync();
                    }
                    else
                    {
                        VarName = exec.ReadVar;
                    }
                }
                else
                {
                    VarName = exec.ReadVar;
                }
                VarValue = "";
            }
        }

        RelayCommand _polizProg;
        public RelayCommand PolizProg
        {
            get
            {
                return _polizProg ?? (_polizProg = new RelayCommand(ExecutePolizProg, CanExecuteRun));
            }
        }

        private void ExecutePolizProg(object obj)
        {
            var output = _lexicalAnalizer.Run(ProgramText);
            LexemeText = "Token\tIndex\tRow\tValue" + Environment.NewLine + Environment.NewLine;
            IdentifierText = "Idn\tIndex\tRow" + Environment.NewLine + Environment.NewLine;
            ConstantText = "Const\tIndex\tRow" + Environment.NewLine + Environment.NewLine;

            var tokStr = output.outputTokens.Select(tok => (tok.Value + "\t" + tok.Index + "\t" + tok.Row + "\t" + tok.GeneralizedValue));
            var idnputStr = output.outputIdentifiers.Select(tok => (tok.Value + "\t" + tok.Index + "\t" + tok.Row));
            var conputStr = output.outoutConstans.Select(tok => (tok.Value + "\t" + tok.Index + "\t" + tok.Row));
            foreach (var el in tokStr)
            {
                LexemeText += el + Environment.NewLine;
            }

            foreach (var el in idnputStr)
            {
                IdentifierText += el + Environment.NewLine;
            }

            foreach (var el in conputStr)
            {
                ConstantText += el + Environment.NewLine;
            }

            var syntaxAnalizer = new ParserAscent();
            SyntaxAnalizerText = DateTimeOffset.Now.ToUnixTimeMilliseconds() + ": " + syntaxAnalizer.process(output.outputTokens);

            var currentAV = ApplicationView.GetForCurrentView();
            var newAV = CoreApplication.CreateNewView();
            
        }

        RelayCommand _poliz;
        public RelayCommand Poliz
        {
            get
            {
                return _poliz ?? (_poliz = new RelayCommand(ExecutePoliz));
            }
        }

        private async void ExecutePoliz(object obj)
        {
            var currentAV = ApplicationView.GetForCurrentView();
            var newAV = CoreApplication.CreateNewView();
            await newAV.Dispatcher.RunAsync(
                            CoreDispatcherPriority.Normal,
                            async () =>
                            {
                                var newWindow = Window.Current;
                                var newAppView = ApplicationView.GetForCurrentView();
                                newAppView.Title = "New window";

                                var frame = new Frame();
                                frame.Navigate(typeof(PolizPage), null);
                                newWindow.Content = frame;
                                newWindow.Activate();

                                await ApplicationViewSwitcher.TryShowAsStandaloneAsync(
                                    newAppView.Id,
                                    ViewSizePreference.UseMinimum,
                                    currentAV.Id,
                                    ViewSizePreference.UseMinimum);
                            });
        }

        private bool CanExecuteRun(object obj)
        {
            if (string.IsNullOrWhiteSpace(ProgramText))
                return false;
            return true;
        }

        private void ExecuteRun(object obj)
        {
            LexemeText = "";
            SyntaxAnalizerText = "";
            IdentifierText = "";
            ConstantText = "";
            var output = _lexicalAnalizer.Run(ProgramText);

            var tokStr = output.outputTokens.Select(tok => (tok.Value + "\t" + tok.Index + "\t" + tok.Row + "\t" + tok.GeneralizedValue));
            var idnputStr = output.outputIdentifiers.Select(tok => (tok.Value + "\t" + tok.Index + "\t" + tok.Row));
            var conputStr = output.outoutConstans.Select(tok => (tok.Value + "\t" + tok.Index + "\t" + tok.Row));
            foreach (var el in tokStr)
            {
                LexemeText += el + Environment.NewLine;
            }

            foreach (var el in idnputStr)
            {
                IdentifierText += el + Environment.NewLine;
            }

            foreach (var el in conputStr)
            {
                ConstantText += el + Environment.NewLine;
            }

            SyntaxAnalizerText = new SyntaxAnalizerRecursiveDescent().Process(output.outputTokens);
            if (SyntaxAnalizerText.Equals("Success"))
            {
                var icg = new IntermediateCodeGenerator(output.outputTokens);
                icg.process();
                String intermediateCode = icg.Information[icg.Information.Count - 1].Result;
                SyntaxAnalizerText = Environment.NewLine + intermediateCode;
                exec = new Executor(icg.Result, output.outputIdentifiers);
                Cond = exec.process();
                if (!Cond)
                    VarName = exec.ReadVar;
                WriteVar.RaiseCanExecuteChanged();
            }
        }
    }
}
