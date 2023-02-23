using AutoMapper;
using CustomerService.Common.Models.DTO;
using CustomerService.Common.Models.Enum;
using CustomerService.Common.Repository.Interfaces;
using CustomerService.Common.Services.Interfaces;
using CustomerService.EF;
using Kafka.SDKs.MessageFormatting;
using MilestoneService.DTO.API.V1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomerService.Common.Services.Implementation
{
    public class SubscriberService : ISubscriberService
    {
        private readonly ISubscriberRepository _repository;
        private readonly IMapper _mapper;

        private enum FilterableField
        {
            EventTypeCode,
            TransportMode,
            FacilityTypeCode,
            EventClassifierCode
        }

        public SubscriberService(ISubscriberRepository repository, IMapper mapper)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<IList<SubscriberDTO>> GetSubscriber(string customerCode, Dictionary<string, string> messageHeaders)
        {
            var subscribers = await _repository.GetAsync(customerCode);

            var subscriptionFilter = subscribers?.SelectMany(s => s.SubscriptionFilters)
                                                 .Where(w => w.IsActive && w.FilterSource.HasFlag(FilterSourceEnum.Webhook));

            if (subscriptionFilter != null && subscriptionFilter.Any())
            {
                var filters = _mapper.Map<List<SubscriptionFilterDTO>>(subscriptionFilter);
                var subscriberIds = ApplySubscriptionFiltering(filters, messageHeaders);

                if (subscriberIds.Any())
                {
                    return _mapper.Map<List<SubscriberDTO>>(subscribers.Where(s => subscriberIds.Contains(s.Id)));
                }
            }
            return null;
        }

        public async Task<SubscriberDTO> GetSubscriber(Guid subscriptionId)
        {
            var subscriber = await _repository.GetAsync(subscriptionId);
            return _mapper.Map<SubscriberDTO>(subscriber);
        }

        private IEnumerable<int> ApplySubscriptionFiltering(List<SubscriptionFilterDTO> filters,
                                                                             Dictionary<string, string> messageHeaders)
        {
            List<SubscriptionFilterDTO> subscriptions = new List<SubscriptionFilterDTO>();
            messageHeaders.TryGetValue(FilterableField.EventTypeCode.ToString(), out string eventTypeCode);
            messageHeaders.TryGetValue(FilterableField.TransportMode.ToString(), out string source);
            messageHeaders.TryGetValue(FilterableField.FacilityTypeCode.ToString(), out string facilityTypeCode);
            messageHeaders.TryGetValue(FilterableField.EventClassifierCode.ToString(), out string eventClassifierCode);
            messageHeaders.TryGetValue(KafkaMessageWrapper.MessageTypeHeaderName, out string messageType);
            messageHeaders.TryGetValue(KafkaMessageWrapper.MessageVersionHeaderName, out string messageVersion);

            foreach (var filter in filters)
            {
                var rules = filter.Rules.Any(w => w.Message.MessageType == messageType
                                          && w.Message.MessageVersion == messageVersion
                                          && (string.IsNullOrEmpty(w.EventTypeCode) || w.EventTypeCode == eventTypeCode)
                                          // the source field should only apply when both the rule and the header exist. this allows us to 
                                          // put in the rule before corresponding code changes are made
                                          && (string.IsNullOrEmpty(w.TransportMode) || string.IsNullOrEmpty(source) || w.TransportMode.Equals(source, StringComparison.InvariantCultureIgnoreCase))
                                          && (string.IsNullOrEmpty(w.FacilityTypeCode) || w.FacilityTypeCode == facilityTypeCode)
                                          && (string.IsNullOrEmpty(w.EventClassifierCode) || w.EventClassifierCode == eventClassifierCode));
                if (rules)
                {
                    subscriptions.Add(filter);
                }
            }

            return subscriptions.Select(s => s.SubscriberId);
        }

        public async Task<IEnumerable<OrderMilestoneDTO>> ApplySubscriptionFiltering(IList<OrderMilestoneDTO> milestoneDTOs, string customerCode)
        {
            var subscribers = await _repository.GetAsync(customerCode);

            var subscriptionFilters = subscribers?.SelectMany(s => s.SubscriptionFilters).Where(w => w.IsActive && w.FilterSource.HasFlag(FilterSourceEnum.Api));

            var filters = _mapper.Map<List<SubscriptionFilterDTO>>(subscriptionFilters);

            IEnumerable<OrderMilestoneDTO> filteredMilestones = new List<OrderMilestoneDTO>();
            foreach (var filter in filters.SelectMany(s=> s.Rules))
            {
                var equipmentMilestones = milestoneDTOs.Where(w =>
                                                              w.CustomerCode == customerCode
                                                              && (string.IsNullOrEmpty(filter.Message.MessageType) || filter.Message.MessageType == MessageType.EquipmentMilestone.ToString())
                                                              && w.Equipments.Any(equipment => equipment.EquipmentMilestones.Any(milestone =>
                                                              (string.IsNullOrEmpty(filter.EventTypeCode) || filter.EventTypeCode == milestone.EventTypeCode)
                                                              && (string.IsNullOrEmpty(filter.FacilityTypeCode) || filter.EventTypeCode == milestone.FacilityTypeCode)
                                                              && (string.IsNullOrEmpty(filter.EventClassifierCode) || filter.EventClassifierCode == milestone.EventClassifierCode))));

                var transportMilestones = milestoneDTOs.Where(w =>
                                                              w.CustomerCode == customerCode
                                                              && (string.IsNullOrEmpty(filter.Message.MessageType) || filter.Message.MessageType == MessageType.TransportMilestone.ToString())
                                                              && w.Bookings.Any(booking => booking.TransportMilestones.Any(milestone =>
                                                              (string.IsNullOrEmpty(filter.EventTypeCode) || filter.EventTypeCode == milestone.EventTypeCode)
                                                              && (string.IsNullOrEmpty(filter.FacilityTypeCode) || filter.EventTypeCode == milestone.FacilityTypeCode)
                                                              && (string.IsNullOrEmpty(filter.EventClassifierCode) || filter.EventClassifierCode == milestone.EventClassifierCode))));

                filteredMilestones = filteredMilestones.Union(equipmentMilestones).Union(transportMilestones);
            }
            return filteredMilestones;
        }
    }
}