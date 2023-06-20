using Microsoft.Data.SqlClient;
using Repertory.Dtos;

namespace Repertory.Repositories {
    public class InstrumentRepository : Repository {
        public InstrumentRepository(SqlConnection connection) : base(connection) {
        }

        public async IAsyncEnumerable<Instrument> GetFullNameOfCompositionAsync(List<string> abreviations) {
            using (_conn) {
                await OpenConnectionAsync();
                using (SqlCommand command = new SqlCommand()) {
                    command.Connection = _conn;
                    List<string> orConditions = new List<string>();
                    for (int i = 0; i < abreviations.Count; i++) {
                        orConditions.Add($"Abreviation = @{i}");
                        command.Parameters.Add(new SqlParameter("@" + i.ToString(), abreviations[i].Replace("%2F", "/")));
                    }
                    command.CommandText = $"SELECT FullName, Abreviation FROM Instruments WHERE {string.Join(" OR ", orConditions)} ORDER BY Id";
                    using (SqlDataReader reader = await command.ExecuteReaderAsync()) {
                        while (await reader.ReadAsync()) {
                            yield return new Instrument() {
                                FullName = reader["FullName"].ToString() ?? "Unfound",
                                Abreviation = reader["Abreviation"].ToString() ?? "Unfound",
                            };
                        }
                    }
                }
            }
        }
    }
}
