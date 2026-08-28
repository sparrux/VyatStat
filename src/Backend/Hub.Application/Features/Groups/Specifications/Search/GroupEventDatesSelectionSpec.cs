using Ardalis.Specification;
using Hub.Domain.Groups;

namespace Hub.Application.Features.Groups.Specifications.Search;

sealed class GroupEventDatesSelectionSpec : Specification<GroupEvent>
{
    public GroupEventDatesSelectionSpec(DateTimeOffset fromDate, DateTimeOffset toDate)
    {
        Query.Where(x => 
            x.Event.DatesRange.StartDate >= fromDate 
            && x.Event.DatesRange.EndDate <= toDate);
    }
}