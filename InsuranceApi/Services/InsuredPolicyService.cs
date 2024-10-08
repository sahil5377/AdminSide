﻿using InsuranceApi.Data;
using InsuranceApi.DTOs;
using Microsoft.EntityFrameworkCore;

namespace InsuranceApi.Services
{
    public interface IInsuredPolicyService
    {
        Task Add(InsuredPolicyDto insuredPolicyDto);
        Task Delete(int id);
        Task<List<InsuredPolicyDto>> GetAll();
        Task<InsuredPolicyDto> GetById(int id);
        Task<int> GetPolicyCount();
        Task Update(InsuredPolicyDto insuredPolicyDto);
        Task UpdateApprovalStatus(int id, string approvalStatus);
    }

    public class InsuredPolicyService : IInsuredPolicyService
    {
        private readonly FnfProjectContext context;

        public InsuredPolicyService(FnfProjectContext context)
        {
            this.context = context;
        }

        public async Task<List<InsuredPolicyDto>> GetAll()
        {
            List<InsuredPolicyDto> insuredPolicyDtos = [];
            await foreach (var insuredPolicyTable in context.InsuredPolicies)
            {
                var insuredPolicyDto = ConvertToDto(insuredPolicyTable);
                insuredPolicyDtos.Add(insuredPolicyDto);
            }
            return insuredPolicyDtos;
        }

        public async Task Delete(int id)
        {
            var found = await context.InsuredPolicies.FirstOrDefaultAsync((insuredPolicyTable) =>
                insuredPolicyTable.InsuredPolicyId == id);
            if (found != null)
            {
                context.InsuredPolicies.Remove(found);
                await context.SaveChangesAsync();
                return;
            }
            throw new NullReferenceException();
        }

        public async Task Add(InsuredPolicyDto insuredPolicyDto)
        {
            InsuredPolicy insuredPolicyTable = new();
            ConvertToTable(insuredPolicyDto, insuredPolicyTable);
            context.InsuredPolicies.Add(insuredPolicyTable);
            await context.SaveChangesAsync();
            return;
        }

        public async Task Update(InsuredPolicyDto insuredPolicyDto)
        {
            var found = await context.InsuredPolicies.FirstOrDefaultAsync((insuredPolicyTable) =>
                insuredPolicyTable.InsuredPolicyId == insuredPolicyDto.InsuredPolicyId);
            if (found != null)
            {
                ConvertToTable(insuredPolicyDto, found);
                await context.SaveChangesAsync();
                return;
            }
            throw new NullReferenceException();
        }

        public async Task<InsuredPolicyDto> GetById(int id)
        {
            var found = await context.InsuredPolicies.FirstOrDefaultAsync((insuredPolicyTable) =>
                insuredPolicyTable.InsuredPolicyId == id);

            if (found != null)
                return ConvertToDto(found);

            throw new NullReferenceException();
        }
        public async Task UpdateApprovalStatus(int id, string approvalStatus)
        {
            var found = await context.InsuredPolicies.FirstOrDefaultAsync(ip => ip.InsuredPolicyId == id);
            if (found != null)
            {
                found.ApprovalStatus = approvalStatus;
                context.InsuredPolicies.Update(found);
                await context.SaveChangesAsync();
                return;
            }
            throw new NullReferenceException();
        }
        public async Task<int> GetPolicyCount()
        {
            return await context.InsuredPolicies.CountAsync();
        }

        private InsuredPolicyDto ConvertToDto(InsuredPolicy insuredPolicyTable)
        {
            InsuredPolicyDto insuredPolicyDto = new()
            {
                InsuredPolicyId = insuredPolicyTable.InsuredPolicyId,
                PolicyId = insuredPolicyTable.PolicyId,
                InsuredId = insuredPolicyTable.InsuredId,
                ApprovalStatus = insuredPolicyTable.ApprovalStatus,
                RenewalStatus = insuredPolicyTable.RenewalStatus,
                AdminId = insuredPolicyTable.AdminId,
                ApprovalDate = DateTime.Parse(insuredPolicyTable.ApprovalDate.ToString()),
            };
            return insuredPolicyDto;
        }

        private void ConvertToTable(InsuredPolicyDto insuredPolicyDto, InsuredPolicy insuredPolicyTable)
        {
            insuredPolicyTable.InsuredPolicyId = insuredPolicyDto.InsuredPolicyId;
            insuredPolicyTable.PolicyId = insuredPolicyDto.PolicyId;
            insuredPolicyTable.InsuredId = insuredPolicyDto.InsuredId;
            insuredPolicyTable.ApprovalStatus = insuredPolicyDto.ApprovalStatus;
            insuredPolicyTable.RenewalStatus = insuredPolicyDto.RenewalStatus;
            insuredPolicyTable.AdminId = insuredPolicyDto.AdminId;
            insuredPolicyTable.ApprovalDate = DateOnly.FromDateTime((DateTime)insuredPolicyDto.ApprovalDate);
            return;
        }
    }
}