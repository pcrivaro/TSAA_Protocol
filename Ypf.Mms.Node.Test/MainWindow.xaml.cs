using System.Net;
using System.Windows;
using Ypf.Mms.Node.Protocols.Tsaa;
using Ypf.Mms.Node.Protocols.Tsaa.Core;
using Ypf.Mms.Node.Protocols.Tsaa.Enumerations;
using Ypf.Mms.Node.Protocols.Tsaa.Messages;

namespace Ypf.Mms.Node.Test
{
    public partial class MainWindow : Window
    {
        private NodeManager _manager = new NodeManager();

        private TsaaManager _tsaaManager = new TsaaManager();

        public MainWindow()
        {
            InitializeComponent();

            _tsaaManager.ReceivedMessage += TsaaManager_ReceivedMessage;

            _manager.Start();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            _manager.Start();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            _manager.Stop();
        }



        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            _tsaaManager.Connect(new IPEndPoint(IPAddress.Parse("10.12.78.45"), 1500));




            var msg = new ReadTriconDataMsg();
            
            msg.Header.NodeNumber = 1;
            msg.Header.SeqNumber = 1;
            msg.Header.Version = 0;
            msg.Header.Flag = 0x03;
            msg.Header.Id = 1;

            msg.Bins.Add(new ReadTriconDataMsg.BinData { BinNumber = BinTypes.BoolOutput, Offset = 1, NumberOfValues = 1 });
            msg.Bins.Add(new ReadTriconDataMsg.BinData { BinNumber = BinTypes.DintInput, Offset = 0, NumberOfValues = 1 });

            msg.Data = msg.GetBytes();






            //TriconDataReqMsg msg = new TriconDataReqMsg
            //{
            //    BinsRequested = BinsRequestedTypes.MasksAnalogInput,
            //    ReqTime = 10000
            //};

            //msg.Header.NodeNumber = 1;
            //msg.Header.SeqNumber = 1;
            //msg.Header.Version = 0;
            //msg.Header.Flag = 0x03;
            //msg.Header.Id = 1;

            //msg.Data = msg.GetBytes();
            




            _tsaaManager.SendMessage(msg);
        }



        private void TsaaManager_ReceivedMessage(object sender, Message e)
        {
            if (e is TriconDataMsg msg)
            {
                Dispatcher.Invoke(() =>
                {
                    var t = msg.GetBoolSystemStatusRead(420);
                });
            }
        }
    }
}
