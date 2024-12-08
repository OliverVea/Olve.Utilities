namespace Olve.Utilities.Operations;

public interface IOperation<in TInput>
{
    void Execute(TInput input);
}