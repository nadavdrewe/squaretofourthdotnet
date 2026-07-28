using Square;
using Square.TeamMembers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace square.pipeline.fourth.com.Services
{
    public class EmployeesService : BaseService
    {
        public EmployeesService(string apiToken, string baseUrl = null) : base(apiToken, baseUrl)
        {
        }

        public async Task<IEnumerable<TeamMember>> GetEmployees()
        {
            var toReturn = new List<TeamMember>();
            var searchRequest = new SearchTeamMembersRequest
            {
                Limit = 100
            };

            var teamMembersResponse = await _client.TeamMembers.SearchAsync(searchRequest);

            if (teamMembersResponse.TeamMembers != null)
            {
                toReturn.AddRange(teamMembersResponse.TeamMembers);
                //get more
                var currentCursor = teamMembersResponse.Cursor;
                while (!String.IsNullOrWhiteSpace(currentCursor))
                {
                    var subsequentResult = await _client.TeamMembers.SearchAsync(
                        new SearchTeamMembersRequest
                        {
                            Cursor = currentCursor,
                            Limit = 100
                        });
                    if (subsequentResult.TeamMembers != null)
                        toReturn.AddRange(subsequentResult.TeamMembers);
                    currentCursor = subsequentResult.Cursor;
                }
            }

            return toReturn;
        }
    }
}
