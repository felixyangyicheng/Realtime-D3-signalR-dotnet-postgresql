﻿namespace RealTime_D3.Contracts
{
    public interface IRepositoryBase<T> where T : class
    {

        Task<T> AddAsync(T entity);
        Task DeleteAsync(int id);
        Task<bool> Exists(int id);
        Task<List<T>> GetAllAsync();
        //Task<VirtualizeResponse<TResult>> GetAllAsync<TResult>(QueryParameters queryParam) where TResult : class;
        Task<T> GetAsync(int? id);
        Task UpdateAsync(T entity);
    }
}
