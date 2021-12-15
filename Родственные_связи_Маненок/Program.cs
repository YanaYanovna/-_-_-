using System;
using System.Collections.Generic;
using System.Linq;

namespace FamilyTies
{
    public enum Gender
    {
        Male,
        Female
    }

    public class Person
    {
        public Person(string name, Gender gender, Person mother, Person father, Person spouse)
        {
            Name = name;
            Gender = gender;
            Mother = mother;
            Father = father;
            _spouse = spouse;
            _children = new List<Person>();
        }

        public string Name { get; set; }
        public Gender Gender { get; set; }
        private List<Person> _children;
        public Person Mother { get; }
        public Person Father { get; }
        private Person _spouse;
        public Person Spouse
        {
            get => _spouse;
            set
            {
                if (_spouse is null && value.Spouse == this)
                    _spouse = value;
            }
        }

        public void AddChild(Person child)
        {
            if (child?.Mother == this || child?.Father == this)
                _children.Add(child);
        }

        public List<Person> GetChildren() => new List<Person>(_children);
    }

    public static class FamilyTreeService
    {
        public static Person CreatePerson(string name, Gender gender, Person mother = null,
            Person father = null, Person spouse = null)
        {
            var person = new Person(name, gender, mother, father, spouse?.Spouse is null ? spouse : null);
            mother?.AddChild(person);
            father?.AddChild(person);
            if (spouse is not null)
            {
                spouse.Spouse ??= person;
            }

            return person;
        }

        public static IList<Person> GetParentsList(Person person)
            => new List<Person>
            {
                person.Father, person.Mother
            };

        public static IEnumerable<Person> GetUnclesAndAuntsList(Person person)
        {
            var parents = GetParentsList(person);
            var grandParents = new List<Person>();
            foreach (var parent in parents)
            {
                if (parent is null)
                    continue;

                grandParents.AddRange(GetParentsList(parent));
            }

            return grandParents.SelectMany(i => i?.GetChildren())
                .Where(i => !parents.Contains(i))
                .ToHashSet();
        }

        public static IEnumerable<Person> GetCousinsList(Person person)
        {
            var result = new List<Person>();

            var unclesAndAunts = GetUnclesAndAuntsList(person);
            foreach (var item in unclesAndAunts)
                result.AddRange(item.GetChildren());

            return result.ToHashSet();
        }

        public static IEnumerable<Person> GetInLawsList(Person person)
            => person.Spouse is null ? null : GetParentsList(person.Spouse);
    }

    class Program
    {
        static void Main(string[] args)
        {
            var grandMotherM = FamilyTreeService.CreatePerson("Бабушка м", Gender.Female);
            var grandFatherM = FamilyTreeService.CreatePerson("Дедушка м", Gender.Male);
            var grandMotherF = FamilyTreeService.CreatePerson("Бабушка п", Gender.Female);
            var grandFatherF = FamilyTreeService.CreatePerson("Дедушка п", Gender.Male);
            var mother = FamilyTreeService.CreatePerson("Мама", Gender.Female, grandMotherM, grandMotherM);
            var father = FamilyTreeService.CreatePerson("Папа", Gender.Male, grandMotherF, grandFatherF);
            var uncleF = FamilyTreeService.CreatePerson("Дядя п", Gender.Male, grandMotherF, grandFatherF);
            var auntF = FamilyTreeService.CreatePerson("Тетя п", Gender.Female, grandMotherF, grandFatherF);
            var auntM = FamilyTreeService.CreatePerson("Тетя м", Gender.Female, grandMotherM, grandFatherM);
            var cousin1 = FamilyTreeService.CreatePerson("кузин 1", Gender.Male, auntM);
            var cousin2 = FamilyTreeService.CreatePerson("кузина 2", Gender.Female, auntF);
            var cousin3 = FamilyTreeService.CreatePerson("кузин 3 ", Gender.Male, null, uncleF);
            var person = FamilyTreeService.CreatePerson("Тот самый", Gender.Male);
            var parents = FamilyTreeService.GetParentsList(person);
            var unclesAndAunts = FamilyTreeService.GetUnclesAndAuntsList(person);
            var cousins = FamilyTreeService.GetCousinsList(person);
            var inLaws = FamilyTreeService.GetInLawsList(person);
        }
    }
}