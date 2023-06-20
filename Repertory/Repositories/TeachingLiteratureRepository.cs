using Microsoft.Data.SqlClient;
using Repertory.Dtos;

namespace Repertory.Repositories {
    public class TeachingLiteratureRepository : Repository {
        public TeachingLiteratureRepository(SqlConnection connection) : base(connection) {
        }

        public async IAsyncEnumerable<TeachingLiteratureCategory> GetCategoryAndCountTitle() {
            using (_conn) {
                await OpenConnectionAsync();
                using(SqlCommand command = new SqlCommand("SELECT p.FormationId, f.Formation_Imp as composition, COUNT(p.id) as Count FROM Partitions as p INNER JOIN formation as f on f.ID_Formation = p.FormationId WHERE f.Formation_Imp LIKE 'ENS LITT%' GROUP BY p.FormationId, f.Formation_Imp ORDER BY p.FormationId", _conn)) {
                    using(SqlDataReader reader = await command.ExecuteReaderAsync()) {
                        while(await reader.ReadAsync()) {
                            yield return new TeachingLiteratureCategory() {
                                GroupId = reader["FormationId"].ToString()!,
                                Name = reader["composition"].ToString()!,
                                CountTitle = Convert.ToInt32(reader["Count"]) 
                            };
                        }
                    }
                }
            }
        }

        public async IAsyncEnumerable<TeachingLiterature> GetFromGroupAsync(long groupId, int page, int resultsPerPage, string author, string title) {
            page--;
            using (_conn) {
                await OpenConnectionAsync();
                using(SqlCommand command = new SqlCommand("SELECT p.id, p.Author, p.Title, p.Publisher, p.ReleaseYear, p.Duration FROM Partitions as p INNER JOIN formation as f on f.ID_Formation = p.FormationId WHERE f.Formation_Imp LIKE 'ENS LITT%' AND p.FormationId = @GroupId AND p.Author LIKE @Author AND Title LIKE @Title ORDER BY p.Author, p.Title OFFSET @Skip ROWS FETCH NEXT @Take ROWS ONLY", _conn)) {
                    command.Parameters.Add(new SqlParameter("Take", resultsPerPage));
                    command.Parameters.Add(new SqlParameter("Skip", resultsPerPage * page));
                    command.Parameters.Add(new SqlParameter("GroupId", groupId));
                    command.Parameters.Add(new SqlParameter("Author", $"%{author}%"));
                    command.Parameters.Add(new SqlParameter("Title", $"%{title}%"));
                    using (SqlDataReader reader = await command.ExecuteReaderAsync()) {
                        while (await reader.ReadAsync()) {
                            yield return new TeachingLiterature() {
                                Id = Convert.ToInt32(reader["id"]),
                                Author = reader["Author"].ToString()!,
                                Title = reader["Title"].ToString()!,
                                Publisher = reader["Publisher"].ToString()!,
                                ReleaseYear = reader["ReleaseYear"] == DBNull.Value ? null : Convert.ToInt32(reader["ReleaseYear"]),
                                Duration = reader["Duration"] == DBNull.Value ? null : reader["Duration"].ToString(),
                            };
                        }
                    }
                }
            }
        }

        public async Task<int> CountSheetMusicFromGroupAsync(string author, string title, long groupId) {
            int count = 0;
            await OpenConnectionAsync();
            using(SqlCommand command = new SqlCommand("SELECT Count(*) as Count FROM Partitions as p INNER JOIN formation as f on f.ID_Formation = p.FormationId WHERE p.Author LIKE @Author AND p.Title LIKE @Title AND p.FormationId = @GroupId", _conn)) {
                command.Parameters.Add(new SqlParameter("@Title", "%"+title+"%"));
                command.Parameters.Add(new SqlParameter("@Author", "%"+author+"%"));
                command.Parameters.Add(new SqlParameter("@GroupId", groupId));
                count = Convert.ToInt32(await command.ExecuteScalarAsync());
            }
            await CloseConnectionAsync();
            return count;
        }
    }
}
