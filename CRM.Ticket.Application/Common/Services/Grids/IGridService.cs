﻿using CRM.Ticket.Application.Common.Models.Grids;

namespace CRM.Ticket.Application.Common.Services.Grids;

public interface IGridService
{
    Task<GridResponse<TResult>> ProcessGridQuery<TEntity, TResult>(
        IQueryable<TEntity> query,
        GridQueryBase gridQuery,
        Func<TEntity, TResult> selector,
        CancellationToken cancellationToken = default)
        where TEntity : class
        where TResult : class;

    IQueryable<T> ApplyFilters<T>(
        IQueryable<T> query,
        Dictionary<string, GridFilterItem> filters) where T : class;

    IQueryable<T> ApplyGlobalFilter<T>(
        IQueryable<T> query,
        string globalFilter,
        string[] searchableProperties,
        string[]? visibleColumns) where T : class;

    IQueryable<T> ApplySorting<T>(
        IQueryable<T> query,
        string sortField,
        string sortDirection) where T : class;

    IQueryable<T> ApplyPagination<T>(
        IQueryable<T> query,
        int pageIndex,
        int pageSize) where T : class;
}