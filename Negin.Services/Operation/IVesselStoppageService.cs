﻿using Microsoft.AspNetCore.Http;
using Negin.Core.Domain.Aggregates.Basic;
using Negin.Core.Domain.Aggregates.Operation;
using Negin.Framework.Exceptions;
using Negin.Framework.Pagination;

namespace Negin.Services.Operation;

public interface IVesselStoppageService
{
    BLMessage AddVesselStoppage(VesselStoppage v, IFormCollection formCollection);
    BLMessage UpdateVesselStoppage(VesselStoppage v, IFormCollection formCollection);
}