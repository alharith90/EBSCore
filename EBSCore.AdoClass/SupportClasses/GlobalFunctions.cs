using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EBSCore.AdoClass
{

    public class GlobalFunctions
    {

        public string FormatDBDate(string SelectedDate)
        {
            if (SelectedDate.Contains('T') || SelectedDate.Contains('-'))
            {
                return DateTime.Parse(SelectedDate).ToString("dd/MM/yyyy HH:mm");
            }
            if (SelectedDate == null)
            {
                return "";

            }

            if (SelectedDate.ToString() == "" | SelectedDate.ToString() == "null")
            {
                return "";

            }

            string[] dateTimeArray = SelectedDate.Split(' ');

            string[] dateArray;

            if ((dateTimeArray[0].ToString().Contains(":") == true))
                dateArray = dateTimeArray[1].Split('/');
            else
                dateArray = dateTimeArray[0].Split('/');



            int nDay =int.Parse( dateArray[0]);
            int nMonth = int.Parse(dateArray[1]);
            int nYear = int.Parse(dateArray[2]);
            string[] timeArray;
            int hour = 0;
            int minute = 0;
            int second = 0;


            if (dateTimeArray.Length >= 2)
            {
                if ((dateTimeArray[0].ToString().Contains(":") == true))
                    timeArray = dateTimeArray[0].Split(':');
                else
                    timeArray = dateTimeArray[1].Split(':');

                hour = int.Parse( timeArray[0]);
                minute = int.Parse(timeArray[1]);
                if (timeArray.Length > 2)
                    second = int.Parse(timeArray[2]);
                else
                    second = int.Parse("00");
            }


            string[] timePMArray;
            if (dateTimeArray.Length >= 3)
            {
                timePMArray = dateTimeArray[2].Split(' ');
                if ((timePMArray[0] == "ã" | timePMArray[0] == "pm" | timePMArray[0] == "PM") & hour != 12)
                    hour = hour + 12;
                if ((timePMArray[0] == "ã" | timePMArray[0] == "am" | timePMArray[0] == "AM") & hour == 12)
                    hour = 0;
            }

            return nYear.ToString() + "-" + nMonth.ToString("00") + "-" + nDay.ToString("00") + " " + hour.ToString("00") + ":" + minute.ToString("00") + ":" + second.ToString("00") + ":000";
        }
        public string getDbFormat(object SelectedDate)
        {
            if (SelectedDate == null)
            {
                return "";
         
            }

            if (SelectedDate.ToString() == "" | SelectedDate.ToString() == "null")
            {
                return "";
              
            }


            DateTime _Date = new DateTime();
            _Date = DateTime.Parse(SelectedDate.ToString());
            return _Date.ToString("dd/MM/yyyy HH:mm");
        }
    }

}
