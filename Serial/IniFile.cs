using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
/// <summary>
/// 该类包含INI文件创建，读取 ，写入等函数；
/// </summary>

namespace yuanbang
{
    class IniFile
    {
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string defVal, StringBuilder retVal,
            int size, string filePath);
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string defVal, Byte[] retVal, int size,
            string filePath);
        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);

        private string FilePath;

        public IniFile(string Path)
        {
            this.FilePath = Path;
        }

        public void IniFileWrite(string section, string key, string value)
        {
            WritePrivateProfileString(section, key, value, this.FilePath);
        }

        public string IniFileRead(string section, string key)
        {
            StringBuilder temp = new StringBuilder(500);
            string defVal = "读取失败";
            int i = GetPrivateProfileString(section, key, defVal, temp, 500, this.FilePath);
            return temp.ToString();
        }

        public byte[] IniFileRead_ValuesL(string section, string key)
        {
            byte[] temp = new byte[255];
            int i = GetPrivateProfileString(section, key, "", temp, 255, this.FilePath);
            return temp;
        }
    }
}
