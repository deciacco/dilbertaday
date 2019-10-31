using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using DilbertADay.com.esynaps.www;

namespace DilbertADay
{
    /// <summary>
    /// 
    /// </summary>
    class Program
    {
        public static int isCompleted = 0;
        public static Byte[] imgData;
        public static int timeout = 10000;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            DailyDilbert wsDd;
            MemoryStream strm;
            Bitmap bmp;

            int start, currentSecond;

            try
            {
                wsDd = new DailyDilbert();
                wsDd.DailyDilbertImageCompleted += new DailyDilbertImageCompletedEventHandler(
                    Program.DailyDilbertImageCompleted_Handler);
                wsDd.DailyDilbertImageAsync();

                start = DateTime.Now.Millisecond;
                currentSecond = start;

                Console.WriteLine("Sending request");
                while (Program.isCompleted == 0 && (currentSecond - start) < Program.timeout)
                {
                    if (currentSecond < DateTime.Now.Millisecond)
                    {
                        currentSecond = DateTime.Now.Millisecond;
                        Console.Write(".");
                    }
                }
                Console.Write("\n");

                if (Program.isCompleted == 1)
                {
                    Console.WriteLine("Generating image");
                    strm = new MemoryStream(Program.imgData);
                    bmp = new Bitmap(strm);
                    bmp.Save(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory)
                        + "\\dilbert.jpg", ImageFormat.Jpeg);

                    bmp.Dispose();
                    strm.Close();
                    wsDd.Dispose();
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }

            Console.WriteLine("Press any key too continue...");
            Console.ReadKey();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public static void DailyDilbertImageCompleted_Handler(object sender, DailyDilbertImageCompletedEventArgs args)
        {
            try
            {
                Program.imgData = args.Result;
                Program.isCompleted = 1;
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                Program.isCompleted = 2;
            }
        } 
    }
}
