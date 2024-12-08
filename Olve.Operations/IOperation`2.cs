namespace Olve.Operations;

public interface IOperation<in TRequest, out TResult>
{
    TResult Execute(TRequest request);
}