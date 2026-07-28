using data.pipeline.fourth.com.Models;
using domain.pipeline.fourth.com.Services.Fourth.Timesheets;
using square.pipeline.fourth.com.Services;
using Square;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;

namespace domain.pipeline.fourth.com.Services.Square.SquareToFourthTimesheetsApi
{
    public class SquareToFourthTimesheetXmlGenerator
    {
        private readonly LaborService _laborService;
        private readonly List<Timecard> _timecards = new List<Timecard>();

        public SquareToFourthTimesheetXmlGenerator(string squareToken, string squareBaseUrl = null)
            : this(new LaborService(squareToken, squareBaseUrl))
        {
        }

        public SquareToFourthTimesheetXmlGenerator(LaborService laborService)
        {
            _laborService = laborService;
        }

        public IReadOnlyCollection<Timecard> Timecards => _timecards;

        public async Task<IReadOnlyCollection<Timecard>> GatherDataForLocation(
            DateTime startTimeUtc,
            DateTime endTimeUtc,
            Location location)
        {
            var locationTimecards = await _laborService.GetTimecardsForLocationByDateTimeUTC(
                location.Id,
                startTimeUtc,
                endTimeUtc);

            _timecards.AddRange(locationTimecards ?? Enumerable.Empty<Timecard>());
            return _timecards;
        }

        public IReadOnlyCollection<FourthTimeSheetEntry> CreateTimesheetEntries(
            string locationCode,
            IReadOnlyDictionary<string, string> employeeNumberByTeamMemberId = null,
            IEnumerable<string> timecardIds = null)
        {
            var requestedIds = timecardIds == null
                ? null
                : new HashSet<string>(timecardIds);
            var timecards = requestedIds == null
                ? _timecards
                : _timecards.Where(x => requestedIds.Contains(x.Id)).ToList();

            return SquareTimecardToFourthTimesheetMapper
                .Map(timecards, locationCode, employeeNumberByTeamMemberId)
                .ToList();
        }

        public XmlDocument CreateTimesheetXml(
            string locationCode,
            DateTime timesheetDateTime,
            string groupGuid,
            IReadOnlyDictionary<string, string> employeeNumberByTeamMemberId = null,
            IEnumerable<string> timecardIds = null)
        {
            var entries = CreateTimesheetEntries(locationCode, employeeNumberByTeamMemberId, timecardIds);
            return TimesheetsService.ConvertToTimesheetXML(entries, timesheetDateTime, groupGuid);
        }
    }
}
