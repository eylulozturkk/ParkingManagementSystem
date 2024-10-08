﻿using ParkingManagementSystem.BL.Dto.Request;
using ParkingManagementSystem.BL.Dto.Response;

namespace ParkingManagementSystem.BL.Interface
{
    public interface IAuditService
    {
        Task<PageListResponse<List<AuditResponse>>> GetAsync(AuditRequest request);
    }
}
