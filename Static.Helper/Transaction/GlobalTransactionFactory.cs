using System.Transactions;

namespace Static.Helper.Transaction;

/// <summary>
/// Provides a factory for creating <see cref="TransactionScope"/> instances with predefined settings.
/// </summary>
public static class GlobalTransactionFactory
{
    /// <summary>
    /// The default isolation level used for transactions, which is <see cref="System.Transactions.IsolationLevel.Serializable"/>.
    /// </summary>
    private const IsolationLevel IsolationLevel = System.Transactions.IsolationLevel.Serializable;

    /// <summary>
    /// Creates a new <see cref="TransactionScope"/> with the specified options and a default isolation level.
    /// </summary>
    /// <param name="scopeOption">The option for the transaction scope, determining whether the transaction is required or not. Defaults to <see cref="TransactionScopeOption.Required"/>.</param>
    /// <param name="timeOut">An optional timeout value for the transaction scope. If not provided, the default timeout from the <see cref="TransactionManager"/> is used.</param>
    /// <returns>A new <see cref="TransactionScope"/> instance configured with the provided options.</returns>
    public static TransactionScope CreateAsyncScope(
        TransactionScopeOption scopeOption = TransactionScopeOption.Required,
        TimeSpan? timeOut = null) => new(
        scopeOption: scopeOption,
        transactionOptions: new() { IsolationLevel = IsolationLevel, Timeout = timeOut ?? TransactionManager.DefaultTimeout, },
        asyncFlowOption: TransactionScopeAsyncFlowOption.Enabled);
}