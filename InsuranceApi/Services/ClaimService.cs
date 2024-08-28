using InsuranceApi.Data;
using InsuranceApi.DTOs;
using Microsoft.EntityFrameworkCore;

namespace InsuranceApi.Services
{
    public interface IClaimService
    {
        Task Add(ClaimDto claimDto);
        Task Delete(int id);
        Task<List<ClaimDto>> GetAll();
        Task<ClaimDto> GetById(int id);
        Task Update(ClaimDto claimDto);
        Task<int> GetClaimCount();
        Task UpdateStatus(int id, string status, decimal dispenseAmount);
    }

    public class ClaimService : IClaimService
    {
        private readonly FnfProjectContext context;

        public ClaimService(FnfProjectContext context)
        {
            this.context = context;
        }

        public async Task<List<ClaimDto>> GetAll()
        {
            var claims = await context.Claims.AsNoTracking().ToListAsync();
            var claimDtos = claims.Select(claim => ConvertToDto(claim)).ToList();
            return claimDtos;
        }

        public async Task<ClaimDto> GetById(int id)
        {
            var found = await context.Claims.AsNoTracking().FirstOrDefaultAsync(claim => claim.ClaimId == id);

            if (found != null)
                return ConvertToDto(found);

            throw new NullReferenceException($"Claim with ID {id} not found.");
        }

        public async Task Add(ClaimDto claimDto)
        {
            Claim claimTable = new();
            ConvertToTable(claimDto, claimTable);
            context.Claims.Add(claimTable);
            await context.SaveChangesAsync();
        }

        public async Task Update(ClaimDto claimDto)
        {
            var found = await context.Claims.FirstOrDefaultAsync(claim => claim.ClaimId == claimDto.ClaimId);

            if (found != null)
            {
                ConvertToTable(claimDto, found);
                await context.SaveChangesAsync();
                return;
            }

            throw new NullReferenceException($"Claim with ID {claimDto.ClaimId} not found.");
        }

        public async Task Delete(int id)
        {
            var found = await context.Claims.FirstOrDefaultAsync(claim => claim.ClaimId == id);
            if (found != null)
            {
                context.Claims.Remove(found);
                await context.SaveChangesAsync();
                return;
            }
            throw new NullReferenceException($"Claim with ID {id} not found.");
        }
        public async Task<int> GetClaimCount()
        {
            return await context.Claims.CountAsync();
        }

        public async Task UpdateStatus(int id, string status, decimal dispenseAmount)
        {
            var found = await context.Claims.FirstOrDefaultAsync(claim => claim.ClaimId == id);

            if (found != null)
            {
                found.ClaimStatus = status;
                found.DispenseAmount = dispenseAmount;
                await context.SaveChangesAsync();
                return;
            }

            throw new NullReferenceException($"Claim with ID {id} not found.");
        }

        private ClaimDto ConvertToDto(Claim claimTable)
        {
            return new ClaimDto
            {
                ClaimId = claimTable.ClaimId,
                PolicyHolderId = claimTable.PolicyHolderId,
                InsuredPolicyId = claimTable.InsuredPolicyId,
                ClaimAmount = claimTable.ClaimAmount,
                ClaimStatus = claimTable.ClaimStatus,
                DispenseAmount = claimTable.DispenseAmount,
                DocumentPath = claimTable.DocumentPath,
                DocumentType = claimTable.DocumentType,
                HospitalId = claimTable.HospitalId,
                ClaimDate = DateTime.Parse(claimTable.ClaimDate.ToString()),
            };
        }

        private void ConvertToTable(ClaimDto claimDto, Claim claimTable)
        {
            claimTable.ClaimId = claimDto.ClaimId;
            claimTable.PolicyHolderId = claimDto.PolicyHolderId;
            claimTable.InsuredPolicyId = claimDto.InsuredPolicyId;
            claimTable.ClaimAmount = claimDto.ClaimAmount;
            claimTable.ClaimStatus = claimDto.ClaimStatus;
            claimTable.DispenseAmount = claimDto.DispenseAmount;
            claimTable.DocumentPath = claimDto.DocumentPath;
            claimTable.DocumentType = claimDto.DocumentType;
            claimTable.ClaimDate = DateOnly.FromDateTime(claimDto.ClaimDate);
            claimTable.HospitalId = claimDto.HospitalId;
        }
    }
}