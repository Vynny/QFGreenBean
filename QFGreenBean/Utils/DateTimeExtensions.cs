using System;

namespace QFGreenBean.Utils
{
    public  static class DateTimeExtensions
    {
        // Convert old DATETIME object to equivalent Day and Time of the week today
        public static DateTime EqualTodayWeekDayTime(this DateTime oldDate)
        {
            int num1 = (int)oldDate.DayOfWeek;
            int num2 = (int)DateTime.Today.DayOfWeek;
            DateTime result = DateTime.Today.AddDays(num1 - num2);

            return (result + oldDate.TimeOfDay);
        }
    }

    // Code Example:
    // DateTime dtOld = new DateTime(2015, 09, 11, 11, 30, 00);
    // DateTime result = dtOld.EqualTodayWeekDayTime();
    // Console.WriteLine(result.ToString());
    // Console.WriteLine(result.ToString("ddd"));
}
