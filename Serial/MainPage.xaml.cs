using System.IO.Ports;

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

    private void btnSend_Clicked(object sender, EventArgs e)
    {

    }


}

