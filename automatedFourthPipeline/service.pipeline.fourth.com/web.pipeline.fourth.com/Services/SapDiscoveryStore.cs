using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using web.pipeline.fourth.com.Models;

namespace web.pipeline.fourth.com.Services;

public sealed class SapDiscoveryStore(IConfiguration configuration)
{
    // Dedicated tables in the existing SQL database; the deployment script provisions them.
    async Task<SqlConnection> Open()
    {
        var connection = new SqlConnection(configuration.GetConnectionString("FourthSalesPipelineContext"));
        await connection.OpenAsync();
        return connection;
    }
    public async Task Create(SapDiscovery document, string hash)
    {
        await using var connection = await Open();
        using var command = connection.CreateCommand();
        command.CommandText = "INSERT INTO SapDiscoveryDocuments(Id,AccessHash,Revision,DocumentJson,UpdatedUtc) VALUES (@id,@hash,0,@json,SYSUTCDATETIME())";
        command.Parameters.AddWithValue("@id", document.Id);
        command.Parameters.AddWithValue("@hash", hash);
        command.Parameters.AddWithValue("@json", JsonSerializer.Serialize(document));
        await command.ExecuteNonQueryAsync();
    }
    public async Task<(SapDiscovery Document, string Hash)> Get(string id)
    {
        await using var connection = await Open();
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT DocumentJson, AccessHash FROM SapDiscoveryDocuments WHERE Id=@id";
        command.Parameters.AddWithValue("@id", id ?? "");
        await using var reader = await command.ExecuteReaderAsync();
        return await reader.ReadAsync() ? (JsonSerializer.Deserialize<SapDiscovery>(reader.GetString(0)), reader.GetString(1)) : (null, null);
    }
    public async Task<bool> Save(SapDiscovery document, int expectedRevision, string actor)
    {
        await using var connection = await Open();
        using var transaction = connection.BeginTransaction();
        document.Revision = expectedRevision + 1;
        document.UpdatedUtc = DateTimeOffset.UtcNow;
        var json = JsonSerializer.Serialize(document);
        using var command = connection.CreateCommand();
        command.Transaction = transaction;
        command.CommandText = "UPDATE SapDiscoveryDocuments SET DocumentJson=@json, Revision=@new, UpdatedUtc=SYSUTCDATETIME() WHERE Id=@id AND Revision=@old";
        command.Parameters.AddWithValue("@json", json);
        command.Parameters.AddWithValue("@id", document.Id);
        command.Parameters.AddWithValue("@old", expectedRevision);
        command.Parameters.AddWithValue("@new", document.Revision);
        if (await command.ExecuteNonQueryAsync() != 1) { transaction.Rollback(); return false; }
        command.CommandText = "INSERT INTO SapDiscoveryRevisions(DocumentId,Revision,Actor,Status,ChangedUtc,DocumentJson) VALUES(@id,@new,@actor,@status,SYSUTCDATETIME(),@json)";
        command.Parameters.AddWithValue("@actor", actor);
        command.Parameters.AddWithValue("@status", document.Status);
        await command.ExecuteNonQueryAsync();
        transaction.Commit();
        return true;
    }
    public async Task<List<SapDiscovery>> List()
    {
        await using var connection = await Open();
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT TOP (250) DocumentJson FROM SapDiscoveryDocuments ORDER BY UpdatedUtc DESC";
        await using var reader = await command.ExecuteReaderAsync();
        var result = new List<SapDiscovery>();
        while (await reader.ReadAsync()) result.Add(JsonSerializer.Deserialize<SapDiscovery>(reader.GetString(0)));
        return result;
    }
    public async Task<List<SapRevision>> History(string id)
    {
        await using var connection = await Open();
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT Revision, Actor, Status, ChangedUtc, DocumentJson FROM SapDiscoveryRevisions WHERE DocumentId=@id ORDER BY Revision DESC";
        command.Parameters.AddWithValue("@id", id);
        await using var reader = await command.ExecuteReaderAsync();
        var result = new List<SapRevision>();
        while (await reader.ReadAsync()) result.Add(new(reader.GetInt32(0), reader.GetString(1), reader.GetString(2), new DateTimeOffset(DateTime.SpecifyKind(reader.GetDateTime(3), DateTimeKind.Utc)), reader.GetString(4)));
        return result;
    }
}
