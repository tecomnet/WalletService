namespace Wallet.Funcionalidad.Helper
{
    public static class DateRangeHelper
    {
        private const int MaxDays = 365; // Número máximo de días para considerar un rango como "un año".

        /// <summary>
        /// Valida que el rango de fechas sea menor o igual a un año.
        /// </summary>
        /// <param name="fromDate">Fecha de inicio del rango a validar.</param>
        /// <param name="toDate">Fecha de fin del rango a validar.</param>
        /// <returns>Verdadero si el rango de fechas es menor o igual a un año, falso en caso contrario.</returns>
        public static bool DateRangeIsYear(DateTime fromDate, DateTime toDate)
        {
            // Normaliza las fechas a medianoche para asegurar que la diferencia se calcule solo en días completos,
            // ignorando la parte de la hora.
            var startDate = fromDate.Date;
            var endDate = toDate.Date;

            // Calcula la diferencia entre las fechas.
            var dateDifference = endDate - startDate;

            // Obtiene el número total de días de la diferencia.
            var days = (int)dateDifference.TotalDays;

            // Retorna verdadero si el número total de días es menor o igual al máximo permitido (un año).
            return days <= MaxDays;
        }

        /// <summary>
        /// Valida que la fecha de fin no sea anterior a la fecha de inicio.
        /// </summary>
        /// <param name="fromDate">Fecha de inicio del rango a validar.</param>
        /// <param name="toDate">Fecha de fin del rango a validar.</param>
        /// <returns>Verdadero si la fecha de fin es igual o posterior a la fecha de inicio, falso en caso contrario.</returns>
        public static bool DateRangeIsValid(DateTime fromDate, DateTime toDate)
        {
            // Retorna verdadero si la fecha de fin es mayor o igual que la fecha de inicio.
            return toDate >= fromDate;
        }

        /// <summary>
        /// Valida si dos fechas y horas son exactamente iguales.
        /// </summary>
        /// <param name="frontTimeStamp">Primera marca de tiempo a comparar.</param>
        /// <param name="backEndTimeStamp2">Segunda marca de tiempo a comparar.</param>
        /// <returns>Verdadero si ambas marcas de tiempo son idénticas, falso en caso contrario.</returns>
        public static bool IsTheSameTimeStamp(
            DateTime frontTimeStamp,
            DateTime backEndTimeStamp2
        )
        {
            // Retorna verdadero si ambas fechas y horas son iguales.
            return frontTimeStamp == backEndTimeStamp2;
        }
    }
}