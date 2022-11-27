using System.IO.Ports;
using System.Text;

namespace Serial;

public partial class MainPage : ContentPage
{
		private bool bPortOpen=false;
	private string newPacket = "";
	SerialPort serialPort = new SerialPort();	
	

	public MainPage()
	{
		InitializeComponent();

		string[] ports = SerialPort.GetPortNames();
		portPicker.ItemsSource = ports;
		portPicker.SelectedIndex = ports.Length;
		Loaded += MainPage_Loaded;
		//foreach (string port in ports)
		//{
		//	portPicker.Items.Add(port);
		//}
	}



    private void MainPage_Loaded(object sender, EventArgs e)

    {
		serialPort.BaudRate = 115200;
		serialPort.ReceivedBytesThreshold = 1;
		serialPort.DataReceived += SerialPort_DataReceived;

    }

	private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
	{
		newPacket = serialPort.ReadLine();
		MainThread.BeginInvokeOnMainThread(MyMainThreadCode);				
		
	}

	private void MyMainThreadCode()
	{
		if (checkBoxHistory.IsChecked == true)
		{
			labelRxdata.Text = newPacket + labelRxdata.Text;
		}
		else
		{
			labelRxdata.Text = newPacket;
		}
		int calChkSum = 0;
		if (newPacket.Length > 37)
		{
			if (newPacket.Substring(0, 3) == "###")
			{
				string parsedData = $"{newPacket.Length,-13}" +
									$"{newPacket.Substring(0, 3),-14}" +
									$"{newPacket.Substring(3, 3),-14}" +
									$"{newPacket.Substring(6, 4),-14}" +
									$"{newPacket.Substring(10, 4),-14}" +
									$"{newPacket.Substring(14, 4),-14}" +
									$"{newPacket.Substring(18, 4),-14}" +
									$"{newPacket.Substring(22, 4),-14}" +
									$"{newPacket.Substring(26, 4),-14}" +
									$"{newPacket.Substring(30, 4),-14}" +
									$"{newPacket.Substring(34, 4),-14}" + "\r\n";




				if (checkBoxParsedHistory.IsChecked == true)
				{
					labelParsedData.Text = parsedData + labelParsedData.Text;
				}
				else
				{
					labelParsedData.Text = parsedData;
				}
			}
        }
	}

	private void btnOpenClose_Clicked(object sender, EventArgs e)
	{
		if (!bPortOpen)
		{
			serialPort.PortName=portPicker.SelectedItem.ToString();
			serialPort.Open();
			btnOpenClose.Text = "Close";
			bPortOpen=true;
		}
		else
		{
			serialPort.Close();
			btnOpenClose.Text = "Open";
			bPortOpen = false;
		}
	}
    private void btnClear_Clicked(object sender, EventArgs e)
    {

    }

    private async void btnSend_Clicked(object sender, EventArgs e)
    {
		try
		{
            string messageOut = entrySend.Text;
            messageOut += "\r\n";
            byte[] messageBytes = Encoding.UTF8.GetBytes(messageOut);
            serialPort.Write(messageBytes, 0, messageBytes.Length);
        }
		catch(Exception ex)
		{
			await DisplayAlert("Alert",ex.Message, "OK")
		}


    }


}

