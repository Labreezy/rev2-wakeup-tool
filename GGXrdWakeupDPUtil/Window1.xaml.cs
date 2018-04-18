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
        }

        private ReversalTool2 reversalTool;

        private CancellationTokenSource dummyTokenSource = new CancellationTokenSource();
        private CancellationToken dummyToken;
        private NameWakeupData currentDummy;
        private CancellationTokenSource reversalTokenSource = new CancellationTokenSource();
        private CancellationToken reversalToken;

        private CancellationTokenSource frameWaitTokenSource = new CancellationTokenSource();
        private CancellationToken frameWaitToken;


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            reversalTool = new ReversalTool2(Dispatcher);

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
        private void Window_Closed(object sender, EventArgs e)
        {
            dummyTokenSource?.Cancel();
            reversalTool?.Dispose();
        }
        private void Slot1Button_Click(object sender, RoutedEventArgs e)
        {
            int slotNumber = 0;
            //ReversalType reversalType = ReversalType.WakeUp;
            int delay = 0;
            string input = slot1Input.Text;

            input = "6,2,!3H";
            var slotInput = reversalTool.SetInputInSlot(slotNumber, input);

            StartReversalLoop(slotInput);
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

        private void StartReversalLoop(SlotInput slotInput)
        {
            reversalToken = reversalTokenSource.Token;

            Task.Run(() =>
            {
                while (!reversalToken.IsCancellationRequested)
                {
                    try
                    {
                        int wakeupTiming = reversalTool.GetWakeupTiming(currentDummy);

                        Task framewait = Task.Run(() => WaitFrames(wakeupTiming - slotInput.WakeupFrameIndex - 1), frameWaitToken);
                        framewait.Wait(frameWaitToken);

                        if (wakeupTiming == 0)
                        {
                            continue;
                        }

                        else
                        {

                            frameWaitTokenSource = new CancellationTokenSource();
                            frameWaitToken = frameWaitTokenSource.Token;


                            reversalTool.PlayReversal();
                        }

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"P2's animation address broke/has changed while enabled.  This program will now shut down.  If this issue persists, contact me.{Environment.NewLine}{ex.Message}");
                        Application.Current.Shutdown();
                        //TODO corriger
                        return;
                    }
                }

            }, reversalToken);
        }

        private void WaitFrames(int frames)
        {
            int fc = reversalTool.FrameCount();
            while (reversalTool.FrameCount() < fc + frames && !frameWaitToken.IsCancellationRequested)
            {

            }
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
