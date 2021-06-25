using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkTranslate
{
    class Program
    {
        static void Main(string[] args)
        {
            List<(string, string, string)> ipLists = new List<(string, string, string)>();

            //此范围计算不准确
            ipLists.Add(("运营商网络部分", "学生寝室电信网络IP地址", "110.191.168.2 - 110.191.177.254"));

            //下面可以正常计算
            ipLists.Add(("运营商网络部分", "学生寝室电信网络IP地址", "111.9.11.128/25"));
            ipLists.Add(("运营商网络部分", "学生寝室电信网络IP地址", "111.9.12.160 / 27"));
            ipLists.Add(("运营商网络部分", "学生寝室电信网络IP地址", "111.9.12.64 / 26"));
            ipLists.Add(("运营商网络部分", "学生寝室电信网络IP地址", "118.113.96.0/255.255.240.0"));

            IPTranslate iP = new IPTranslate();
            foreach (var item in ipLists)
            {
                Console.WriteLine(item.Item1 + "," + item.Item2 + "," + item.Item3);
                iP.Parse(item.Item3);

                Console.WriteLine(iP);
                Console.WriteLine("IP地址:\t" + iP.IPAddress + "/" + iP.NetMaksLength);
                Console.WriteLine("可用IP数量:\t" + iP.IPCount);
                Console.WriteLine("子网掩码:\t" + iP.NetMask);
                Console.WriteLine("网络地址:\t" + iP.Network);
                Console.WriteLine("第一可用地址:\t" + iP.IPAddressFirst);
                Console.WriteLine("最后可用地址:\t" + iP.IPAddressLast);
                Console.WriteLine("广播地址:\t" + iP.IPBroadcast);
                Console.WriteLine();
            }
        }
    }
}
