
using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;

namespace Negin.Framework.Utilities;

public static class DateTimeExtension
{
	public static DateTime ShamsiToMiladi(this DateTime dateTime)
	{
		DateTime result;
		DateTime.TryParse(dateTime.ToString(), new CultureInfo("fa-IR"), out result);
		return result;
	}

	public static Nullable<DateTime> MiladiToShamsi(this DateTime dateTime)
	{
		if (dateTime.Year < 623)
		{
			return null;
		}
        PersianCalendar pc = new PersianCalendar();
		int year = pc.GetYear(dateTime);
		int month = pc.GetMonth(dateTime);
		int day = pc.GetDayOfMonth(dateTime);
		int hour = pc.GetHour(dateTime);
		int minute = pc.GetMinute(dateTime);
        return new DateTime(year, month, day, hour, minute, 0); ;
	}

	public static string MiladiToShamsiDateString(this DateTime dateTime)
	{
		var shamsi = MiladiToShamsi(dateTime);
		return $"{shamsi?.Year}/{shamsi?.Month}/{shamsi?.Day}";
	}

	public static string ToShamsiDateString(this DateTime? dateTime, System.DayOfWeek? dayOfWeek)
	{
		string result = $"{dayOfWeek?.PersianDayOfWeek()} {dateTime?.Year}/{dateTime?.Month}/{dateTime?.Day} <br/>  ساعت: {dateTime?.Hour}:{dateTime?.Minute}";
		return result;
	}

    public static string ToShortShamsiDateString(this DateTime dateTime)
	{
		string result = $"{dateTime.Year}/{dateTime.Month}/{dateTime.Day}";
		return result;
	}

    public static string PersianDayOfWeek(this DayOfWeek date)
    {
        switch (date)
        {
            case DayOfWeek.Saturday:
                return "شنبه";
            case DayOfWeek.Sunday:
                return "یکشنبه";
            case DayOfWeek.Monday:
                return "دوشنبه";
            case DayOfWeek.Tuesday:
                return "سه شنبه";
            case DayOfWeek.Wednesday:
                return "چهارشنبه";
            case DayOfWeek.Thursday:
                return "پنجشنبه";
            case DayOfWeek.Friday:
                return "جمعه";
            default:
                throw new Exception();
        }
    }
}