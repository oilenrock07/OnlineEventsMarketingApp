
using System.Collections.Generic;
using System.Linq;

namespace OnlineEventsMarketingApp.Services.DataTransferObjects
{
    public class WeeklyReportDTO
    {

        public int DIS { get; set; }

        public int TE { get; set; }

        public string TM { get; set; }

        public string InHouse { get; set; }

        public IEnumerable<TagReportDTO> TagReport { get; set; }

        public int Total
        {
            get
            {
                return TagReport != null ? TagReport.Sum(x => x.TagCounts) : 0;
            }
        }

        public int Variance
        {
            get { return Total - 4; }
        }

        public int GetTagCount(int tagId)
        {
            if (TagReport != null)
            {
                var tag = TagReport.FirstOrDefault(x => x.TagId == tagId);
                if (tag != null)
                    return tag.TagCounts;
            }

            return 0;
        }
    }
}
