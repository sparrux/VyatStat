using Ardalis.Result;
using Hub.Application.Abstractions;
using Hub.Application.Features.Common.Contracts;
using Hub.Application.Features.Payments.Contracts;
using Hub.Application.Pipelines;
using Hub.Domain.Payments;
using Microsoft.EntityFrameworkCore;

namespace Hub.Application.Features.Payments.Queries.GetDonations;

sealed class GetDonationsQueryHandler(
    IPaymentsDbContext paymentsDbContext
) : IRequestHandler<GetDonationsQuery, ListResponse<DonationResponse>>
{
    public async Task<Result<ListResponse<DonationResponse>>> Handle(
        GetDonationsQuery query, CancellationToken cancellationToken)
    {
        var donationsQuery = paymentsDbContext.Donations
            .Where(x => x.CustomerId == query.UserId)
            .OrderByDescending(x => x.CreatedAt);
        
        var donations = await donationsQuery
            .Skip(query.Skip)
            .Take(query.Take)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var donationsMap = donations.ToDictionary(x => x.Id, x => x);
        
        var donationIds = donations.Select(x => x.Id).ToList();
        
        var payments = await paymentsDbContext.Payments
            .Include(x => x.Attempts.OrderByDescending(a => a.AttemptNumber))
            .Where(payment => payment.Purpose == PaymentPurpose.Donation && donationIds.Contains(payment.ReferenceId))
            .OrderByDescending(x => x.CreatedAt)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
        
        var paymentsMap = payments.ToDictionary(x => x.Id, x => x);

        List<DonationResponse> responses = [];
        foreach (var donationId in donationIds)
        {
            var donation = donationsMap[donationId];
            
            if (donation.PaymentId is null)
                continue;

            paymentsMap.TryGetValue(donation.PaymentId.Value, out var payment);
            
            responses.Add(DonationResponse.From(donation, payment));
        }

        return new ListResponse<DonationResponse>(
            responses,
            await donationsQuery.CountAsync(cancellationToken));
    }
}