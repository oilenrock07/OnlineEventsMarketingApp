using OnlineEventsMarketingApp.Common.Extensions;

namespace OnlineEventsMarketingApp.Models.Reports
{
    public class MonthlyReportData
    {
        public string Inhouse { get; set; }
        public string Month { get; set; }
        public int NoOfRuns { get; set; }
        public int ConsultationTGT
        {
            get { return NoOfRuns * 20; }
        }

        public int ConsultationACT{ get; set; }

        public string ConsultationACTVsTGT
        {
            get { return (ConsultationTGT != 0 ? (decimal)ConsultationACT/ConsultationTGT : 0).FormatDecimal(); }
        }

        public int NUTGT
        {
            get { return NoOfRuns*2; }
        }

        public int NUACT { get; set; }

        public string NUACTVsTGT
        {
            get { return (NUTGT != 0 ? (decimal)NUACT / NUTGT : 0).FormatDecimal(); }
        }
    }
}