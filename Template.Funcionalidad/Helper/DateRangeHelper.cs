namespace Template.Funcionalidad.Helper
{
    public static class DateRangeHelper
    {
        private const int MaxDays = 365;

        /// <summary>
        /// Validate that date range is less that year
        /// </summary>
        /// <param name="fromDate">date to validate</param>
        /// <param name="toDate">date to validate</param>
        /// <returns></returns>
        public static bool DateRangeIsYear(DateTime fromDate, DateTime toDate)
        {
            //convert fromDate to date time
            var startDate = DateTime.Parse(fromDate.ToShortDateString());
            //convert toDate to date time
            var endDate = DateTime.Parse(toDate.ToShortDateString());
            //The date difference of the range is obtained 
            var dateDifference = endDate - startDate;
            //Get total days of dateDifference
            var days = (int)dateDifference.TotalDays;
            //If the total number of days is less than or equal to the number of days in a year
            if (days <= MaxDays)
                // return true
                return true;
            else
                //return false
                return false;
        }

        /// <summary>
        /// Validate that date range is valid
        /// </summary>
        /// <param name="fromDate">date to validate</param>
        /// <param name="toDate">date to validate</param>
        /// <returns></returns>
        public static bool DateRangeIsValid(DateTime fromDate, DateTime toDate)
        {
            //if toDate is less than fromDate
            if (toDate < fromDate)
                //return false
                return false;
            else
                //return true
                return true;
        }

        /// <summary>
        /// Valid if two dates are the same
        /// </summary>
        /// <param name="frontTimeStamp">First time stamp to validate</param>
        /// <param name="backEndTimeStamp2">Second time stamp to validate</param>
        /// <returns>True if both dates are the same, else false</returns>
        public static bool IsTheSameTimeStamp(
            DateTime frontTimeStamp,
            DateTime backEndTimeStamp2
        )
        {
            if (frontTimeStamp == backEndTimeStamp2)
                return true;
            else
                return false;
        }
    }
}