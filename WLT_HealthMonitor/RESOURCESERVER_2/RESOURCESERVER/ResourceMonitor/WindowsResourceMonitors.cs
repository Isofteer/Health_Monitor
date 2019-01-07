using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
using System.Web;
using System.Messaging;
using System.Management;
using Microsoft.VisualBasic;

namespace RESOURCESERVER.ResourceMonitor
{
   
    public class WindowsResourceMonitors
    {
       
         
        private double getCPUUsage()
        {
            var _cpu = 0.0;           

            ManagementObjectSearcher searcher = new ManagementObjectSearcher("select * from Win32_PerfFormattedData_PerfOS_Processor");

            foreach (ManagementObject obj in searcher.Get())
            {
                _cpu = Convert.ToDouble( obj["PercentProcessorTime"]);

                var name = obj["Name"];
                            
            }
            
            return _cpu;
        }

        /// <summary>
        /// Gets memory usage in %
        /// </summary>
        /// <returns></returns>
        private double getMemoryUsage()
        {
            double memAvailable, memPhysical;

            PerformanceCounter pCntr = new PerformanceCounter("Memory", "Available KBytes");
            memAvailable = (double)pCntr.NextValue();

            memPhysical = GetTotalMemoryInBytes()/1024;

            return (memPhysical - memAvailable) * 100 / memPhysical;
        }
               
        public string getAllReadings()
        {

          

              var cpuCounter = new PerformanceCounter("Processor Information", "% Processor Time", "_Total");

            var ramCounter = new PerformanceCounter("Memory", "Available MBytes");

            var diskCounter = new PerformanceCounter("PhysicalDisk", "% Disk Time", "_Total");

            var ipProperties = IPGlobalProperties.GetIPGlobalProperties();


            var allIdle = new PerformanceCounter("Processor", "% Idle Time", "_Total");
            
            allIdle.NextValue();

            CounterSample cs1 = cpuCounter.NextSample();

            cpuCounter.NextValue();

            diskCounter.NextValue();

            System.Threading.Thread.Sleep(1000);

            int cpu = 100 - (int) allIdle.NextValue();

            CounterSample cs2 = cpuCounter.NextSample();

             float finalCpuCounter = CounterSample.Calculate(cs1, cs2);

            var _cpuCounter = cpuCounter.NextValue();            

            var _ModelResource = new ModelResource();

            _ModelResource.CPU = VMwareGetCpuUsage();// getCPUUsage();

            _ModelResource.RAM = getMemoryUsage();

            _ModelResource.DISK = diskCounter.NextValue();

            _ModelResource.COMPUTERNAME = string.Format("{0}.{1}", ipProperties.HostName, ipProperties.DomainName);

            _ModelResource.SendDate = DateTime.UtcNow;

            _ModelResource.MSMQ = MessagingQueue_();

            var MSMQs = MessagingQueue();

            var MSMQ_ALERTS = MSMQs.Where(n => n.Item1 == "private$\\alerts").FirstOrDefault();

            var MSMQ_UTSIN = MSMQs.Where(n => n.Item1 == "private$\\uts_mq_in").FirstOrDefault();

            _ModelResource.MSMQ_ALERTS = MSMQ_ALERTS ==null? 0: MSMQ_ALERTS.Item2;

            _ModelResource.MSMQ_UTSIN = MSMQ_UTSIN ==null?0: MSMQ_UTSIN.Item2;

            // GetRandomNumber(0, 1000);  //
            _ModelResource.CORES = VMwareGetCpuUsagesJSON();

            string returnstatement = JsonConvert.SerializeObject(_ModelResource);

            return returnstatement;
        }
        private static readonly Random getrandom = new Random();

        static ulong GetTotalMemoryInBytes()
        {
            return new Microsoft.VisualBasic.Devices.ComputerInfo().TotalPhysicalMemory;
        }
        public static int GetRandomNumber(int min, int max)
        {
            lock (getrandom) // synchronize
            {
                return getrandom.Next(min, max);
            }
        }
        public double MessagingQueue_()
        {

            MessageQueue[] QueueList =
                MessageQueue.GetPrivateQueuesByMachine(".");

            var _total = 0.0;
            foreach (MessageQueue queueItem in QueueList)
            {
                var dtrimg = String.Format(@".\{0}", queueItem.QueueName);


                //@".\Private$\IDG"
                MessageQueue messageQueue = new MessageQueue(dtrimg);
                _total += messageQueue.Count();
                // Console.WriteLine(queueItem.Path);
            }
            return _total;

        }


        public List <Tuple<string,long>> MessagingQueue()
        {
            var array = new List<Tuple<string, long>>();

            MessageQueue[] QueueList = MessageQueue.GetPrivateQueuesByMachine(".");

            var _total = 0.0;
            foreach (MessageQueue queueItem in QueueList)
            {
                var data = queueItem.QueueName.Trim();

                if (queueItem.QueueName.Trim() == "private$\\alerts".Trim() || queueItem.QueueName.Trim() == "private$\\uts_mq_in".Trim())
                {
                    var strimg = String.Format(@".\{0}", queueItem.QueueName);
                    MessageQueue messageQueue = new MessageQueue(strimg);
                    array.Add( new Tuple <string,long>(queueItem.QueueName.Trim(), messageQueue.Count()));
                  
                }

            }
            return array;
            ;

        }
        public double VMwareGetCpuUsage() {

            ManagementObjectSearcher searcher = new ManagementObjectSearcher("select * from Win32_PerfFormattedData_PerfOS_Processor");
            //var cpuTimes = searcher.Get();
            var queryCollection = (from ManagementObject x in searcher.Get()
                                  select new UsageComponent() {

                                      Usage = Convert.ToDouble(x["PercentProcessorTime"]),
                                      Name = Convert.ToString(x["Name"])

                                  }).ToList();


   
            var cores = queryCollection.Where(n => Information.IsNumeric(n.Name)).Count();

            var devident = 1;

            if (cores > 1 && cores % 2 == 0)
            {
                devident = 2;


            }

            return queryCollection.Where(n => n.Name == "_Total").SingleOrDefault().Usage;

          

        }
        public string VMwareGetCpuUsagesJSON()
        {

            ManagementObjectSearcher searcher = new ManagementObjectSearcher("select * from Win32_PerfFormattedData_PerfOS_Processor");
            //var cpuTimes = searcher.Get();
            var queryCollection = (from ManagementObject x in searcher.Get()
                                   select new UsageComponent()
                                   {

                                       Usage = Convert.ToDouble(x["PercentProcessorTime"]),
                                       Name = Convert.ToString(x["Name"])

                                   }).ToList();
                       

            return    JsonConvert.SerializeObject( queryCollection);
                       
        }

        private void  messageInsert() {


            MessageQueue messageQueue = null;

            string description = "This is a test queue.";

            string message = "This is a test message.";

            string path = @".\Private$\uts_mq_in";

            try

            {

                if (MessageQueue.Exists(path))

                {

                    messageQueue = new MessageQueue(path);

                    messageQueue.Label = description;

                }

                else

                {
                    MessageQueue.Create(path);

                    messageQueue = new MessageQueue(path);

                    messageQueue.Label = description;

                }

                messageQueue.Send(message);

            }

            catch

            {

                throw;

            }

            finally

            {

                messageQueue.Dispose();

            }

        }
    }
    
    }
    public static class Methods
    {
        public static long Count(this MessageQueue messageQueue)
        {
            var enumerator = messageQueue.GetMessageEnumerator2();
            long counter = 0;
            while (enumerator.MoveNext())
            {
                counter++;
            }
            return counter;
        }
    }
    public class UsageComponent {

     public   string Name { get; set; }
        public double Usage { get; set; }
    }
