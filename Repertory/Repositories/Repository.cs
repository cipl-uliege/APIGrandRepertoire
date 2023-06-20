using Microsoft.Data.SqlClient;

namespace Repertory.Repositories {
    public class Repository {
        protected readonly SqlConnection _conn;
        public Repository(SqlConnection connection)
        {
            _conn = connection;
        }

        protected async Task OpenConnectionAsync() {
            if (_conn.State == System.Data.ConnectionState.Closed)
                await _conn.OpenAsync();
        }

        public async Task DisposeConnectionAsync() {
            await _conn.DisposeAsync();
        }

        protected async Task CloseConnectionAsync() {
            await _conn.CloseAsync();
        }
    }
}
