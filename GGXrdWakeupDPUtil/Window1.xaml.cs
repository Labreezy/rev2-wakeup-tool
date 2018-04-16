using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace GGXrdWakeupDPUtil
{
    /// <summary>
    /// Logique d'interaction pour Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();
        }

        private ReversalTool2 reversalTool;

        private CancellationTokenSource dummyTokenSource = new CancellationTokenSource();
        private CancellationToken dummyToken;
        private NameWakeupData currentDummy;



        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                reversalTool = new ReversalTool2();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Config invalid{Environment.NewLine}{ex.Message}");

                Application.Current.Shutdown();
                return;
            }

            try
            {
                reversalTool.AttachToProcess();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Process not found!{Environment.NewLine}{ex.Message}");
                Application.Current.Shutdown();
                return;
            }

            StartDummyLoop();
        }





        private void StartDummyLoop()
        {
            dummyToken = dummyTokenSource.Token;

            Task.Run(() =>
            {
                while (!dummyToken.IsCancellationRequested)
                {
                    var dummy = reversalTool.GetDummy();

                    if (!Equals(dummy, currentDummy))
                    {
                        SetUIDummy(dummy);

                        currentDummy = dummy;
                    }
                    Thread.Sleep(1000);
                }

            }, dummyToken);
        }




        //TODO Refactor with MVVM
        #region UI
        private void SetUIDummy(NameWakeupData dummy)
        {
            Dispatcher.Invoke(() =>
            {
                dummyTextBlock.Text = $"Current Dummy :{dummy.CharName}";
            });
        }
        #endregion
    }
}
