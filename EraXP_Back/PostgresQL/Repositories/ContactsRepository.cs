using System.Data.Common;
using EraXP_Back.Models.Database;
using EraXP_Back.Persistence;
using EraXP_Back.Persistence.Repositories;
using EraXP_Back.Persistence.Utils;

namespace EraXP_Back.PostgresQL.Repositories;

public class ContactsRepository(IDbExec exec) : IContactsRepository
{
    public async Task<List<Contact>> Get(Guid? id = null, Guid? depId = null)
    {
        QueryBuilder builder = new QueryBuilder('@');

        builder.And();
        
        if (id != null)
        {
            builder.Add("id", "Id", id);
        }

        if (depId != null)
        {
            builder.Add("department_id", "DepId", depId);
        }

        string sql =
            $"""
            select
                *
            from
                contacts
            {builder}
            """;
        await using DbDataReader reader = await exec.QueryAsync(sql, builder.DbParams);
        return await GetContactsFromReaderAsync(reader);
    }

    private async Task<List<Contact>> GetContactsFromReaderAsync(DbDataReader reader)
    {
        int idOrdinal = reader.GetOrdinal("id");
        int departmentIdOrdinal = reader.GetOrdinal("department_id");
        int nameOrdinal = reader.GetOrdinal("name");
        int lastnameOrdinal = reader.GetOrdinal("lastname");
        int emailOrdinal = reader.GetOrdinal("email");
        int phoneNumberOrdinal = reader.GetOrdinal("phone_number");

        List<Contact> contacts = new List<Contact>();
        while (await reader.ReadAsync())
        {
            Contact contact = new Contact(
                reader.GetGuid(idOrdinal),
                reader.GetGuid(departmentIdOrdinal),
                reader.GetString(nameOrdinal),
                reader.GetString(lastnameOrdinal),
                reader.GetString(emailOrdinal),
                reader.GetString(phoneNumberOrdinal)
            );
            contacts.Add(contact);
        }

        return contacts;
    }
}