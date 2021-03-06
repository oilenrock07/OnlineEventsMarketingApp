﻿using System;
using System.Collections.Generic;
using System.Linq;
using OnlineEventsMarketingApp.Entities;
using OnlineEventsMarketingApp.Entities.Users;
using OnlineEventsMarketingApp.Infrastructure.Interfaces;
using OnlineEventsMarketingApp.Services.DataTransferObjects;
using OnlineEventsMarketingApp.Services.Interfaces;

namespace OnlineEventsMarketingApp.Services.Implementations
{
    public class TagService : BaseRepository, ITagService
    {
        private readonly IRepository<Tag> _tagRepository;

        public TagService(IDatabaseFactory databaseFactory, IRepository<Tag> tagRepository)
            : base(databaseFactory)
        {
            _tagRepository = tagRepository;
        }

        public IEnumerable<Tag> GetTags(int year, int month)
        {
            return _tagRepository.Find(x => x.Month == month && x.Year == year && !x.IsDeleted).ToList();
        }

        public IEnumerable<Tag> GetTags(IEnumerable<DateTime> dates)
        {
            var months = dates.Select(x => x.Month).Distinct();
            var years = dates.Select(x => x.Year).Distinct();
            return _tagRepository.Find(x => !x.IsDeleted && months.Contains(x.Month) && years.Contains(x.Year)).ToList();
        }

        public bool HasTags(int year, int month)
        {
            return _tagRepository.Find(x => !x.IsDeleted && x.Year == year && x.Month == month).Any();
        }
    }
}
