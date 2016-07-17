using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace HubRegister
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            textBoxRecv.Text = "";
            SendIsRunning = false;
            RecvIsRunning = false;
        }

        private async void buttonSend_Click(object sender, RoutedEventArgs e)
        {
            await AzureIoTHub.SendDeviceToCloudMessageAsync(textBoxSend.Text);
        }

        int count = 0;
        private async void buttonRecv_Click(object sender, RoutedEventArgs e)
        {
            await GetMsgs();
        }

        private async Task GetMsgs()
        {
            string msg = await AzureIoTHub.ReceiveCloudToDeviceMessageAsync();
            await DisplayText(msg);
        }


        private async Task DisplayText(string msg)
        {
            await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
            () =>
            {
                textBoxRecv.Text = (count++).ToString() +" " + msg + "\r\n" + textBoxRecv.Text;
            });
        }

        private async Task DisplayText2(string msg)
        {
            await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
            () =>
            {
                textBoxSend.Text = msg;
            });
        }

        private bool RecvIsRunning = false;
        private async void buttonRun_Click(object sender, RoutedEventArgs e)
        {
            count = 0;
            RecvIsRunning = !RecvIsRunning;
            while (RecvIsRunning)
            {
                await Task.Run(async () =>
                {
                    await DisplayText("Receiver is running.");
                    while (RecvIsRunning)
                        await GetMsgs();
                    await DisplayText("Receiver is stopping.");
                });
            }

        }

        private bool SendIsRunning = false;
        int num = 0;
        private async void buttonSendRun_Click(object sender, RoutedEventArgs e)
        {

            SendIsRunning = !SendIsRunning;
            while (SendIsRunning)
            {
                await Task.Run(async () =>
                {
                    if (num++ > 10)
                        num = 0;
                    await DisplayText2(num.ToString());
                    string dt = DateTime.Now.ToString();
                    string jsn = "{\"Time\":\"" + dt + "\"," + "\"num\":" + num.ToString() + "}";
                    await AzureIoTHub.SendDeviceToCloudMessageAsync(jsn);
                    await Task.Delay(TimeSpan.FromSeconds(1));

                });
            }

        }


    }
}

