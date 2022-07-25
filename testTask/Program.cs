using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace testTask
{
    public class DeviceInfo
    {
        public Device device { get; set; }
        public Brigade brigade { get; set; }
    }

    public class Device
    {
        public string serialNumber { get; set; }
        public bool isOnline { get; set; }
    }

    public class Brigade
    {
        public string code { get; set; }
    }

    public class Conflict
    {
        public string BrigadeCode { get; set; }
        public string[] DevicesSerials { get; set; }
    }
    
    public class Root
    {
        public Device device { get; set; }
        public Brigade brigade { get; set; }
    }
   
    
    class Program
    {
        
        
        static void Main(string[] args)
        {
            string text = System.IO.File.ReadAllText(@"Devices.json");
            DeviceInfo[] Root = JsonSerializer.Deserialize<DeviceInfo[]>(text);
            
            Console.WriteLine(Root.Length);
            List<DeviceInfo>[] Groups = new List<DeviceInfo>[(Root.Length*100)];
            for (int i = 0; i < (Root.Length*100); i++)
            {
                Groups[i] = new List<DeviceInfo>();
            }
            foreach (DeviceInfo info in Root)
            {
                Groups[Int32.Parse(info.brigade.code)%(Root.Length*100)].Add(info);
            }

            List<Conflict> conflicts = new List<Conflict>();
            
            foreach (List<DeviceInfo> info in Groups)
            {
                if (info.Count <= 1)
                {
                    continue;
                }

                Conflict conf = new Conflict();
                conf.DevicesSerials = new String[info.Count];
                int j = 0; bool ok = false;
                foreach (DeviceInfo deviceInfo in info)
                {
                    conf.DevicesSerials[j++] = deviceInfo.device.serialNumber;
                    conf.BrigadeCode = deviceInfo.brigade.code;
                    if (deviceInfo.device.isOnline)
                    {
                        ok = true;
                    }
                }

                if (ok)
                {
                    conflicts.Add(conf);
                }
            }
            foreach (Conflict conflict in conflicts)
            {
                Console.WriteLine(conflict.BrigadeCode);
                Console.WriteLine(String.Join(",",conflict.DevicesSerials));
                Console.WriteLine();
            }

            string jsonString = JsonSerializer.Serialize(conflicts);//,  new JsonSerializerOptions { WriteIndented = true });
            Console.WriteLine(jsonString);
            File.WriteAllText( "out.json", jsonString);

        }
    }
}