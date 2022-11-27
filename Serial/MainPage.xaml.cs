
using System.IO.Ports;
using System.Text;

namespace Serial;

public partial class MainPage : ContentPage
{
	private bool bPortOpen = false;
	private string newPacket = "";
	private int oldPacketNumber = -1;
	private int newPacketNumber = 0;
	private int lostPacketCount = 0;
	private int packetRollover = 0;
	private int chkSumError = 0;


	StringBuilder stringBuilderSend = new StringBuilder("###1111196");

	SerialPort serialPort = new SerialPort();
	public MainPage()
	{
		InitializeComponent();

		string[] ports = SerialPort.GetPortNames();
		portPicker.ItemsSource = ports;
		portPicker.SelectedIndex = ports.Length;
		Loaded += MainPage_Loaded;
	}

	private void MainPage_Loaded(object sender, EventArgs e)

	{
		serialPort.BaudRate = 115200;
		serialPort.ReceivedBytesThreshold = 1;
		serialPort.DataReceived += SerialPort_DataReceived;
	}

	private void SetUpSerialPorts()
	{

	}

	private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
	{
		newPacket = serialPort.ReadLine();
		//
		MainThread.BeginInvokeOnMainThread(MyMainThreadCode);

	}
	private void MyMainThreadCode()
	{
		//Code to run in main thread
		if (checkBoxHistory.IsChecked == true)
		{
			labelRxdata.Text = newPacket + labelRxdata.Text;
		}
		else
		{
			labelRxdata.Text = newPacket;
		}
		labelPacketLength.Text = newPacket.Length.ToString();
		int calChkSum = 0;
		if (newPacket.Length > 37)
		{
			if (newPacket.Substring(0, 3) == "###")
			{
				labelPacketNum.Text = newPacket.Substring(3, 3);
				newPacketNumber = Convert.ToInt32(labelPacketNum.Text);
				labelAN0.Text = newPacket.Substring(6, 4);
				labelAN1.Text = newPacket.Substring(10, 4);
				labelAN2.Text = newPacket.Substring(14, 4);
				labelAN3.Text = newPacket.Substring(18, 4);
				labelAN4.Text = newPacket.Substring(22, 4);
				labelAN5.Text = newPacket.Substring(26, 4);
				labelBin.Text = newPacket.Substring(30, 4);
				labelRxChkSum.Text = newPacket.Substring(34, 3);

				if (oldPacketNumber > -1)
				{
					if (newPacketNumber < oldPacketNumber)
					{
						packetRollover++;
						labelPacketRollover.Text = packetRollover.ToString();
						if (oldPacketNumber != 000)
						{
							lostPacketCount += 999 - oldPacketNumber;
							labelPacketLost.Text = lostPacketCount.ToString();

						}
					}
					else
					{
						if (newPacketNumber != oldPacketNumber + 1)
						{
							lostPacketCount += newPacketNumber - oldPacketNumber - 1;
							labelPacketLost.Text = lostPacketCount.ToString();
						}
					}
				}
				for (int i = 3; i < 34; i++)
				{
					calChkSum += (byte)newPacket[i];
				}
				labelChkSum.Text = Convert.ToString(calChkSum);
				calChkSum %= 1000;
				int recChkSum = Convert.ToInt32(newPacket.Substring(34, 3));
				if (recChkSum == calChkSum)
				{
					oldPacketNumber = newPacketNumber;
				}
				else
				{
					chkSumError++;
					labelChkSumError.Text = chkSumError.ToString();
				}

			}


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



	private void btnOpenClose_Clicked(object sender, EventArgs e)
	{
		if (!bPortOpen)
		{
			serialPort.PortName = portPicker.SelectedItem.ToString();
			serialPort.Open();
			btnOpenClose.Text = "Close";
			bPortOpen = true;
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
		catch (Exception ex)
		{
			await DisplayAlert("Alert", ex.Message, "OK");
		}


	}




}