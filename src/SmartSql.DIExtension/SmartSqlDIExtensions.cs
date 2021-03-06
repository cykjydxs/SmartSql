﻿using System;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SmartSql;
using SmartSql.Configuration;
using SmartSql.DbSession;
using SmartSql.DIExtension;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class SmartSqlDIExtensions
    {
        public static SmartSqlDIBuilder AddSmartSql(this IServiceCollection services, Func<IServiceProvider, SmartSqlBuilder> setup)
        {
            services.AddSingleton<SmartSqlBuilder>(sp => setup(sp).Build());
            AddOthers(services);
            return new SmartSqlDIBuilder(services);
        }

        public static SmartSqlDIBuilder AddSmartSql(this IServiceCollection services, Action<IServiceProvider, SmartSqlBuilder> setup)
        {
            return services.AddSmartSql(sp =>
            {
                var configPath = ResolveConfigPath(sp);
                var loggerFactory = sp.GetService<ILoggerFactory>();
                var smartSqlBuilder = new SmartSqlBuilder()
                .UseLoggerFactory(loggerFactory)
                .UseXmlConfig(SmartSql.ConfigBuilder.ResourceType.File, configPath);
                setup(sp, smartSqlBuilder);
                return smartSqlBuilder;
            });
        }

        public static SmartSqlDIBuilder AddSmartSql(this IServiceCollection services, String alias = SmartSqlBuilder.DEFAULT_ALIAS)
        {
            return services.AddSmartSql((sp, builder) =>
            {
                builder.UseAlias(alias);
            });
        }
        public static SmartSqlDIBuilder AddSmartSql(this IServiceCollection services, Action<SmartSqlBuilder> setup)
        {
            return services.AddSmartSql((sp, builder) => { setup(builder); });
        }
        private static string ResolveConfigPath(IServiceProvider sp)
        {
            var env = sp.GetService<IHostingEnvironment>();
            var configPath = SmartSqlBuilder.DEFAULT_SMARTSQL_CONFIG_PATH;
            if (env != null && !env.IsProduction())
            {
                configPath = $"SmartSqlMapConfig.{env.EnvironmentName}.xml";
            }
            if (!File.Exists(configPath))
            {
                configPath = SmartSqlBuilder.DEFAULT_SMARTSQL_CONFIG_PATH;
            }
            return configPath;
        }

        private static void AddOthers(IServiceCollection services)
        {
            services.AddSingleton<IDbSessionFactory>(sp => sp.GetRequiredService<SmartSqlBuilder>().DbSessionFactory);
            services.AddSingleton<ISqlMapper>(sp => sp.GetRequiredService<SmartSqlBuilder>().SqlMapper);
            services.AddSingleton<ITransaction>(sp => sp.GetRequiredService<SmartSqlBuilder>().SqlMapper);
            services.AddSingleton<IDbSessionStore>(sp => sp.GetRequiredService<SmartSqlBuilder>().SmartSqlConfig.SessionStore);
        }

        public static SmartSqlBuilder GetSmartSql(this IServiceProvider sp, string alias = SmartSqlBuilder.DEFAULT_ALIAS)
        {
            return sp.GetServices<SmartSqlBuilder>().FirstOrDefault(m => m.Alias == alias);
        }
    }
}
