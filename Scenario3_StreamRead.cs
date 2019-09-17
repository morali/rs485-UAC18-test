//
// Copyright (c) 2018 The nanoFramework project contributors
// See LICENSE file in the project root for full license information.
//

using System;
using System.Text;
using System.Threading;
using Windows.Devices.SerialCommunication;
using Windows.Storage.Streams;


namespace SerialCommunication
{
    public class Scenario3_StreamRead
    {
        public static void Execute(ref SerialDevice serialDevice)
        {
            serialDevice.ReadTimeout = new TimeSpan(0, 0, 3);
            UTF8Encoding utf8 = new UTF8Encoding();

            /* setup data read for Serial Device input stream */
            DataReader inputDataReader = new DataReader(serialDevice.InputStream);
            inputDataReader.InputStreamOptions = InputStreamOptions.Partial;
            
            /* attempt to read 5 bytes from the Serial Device input stream (wait until ReadTimeout)*/
            int bytes_read = (int) inputDataReader.Load(8);
            Thread.Sleep(200);
            byte[] modbus_frame = new byte[bytes_read];

            if (bytes_read > 0)
            {
                /* Received some bytes. Output to the user number of bytes and message */
                //  inputDataReader.ReadBytes(modbus_frame);

                inputDataReader.ReadBytes(modbus_frame);

                String modbus_text = BitConverter.ToString(modbus_frame);
                Console.WriteLine("Input frame: >>" + modbus_text + "<< ");
                
                //modbus_text = modbus_frame.ToString();
                //    Console.WriteLine("Response frame: " + modbus_text + " .");
            }
            else
            {
                /* If received zero bytes inform the user */
                Console.WriteLine("NOTHING RECEIVED FROM " + serialDevice.PortName + ".");
            }
        }
    }
}
