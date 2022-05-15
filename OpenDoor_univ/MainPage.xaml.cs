using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.Networking.Sockets;
using Windows.ApplicationModel.Background;
using Windows.Storage;
using Windows.Networking;
using Windows.Storage.Streams;
using System.IO;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using System.Xml.Linq;
using System.Collections.Generic;

// 빈 페이지 항목 템플릿은 http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409 에 문서화되어 있습니다.

namespace OpenDoor_univ
{
    /// <summary>
    /// 자체적으로 사용하거나 프레임 내에서 탐색할 수 있는 빈 페이지입니다.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private string IP = "";
        private string read_xml = string.Empty;
        private string[] read_xml_array;

        public MainPage()
        {
            this.InitializeComponent();
        }
        #region 클라이언트
        private async void echo_client()
        {
            try
            {

                //Create the StreamSocket and establish a connection to the echo server.
                StreamSocket socket = new StreamSocket();

                read_xml = ReadIP();
                read_xml_array = read_xml.Split('#');



                //Host IP
                HostName serverHost = new HostName(read_xml_array[0].ToString()); //

                //Port Number
                string serverPort = "8090";
                await socket.ConnectAsync(serverHost, serverPort);

                //Write data to the echo server.
                Stream streamOut = socket.OutputStream.AsStreamForWrite();
                StreamWriter writer = new StreamWriter(streamOut);
                string request = DateTime.Now.ToString("open");
                await writer.WriteLineAsync(request);
                await writer.FlushAsync();

                //Read data from the echo server. 
                Stream streamIn = socket.InputStream.AsStreamForRead();
                StreamReader reader = new StreamReader(streamIn);
                string response = await reader.ReadLineAsync();

                TextBox1.Text = response;
                await Task.Delay(2000);
                TextBox1.Text = string.Empty;
                
                socket.Dispose();
            }
            catch (Exception e)
            {
                TextBox1.Text = e.Message;
                //Handle exception here. 
            }
        }
        #endregion
        private void button_Click(object sender, RoutedEventArgs e)
        {            
             echo_client();                      
        }

        public string ReadIP()
        {
            string config_text = string.Empty;

            string XMLFilePath = Path.Combine(Package.Current.InstalledLocation.Path, "Configration.xml");

            XDocument xdoc = XDocument.Load(XMLFilePath);
            IEnumerable<XElement> emps = xdoc.Root.Elements();
            foreach (var emp in emps)
            {
                config_text += emp.Value + "#";
            }

            return config_text;
        }

    }
}
