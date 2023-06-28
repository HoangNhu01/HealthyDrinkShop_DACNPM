using eShopSolution.ViewModels.AppSystem.Roles;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace eShopSolution.Application.AppSystem.Roles
{
    public interface IRoleService
    {
        Task<List<RoleVm>> GetAll();
    }
}