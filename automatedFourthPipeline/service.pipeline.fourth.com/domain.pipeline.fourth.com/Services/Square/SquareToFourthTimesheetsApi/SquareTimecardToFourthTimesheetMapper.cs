using com.fourth.pipeline.pos.Enum;
using data.pipeline.fourth.com.Models;
using Square;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace domain.pipeline.fourth.com.Services.Square.SquareToFourthTimesheetsApi
{
    public static class SquareTimecardToFourthTimesheetMapper
    {
        public static IEnumerable<FourthTimeSheetEntry> Map(
            IEnumerable<Timecard> timecards,
            string locationCode,
            IReadOnlyDictionary<string, string> employeeNumberByTeamMemberId = null)
        {
            if (timecards == null)
            {
                return Enumerable.Empty<FourthTimeSheetEntry>();
            }

            return timecards
                .Where(x => x != null && !string.IsNullOrWhiteSpace(x.TeamMemberId) && !string.IsNullOrWhiteSpace(x.StartAt))
                .Select(x => Map(x, locationCode, employeeNumberByTeamMemberId))
                .Where(x => x != null)
                .ToList();
        }

        public static FourthTimeSheetEntry Map(
            Timecard timecard,
            string locationCode,
            IReadOnlyDictionary<string, string> employeeNumberByTeamMemberId = null)
        {
            if (timecard == null || string.IsNullOrWhiteSpace(timecard.TeamMemberId) || string.IsNullOrWhiteSpace(timecard.StartAt))
            {
                return null;
            }

            var checkIn = ParseSquareDateTime(timecard.StartAt);
            var checkOut = string.IsNullOrWhiteSpace(timecard.EndAt)
                ? null
                : ParseSquareDateTime(timecard.EndAt);

            return new FourthTimeSheetEntry
            {
                EmpNo = GetEmployeeNumber(timecard.TeamMemberId, employeeNumberByTeamMemberId),
                Location = locationCode,
                ClockStatus = checkOut.HasValue ? TimesheetClockStatus.ClockOut : TimesheetClockStatus.ClockIn,
                CheckIn = checkIn,
                CheckOut = checkOut,
                Notes = CreateNotes(timecard)
            };
        }

        private static string GetEmployeeNumber(
            string teamMemberId,
            IReadOnlyDictionary<string, string> employeeNumberByTeamMemberId)
        {
            if (employeeNumberByTeamMemberId != null
                && employeeNumberByTeamMemberId.TryGetValue(teamMemberId, out var employeeNumber)
                && !string.IsNullOrWhiteSpace(employeeNumber))
            {
                return employeeNumber;
            }

            return teamMemberId;
        }

        private static DateTime? ParseSquareDateTime(string value)
        {
            if (DateTimeOffset.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out var parsed))
            {
                return parsed.UtcDateTime;
            }

            return null;
        }

        private static string CreateNotes(Timecard timecard)
        {
            var parts = new List<string>();
            if (!string.IsNullOrWhiteSpace(timecard.Id))
            {
                parts.Add($"SquareTimecard:{timecard.Id}");
            }

            if (!string.IsNullOrWhiteSpace(timecard.Wage?.Title))
            {
                parts.Add($"Role:{timecard.Wage.Title}");
            }

            return string.Join(" ", parts);
        }
    }
}
