// --------------------------------------------------------------------------
// <copyright file="OdbcAseForeignKeysTool.cs" company="Devart">
//
// Copyright (c) Devart. ALL RIGHTS RESERVED
// Use of the source code is permitted under the license.
// </copyright>
// --------------------------------------------------------------------------

using System;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Devart.AI.McpServer.Extensions;
using Devart.AI.McpServer.Tools;
using Devart.AI.McpServer.Interfaces;

namespace Devart.AI.McpServer.Odbc.Ase.Tools
{
  internal sealed class OdbcAseForeignKeysTool(McpConfiguration serverConfiguration) : ForeignKeysTool(serverConfiguration)
  {
    protected override async Task<DataTable> GetMetadataTable(
      DbConnection connection, 
      string schema, 
      string tableName, 
      IServiceProvider services, 
      CancellationToken cancellationToken)
    {
      const string sql =
"""
SELECT 
  object_name(sr.constrid) AS FK_NAME,
  col_name(ft.id, sr.fokey1) AS FKCOLUMN_NAME,
  user_name(pt.uid) AS PKTABLE_SCHEM,
  pt.name AS PKTABLE_NAME,
  col_name(pt.id, sr.refkey1) AS PKCOLUMN_NAME,
  'NO ACTION' AS UPDATE_RULE,
  'NO ACTION' AS DELETE_RULE
  FROM
    sysreferences sr,
    sysobjects pt,
    sysobjects ft,
    sysindexes si
  WHERE pt.type in ('S', 'U')
    AND ft.type in ('S', 'U')
    AND db_name() = ?
    AND user_name(ft.uid) = ?
    AND ft.name = ?
    AND sr.reftabid = pt.id
    AND sr.tableid = ft.id
    AND si.status > 2048
    AND si.status < 32768
    AND si.id = pt.id
  ORDER BY FK_NAME
""";
      var database = services.GetRequiredService<IDatabase>();
      var commandHelper = services.GetRequiredService<ICommandHelper>();

      await using var reader = await database.ExecuteReaderAsync(
        connection,
        sql,
        cmd =>
        {
          commandHelper.AddParameter(cmd, connection.Database);
          commandHelper.AddParameter(cmd, schema);
          commandHelper.AddParameter(cmd, tableName);
        },
        cancellationToken
      ).ConfigureAwait(false);

      return await reader.ToDataTableAsync(OdbcConstants.ForeignKeysCollectionName, cancellationToken).ConfigureAwait(false);
    }
  }
}
