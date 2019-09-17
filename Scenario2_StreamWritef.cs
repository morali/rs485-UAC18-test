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
    public class Scenario2_StreamWrite
    {
        public static void Execute(ref SerialDevice serialDevice)
        {
            serialDevice.WriteTimeout = new TimeSpan(0, 0, 1);
            serialDevice.ReadTimeout = new TimeSpan(0, 0, 2);
            UTF8Encoding utf8 = new UTF8Encoding();

            /* setup data writer for Serial Device output stream */
            DataWriter outputDataWriter = new DataWriter(serialDevice.OutputStream);
            /* setup data read for Serial Device input stream */
            DataReader inputDataReader = new DataReader(serialDevice.InputStream);
            inputDataReader.InputStreamOptions = InputStreamOptions.Partial;

            byte[] modbus_frame = new byte[] { 0x02, 0x03, 0x00, 0x00, 0x00, 0x01, 0x84, 0x39 };
            byte[] response_frame = new byte[] { 0x02, 0x03, 0x02, 0x3c, 0x32, 0x6c, 0x91 };

            while (true)
            {
                Thread.Sleep(300);
                uint bytes_written = 0;

                /* Write data to stream */
                outputDataWriter.WriteBytes(modbus_frame);

                /* Store method sends data to remote device */
                try
                {
                    bytes_written = outputDataWriter.Store();
                    Console.WriteLine("Sent frame to " + serialDevice.PortName + "." + "(" + bytes_written + " bytes)");
                }
                catch (Exception e)
                {
                    Console.WriteLine("RS485 Write timeout.");
                }

                /* attempt to read 7 bytes from the Serial Device input stream (wait until ReadTimeout)*/
                try
                {
                    int bytes_read = (int)inputDataReader.Load(7);
                    if (bytes_read > 0)
                    {
                        byte[] received_frame = new byte[bytes_read];

                        /*Received some bytes.Output to the user number of bytes and message*/
                        inputDataReader.ReadBytes(received_frame);

                        String modbus_text = BitConverter.ToString(received_frame);
                        Console.WriteLine("Input frame: >>" + modbus_text + "<< ");
                        int i = 0;
                        for (i = 0; i < 7; i++)
                        {
                            if (received_frame[i] != response_frame[i])
                                break;
                        }
                        if (i == 7)
                        {
                            Console.WriteLine("Completed correct transmission cycle.");
                        }
                    }
                    else
                    {
                        /* If received zero bytes inform the user*/
                        Console.WriteLine("NOTHING RECEIVED FROM " + serialDevice.PortName + ".");
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("RS485 Read timeout.");
                }

                
            }
        }
    }    
}
