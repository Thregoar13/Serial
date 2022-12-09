
using System.IO.Ports;
using System.Net.Sockets;
using System.Text;
using Windows.Devices.PointOfService;

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

    SolarCalc solarCalc = new SolarCalc();
 
    


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
        //labelPacketLength.Text = newPacket.Length.ToString();
        int calChkSum = 0;
        if (newPacket.Length > 37)
        {
            if (newPacket.Substring(0, 3) == "###")
            {
                newPacketNumber = Convert.ToInt32(newPacket.Substring(3, 3));
                /*  labelPacketNum.Text = newPacket.Substring(3, 3);
                  newPacketNumber = Convert.ToInt32(labelPacketNum.Text);
                  labelAN0.Text = newPacket.Substring(6, 4);
                  labelAN1.Text = newPacket.Substring(10, 4);
                  labelAN2.Text = newPacket.Substring(14, 4);
                  labelAN3.Text = newPacket.Substring(18, 4);
                  labelAN4.Text = newPacket.Substring(22, 4);
                  labelAN5.Text = newPacket.Substring(26, 4);
                  labelBin.Text = newPacket.Substring(30, 4);
                  labelRxChkSum.Text = newPacket.Substring(34, 3);
                */

                if (oldPacketNumber > -1)
                {
                    if (newPacketNumber < oldPacketNumber)
                    {
                        packetRollover++;
                        //labelPacketRollover.Text = packetRollover.ToString();
                        if (oldPacketNumber != 000)
                        {
                            lostPacketCount += 999 - oldPacketNumber;
                            //labelPacketLost.Text = lostPacketCount.ToString();

                        }
                    }
                    else
                    {
                        if (newPacketNumber != oldPacketNumber + 1)
                        {
                            lostPacketCount += newPacketNumber - oldPacketNumber - 1;
                            //labelPacketLost.Text = lostPacketCount.ToString();
                        }
                    }
                }
                for (int i = 3; i < 34; i++)
                {
                    calChkSum += (byte)newPacket[i];
                }
                //labelChkSum.Text = Convert.ToString(calChkSum);
                calChkSum %= 1000;
                int recChkSum = Convert.ToInt32(newPacket.Substring(34, 3));
                if (recChkSum == calChkSum)
                {

                    DisplaySolarData(newPacket);
                    oldPacketNumber = newPacketNumber;
                }
                else
                {
                    chkSumError++;
                    // labelChkSumError.Text = chkSumError.ToString();
                }

            }


            string parsedData = $"{newPacket.Length,-14}" +
                                $"{newPacket.Substring(0, 3),-14}" +
                                $"{newPacket.Substring(3, 3),-14}" +
                                $"{newPacket.Substring(6, 4),-14}" +
                                $"{newPacket.Substring(10, 4),-14}" +
                                $"{newPacket.Substring(14, 4),-14}" +
                                $"{newPacket.Substring(18, 4),-14}" +
                                $"{newPacket.Substring(22, 4),-14}" +
                                $"{newPacket.Substring(26, 4),-14}" +
                                $"{newPacket.Substring(30, 4),-14}" +
                                $"{newPacket.Substring(34, 3),-14}  " +
                                $"{calChkSum,-14}    " +
                                $"{lostPacketCount,-14}        " +
                                $"{chkSumError,-14}    " +
                                $"{packetRollover,-14}    " +
                                "\r\n";




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

    private void DisplaySolarData(string validPacket)
    {
        solarCalc.ParseSolarData(validPacket);
        labelSolarVoltage.Text = solarCalc.GetVoltage(solarCalc.analogVoltage[0]);
        labelBatteryVoltage.Text = solarCalc.GetVoltage(solarCalc.analogVoltage[2]);
        labelBatteryCurrent.Text = solarCalc.GetCurrent(solarCalc.analogVoltage[1], solarCalc.analogVoltage[2]);
        labelLED1Current.Text = solarCalc.GetLedCurrent(solarCalc.analogVoltage[1], solarCalc.analogVoltage[4]);
        labelLED2Current.Text = solarCalc.GetLedCurrent(solarCalc.analogVoltage[1], solarCalc.analogVoltage[3]);
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

    private void btnBits0_Clicked(object sender, EventArgs e)
    {
        ButtonClicked(0);
    }
    private void btnBits1_Clicked(object sender, EventArgs e)
    {
        ButtonClicked(1);
    }


    private void btnBits2_Clicked(object sender, EventArgs e)
    {
        ButtonClicked(2);
    }

    private void btnBits3_Clicked(object sender, EventArgs e)
    {
        ButtonClicked(3);
    }


    private void ButtonClicked(int i)
    {

        Button[] btnBits = new Button[] { btnBits0, btnBits1, btnBits2, btnBits3 };
        if (btnBits[i].Text == "0")
        {
            btnBits[i].Text = "1";
            stringBuilderSend[i + 3] = '1';
            switch(i)
            {
                case 0:
                    imgLED1.Source = "lightoff.jpg";
                    break;
                case 1:
                    imgLED2.Source = "lightoff.jpg";
                    break;
            }
        }
        else
        {
            btnBits[i].Text = "0";
            stringBuilderSend[i + 3] = '0';
            switch (i)
            {
                case 0:
                    imgLED1.Source = "lighton.jpg";
                    break;
                case 1:
                    imgLED2.Source = "lighton.jpg";
                    break;
            }
        }
        sendPacket();
    }

    private void sendPacket()
    {
        int calSendChkSum = 0;
        try
        {

            for (int i = 3; i < 7; i++)
            {
                calSendChkSum += (byte)stringBuilderSend[i];
            }
            calSendChkSum %= 1000;
            stringBuilderSend.Remove(7, 3);
            stringBuilderSend.Insert(7, calSendChkSum.ToString());
            string messageOut = stringBuilderSend.ToString();
            entrySend.Text = stringBuilderSend.ToString();
            messageOut += "\r\n";
            byte[] messageBytes = Encoding.UTF8.GetBytes(messageOut);
            serialPort.Write(messageBytes, 0, messageBytes.Length);
        }
        catch (Exception ex)
        {
            DisplayAlert("Alert", ex.Message, "OK");
        }

    }

    private void imgLED1_Clicked(object sender, EventArgs e)
    {
        ButtonClicked(0);
    }

    private void imgLED2_Clicked(object sender, EventArgs e)
    {
        ButtonClicked(1);
    }


}
