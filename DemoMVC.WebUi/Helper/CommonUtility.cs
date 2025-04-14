using DemoMVC.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using static DemoMVC.Helper.Constants;

namespace DemoMVC.WebUi.Helper
{
    public class CommonUtility
    {
        public static string GetSenderByRequirement(string reqsender)
        {
            string fromemailId = ConfigurationManager.AppSettings["SENDER"].ToString(); ;
            if (!String.IsNullOrWhiteSpace(reqsender))
            {
                fromemailId = reqsender.Trim();
            }
            return fromemailId;
        }
        public static string RemoveDuplicateRecord(string oldstring)
        {
            char[] delimiters = new[] { ',', ';' };
            List<string> uniques = new List<string>();
            var uniquelist = oldstring.Split(delimiters).Reverse().Distinct().ToList();
            if (uniquelist.Count > 0)
            {
                uniques = uniquelist.Where(x => x != "").ToList();
            }
            string newStr = string.Join(",", uniques);
            return newStr;
        }
        public static string FirstCharToUpper(string input)
        {
            string unitcapitalname = string.Empty;
            if (!String.IsNullOrEmpty(input))
            {
                foreach (var item in input.Split(' '))
                {
                    if (item != "")
                    {
                        unitcapitalname += item.First().ToString().ToUpper() + item.ToString().ToLower().Substring(1) + " ";
                    }
                }
                unitcapitalname = unitcapitalname.Substring(0, unitcapitalname.Length - 1);
            }
            return unitcapitalname;
        }
        public static string GetDateFormat(bool isFullFormat)
        {
            if (isFullFormat)
                return "MM/dd/yyyy hh:mm:ss tt";
            else
                return "MM/dd/yyyy";
        }
        public static string Encrypt(String input)
        {
            byte[] source;
            int length, length2;
            int blockCount;
            int paddingCount;

            if (input == null || input.Length == 0)
            {
                return String.Empty;
            }

            source = System.Text.Encoding.ASCII.GetBytes(input); ;
            length = input.Length;

            if ((length % 3) == 0)
            {
                paddingCount = 0;
                blockCount = length / 3;
            }
            else
            {
                paddingCount = 3 - (length % 3);//need to add padding
                blockCount = (length + paddingCount) / 3;
            }
            length2 = length + paddingCount;//or blockCount *3
            byte[] source2;
            source2 = new byte[length2];
            //copy data over insert padding
            for (int x = 0; x < length2; x++)
            {
                if (x < length)
                {
                    source2[x] = source[x];
                }
                else
                {
                    source2[x] = 0;
                }
            }

            byte b1, b2, b3;
            byte temp, temp1, temp2, temp3, temp4;
            byte[] buffer = new byte[blockCount * 4];
            char[] result = new char[blockCount * 4];
            for (int x = 0; x < blockCount; x++)
            {
                b1 = source2[x * 3];
                b2 = source2[x * 3 + 1];
                b3 = source2[x * 3 + 2];

                temp1 = (byte)((b1 & 252) >> 2);//first

                temp = (byte)((b1 & 3) << 4);
                temp2 = (byte)((b2 & 240) >> 4);
                temp2 += temp; //second

                temp = (byte)((b2 & 15) << 2);
                temp3 = (byte)((b3 & 192) >> 6);
                temp3 += temp; //third

                temp4 = (byte)(b3 & 63); //fourth

                buffer[x * 4] = temp1;
                buffer[x * 4 + 1] = temp2;
                buffer[x * 4 + 2] = temp3;
                buffer[x * 4 + 3] = temp4;

            }

            for (int x = 0; x < blockCount * 4; x++)
            {
                result[x] = sixbit2char(buffer[x]);
            }

            //covert last "A"s to "=", based on paddingCount
            switch (paddingCount)
            {
                case 0: break;
                case 1: result[blockCount * 4 - 1] = '='; break;
                case 2:
                    result[blockCount * 4 - 1] = '=';
                    result[blockCount * 4 - 2] = '=';
                    break;
                default: break;
            }
            return new String(result);
        }
        public static string Decrypt(String input)
        {
            char[] source;
            int length, length2, length3;
            int blockCount;
            int paddingCount;
            int temp = 0;

            if (input == null || input.Length == 0)
            {
                return String.Empty;
            }

            source = input.ToCharArray();
            length = input.Length;

            //find how many padding are there
            for (int x = 0; x < 2; x++)
            {
                if (input[length - x - 1] == '=')
                    temp++;
            }
            paddingCount = temp;
            //calculate the blockCount;
            //assuming all whitespace and carriage returns/newline were removed.
            blockCount = length / 4;
            length2 = blockCount * 3;

            byte[] buffer = new byte[length];//first conversion result
            byte[] buffer2 = new byte[length2];//decoded array with padding

            for (int x = 0; x < length; x++)
            {
                buffer[x] = char2sixbit(source[x]);
            }

            byte b, b1, b2, b3;
            byte temp1, temp2, temp3, temp4;

            for (int x = 0; x < blockCount; x++)
            {
                temp1 = buffer[x * 4];
                temp2 = buffer[x * 4 + 1];
                temp3 = buffer[x * 4 + 2];
                temp4 = buffer[x * 4 + 3];

                b = (byte)(temp1 << 2);
                b1 = (byte)((temp2 & 48) >> 4);
                b1 += b;

                b = (byte)((temp2 & 15) << 4);
                b2 = (byte)((temp3 & 60) >> 2);
                b2 += b;

                b = (byte)((temp3 & 3) << 6);
                b3 = temp4;
                b3 += b;

                buffer2[x * 3] = b1;
                buffer2[x * 3 + 1] = b2;
                buffer2[x * 3 + 2] = b3;
            }
            //remove paddings
            length3 = length2 - paddingCount;
            byte[] result = new byte[length3];

            for (int x = 0; x < length3; x++)
            {
                result[x] = buffer2[x];
            }

            return System.Text.Encoding.UTF8.GetString(result);
        }

        public static DateTime ConvertDataBaseDateFromUTCDate(DateTime dateTime)
        {
            TimeZoneInfo objTimeZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
            return TimeZoneInfo.ConvertTimeFromUtc(dateTime, objTimeZone);
        }

        public static char sixbit2char(byte b)
        {
            char[] lookupTable = new char[64]
                    {
                        'A','B','C','D','E','F','G','H','I','J','K','L','M','N','O','P','Q','R','S','T','U','V','W','X','Y','Z',
                        'a','b','c','d','e','f','g','h','i','j','k','l','m','n','o','p','q','r','s','t','u','v','w','x','y','z',
                        '0','1','2','3','4','5','6','7','8','9','+','/'};

            if ((b >= 0) && (b <= 63))
            {
                return lookupTable[(int)b];
            }
            else
            {
                //should not happen;
                return ' ';
            }
        }
        public static byte char2sixbit(char c)
        {
            char[] lookupTable = new char[64]
                    {
                        'A','B','C','D','E','F','G','H','I','J','K','L','M','N','O','P','Q','R','S','T','U','V','W','X','Y','Z',
                        'a','b','c','d','e','f','g','h','i','j','k','l','m','n','o','p','q','r','s','t','u','v','w','x','y','z',
                        '0','1','2','3','4','5','6','7','8','9','+','/'};
            if (c == '=')
                return 0;
            else
            {
                for (int x = 0; x < 64; x++)
                {
                    if (lookupTable[x] == c)
                        return (byte)x;
                }
                //should not reach here
                return 0;
            }
        }


        public static TimeZoneInfo OlsonTimeZoneToTimeZoneInfo(string olsonTimeZoneId)
        {
            var olsonWindowsTimes = new Dictionary<string, string>()
    {
        { "Africa/Bangui", "W. Central Africa Standard Time" },
        { "Africa/Cairo", "Egypt Standard Time" },
        { "Africa/Casablanca", "Morocco Standard Time" },
        { "Africa/Harare", "South Africa Standard Time" },
        { "Africa/Johannesburg", "South Africa Standard Time" },
        { "Africa/Lagos", "W. Central Africa Standard Time" },
        { "Africa/Monrovia", "Greenwich Standard Time" },
        { "Africa/Nairobi", "E. Africa Standard Time" },
        { "Africa/Windhoek", "Namibia Standard Time" },
        { "America/Anchorage", "Alaskan Standard Time" },
        { "America/Argentina/San_Juan", "Argentina Standard Time" },
        { "America/Asuncion", "Paraguay Standard Time" },
        { "America/Bahia", "Bahia Standard Time" },
        { "America/Bogota", "SA Pacific Standard Time" },
        { "America/Buenos_Aires", "Argentina Standard Time" },
        { "America/Caracas", "Venezuela Standard Time" },
        { "America/Cayenne", "SA Eastern Standard Time" },
        { "America/Chicago", "Central Standard Time" },
        { "America/Chihuahua", "Mountain Standard Time (Mexico)" },
        { "America/Cuiaba", "Central Brazilian Standard Time" },
        { "America/Denver", "Mountain Standard Time" },
        { "America/Fortaleza", "SA Eastern Standard Time" },
        { "America/Godthab", "Greenland Standard Time" },
        { "America/Guatemala", "Central America Standard Time" },
        { "America/Halifax", "Atlantic Standard Time" },
        { "America/Indianapolis", "US Eastern Standard Time" },
        { "America/Indiana/Indianapolis", "US Eastern Standard Time" },
        { "America/La_Paz", "SA Western Standard Time" },
        { "America/Los_Angeles", "Pacific Standard Time" },
        { "America/Mexico_City", "Mexico Standard Time" },
        { "America/Montevideo", "Montevideo Standard Time" },
        { "America/New_York", "Eastern Standard Time" },
        { "America/Noronha", "UTC-02" },
        { "America/Phoenix", "US Mountain Standard Time" },
        { "America/Regina", "Canada Central Standard Time" },
        { "America/Santa_Isabel", "Pacific Standard Time (Mexico)" },
        { "America/Santiago", "Pacific SA Standard Time" },
        { "America/Sao_Paulo", "E. South America Standard Time" },
        { "America/St_Johns", "Newfoundland Standard Time" },
        { "America/Tijuana", "Pacific Standard Time" },
        { "Antarctica/McMurdo", "New Zealand Standard Time" },
        { "Atlantic/South_Georgia", "UTC-02" },
        { "Asia/Almaty", "Central Asia Standard Time" },
        { "Asia/Amman", "Jordan Standard Time" },
        { "Asia/Baghdad", "Arabic Standard Time" },
        { "Asia/Baku", "Azerbaijan Standard Time" },
        { "Asia/Bangkok", "SE Asia Standard Time" },
        { "Asia/Beirut", "Middle East Standard Time" },
        { "Asia/Calcutta", "India Standard Time" },
        { "Asia/Colombo", "Sri Lanka Standard Time" },
        { "Asia/Damascus", "Syria Standard Time" },
        { "Asia/Dhaka", "Bangladesh Standard Time" },
        { "Asia/Dubai", "Arabian Standard Time" },
        { "Asia/Irkutsk", "North Asia East Standard Time" },
        { "Asia/Jerusalem", "Israel Standard Time" },
        { "Asia/Kabul", "Afghanistan Standard Time" },
        { "Asia/Kamchatka", "Kamchatka Standard Time" },
        { "Asia/Karachi", "Pakistan Standard Time" },
        { "Asia/Katmandu", "Nepal Standard Time" },
        { "Asia/Kolkata", "India Standard Time" },
        { "Asia/Krasnoyarsk", "North Asia Standard Time" },
        { "Asia/Kuala_Lumpur", "Singapore Standard Time" },
        { "Asia/Kuwait", "Arab Standard Time" },
        { "Asia/Magadan", "Magadan Standard Time" },
        { "Asia/Muscat", "Arabian Standard Time" },
        { "Asia/Novosibirsk", "N. Central Asia Standard Time" },
        { "Asia/Oral", "West Asia Standard Time" },
        { "Asia/Rangoon", "Myanmar Standard Time" },
        { "Asia/Riyadh", "Arab Standard Time" },
        { "Asia/Seoul", "Korea Standard Time" },
        { "Asia/Shanghai", "China Standard Time" },
        { "Asia/Singapore", "Singapore Standard Time" },
        { "Asia/Taipei", "Taipei Standard Time" },
        { "Asia/Tashkent", "West Asia Standard Time" },
        { "Asia/Tbilisi", "Georgian Standard Time" },
        { "Asia/Tehran", "Iran Standard Time" },
        { "Asia/Tokyo", "Tokyo Standard Time" },
        { "Asia/Ulaanbaatar", "Ulaanbaatar Standard Time" },
        { "Asia/Vladivostok", "Vladivostok Standard Time" },
        { "Asia/Yakutsk", "Yakutsk Standard Time" },
        { "Asia/Yekaterinburg", "Ekaterinburg Standard Time" },
        { "Asia/Yerevan", "Armenian Standard Time" },
        { "Atlantic/Azores", "Azores Standard Time" },
        { "Atlantic/Cape_Verde", "Cape Verde Standard Time" },
        { "Atlantic/Reykjavik", "Greenwich Standard Time" },
        { "Australia/Adelaide", "Cen. Australia Standard Time" },
        { "Australia/Brisbane", "E. Australia Standard Time" },
        { "Australia/Darwin", "AUS Central Standard Time" },
        { "Australia/Hobart", "Tasmania Standard Time" },
        { "Australia/Perth", "W. Australia Standard Time" },
        { "Australia/Sydney", "AUS Eastern Standard Time" },
        { "Etc/GMT", "UTC" },
        { "Etc/GMT+11", "UTC-11" },
        { "Etc/GMT+12", "Dateline Standard Time" },
        { "Etc/GMT+2", "UTC-02" },
        { "Etc/GMT-12", "UTC+12" },
        { "Europe/Amsterdam", "W. Europe Standard Time" },
        { "Europe/Athens", "GTB Standard Time" },
        { "Europe/Belgrade", "Central Europe Standard Time" },
        { "Europe/Berlin", "W. Europe Standard Time" },
        { "Europe/Brussels", "Romance Standard Time" },
        { "Europe/Budapest", "Central Europe Standard Time" },
        { "Europe/Dublin", "GMT Standard Time" },
        { "Europe/Helsinki", "FLE Standard Time" },
        { "Europe/Istanbul", "GTB Standard Time" },
        { "Europe/Kiev", "FLE Standard Time" },
        { "Europe/London", "GMT Standard Time" },
        { "Europe/Minsk", "E. Europe Standard Time" },
        { "Europe/Moscow", "Russian Standard Time" },
        { "Europe/Paris", "Romance Standard Time" },
        { "Europe/Sarajevo", "Central European Standard Time" },
        { "Europe/Warsaw", "Central European Standard Time" },
        { "Indian/Mauritius", "Mauritius Standard Time" },
        { "Pacific/Apia", "Samoa Standard Time" },
        { "Pacific/Auckland", "New Zealand Standard Time" },
        { "Pacific/Fiji", "Fiji Standard Time" },
        { "Pacific/Guadalcanal", "Central Pacific Standard Time" },
        { "Pacific/Guam", "West Pacific Standard Time" },
        { "Pacific/Honolulu", "Hawaiian Standard Time" },
        { "Pacific/Pago_Pago", "UTC-11" },
        { "Pacific/Port_Moresby", "West Pacific Standard Time" },
        { "Pacific/Tongatapu", "Tonga Standard Time" }
    };

            var windowsTimeZoneId = default(string);
            var windowsTimeZone = default(TimeZoneInfo);
            if (olsonWindowsTimes.TryGetValue(olsonTimeZoneId, out windowsTimeZoneId))
            {
                try { windowsTimeZone = TimeZoneInfo.FindSystemTimeZoneById(windowsTimeZoneId); }
                catch (TimeZoneNotFoundException) { }
                catch (InvalidTimeZoneException) { }
            }
            return windowsTimeZone;
        }

        public static DateTime ConvertToApplicationDateTime(DateTime date)
        {
            //DateTime dateTime=_DateTime;
            try
            {
                TimeZoneInfo objTimeZone = TimeZoneInfo.FindSystemTimeZoneById(SessionHelper.DefaultTimeZone);
                return TimeZoneInfo.ConvertTimeFromUtc(date, objTimeZone);
            }
            catch
            {
                return date;
            }
        }

        public static string SaveThumbImage(string MainImageName, string ThumbImageName, int ThumbWidth, int ThumbHeight)
        {

            #region File variables
            string _FileName = string.Empty;
            string _FileNameWTime = string.Empty;
            string _FileExtension = string.Empty;
            string _FileType = string.Empty;


            string _FilePath = string.Empty;
            string _ThumbPath = string.Empty;
            string _FullFilePath = string.Empty;

            string _InvalideFile = string.Empty;
            string FP_folder = string.Empty;
            bool isCustomeSize = false;
            #endregion
            string OriginalFileName = MainImageName;
            _FileName = Path.GetFileNameWithoutExtension(MainImageName);
            _FileExtension = Path.GetExtension(MainImageName).ToLower();
            _FileType = CommonUtility.GetFileType(_FileExtension);
            if (_FileType != null && _FileType.Length > 0)
            {
                #region Thumb file Save
                if (_FileType == FileType.Image.ToString())
                {
                    Image img = Image.FromFile(MainImageName);
                    int[] intarr = CommonUtility.GetImageThumbSize();
                    if (ThumbWidth > 0 && ThumbHeight > 0)
                    {
                        isCustomeSize = true;
                        intarr[0] = ThumbWidth;
                        intarr[1] = ThumbHeight;
                    }

                    Size ThumbSize = new Size(intarr[0], intarr[1]);
                    int imgHeight = ThumbSize.Height;
                    int imgWidth = ThumbSize.Width;
                    if (img.Width < img.Height)
                    {
                        //portrait image  
                        imgHeight = ThumbSize.Height;
                        var imgRatio = (float)imgHeight / (float)img.Height;
                        imgWidth = Convert.ToInt32(img.Height * imgRatio);
                    }
                    else if (img.Height < img.Width)
                    {
                        //landscape image  
                        imgWidth = ThumbSize.Width;
                        var imgRatio = (float)imgWidth / (float)img.Width;
                        imgHeight = Convert.ToInt32(img.Height * imgRatio);
                        if (isCustomeSize)
                        {
                            imgHeight = ThumbHeight;
                        }
                    }
                    Bitmap bmPhoto = new Bitmap(imgWidth, imgHeight);
                    Graphics g = Graphics.FromImage(bmPhoto);
                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                    g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                    g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                    g.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceOver;
                    g.DrawImage(img, 0, 0, imgWidth, imgHeight);
                    bmPhoto.Save(ThumbImageName, System.Drawing.Imaging.ImageFormat.Jpeg);
                    g.Dispose();
                    //thumb.Dispose();
                    img.Dispose();
                    bmPhoto.Dispose();
                }


                #endregion
            }
            return ThumbImageName;
        }

        public static string GetFileType(string Extension)
        {
            string[] ImageExtention = GetImageExtension().Split(',');
            string[] DocumentExtention = GetDocumentExtension().Split(',');
            string[] VideoExtention = GetVideoExtension().Split(',');

            if (ImageExtention.Contains(Extension))
            {
                return FileType.Image.ToString();
            }
            else if (DocumentExtention.Contains(Extension))
            {
                return FileType.Document.ToString();
            }
            else if (VideoExtention.Contains(Extension))
            {
                return FileType.Video.ToString();
            }
            return null;
        }

        public static int[] GetImageThumbSize()
        {
            string[] ThumbSizeArr = GetThumbSize().Split(',');
            int[] ThumbSize = ThumbSizeArr.Select(int.Parse).ToArray();
            return ThumbSize;
        }
        public static string GetThumbSize()
        {

            string value = ConfigurationStaticValues.ThumbSize;
            if (value != null)
                return value;
            return null;
        }

        public static string GetImageExtension()
        {
            string value = ConfigurationStaticValues.ImageExtension;
            return value;
        }
        public static string GetDocumentExtension()
        {
            string value = ConfigurationStaticValues.DocumentExtension;
            return value;
        }
        public static string GetVideoExtension()
        {
            string value = ConfigurationStaticValues.VideoExtension;
            return value;
        }
    }
}