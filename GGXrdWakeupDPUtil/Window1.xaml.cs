using System;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using GGXrdWakeupDPUtil.Library;

namespace GGXrdWakeupDPUtil
{
    /// <summary>
    /// Logique d'interaction pour Window1.xaml
    /// </summary>
    public partial class Window1
    {
        public Window1()
        {
            InitializeComponent();

            InputTextBox.TextChanged += inputTextBox_TextChanged;
        }

        private ReversalTool _reversalTool;

        private static bool _runDummyThread;
        private static readonly object RunDummyThreadLock = new object();

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _reversalTool = new ReversalTool(Dispatcher);

            try
            {
                _reversalTool.AttachToProcess();
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
            _reversalTool?.Dispose();
        }

        private void enableButton_Click(object sender, RoutedEventArgs e)
        {
            int slotNumber = 0;

            if (Slot1R.IsChecked != null && Slot1R.IsChecked.Value)
            {
                slotNumber = 1;
            }
            else if (Slot2R.IsChecked != null && Slot2R.IsChecked.Value)
            {
                slotNumber = 2;
            }
            else if (Slot3R.IsChecked != null && Slot3R.IsChecked.Value)
            {
                slotNumber = 3;
            }

            var slotInput = _reversalTool.SetInputInSlot(slotNumber, InputTextBox.Text);

            Action action = () =>
            {
                Dispatcher.Invoke(StopReversal);
            };

            _reversalTool.StartReversalLoop(slotInput, action);

            EnableButton.IsEnabled = false;
            DisableButton.IsEnabled = true;
            InputTextBox.IsEnabled = false;

            Slot1R.IsEnabled = false;
            Slot2R.IsEnabled = false;
            Slot3R.IsEnabled = false;
        }

        private void disableButton_Click(object sender, RoutedEventArgs e)
        {
            StopReversal();
        }



        private void inputTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            CheckValidInput();
        }

        private void CheckValidInput()
        {
            var text = InputTextBox.Text;

            if (string.IsNullOrEmpty(text))
            {
                ErrorTextBlock.Visibility = Visibility.Hidden;
                EnableButton.IsEnabled = false;
            }



            bool validInput = _reversalTool != null && _reversalTool.CheckValidInput(text);

            if (validInput)
            {
                ErrorTextBlock.Visibility = Visibility.Hidden;
                EnableButton.IsEnabled = true;
            }
            else
            {
                ErrorTextBlock.Visibility = Visibility.Visible;
                ErrorTextBlock.Text = "Invalid Input";
                EnableButton.IsEnabled = false;
            }
        }

        private void StartDummyLoop()
        {
            lock (RunDummyThreadLock)
            {
                _runDummyThread = true;
            }

            Thread dummyThread = new Thread(() =>
            {
                NameWakeupData currentDummy = null;
                bool localRunDummyThread = true;

                while (localRunDummyThread)
                {
                    try
                    {
                        var dummy = _reversalTool.GetDummy();

                        if (!Equals(dummy, currentDummy))
                        {
                            currentDummy = dummy;

                            SetDummyName(currentDummy?.CharName);
                        }
                    }
                    catch (Win32Exception)
                    {
                        StopDummyLoop();

                        Application.Current.Shutdown();
                        return;
                    }
                    catch (Exception)
                    {
                        StopDummyLoop();
                        MessageBox.Show("Can't read Dummy!");
                        Application.Current.Shutdown();
                        return;
                    }

                    lock (RunDummyThreadLock)
                    {
                        localRunDummyThread = _runDummyThread;
                    }

                    Thread.Sleep(2000);
                }
            })
            { Name = "dummyThread" };

            dummyThread.Start();

        }

        private void StopDummyLoop()
        {
            lock (RunDummyThreadLock)
            {
                _runDummyThread = false;
            }
        }

        private void SetDummyName(string dummyName)
        {
            lock (RunDummyThreadLock)
            {
                if (_runDummyThread)
                {
                    Dispatcher.Invoke(() =>
                    {
                        DummyTextBlock.Text = $"Current Dummy: {dummyName}";
                    });
                }


            }

        }

        private void StopReversal()
        {
            EnableButton.IsEnabled = true;
            DisableButton.IsEnabled = false;
            InputTextBox.IsEnabled = true;

            Slot1R.IsEnabled = true;
            Slot2R.IsEnabled = true;
            Slot3R.IsEnabled = true;
            _reversalTool.StopReversalLoop();
        }
    }
}
