﻿using SmartSql.Configuration;
using SmartSql.DataSource;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SmartSql.Middlewares
{
    public class DataSourceFilterMiddleware : IMiddleware
    {
        private readonly IDataSourceFilter _dataSourceFilter;
        public IMiddleware Next { get; set; }
        public DataSourceFilterMiddleware(SmartSqlConfig smartSqlConfig)
        {
            _dataSourceFilter = smartSqlConfig.DataSourceFilter;
        }
        public void Invoke<TResult>(ExecutionContext executionContext)
        {
            SetDataSource(executionContext);
            Next.Invoke<TResult>(executionContext);
        }
        public Task InvokeAsync<TResult>(ExecutionContext executionContext)
        {
            SetDataSource(executionContext);
            return Next.InvokeAsync<TResult>(executionContext);
        }
        private void SetDataSource(ExecutionContext executionContext)
        {
            if (executionContext.DbSession.DataSource != null) { return; }
            var dataSource = _dataSourceFilter.Elect(executionContext.Request);
            executionContext.DbSession.SetDataSource(dataSource);
        }
    }
}
