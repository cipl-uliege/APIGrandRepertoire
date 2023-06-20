using Microsoft.Data.SqlClient;
using Repertory.Dtos;

namespace Repertory.Repositories {
    public class GroupRepository : Repository {
        public GroupRepository(SqlConnection connection) : base(connection) {
        }

        public async Task<string> GetCompositionByIdAsync(long groupId) {
            using (_conn) {
                await OpenConnectionAsync();
                string querySql = "SELECT f.Formation_Imp as Composition FROM Formation as f WHERE ID_Formation = @GroupId ORDER BY f.ID_cipl OFFSET 0 ROWS FETCH NEXT 1 ROWS ONLY";
                using(SqlCommand command = new SqlCommand(querySql, _conn)) {
                    command.Parameters.Add(new SqlParameter("GroupId", groupId));
                    var composition = await command.ExecuteScalarAsync();
                    return composition?.ToString() ?? "";
                }
            }
        }

        public async IAsyncEnumerable<Group> GetFromCategoryAsync(int id) {
            using (_conn) {
                await OpenConnectionAsync();
                string querySql = "SELECT f.Formation_Imp as Composition, f.ID_Formation as Id , Count(p.id) as CountSheetMusic FROM Formation as f  INNER JOIN Categories as c on c.id = f.ID_Category INNER JOIN Partitions as p on p.FormationId = f.ID_Formation WHERE c.id = @CategoryId GROUP BY f.ID_Formation, f.Formation_Imp ORDER BY f.ID_Formation";

                using (SqlCommand command = new SqlCommand(querySql, _conn)) {
                    command.Parameters.Add(new SqlParameter("CategoryId", id));

                    using (SqlDataReader reader = await command.ExecuteReaderAsync()) {
                        while (await reader.ReadAsync()) {
                            yield return new Group() {
                                Id = reader["Id"].ToString() ?? "Unfound",
                                Composition = reader["Composition"].ToString() ?? "Unfound",
                                CountSheetMusic = Convert.ToInt32(reader["CountSheetMusic"])
                            };
                        }
                    }

                }
            }
        }
    }
}
