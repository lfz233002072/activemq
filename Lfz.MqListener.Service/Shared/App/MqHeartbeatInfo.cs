// /*======================================================================
// *
// *        Copyright (C)  1996-2012  lfz    
// *        All rights reserved
// *
// *        Filename :AppRegisterModel.cs
// *        DESCRIPTION :
// *
// *        Created By 林芳崽 at 2016-01-27 16:08
// *        https://git.oschina.net/lfz
// *
// *======================================================================*/
 

namespace Lfz.MqListener.Shared.App
{
    /// <summary>
    /// 
    /// </summary>
    public class MqHeartbeatInfo 
    {  
        /// <summary>
        /// 
        /// </summary>
        public string AppId { get; set; }
             
    }

    /// <summary>
    /// 
    /// </summary>
    public class MqAppRegisterInfo
    {

        /// <summary>
        /// 
        /// </summary>
        public string AppId { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public string ComputerName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string IpAddress { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string MacAddress { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string ProcessDirectory { get; set; }

        /// <summary>
        /// 运行版本
        /// </summary>
        public string Version { get; set; }
    }

    public class MqDeviceHeatInfo
    { 
        /// <summary>
        /// 
        /// </summary>
        public string DeviceId { get; set; }
         
    }


    public class MqDeviceRegisterInfo
    { 
        public string Name { get; set; }
         

        /// <summary>
        ///  设备IP地址
        /// </summary> 
        public string IpAddress { get; set; }

        /// <summary>
        /// 设备MAC地址
        /// </summary> 
        public string MacAddress { get; set; }
         
        public string Description { get; set; }


        /// <summary>
        /// 运行系统版本
        /// </summary> 
        public string OSVersion { get; set; }

        /// <summary>
        /// 驱动版本
        /// </summary> 
        public string DriverVersion { get; set; }

        /// <summary>
        /// 远程访问信息
        /// </summary> 
        public string RemoteAccessInfo { get; set; }

        /// <summary>
        /// 工作时间段
        /// </summary> 
        public string WorkTimePeriod { get; set; }
    }
}