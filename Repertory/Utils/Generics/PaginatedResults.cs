namespace Repertory.Utils.Generics {
    public class PaginatedResults<T> {
        public int TotalNbrOfResults { get; set; }
        public int Page { get; set; }
        public int ResultsPerPage { get; set; }
        public int MaxNbrOfPages { get; set; }
        public T Results { get; set; } = default!;

        //public PaginatedResults(T results, int nbrOfResults, int page, int nbrOfResultsPerPage) {
        //    Results = results;
        //    NbrOfResults = nbrOfResults;
        //    Page = page;
        //    ResultsPerPage = nbrOfResultsPerPage;
        //}
    }
}
