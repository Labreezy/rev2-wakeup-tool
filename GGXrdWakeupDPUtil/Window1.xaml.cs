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
using GGXrdWakeupDPUtil.Library;

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

            inputTextBox.TextChanged += inputTextBox_TextChanged;
        }

        private ReversalTool2 reversalTool;

        private static bool _runDummyThread;
        private static object _runDummyThreadLock = new object();

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            reversalTool = new ReversalTool2(Dispatcher);

            try
            {
                reversalTool.AttachToProcess();
            }
            catch (Exception exception)
            {
                MessageBox.Show($"Guilty Gear not found open!  Remember, be in training mode paused when you open this program.  This program will now close.{Environment.NewLine}{exception.Message}");
                Application.Current.Shutdown();
                return;
            }


            StartDummyLoop();


        }
        private void Window_Closed(object sender, EventArgs e)
        {
            StopDummyLoop();
            reversalTool?.Dispose();
        }

        private void enableButton_Click(object sender, RoutedEventArgs e)
        {
            int slotNumber = 0;

            if (Slot1R.IsChecked != null && Slot1R.IsChecked.Value)
            {
                slotNumber = 1;
            }
            else if (Slot1R.IsChecked != null && Slot1R.IsChecked.Value)
            {
                slotNumber = 2;
            }
            else if (Slot1R.IsChecked != null && Slot1R.IsChecked.Value)
            {
                slotNumber = 3;
            }

            var slotInput = reversalTool.SetInputInSlot(slotNumber, inputTextBox.Text);

            reversalTool.StartReversalLoop(slotInput);

            enableButton.IsEnabled = false;
            disableButton.IsEnabled = true;
            inputTextBox.IsEnabled = false;
        }

        private void disableButton_Click(object sender, RoutedEventArgs e)
        {
            enableButton.IsEnabled = true;
            disableButton.IsEnabled = false;
            inputTextBox.IsEnabled = true;
            reversalTool.StopReversalLoop();
        }

        private void inputTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            CheckValidInput();
        }

        private void CheckValidInput()
        {
            var text = inputTextBox.Text;

            if (string.IsNullOrEmpty(text))
            {
                errorTextBlock.Visibility = Visibility.Hidden;
                enableButton.IsEnabled = false;
            }



            bool validInput = reversalTool != null && reversalTool.CheckValidInput(text);

            if (validInput)
            {
                errorTextBlock.Visibility = Visibility.Hidden;
                enableButton.IsEnabled = true;
            }
            else
            {
                errorTextBlock.Visibility = Visibility.Visible;
                errorTextBlock.Text = "Invalid Input";
                enableButton.IsEnabled = false;
            }
        }

        private void StartDummyLoop()
        {
            lock (_runDummyThreadLock)
            {
                _runDummyThread = true;
            }
            Thread dummyThread = new Thread(() =>
            {
                NameWakeupData currentDummy = null;
                bool localRunDummyThread = true;

                while (localRunDummyThread)
                {
                    var dummy = reversalTool.GetDummy();

                    if (!Equals(dummy, currentDummy))
                    {
                        currentDummy = dummy;

                        SetDummyName(currentDummy.CharName);

                    }

                    Thread.Sleep(2000);
                }

            });

            dummyThread.Name = "dummyThread";
            dummyThread.Start();

        }

        private void StopDummyLoop()
        {
            lock (_runDummyThreadLock)
            {
                _runDummyThread = false;
            }
        }

        private void SetDummyName(string dummyName)
        {
            Dispatcher.Invoke(() =>
            {
                dummyTextBlock.Text = $"Current Dummy: {dummyName}";
            });
            
        }
    }
}
