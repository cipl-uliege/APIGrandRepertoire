using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Repertory.Dtos;

namespace Repertory.Repositories {
    public class SheetMusicRepository : Repository {
        public SheetMusicRepository(SqlConnection connection) : base(connection) {
        }

        public async IAsyncEnumerable<SheetMusic> GetFromGroupAsync(long groupId, int page, string author, string title, int resultsPerPage) {
            page--;
            using (_conn) {
                await OpenConnectionAsync();
                using(SqlCommand command = new SqlCommand("SELECT * FROM Partitions WHERE FormationId = @GroupId AND Author LIKE @Author AND Title LIKE @Title ORDER BY Author, Title OFFSET @Skip ROWS FETCH NEXT @Take ROWS ONLY;", _conn)) {
                    command.Parameters.Add(new SqlParameter("Take", resultsPerPage));
                    command.Parameters.Add(new SqlParameter("Skip", resultsPerPage * page));
                    command.Parameters.Add(new SqlParameter("GroupId", groupId));
                    command.Parameters.Add(new SqlParameter("Author", $"%{author}%"));
                    command.Parameters.Add(new SqlParameter("Title", $"%{title}%"));
                    using (SqlDataReader reader = await command.ExecuteReaderAsync()) {
                        while(await reader.ReadAsync()) {
                            yield return new SheetMusic() {
                                Id = Convert.ToInt32(reader["Id"]),
                                GroupId = reader["FormationId"].ToString()!,
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
        public async IAsyncEnumerable<SheetMusicWithComposition> GetFromAuthorOrTitle(int page, string author, string title, int resultsPerPage, int nbrInstrumentalists) {
            page--;
            using (_conn) {
                await OpenConnectionAsync();
                using (SqlCommand command = new SqlCommand($"SELECT p.id, p.Author, p.Title, p.Publisher, p.ReleaseYear, p.Duration, f.Formation_Imp as Composition FROM Partitions as p INNER JOIN Formation as f ON f.ID_Formation = p.FormationId INNER JOIN Categories as c ON f.ID_Category = c.id WHERE Author LIKE @Author AND Title LIKE @Title {(nbrInstrumentalists != -1 ? "AND c.NbrInstrumentalists = @Nbr" : "")} ORDER BY p.Author, p.Title OFFSET @Skip ROWS FETCH NEXT @Take ROWS ONLY;", _conn)) {
                    command.Parameters.Add(new SqlParameter("Take", resultsPerPage));
                    command.Parameters.Add(new SqlParameter("Skip", resultsPerPage * page));
                    command.Parameters.Add(new SqlParameter("Author", $"%{author}%"));
                    command.Parameters.Add(new SqlParameter("Title", $"%{title}%"));
                    if (nbrInstrumentalists != -1)
                        command.Parameters.Add(new SqlParameter("Nbr", nbrInstrumentalists));

                    using (SqlDataReader reader = await command.ExecuteReaderAsync()) {
                        while (await reader.ReadAsync()) {
                            yield return new SheetMusicWithComposition() {
                                Id = Convert.ToInt32(reader["Id"]),
                                Composition = reader["Composition"].ToString()!,
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

        public async IAsyncEnumerable<SheetMusicWithComposition> GetFavorites(List<int> favoriteIds, int page, int resultsPerPage, string author, string title, int nbrInstrumentalists) {
            page--;
            using (_conn) {
                await OpenConnectionAsync();
                using (SqlCommand command = new SqlCommand()) {
                    command.Connection = _conn;
                    List<string> inConditions = new List<string>();
                    for (int i = 0; i < favoriteIds.Count; i++) {
                        inConditions.Add("@" + i.ToString());
                        command.Parameters.Add(new SqlParameter("@" + i.ToString(), favoriteIds[i]));
                    }
                    command.Parameters.Add(new SqlParameter("Take", resultsPerPage));
                    command.Parameters.Add(new SqlParameter("Skip", resultsPerPage * page));
                    command.Parameters.Add(new SqlParameter("Author", $"%{author}%"));
                    command.Parameters.Add(new SqlParameter("Title", $"%{title}%"));
                    if (nbrInstrumentalists != -1)
                        command.Parameters.Add(new SqlParameter("Nbr", nbrInstrumentalists));

                    command.CommandText = $"SELECT p.id, p.Author, p.Title, p.Publisher, p.ReleaseYear, p.Duration, f.Formation_Imp as Composition FROM Partitions as p INNER JOIN Formation as f ON f.ID_Formation = p.FormationId INNER JOIN Categories as c ON f.ID_Category = c.id WHERE Author LIKE @Author AND Title LIKE @Title {(nbrInstrumentalists != -1 ? "AND c.NbrInstrumentalists = @Nbr" : "")} AND p.id IN({string.Join(',', inConditions)}) ORDER BY p.Author, p.Title OFFSET @Skip ROWS FETCH NEXT @Take ROWS ONLY;";
                    using (SqlDataReader reader = await command.ExecuteReaderAsync()) {
                        while (await reader.ReadAsync()) {
                            yield return new SheetMusicWithComposition() {
                                Id = Convert.ToInt32(reader["Id"]),
                                Composition = reader["Composition"].ToString()!,
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

        public async Task<int> CountSheetMusicFromGroupAsync(long groupId, string author, string title) {
            await OpenConnectionAsync();
            int count = 0;
            using(SqlCommand command = new SqlCommand("SELECT COUNT(*) as Count FROM Partitions WHERE FormationId = @GroupId AND Author LIKE @Author AND Title LIKE @Title", _conn)) {
                command.Parameters.Add(new SqlParameter("GroupId", groupId));
                command.Parameters.Add(new SqlParameter("Author", $"%{author}%"));
                command.Parameters.Add(new SqlParameter("Title", $"%{title}%"));
                count = Convert.ToInt32(await command.ExecuteScalarAsync());
            }
            await CloseConnectionAsync();
            return count;
        }

        public async Task<int> CountSheetMusicFromAuthorOrtitle(string author, string title, int nbrInstrumentalists) {
            await OpenConnectionAsync();
            int count = 0;
            using (SqlCommand command = new SqlCommand($"SELECT COUNT(p.id) as Count FROM Partitions as p INNER JOIN Formation as f ON f.ID_Formation = p.FormationId INNER JOIN Categories as c ON f.ID_Category = c.id WHERE Author LIKE @Author AND Title LIKE @Title {(nbrInstrumentalists != -1 ? "AND c.NbrInstrumentalists = @Nbr" : "" )}", _conn)) {
                command.Parameters.Add(new SqlParameter("Author", $"%{author}%"));
                command.Parameters.Add(new SqlParameter("Title", $"%{title}%"));
                if (nbrInstrumentalists != -1)
                    command.Parameters.Add(new SqlParameter("Nbr", nbrInstrumentalists));
                count = Convert.ToInt32(await command.ExecuteScalarAsync());
            }
            await CloseConnectionAsync();
            return count;
        }

        public async Task<int> CountSheetMusicFromFavorites(List<int>favoriteIds, string author, string title, int nbrInstrumentalists) {
            if(favoriteIds.Any() == false)
                return 0;

            await OpenConnectionAsync();
            int count = 0;
            using (SqlCommand command = new SqlCommand()) {
                command.Connection = _conn;
                List<string> inConditions = new List<string>();
                for(int i = 0; i < favoriteIds.Count; i++) {
                    inConditions.Add("@" + i.ToString());
                    command.Parameters.Add(new SqlParameter("@" + i.ToString(), favoriteIds[i]));
                }
                command.Parameters.Add(new SqlParameter("Author", $"%{author}%"));
                command.Parameters.Add(new SqlParameter("Title", $"%{title}%"));
                if (nbrInstrumentalists != -1)
                    command.Parameters.Add(new SqlParameter("Nbr", nbrInstrumentalists));

                command.CommandText = $"SELECT COUNT(p.id) as Count FROM Partitions as p INNER JOIN Formation as f ON f.ID_Formation = p.FormationId INNER JOIN Categories as c ON f.ID_Category = c.id WHERE Author LIKE @Author AND Title LIKE @Title {(nbrInstrumentalists != -1 ? "AND c.NbrInstrumentalists = @Nbr" : "")} AND p.id IN({string.Join(',', inConditions)});";
                count = Convert.ToInt32(await command.ExecuteScalarAsync());
            }
            await CloseConnectionAsync();
            return count;
        }
    }
}
