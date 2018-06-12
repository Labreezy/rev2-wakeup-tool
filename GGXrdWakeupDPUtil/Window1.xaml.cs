using System;
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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _reversalTool = new ReversalTool();

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


            _reversalTool.DummyChanged += _reversalTool_DummyChanged;
            _reversalTool.ReversalLoopErrorOccured += _reversalTool_ReversalLoopErrorOccured;


        }
        private void Window_Closed(object sender, EventArgs e)
        {
            _reversalTool?.Dispose();
        }

        #region Reversal tool events
        private void _reversalTool_ReversalLoopErrorOccured(Exception ex)
        {
            StopReversal();
        }

        private void _reversalTool_DummyChanged(NameWakeupData dummy)
        {
            SetDummyName(dummy.CharName);
        }
        #endregion


        #region Reversal

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



            _reversalTool.StartReversalLoop(slotInput);

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



        private void SetDummyName(string dummyName)
        {
            Dispatcher.Invoke(() =>
            {
                DummyTextBlock.Text = $"Current Dummy: {dummyName}";
            });
        }

        private void StopReversal()
        {
            Dispatcher.Invoke(() =>
            {
                EnableButton.IsEnabled = true;
                DisableButton.IsEnabled = false;
                InputTextBox.IsEnabled = true;

                Slot1R.IsEnabled = true;
                Slot2R.IsEnabled = true;
                Slot3R.IsEnabled = true;
            });

        }
        #endregion

        #region Burst
        private void enableBurstButton_Click(object sender, RoutedEventArgs e)
        {
            int slotNumber = 0;

            if (Slot1RBurst.IsChecked != null && Slot1RBurst.IsChecked.Value)
            {
                slotNumber = 1;
            }
            else if (Slot2RBurst.IsChecked != null && Slot2RBurst.IsChecked.Value)
            {
                slotNumber = 2;
            }
            else if (Slot3RBurst.IsChecked != null && Slot3RBurst.IsChecked.Value)
            {
                slotNumber = 3;
            }

            int min = NumericUpDownMinBurst.Value;
            int max = NumericUpDownMaxBurst.Value;
            bool alwaysBurst = AlwaysBurstCheckBox.IsChecked.HasValue && AlwaysBurstCheckBox.IsChecked.Value;

            _reversalTool.StartRandomBurstLoop(min, max, slotNumber, alwaysBurst);



            EnableBurstButton.IsEnabled = false;
            DisableBurstButton.IsEnabled = true;
            NumericUpDownMinBurst.IsEnabled = false;
            NumericUpDownMaxBurst.IsEnabled = false;
            AlwaysBurstCheckBox.IsEnabled = false;

            Slot1RBurst.IsEnabled = false;
            Slot2RBurst.IsEnabled = false;
            Slot3RBurst.IsEnabled = false;
        }

        private void disableBurstButton_Click(object sender, RoutedEventArgs e)
        {
            StopBurst();
        }
        private void StopBurst()
        {
            Dispatcher.Invoke(() =>
            {
                EnableBurstButton.IsEnabled = true;
                DisableBurstButton.IsEnabled = false;
                NumericUpDownMinBurst.IsEnabled = true;
                NumericUpDownMaxBurst.IsEnabled = true;
                AlwaysBurstCheckBox.IsEnabled = true;

                Slot1RBurst.IsEnabled = true;
                Slot2RBurst.IsEnabled = true;
                Slot3RBurst.IsEnabled = true;
            });

        }
        private void NumericUpDownMinBurst_ValueChanged(object sender, RoutedEventArgs e)
        {
            if (NumericUpDownMinBurst != null && NumericUpDownMaxBurst != null)
            {
                NumericUpDownMaxBurst.Minimum = NumericUpDownMinBurst.Value; 
            }
        }
        private void NumericUpDownMaxBurst_ValueChanged(object sender, RoutedEventArgs e)
        {
            if (NumericUpDownMinBurst != null && NumericUpDownMaxBurst != null)
            {
                NumericUpDownMinBurst.Maximum = NumericUpDownMaxBurst.Value; 
            }
        }
        #endregion


    }
}
