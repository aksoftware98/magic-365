using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Magic365.Core.Models;

namespace Magic365.Core.Interfaces;
public interface IPlansStorageService
{

    Task<IEnumerable<PlanMetadata>> ListPlansAsync(string userId);

    Task SavePlanAsync(PlanMetadata plan);

}
