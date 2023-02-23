using AutoMapper;
using Common.SharedAppInterfaces;
using Common.SharedAppInterfaces.Repository;
using CustomerService.Common.Models.DTO;
using CustomerService.Common.Repository.Interfaces;
using CustomerService.EF;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomerService.Common.Repository.Implementation
{
    public class SubscriberRepository : BaseRepository, ISubscriberRepository
    {
        private readonly CustomerDbContext _context;
        private readonly IMapper _mapper;

        public SubscriberRepository(CustomerDbContext context, IMapper mapper, IWrappedLogger logger) : base(logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<Subscriber> GetAsync(Guid subscriptionId)
        {
            return await InvokeTrackedDependencyAsync("SQL", "Customer Database", "Get subscribtion by subscriptionId", $"SubscriptionId: {subscriptionId}", null,
                async () =>
                {
                    return await _context.Subscriber.SingleOrDefaultAsync(s => s.SubscriptionId == subscriptionId && s.IsActive);
                });
        }

        public async Task<IList<Subscriber>> GetAsync(string customerCode)
        {
            return await InvokeTrackedDependencyAsync("SQL", "Customer Database", "Get subscribtion by customerCode", $"CustomerCode: {customerCode}", null,
                async () =>
                {

                    var subscribers = await _context.Subscriber.Include(s => s.SubscriptionFilters)
                                                              .Where(s => s.CustomerCode == customerCode && s.IsActive)
                                                              .ToListAsync();
                    return subscribers;
                });
        }

        /// <summary>
        /// TODO: Added to allow testing using memory db. Should be updated for final service use case
        /// </summary>
        /// <param name="subscriber"></param>
        /// <returns></returns>
        public async Task<int> UpsertAsync(SubscriberDTO subscriberDto)
        {

            return await InvokeTrackedDependencyAsync("SQL", "Customer Database", "Add subscribtion", $"CustomerCode: {subscriberDto.CustomerCode}", null,
                async () =>
                {
                    var subscriber = _mapper.Map<Subscriber>(subscriberDto);

                    //TODO: using string conversion has a negative effect on performance
                    var existingSubscriber = await _context.Subscriber.SingleOrDefaultAsync(s =>s.CustomerCode == subscriber.CustomerCode);
                    if (existingSubscriber != null)
                    {
                        //maybe set some other properties ?
                        existingSubscriber.IsActive = subscriberDto.IsActive;
                    }
                    else
                    {
                        await _context.Subscriber.AddAsync(subscriber);
                    }

                    return await _context.SaveChangesAsync();
                });

        }
    }
}