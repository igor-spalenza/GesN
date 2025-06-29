﻿using GesN.Web.Models;

namespace GesN.Web.Interfaces.Repositories
{
    public interface IPedidoRepository
    {
        Task<Pedido> GetByIdAsync(int id);
        Task<IEnumerable<Pedido>> GetAllAsync();
        Task<int> AddAsync(Pedido pedido);
        Task<(bool Success, string ErrorMessage)> UpdateAsync(Pedido pedido);
        Task DeleteAsync(int id);
    }
}
