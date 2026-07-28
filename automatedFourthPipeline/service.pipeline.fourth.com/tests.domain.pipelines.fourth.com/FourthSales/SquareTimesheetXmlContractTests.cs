using System;
using System.Collections.Generic;
using System.Linq;
using com.fourth.pipeline.pos.Enum;
using domain.pipeline.fourth.com.Services.Fourth.Timesheets;
using domain.pipeline.fourth.com.Services.Square.SquareToFourthTimesheetsApi;
using NUnit.Framework;
using Shouldly;
using Square;

namespace tests.domain.pipelines.fourth.com.FourthSales
{
    [TestFixture]
    public class SquareTimesheetXmlContractTests
    {
        [Test]
        public void Map_CreatesFourthTimesheetEntries_ForClosedAndOpenSquareTimecards()
        {
            var timecards = new[]
            {
                new Timecard
                {
                    Id = "tc-closed",
                    TeamMemberId = "tm-1",
                    LocationId = "loc-1",
                    StartAt = "2026-04-18T08:00:00Z",
                    EndAt = "2026-04-18T16:30:00Z",
                    Wage = new TimecardWage
                    {
                        Title = "Front of House"
                    }
                },
                new Timecard
                {
                    Id = "tc-open",
                    TeamMemberId = "tm-2",
                    LocationId = "loc-1",
                    StartAt = "2026-04-18T09:15:00Z"
                }
            };
            var employeeMap = new Dictionary<string, string>
            {
                ["tm-1"] = "EMP001"
            };

            var entries = SquareTimecardToFourthTimesheetMapper
                .Map(timecards, "SANDBOX_UNIT", employeeMap)
                .ToList();

            entries.Count.ShouldBe(2);

            var closedEntry = entries[0];
            closedEntry.EmpNo.ShouldBe("EMP001");
            closedEntry.Location.ShouldBe("SANDBOX_UNIT");
            closedEntry.ClockStatus.ShouldBe(TimesheetClockStatus.ClockOut);
            closedEntry.CheckIn.ShouldBe(new DateTime(2026, 4, 18, 8, 0, 0, DateTimeKind.Utc));
            closedEntry.CheckOut.ShouldBe(new DateTime(2026, 4, 18, 16, 30, 0, DateTimeKind.Utc));
            closedEntry.Notes.ShouldContain("SquareTimecard:tc-closed");
            closedEntry.Notes.ShouldContain("Role:Front of House");

            var openEntry = entries[1];
            openEntry.EmpNo.ShouldBe("tm-2");
            openEntry.ClockStatus.ShouldBe(TimesheetClockStatus.ClockIn);
            openEntry.CheckIn.ShouldBe(new DateTime(2026, 4, 18, 9, 15, 0, DateTimeKind.Utc));
            openEntry.CheckOut.ShouldBeNull();
        }

        [Test]
        public void ConvertToTimesheetXML_ProducesFourthRootAttributesAndRecordElements()
        {
            var entries = new[]
            {
                new data.pipeline.fourth.com.Models.FourthTimeSheetEntry
                {
                    EmpNo = "EMP001",
                    Location = "SANDBOX_UNIT",
                    ClockStatus = TimesheetClockStatus.ClockOut,
                    CheckIn = new DateTime(2026, 4, 18, 8, 0, 0, DateTimeKind.Utc),
                    CheckOut = new DateTime(2026, 4, 18, 16, 30, 0, DateTimeKind.Utc),
                    Notes = "SquareTimecard:tc-closed"
                }
            };

            var xml = TimesheetsService.ConvertToTimesheetXML(
                entries,
                new DateTime(2026, 4, 18, 19, 4, 5),
                "group-1");

            xml.DocumentElement.Name.ShouldBe("Root");
            xml.DocumentElement.GetAttribute("GroupGUID").ShouldBe("group-1");
            xml.DocumentElement.GetAttribute("DateTime").ShouldBe("2026-04-18T19:04:05");
            xml.SelectSingleNode("/Root/Record/EmpNo")?.InnerText.ShouldBe("EMP001");
            xml.SelectSingleNode("/Root/Record/Location")?.InnerText.ShouldBe("SANDBOX_UNIT");
            xml.SelectSingleNode("/Root/Record/ClockStatus")?.InnerText.ShouldBe("ClockOut");
            xml.SelectSingleNode("/Root/Record/Notes")?.InnerText.ShouldBe("SquareTimecard:tc-closed");
        }
    }
}
