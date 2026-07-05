using FluentResults;
using Tracker.Application.Contracts.GroupMembers.Responses;
using Tracker.Application.Contracts.Groups.Requests;
using Tracker.Application.Contracts.Groups.Responses;

namespace Tracker.Application.Interfaces.Groups;

public interface IGroupsService
{
    Task<Result<GroupsListResponse>> GetListAsync(int offset, int take, CancellationToken ctk = default);
    Task<Result<GroupMembersListResponse>> GetMembersListAsync(Guid groupId, int offset, int take, CancellationToken ctk = default);
    Task<Result<GroupSummaryResponse>> CreateAsync(Guid userId, CreateGroupRequest request, CancellationToken ctk = default);
    Task<Result<GroupMemberSummaryResponse>> JoinAsync(Guid userId, Guid groupId, CancellationToken ctk = default);
    Task<Result> LeftAsync(Guid userId, Guid groupId, CancellationToken ctk = default);
    Task<Result> UpdateAsync(Guid groupId, UpdateGroupRequest request, CancellationToken ctk = default);
}