using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sundays.Model;
using SundaysApp.Model;

namespace SundaysApp.Services
{
    public interface ISundayService
    {
        Task<IEnumerable<Sunday>> Get(DateTime from, DateTime to);

        bool IsConfigured { get; }
    }
}