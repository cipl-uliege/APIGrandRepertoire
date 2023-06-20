namespace Repertory.Utils {
    public static class InstrumentTools {
        public static async Task<IEnumerable<string>> RetrieveOnlyAbreviationAsync(string composition) {
            IEnumerable<string> abreviations = composition
                .Split(' ')
                .Where(a => string.IsNullOrWhiteSpace(a) == false);
            await Task.Run(() => {
                abreviations = abreviations.Select(a => char.IsDigit(a[0]) ? a.Substring(1) : a).Distinct();
            });

            return abreviations;
        }
    }
}
