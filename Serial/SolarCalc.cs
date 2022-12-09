using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Serial
{
    class SolarCalc
    {

        //Field
        private static int ResistorValue;
        private static int LEDresistor;
        private static double Vref;
        private static int steps;

        private NumberFormatInfo provider = new NumberFormatInfo();

        public int[] analogVoltage = new int[5];


        //Constructor, takes no arguments
        public SolarCalc()
        {
            ResistorValue = 100;
            LEDresistor = 220;
            Vref = 2.57;
            steps = 5100;

            


            
           
        }

        //methods

        public string ParseSolarData(string solarData)
        {

           
            analogVoltage[0] = Convert.ToInt32(solarData.Substring(6, 4));    //converting analog to ints 
            analogVoltage[1] = Convert.ToInt32(solarData.Substring(10, 4));
            analogVoltage[2] = Convert.ToInt32(solarData.Substring(14, 4));
            analogVoltage[3] = Convert.ToInt32(solarData.Substring(18, 4));
            analogVoltage[4] = Convert.ToInt32(solarData.Substring(22, 4));
            return solarData;
        }


        public string GetVoltage(int an0)
        {
            double dAn0 = an0 * Vref / steps;
            return dAn0.ToString("0.0000");
        }


        public string GetCurrent(int an1, int an2)
        {
            int ShuntAnalog = an1 - an2;
            double ShuntVoltage = ShuntAnalog * Vref / steps;
            double dBatCurrent = ShuntVoltage / ResistorValue;
            dBatCurrent = dBatCurrent * 1000;
            return dBatCurrent.ToString("0.000000");
        }
        public string GetLedCurrent(int LEDAnalog, int an1)
        {
            int ShuntAnalog = an1 - LEDAnalog;
            double ShuntVoltage = ShuntAnalog * Vref / steps;
            double dLEDCurrent = ShuntVoltage / LEDresistor;

            if (dLEDCurrent < 0.0001)
            {
                dLEDCurrent = 0;
            }
            dLEDCurrent = dLEDCurrent * 1000;
            return dLEDCurrent.ToString("0.000000");
        }
    }
}
