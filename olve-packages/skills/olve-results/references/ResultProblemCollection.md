# ResultProblemCollection

https://olivervea.github.io/Olve.Utilities/api/Olve.Results.ResultProblemCollection.html

Class. Immutable collection of ResultProblem. Implements `IEnumerable<ResultProblem>`.

Constructor:
- `ResultProblemCollection(params IEnumerable<ResultProblem> problems)`

Methods:
- `IEnumerator<ResultProblem> GetEnumerator()`
- `ResultProblemCollection Append(params IEnumerable<ResultProblem> resultProblems)` — returns new collection with problems appended
- `ResultProblemCollection Prepend(params IEnumerable<ResultProblem> resultProblems)` — returns new collection with problems prepended
- `ResultProblemCollection Prepend(string message, params object[] args)` — creates and prepends a formatted problem
- `ResultProblemCollection Prepend(Exception exception, string message, params object[] args)` — creates and prepends a problem from an exception
- `bool TryPickProblem<TProblem>(out TProblem? problem) where TProblem : ResultProblem` — first problem assignable to `TProblem`
- `IEnumerable<TProblem> PickProblems<TProblem>() where TProblem : ResultProblem` — all problems assignable to `TProblem` (`OfType`)
- `static ResultProblemCollection Merge(params IEnumerable<ResultProblemCollection> problemCollections)` — merges multiple collections into one
