﻿using InsuranceApi.Data;
using InsuranceApi.DTOs;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace InsuranceApi.Services
{
    public interface IPolicyHolderService
    {
        Task Add(PolicyHolderDto policyHolderDto);
        Task Delete(int id);
        Task<List<PolicyHolderDto>> GetAll();
        Task<PolicyHolderDto> GetById(int id);
        Task<int> GetCustomerCount();
        Task Update(PolicyHolderDto policyHolderDto);
        Task UpdateStatus(int id, int status);
        Task<List<PolicyHolderDto>> SearchPolicyHoldersAsync(string searchTerm);
    }

    public class PolicyHolderService : IPolicyHolderService
    {
        private readonly FnfProjectContext context;

        public PolicyHolderService(FnfProjectContext context)
        {
            this.context = context;
        }

        public async Task<List<PolicyHolderDto>> GetAll()
        {
            List<PolicyHolderDto> policyHolderDtos = new List<PolicyHolderDto>();
            await foreach (var policyHolderTable in context.PolicyHolders)
            {
                var policyHolderDto = ConvertToDto(policyHolderTable);
                policyHolderDtos.Add(policyHolderDto);
            }
            return policyHolderDtos;
        }

        public async Task Delete(int id)
        {
            var found = await context.PolicyHolders.FirstOrDefaultAsync((policyHolderTable) =>
                policyHolderTable.PolicyHolderId == id);
            if (found != null)
            {
                context.PolicyHolders.Remove(found);
                await context.SaveChangesAsync();
                return;
            }
            throw new NullReferenceException();
        }

        public async Task Add(PolicyHolderDto policyHolderDto)
        {
            PolicyHolder policyHolderTable = new();
            ConvertToTable(policyHolderDto, policyHolderTable);
            context.PolicyHolders.Add(policyHolderTable);
            await context.SaveChangesAsync();
            return;
        }

        public async Task Update(PolicyHolderDto policyHolderDto)
        {
            var found = await context.PolicyHolders.FirstOrDefaultAsync((policyHolderTable) =>
                policyHolderTable.PolicyHolderId == policyHolderDto.PolicyHolderId);
            if (found != null)
            {
                ConvertToTable(policyHolderDto, found);
                await context.SaveChangesAsync();
                return;
            }
            throw new NullReferenceException();
        }

        public async Task<PolicyHolderDto> GetById(int id)
        {
            var found = await context.PolicyHolders.FirstOrDefaultAsync((policyHolderTable) =>
                policyHolderTable.PolicyHolderId == id);

            if (found != null)
                return ConvertToDto(found);

            throw new NullReferenceException();
        }

        public async Task UpdateStatus(int id, int status)
        {
            var found = await context.PolicyHolders.FirstOrDefaultAsync(ph => ph.PolicyHolderId == id);
            if (found != null)
            {
                found.Status = status;
                await context.SaveChangesAsync();
                return;
            }
            throw new NullReferenceException();
        }
        public async Task<int> GetCustomerCount()
        {
            return await context.PolicyHolders.CountAsync();
        }
        public async Task<List<PolicyHolderDto>> SearchPolicyHoldersAsync(string searchTerm)
        {
            var query = context.PolicyHolders.AsQueryable();

            if (int.TryParse(searchTerm, out int id))
            {
                // Search by PolicyHolderId
                query = query.Where(ph => ph.PolicyHolderId == id);
            }
            else
            {
                // Search by Name
                query = query.Where(ph => ph.Name.Contains(searchTerm));
            }

            var policyHolders = await query.ToListAsync();
            return policyHolders.Select(ConvertToDto).ToList();
        }


        private PolicyHolderDto ConvertToDto(PolicyHolder policyHolderTable)
        {
            PolicyHolderDto policyHolderDto = new()
            {
                PolicyHolderId = policyHolderTable.PolicyHolderId,
                Name = policyHolderTable.Name,
                Address = policyHolderTable.Address,
                Email = policyHolderTable.Email,
                PasswordHash = policyHolderTable.PasswordHash,
                Phone = policyHolderTable.Phone,
                Status = policyHolderTable.Status,
            };
            return policyHolderDto;
        }

        private void ConvertToTable(PolicyHolderDto policyHolderDto, PolicyHolder policyHolderTable)
        {
            policyHolderTable.PolicyHolderId = policyHolderDto.PolicyHolderId;
            policyHolderTable.Name = policyHolderDto.Name;
            policyHolderTable.Address = policyHolderDto.Address;
            policyHolderTable.Email = policyHolderDto.Email;
            policyHolderTable.PasswordHash = policyHolderDto.PasswordHash;
            policyHolderTable.Phone = policyHolderDto.Phone;
            policyHolderTable.Status = policyHolderDto.Status;
            return;
        }
    }
}