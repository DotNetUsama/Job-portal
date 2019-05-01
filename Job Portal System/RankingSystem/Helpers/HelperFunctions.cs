using System;

namespace Job_Portal_System.RankingSystem.Helpers
{
    public class HelperFunctions
    {
        public static int GetYears(DateTime from, DateTime to)
        {
            var years = to.Year - from.Year;
            if (from > to.AddYears(-years)) years--;
            return years;
        }
    }
}
