using Microsoft.Data.SqlClient;
using Repertory.Dtos;

namespace Repertory.Repositories {
    public class CategoryRepository : Repository {
        public CategoryRepository(SqlConnection connection) : base(connection) {
        }

        public async IAsyncEnumerable<Category> GetSelectedCategoriesAsync(bool includeOrchestreEnsemble, int nbrInstrumentalists, int page, int resultsPerPage, string[] instrumentFamiliesToInclude) {
            using (_conn) {
                await OpenConnectionAsync();
                page--;

                using (SqlCommand command = new SqlCommand()) {
                    List<string> likeConditions = new List<string>();
                    for(int i = 0; i < instrumentFamiliesToInclude.Length; i++) {
                        likeConditions.Add($"c.Name LIKE @{i}");
                        command.Parameters.Add(new SqlParameter("@" + i.ToString(), "%"+instrumentFamiliesToInclude[i]+"%"));
                    }
                    string querySql = $"SELECT c.Name as CategoryName, c.Id as CategoryId, COUNT(f.ID_Formation) as CountGroup FROM Categories as c INNER JOIN Formation as f on f.ID_Category = c.id WHERE NbrInstrumentalists = @Nbr {(includeOrchestreEnsemble ? "" : " AND ContainsOrchestraOrEnsemble = 0 ")} {(instrumentFamiliesToInclude.Any() ? $" AND {string.Join(" AND ", likeConditions)} " : "")} GROUP BY c.Name, c.id, c.Range ORDER BY c.Range OFFSET @Skip ROWS FETCH NEXT @Take ROWS ONLY;";
                    command.Connection = _conn;
                    command.CommandText = querySql;
                    command.Parameters.Add(new SqlParameter("Nbr", nbrInstrumentalists));
                    command.Parameters.Add(new SqlParameter("Take", resultsPerPage));
                    command.Parameters.Add(new SqlParameter("Skip", resultsPerPage * page));
                    using (SqlDataReader reader = await command.ExecuteReaderAsync()) {
                        while (await reader.ReadAsync()) {
                            yield return new Category() {
                                Id = Convert.ToInt32(reader["CategoryId"]),
                                Name = reader["CategoryName"].ToString() ?? "",
                                CountGroup = Convert.ToInt32(reader["CountGroup"])
                            };
                        }
                    }
                }
            }
        }

        public async Task<int> CountSelectedCategoriesAsync(bool includeOrchestraEnsemble, int nbrInstrumentalists, string[] instrumentFamiliesToInclude) {
            await OpenConnectionAsync();
            int count = 0;
            using (SqlCommand command = new SqlCommand()) {
                List<string> likeConditions = new List<string>();
                for (int i = 0; i < instrumentFamiliesToInclude.Length; i++) {
                    likeConditions.Add($"c.Name LIKE @{i}");
                    command.Parameters.Add(new SqlParameter("@" + i.ToString(), "%"+instrumentFamiliesToInclude[i]+"%"));
                }
                command.Parameters.Add(new SqlParameter("Nbr", nbrInstrumentalists));
                string querySql = $"SELECT Count(*) FROM Categories as c WHERE c.NbrInstrumentalists = @Nbr {(includeOrchestraEnsemble ? "" : " AND c.ContainsOrchestraOrEnsemble = 0 ")} {(instrumentFamiliesToInclude.Any() ? $" AND {string.Join(" AND ", likeConditions)} " : "")}";
                command.Connection = _conn;
                command.CommandText = querySql;
                count = Convert.ToInt32(await command.ExecuteScalarAsync());
            }
            await CloseConnectionAsync();
            return count;
        }
    }
}
