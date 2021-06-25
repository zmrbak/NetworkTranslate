using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace NetworkTranslate
{
    public class IPTranslate
    {
        UInt32 ipIntAddress;
        UInt32 ipIntNetmask;

        public void Parse(string ipWithNetmask)
        {
            //111.9.11.128/25
            //118.113.96.0/255.254.253.0
            if (ipWithNetmask.Contains('/'))
            {
                //拆分
                var tempIP = ipWithNetmask.Split(new char[] { '/', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (tempIP.Length == 2)
                {
                    ipIntAddress = IP2Int(tempIP[0]);

                    //111.9.11.128/25
                    if (tempIP[1].Length < 3)
                    {
                        ipIntNetmask = SetBit(int.Parse(tempIP[1]));
                    }

                    //118.113.96.0/255.254.253.0
                    else
                    {
                        ipIntNetmask = IP2Int(tempIP[1]);
                    }
                }
                else
                {
                    throw new Exception(ipWithNetmask + " 不是一个有效的IP地址");
                }
                return;
            }

            //110.191.249.2-110.191.249.126
            if (ipWithNetmask.Contains('-'))
            {
                var tempIP = ipWithNetmask.Split(new char[] { '-', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (tempIP.Length == 2)
                {
                    var ipStart = IP2Int(tempIP[0]);
                    var ipEnd = IP2Int(tempIP[1]);

                    var ip1 = ipStart;
                    var ip2 = ipEnd;
                    for (UInt16 i = 0; i < 32; i++)
                    {
                        ip1 &= ~(1U << i);
                        ip2 &= ~(1U << i);

                        if (ip1 == ip2)
                        {
                            ipIntAddress = ip1;
                            ipIntNetmask = SetBit(31 - i);
                            break;
                        }
                    }
                }
                else
                {
                    throw new Exception(ipWithNetmask + " 不是一个有效的IP地址");
                }
            }
        }
        /// <summary>
        /// 可用IP地址数量
        /// </summary>
        public UInt32 IPCount => ((ipIntAddress & ipIntNetmask) | ~ipIntNetmask) - (ipIntAddress & ipIntNetmask);
        /// <summary>
        /// IP地址
        /// </summary>
        public string IPAddress => Int2IP(ipIntAddress);
        /// <summary>
        /// 子网掩码
        /// </summary>
        public string NetMask => Int2IP(ipIntNetmask);
        /// <summary>
        /// 掩码位数
        /// </summary>
        public int NetMaksLength => GetMaskBits(ipIntNetmask);
        /// <summary>
        /// 网络
        /// </summary>
        public string Network => Int2IP(ipIntAddress & ipIntNetmask);
        /// <summary>
        /// 本网络第一可用IP地址
        /// </summary>
        public string IPAddressFirst => Int2IP((ipIntAddress & ipIntNetmask) + 1);
        /// <summary>
        /// 本网络最后可用IP地址
        /// </summary>
        public string IPAddressLast => Int2IP(((ipIntAddress & ipIntNetmask) | ~ipIntNetmask) - 1);
        /// <summary>
        /// 广播地址
        /// </summary>
        public string IPBroadcast => Int2IP((ipIntAddress & ipIntNetmask) | ~ipIntNetmask);

        public override string ToString()
        {
            var obj = new { IPCount, IPAddress, NetMask, NetMaksLength, Network, IPAddressFirst, IPAddressLast, IPBroadcast };
            return new JavaScriptSerializer().Serialize(obj);
        }
        /// <summary>
        /// 将IP地址转换为无符号整数
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        UInt32 IP2Int(string ip)
        {
            var ips = ip.Split('.');
            return (UInt32.Parse(ips[0]) << 24 | UInt32.Parse(ips[1]) << 16 | UInt32.Parse(ips[2]) << 8 | UInt32.Parse(ips[3]));
        }

        /// <summary>
        /// 将IP地址转换为无符号整数
        /// </summary>
        /// <param name="ipInt"></param>
        /// <returns></returns>
        string Int2IP(UInt32 ipInt)
        {
            return (ipInt >> 24) + "." + ((ipInt >> 16) & 0xFF) + "." + ((ipInt >> 8) & 0xFF) + "." + ((ipInt) & 0xFF);
        }

        /// <summary>
        /// 设置子网掩码
        /// </summary>
        /// <param name="bitLength"></param>
        /// <returns></returns>
        UInt32 SetBit(int bitLength)
        {
            UInt32 ipInt = 0;
            for (int i = 0; i < bitLength; i++)
            {
                ipInt >>= 1;
                ipInt |= 1U << 31;

            }
            return ipInt;
        }

        /// <summary>
        /// 将字符串的子网掩码转换成数字
        /// </summary>
        /// <param name="netmask"></param>
        /// <returns></returns>
        int GetMaskBits(string netmask)
        {
            int ipInt = 0;
            var mask = IP2Int(netmask);
            for (int i = 0; i < 32; i++)
            {
                if ((mask & (1U << 31)) == 0) break;

                ipInt++;
                mask <<= 1;
            }
            if (mask != 0)
            {
                throw new Exception(netmask + " 不是一个合法的子网掩码！");
            }
            return ipInt;
        }

        int GetMaskBits(UInt32 mask)
        {
            int ipInt = 0;
            for (int i = 0; i < 32; i++)
            {
                if ((mask & (1U << 31)) == 0) break;

                ipInt++;
                mask <<= 1;
            }
            if (mask != 0)
            {
                throw new Exception(Int2IP(mask) + " 不是一个合法的子网掩码！");
            }
            return ipInt;
        }
    }
}
