using Olve.Utilities.Ids;
using TUnit.Assertions.Extensions;
using TUnit.Core;
using Assert = TUnit.Assertions.Assert;

namespace Olve.Utilities.Tests.Ids;

public class UnionIdTests
{
    private readonly Dictionary<Id<Person>, Person> _people = [];
    private readonly Dictionary<Id<int>, int> _integers = [];
    
    private readonly record struct Person(string Name, string PHoneNumber);
    
    [Test]
    public async Task TestUnionIds()
    {
        var (personIdA, personA) = CreatePerson("John Doe", "000-0000-0000");
        var (personIdB, personB) = CreatePerson("Jane Doe", "111-1111-1111");
        var (intIdA, intA) = CreateInt(42);
        var (intIdB, intB) = CreateInt(111);

        List<UnionId<Person, int>> ids = [personIdA, intIdA, personIdB, intIdB];
        List<string> output = [];

        foreach (var id in ids)
        {
            id.Switch(
                personId => output.Add($"person:{personId}:{_people[personId]}"),
                intId => output.Add($"int:{intId}:{_integers[intId]}"));
        }
        
        List<string> expexted = [
            $"person:{personIdA.ToString()}:{personA.ToString()}",
            $"int:{intIdA.ToString()}:{intA.ToString()}",
            $"person:{personIdB.ToString()}:{personB.ToString()}",
            $"int:{intIdB.ToString()}:{intB.ToString()}"];

        await Assert.That(output).IsEquivalentTo(expexted);
    }

    private (Id<Person>, Person) CreatePerson(string name, string phoneNumber)
    {
        Person person = new(name, phoneNumber);
        var personId = Id.New<Person>();
        _people.Add(personId, person);
        return (personId, person);
    }

    private (Id<int>, int) CreateInt(int value)
    {
        var intId = Id.New<int>();
        _integers.Add(intId, value);
        return (intId, value);
    }
}