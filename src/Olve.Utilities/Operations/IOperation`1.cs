namespace Olve.Operations.Operations;

public interface IOperation<in TInput>
{
    void Execute(TInput input);
}