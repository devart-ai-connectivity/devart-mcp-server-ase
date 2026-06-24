// --------------------------------------------------------------------------
// <copyright file="OdbcAseTools.cs" company="Devart">
//
// Copyright (c) Devart. ALL RIGHTS RESERVED
// Use of the source code is permitted under the license.
// </copyright>
// --------------------------------------------------------------------------

using System.Collections.Generic;
using ModelContextProtocol.Server;
using Devart.AI.McpServer.Odbc.Ase.Tools;

namespace Devart.AI.McpServer.Odbc.Ase
{
  internal static class OdbcAseTools
  {
    public static List<McpServerTool> CreateTools(McpConfiguration configuration)
      => OdbcTools.CreateBuilder(configuration)
        .Add(new OdbcAsePrimaryKeysTool(configuration))
        .Add(new OdbcAseForeignKeysTool(configuration))
        .Build();
  }
}
