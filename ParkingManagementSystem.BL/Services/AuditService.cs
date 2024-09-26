using Microsoft.EntityFrameworkCore;
using ParkingManagementSystem.BL.Dto.Request;
using ParkingManagementSystem.BL.Dto.Response;
using ParkingManagementSystem.BL.Interface;
using ParkingManagementSystem.DAL.Entity;
using ParkingManagementSystem.DAL.UOW;

namespace ParkingManagementSystem.BL.Services
{
    public class AuditService : IAuditService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AuditService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<PageListResponse<List<AuditResponse>>> GetAsync(AuditRequest request)
        {
            var auditRepository = _unitOfWork.GetRepository<Audit>();
            var searchQuery = auditRepository.GetAll();

            if (request.EntityId > 0)
            {
                searchQuery = searchQuery.Where(a => a.EntityId == request.EntityId);
            }

            if (!string.IsNullOrEmpty(request.TableName))
            {
                searchQuery = searchQuery.Where(a => a.TableName.Contains(request.TableName));
            }

            if (request.EntityState != null)
            {
                searchQuery = searchQuery.Where(a => a.EntityState == request.EntityState.ToString());
            }

            if (request.StartCreatedDate != null)
            {
                searchQuery = searchQuery.Where(a => a.CreatedAt >= request.StartCreatedDate);
            }

            if (request.EndCreatedDate != null)
            {
                searchQuery = searchQuery.Where(a => a.CreatedAt <= request.EndCreatedDate);
            }

            var totalCount = await searchQuery.CountAsync();

            searchQuery = searchQuery.Skip((request.Index - 1) * request.Size).Take(request.Size);

            var roles = await searchQuery
                              .Select(a => new AuditResponse
                              {
                                  EntityId = a.EntityId,
                                  TableName = a.TableName,
                                  OldValues = a.OldValues,
                                  NewValues = a.NewValues,
                                  EntityState = a.EntityState,
                                  CreatedAt = a.CreatedAt
                              }).AsNoTracking().ToListAsync();

            var result = new PageListResponse<List<AuditResponse>>
            {
                Data = roles,
                Index = request.Index,
                Size = request.Size,
                TotalCount = totalCount,
            };

            return result;
        }
    }
}
