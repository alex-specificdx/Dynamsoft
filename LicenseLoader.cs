using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Dynamsoft
{
    public class LicenseLoader
    {
        [DllImport("kernel32.dll")]
        static extern uint GetSystemDirectory([Out] StringBuilder lpBuffer, uint uSize);

        [DllImport("kernel32.dll")]
        static extern uint GetWindowsDirectory([Out] StringBuilder lpBuffer, uint uSize);

        private static string GetLicenseFilePath()
        {
            StringBuilder sbsSystemDir = new StringBuilder(256);
            uint iLength = 0;
            iLength = GetSystemDirectory(sbsSystemDir, 256);
            if (iLength == 0)
                return null;
            StringBuilder sbsWindowsDir = new StringBuilder(256);
            iLength = GetWindowsDirectory(sbsWindowsDir, 256);

            if (iLength == 0)
                return null;
            if (sbsWindowsDir.Length > 0)
            {
                if (IntPtr.Size == 8)
                {
                    sbsWindowsDir.Append("\\SysWOW64\\DynamsoftDotNetTwain.lic");
                }
                else
                {
                    sbsWindowsDir.Append("\\System32\\DynamsoftDotNetTwain.lic");
                }

            }
            return sbsWindowsDir.ToString();
        }

        private static List<string> ReadSystemLocalLicense(string strlicensePath)
        {
            List<string> liststringTempListLicense = null;
            try
            {
                if (File.Exists(strlicensePath))
                {
                    string[] stringarrTempAllLines = File.ReadAllLines(strlicensePath);
                    foreach (string strTemp in stringarrTempAllLines)
                    {
                        int iTempIndex = strTemp.IndexOf("SerialNo");
                        if (iTempIndex != -1)
                        {
                            int iIndex1 = strTemp.IndexOf("=");
                            if (iIndex1 != -1)
                            {
                                if (liststringTempListLicense == null)
                                    liststringTempListLicense = new List<string>();
                                string strTempLicense = strTemp.Substring((iIndex1 + 1), (strTemp.Length - (iIndex1 + 1)));
                                liststringTempListLicense.Add(strTempLicense);
                            }
                        }
                    }
                }
            }
            catch
            { }
            return liststringTempListLicense;
        }





        private static string GetCurrentPath()
        {
            string strCurrentPath = System.IO.Directory.GetCurrentDirectory();
            string strCurrentPathtxt = strCurrentPath + "\\lic.txt";
            return strCurrentPathtxt;
        }
        private static string ReadLicense(string strPath)
        {
            try
            {
                string strLicenseMessage = System.IO.File.ReadAllText(strPath);
                int beginIndex = strLicenseMessage.IndexOf("=\"");
                int endIndex = strLicenseMessage.IndexOf("\";");
                string strLicense = null;
                if (beginIndex != -1 && endIndex != -1)
                {
                    strLicense = strLicenseMessage.Substring(beginIndex + 2, endIndex - beginIndex - 2);
                }
                return strLicense;
            }
            catch (Exception)
            {
                string strLicense = null;
                return strLicense;
            }
        }

        private static string CombineAllLicense(string strCurrentPathLicense)
        {
            if (strCurrentPathLicense != null)
            {
                try
                {
                    string strTempLicenseFilePath = GetLicenseFilePath();
                    List<string> tempListLicense = ReadSystemLocalLicense(strTempLicenseFilePath);


                    StringBuilder objProductKeys = new StringBuilder();

                    foreach (string temp in tempListLicense)
                    {
                        if (temp != null)
                        {
                            objProductKeys.Append(temp);
                            objProductKeys.Append(";");
                        }
                    }

                    string strTempProductKey = objProductKeys.ToString();
                    strTempProductKey = strTempProductKey + strCurrentPathLicense + ";";
                    return strTempProductKey;
                }
                catch (Exception)
                {
                    string strTempProductKey = strCurrentPathLicense + ";";
                    return strTempProductKey;
                }
            }
            else
            {
                try
                {
                    string steTempLicenseFilePath = GetLicenseFilePath();
                    List<string> liststringTempListLicense = ReadSystemLocalLicense(steTempLicenseFilePath);


                    StringBuilder objProductKeys = new StringBuilder();

                    foreach (string temp in liststringTempListLicense)
                    {
                        if (temp != null)
                        {
                            objProductKeys.Append(temp);
                            objProductKeys.Append(";");
                        }
                    }

                    string strTempProductKey = objProductKeys.ToString();
                    return strTempProductKey;
                }
                catch (Exception)
                {
                    return null;
                }
            }

        }

        public static string ReadLocalLicense()
        {
            string strGetCurrentPath = LicenseLoader.GetCurrentPath();
            string strReadLicense = LicenseLoader.ReadLicense(strGetCurrentPath);
            string strCombineAllLicense = LicenseLoader.CombineAllLicense(strReadLicense);
            return strCombineAllLicense;
        }



    }

}