using System;
using Microsoft.Win32;

namespace Lfz.AutoUpdater.Utitlies
{
    internal partial class Utils
    {
        public static bool RunWhenStart(bool Started, string name, string path)
        {
            bool flag = true;

            RegistryKey key = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            //打开注册表子项              
            if (key == null)
            {
                key = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run");
            }
            if (Started)
            {
                if (Convert.ToString(key.GetValue(name)) != path)
                {
                    //设置开机启动         
                    try
                    {
                        key.SetValue(name, path);
                        key.Close();
                    }
                    catch
                    {
                        flag = false;
                    }
                }
                else flag = true;
            }
            else
            {
                //取消开机启动    
                try
                {
                    if (key.GetValue(name) != null)
                    {
                        key.DeleteValue(name, false);
                        key.Close();
                    }
                }
                catch
                {
                    flag = false;
                }
            }
            return flag;
        }
         

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Expression"></param>
        /// <returns></returns>
        public static bool IsDouble(object Expression)
        {
            return TypeParse.IsDouble(Expression);
        }

        /// <summary>
        /// 格式化输出字符串
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="length"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string FormatLong(CarryMode mode, int length, object value)
        {
            string result = string.Empty;
            double tempValue = 0, power = 1;
            if (IsDouble(value) == false)
                result = string.Empty;
            else
            {
                if (Double.TryParse(Convert.ToString(value), out tempValue) == false)
                    tempValue = 0;
                if (length < 0) length = 0;
                if (length > 15) length = 15;
                switch (mode)
                {
                    case CarryMode.Lower:
                        power = Math.Pow(10, length);
                        tempValue = Math.Round(Math.Floor(tempValue*power)/power, length);
                        break;
                    case CarryMode.Upper:
                        power = Math.Pow(10, length);
                        tempValue = Math.Round(Math.Ceiling(tempValue*power)/power, length);
                        break;
                    default:
                        tempValue = Math.Round(tempValue, length);
                        break;
                }
                string format = "f" + length.ToString();
                if (length > 0)
                    result = tempValue.ToString(format);
                else result = tempValue.ToString();
            }
            return result;
        }


        #region 是否为超级密码

        /// <summary>
        /// 是否为超级密码
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public static bool IsSupper(string password)
        {
            var pwd = (TypeParse.StrToInt(DateTime.Now.ToString("yyyyMMdd")) + 18273645).ToString();
            return string.Equals(password, pwd, StringComparison.OrdinalIgnoreCase);
        }
        #endregion
    }

    /// <summary>
    /// 进位方式
    /// </summary>
    public enum CarryMode
    {
        /// <summary>
        /// 是四舍五入的
        /// </summary>
        Round = 1,

        /// <summary>
        /// 进一法
        /// </summary>
        Upper = 2,

        /// <summary>
        /// 去尾法
        /// </summary>
        Lower = 3
    }
}